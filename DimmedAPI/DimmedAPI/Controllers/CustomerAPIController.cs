using DimmedAPI.BO;
using DimmedAPI.DTOs;
using DimmedAPI.Entidades;
using DimmedAPI.Interfaces;
using DimmedAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerAPIController : ControllerBase
    {
        private readonly ICustomerBO _customerBO;
        private readonly IDynamicBCConnectionService _dynamicBCConnectionService;
        private readonly IDynamicConnectionService _dynamicConnectionService;

        public CustomerAPIController(
            ICustomerBO customerBO,
            IDynamicBCConnectionService dynamicBCConnectionService,
            IDynamicConnectionService dynamicConnectionService)
        {
            _customerBO = customerBO;
            _dynamicBCConnectionService = dynamicBCConnectionService;
            _dynamicConnectionService = dynamicConnectionService;
        }

        [HttpPost("sincronizar")]
        public async Task<IActionResult> Sincronizar([FromBody] CustomerBCDTO dto, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un CustomerBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var customerBO = new CustomerBO(companyContext, bcConn);
                
                var result = await customerBO.SincronizarDesdeBC(dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al sincronizar cliente", detalle = ex.Message });
            }
        }

        [HttpPost("sincronizar-todos")]
        public async Task<IActionResult> SincronizarTodos([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un CustomerBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var customerBO = new CustomerBO(companyContext, bcConn);
                
                var clientes = await customerBO.SincronizeBCAsync();
                
                // Obtener estadísticas
                var clientesNuevos = clientes.Count(c => c.Id == 0); // Los nuevos tendrán Id = 0
                var clientesActualizados = clientes.Count(c => c.Id > 0); // Los actualizados tendrán Id > 0

                var resultado = new
                {
                    totalProcesados = clientes.Count,
                    clientesNuevos = clientesNuevos,
                    clientesActualizados = clientesActualizados,
                    clientes = clientes
                };

                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al sincronizar clientes", detalle });
            }
        }

        [HttpGet("consultar-bc")]
        public async Task<IActionResult> ConsultarBC([FromQuery] string companyCode, [FromQuery] int? take = null, [FromQuery] string systemIdBc = null)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un CustomerBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var customerBO = new CustomerBO(companyContext, bcConn);
                
                var clientes = await customerBO.GetCustomersFromBCAsync(take, systemIdBc);
                return Ok(clientes);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al consultar clientes en BC", detalle });
            }
        }

        [HttpPost("sincronizar-uno")]
        public async Task<IActionResult> SincronizarUno([FromQuery] string companyCode, [FromQuery] string no)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un CustomerBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var customerBO = new CustomerBO(companyContext, bcConn);
                
                // Buscar el cliente en Business Central por el campo No
                var clientes = await customerBO.GetCustomersFromBCAsync(null, no);
                var clienteBC = clientes.FirstOrDefault();
                if (clienteBC == null)
                {
                    return NotFound(new { mensaje = "Cliente no encontrado en Business Central" });
                }
                // Sincronizar el cliente encontrado
                var result = await customerBO.SincronizarDesdeBC(new DTOs.CustomerBCDTO
                {
                    Identification = clienteBC.Identification,
                    IdType = clienteBC.IdType,
                    Address = clienteBC.Address,
                    City = clienteBC.City,
                    Phone = clienteBC.Phone,
                    Email = clienteBC.Email,
                    CertMant = clienteBC.CertMant,
                    RemCustomer = clienteBC.RemCustomer,
                    Observations = clienteBC.Observations,
                    Name = clienteBC.Name,
                    SystemIdBc = clienteBC.SystemIdBc,
                    SalesZone = clienteBC.SalesZone,
                    TradeRepres = clienteBC.TradeRepres,
                    NoCopys = clienteBC.NoCopys,
                    IsActive = clienteBC.IsActive,
                    Segment = clienteBC.Segment,
                    No = clienteBC.No,
                    FullName = clienteBC.FullName,
                    PriceGroup = clienteBC.PriceGroup,
                    ShortDesc = clienteBC.ShortDesc,
                    ExIva = clienteBC.ExIva,
                    IsSecondPriceList = clienteBC.IsSecondPriceList,
                    SecondPriceGroup = clienteBC.SecondPriceGroup,
                    InsurerType = clienteBC.InsurerType,
                    IsRemLot = clienteBC.IsRemLot,
                    LyLOpeningHours1 = clienteBC.LyLOpeningHours1,
                    LyLOpeningHours2 = clienteBC.LyLOpeningHours2,
                    PaymentMethodCode = clienteBC.PaymentMethodCode,
                    PaymentTermsCode = clienteBC.PaymentTermsCode
                });
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al sincronizar cliente específico", detalle });
            }
        }

        [HttpGet("verificar-configuracion")]
        public async Task<IActionResult> VerificarConfiguracion([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                var bcConfig = await _dynamicBCConnectionService.GetBusinessCentralConfigAsync(companyCode);
                var company = await _dynamicConnectionService.GetCompanyByCodeAsync(companyCode);

                return Ok(new
                {
                    Company = new
                    {
                        company?.Id,
                        company?.BusinessName,
                        company?.BCCodigoEmpresa
                    },
                    BusinessCentral = new
                    {
                        urlWS = bcConfig.urlWS,
                        url = bcConfig.url,
                        company = bcConfig.company
                    }
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
