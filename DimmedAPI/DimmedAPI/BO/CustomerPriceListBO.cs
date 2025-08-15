using DimmedAPI.DTOs;
using DimmedAPI.Entidades;
using DimmedAPI.Interfaces;
using DimmedAPI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;

namespace DimmedAPI.BO
{
    public class CustomerPriceListBO : ICustomerPriceListBO
    {
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private readonly IConfiguration _configuration;

        public CustomerPriceListBO(IDynamicConnectionService dynamicConnectionService, IConfiguration configuration)
        {
            _dynamicConnectionService = dynamicConnectionService;
            _configuration = configuration;
        }

        /// <summary>
        /// SUPLE TI: Consulta de listas de precio por id de cliente
        /// </summary>
        /// <param name="customerId">Id del cliente</param>
        /// <returns>Lista de precios asociadas al cliente</returns>
        public async Task<List<CustomerPriceListResponseDTO>> GetPriceListByCustomerIdAsync(int customerId)
        {
            try
            {
                List<CustomerPriceList> customerPriceList = await GetCustomerPriceListSPAsync(customerId);
                
                // Convertir a DTO
                return customerPriceList.Select(cpl => new CustomerPriceListResponseDTO
                {
                    Id = cpl.Id,
                    CustomerId = cpl.CustomerId,
                    Identification = cpl.Identification,
                    Name = cpl.Name,
                    InsurerType = cpl.InsurerType,
                    PriceGroup = cpl.PriceGroup
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener listas de precio para el cliente {customerId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// SUPLE TI: Consulta las listas de precio asociadas a un cliente procedimiento TI_GET_CUSTOMERPRICELIST
        /// </summary>
        /// <param name="customerId">Id del cliente</param>
        /// <returns>Lista de precios del cliente</returns>
        /// <exception cref="Exception"></exception>
        private async Task<List<CustomerPriceList>> GetCustomerPriceListSPAsync(int customerId)
        {
            try
            {
                // Obtener la conexión dinámica para la compañía por defecto
                // Por ahora usamos la conexión por defecto, pero se puede modificar para usar conexión dinámica
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                
                // Crear un contexto temporal para ejecutar el procedimiento almacenado
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDBContext>();
                optionsBuilder.UseSqlServer(connectionString);
                
                using var tempContext = new ApplicationDBContext(optionsBuilder.Options);
                
                // Ejecutar el procedimiento almacenado usando DbCommand
                var sql = "EXEC TI_GET_CUSTOMERPRICELIST @IdCustomer";
                using var command = tempContext.Database.GetDbConnection().CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                
                // SUPLE TI: Si el parametro trae datos lo añado, sino lo envío en null
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@IdCustomer";
                parameter.Value = customerId != 0 ? customerId : DBNull.Value;
                command.Parameters.Add(parameter);
                
                tempContext.Database.OpenConnection();
                using var reader = await command.ExecuteReaderAsync();
                
                List<CustomerPriceList> listPriceList = new List<CustomerPriceList>();
                
                // SUPLE TI: Mapeo de las columnas que devuelve el procedimiento
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        CustomerPriceList row = new CustomerPriceList();
                        row.Id = reader.GetInt32("Id");
                        row.CustomerId = reader.GetInt32("IdCustomer");
                        row.Identification = reader.IsDBNull("Identification") ? null : reader.GetString("Identification");
                        row.Name = reader.IsDBNull("Name") ? null : reader.GetString("Name");
                        row.InsurerType = reader.IsDBNull("InsurerType") ? null : reader.GetString("InsurerType");
                        row.PriceGroup = reader.IsDBNull("PriceGroup") ? null : reader.GetString("PriceGroup");
                        listPriceList.Add(row);
                    }
                }

                return listPriceList; // SUPLE TI: Retorno de los valores que trajo el procedimiento almacenado
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en procedimiento almacenado TI_GET_CUSTOMERPRICELIST: {ex.Message}", ex);
            }
        }
    }
}
