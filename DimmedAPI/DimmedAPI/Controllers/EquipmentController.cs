using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;
using DimmedAPI.DTOs;
using System.Linq;
using System.Data;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/equipment")]
    public class EquipmentController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;

        public EquipmentController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
        }

        // GET: api/equipment?companyCode=xxx
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipmentResponseDTO>>> GetAll([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");

                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                var equipos = await companyContext.Equipment.ToListAsync();

                var equiposDto = equipos.Select(e => new EquipmentResponseDTO
                {
                    Id = e.Id,
                    Code = e.Code ?? "",
                    Name = e.Name ?? "",
                    ShortName = e.ShortName ?? "",
                    Status = e.Status ?? "",
                    ProductLine = e.ProductLine ?? "",
                    Branch = e.Branch ?? "",
                    EstimatedTime = e.EstimatedTime ?? "",
                    Description = e.Description ?? "",
                    IsActive = e.IsActive,
                    TechSpec = e.TechSpec ?? "",
                    DestinationBranch = e.DestinationBranch ?? "",
                    LoanDate = e.LoanDate,
                    ReturnDate = e.ReturnDate,
                    InitDate = e.InitDate,
                    Vendor = e.Vendor ?? "",
                    Brand = e.Brand ?? "",
                    Model = e.Model ?? "",
                    Abc = e.Abc ?? "",
                    EndDate = e.EndDate,
                    Type = e.Type ?? "",
                    SystemIdBC = e.SystemIdBC ?? "",
                    NoBoxes = e.NoBoxes,
                    LastPreventiveMaintenance = e.LastPreventiveMaintenance,
                    LastMaintenance = e.LastMaintenance,
                    Alert = e.Alert ?? "",
                    LocationCode = e.LocationCode ?? "",
                    TransferStatus = e.TransferStatus ?? ""
                }).ToList();

                return Ok(equiposDto);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (System.Data.SqlTypes.SqlNullValueException ex)
            {
                return StatusCode(500, $"Error de datos nulos en la base de datos (SqlNullValueException): {ex.Message}. Verifique que los campos en la base de datos permitan nulos o que el modelo los acepte como nullable.");
            }
            catch (NullReferenceException ex)
            {
                return StatusCode(500, $"Error de datos nulos: {ex.Message}. Es posible que algún campo en la base de datos esté en NULL y el modelo no lo permite. Verifique los campos de fechas y cadenas nulas.");
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"Error de operación inválida: {ex.Message}. Es posible que algún campo en la base de datos esté en NULL y el modelo no lo permite. Verifique los campos de fechas y cadenas nulas.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/equipment/{id}?companyCode=xxx
        [HttpGet("{id}")]
        public async Task<ActionResult<Equipment>> GetById(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");

                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                var equipo = await companyContext.Equipment.FindAsync(id);
                if (equipo == null)
                    return NotFound($"No se encontró el equipo con ID {id}");
                return Ok(equipo);
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

        // (Opcional) GET: api/equipment/by-code/{code}?companyCode=xxx
        [HttpGet("by-code/{code}")]
        public async Task<ActionResult<Equipment>> GetByCode(string code, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");

                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                var equipo = await companyContext.Equipment.FirstOrDefaultAsync(e => e.Code == code);
                if (equipo == null)
                    return NotFound($"No se encontró el equipo con código {code}");
                return Ok(equipo);
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

        // GET: api/equipment/summary?companyCode=xxx&page=1&pageSize=10&filter=xxx
        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetSummary(
            [FromQuery] string companyCode,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string filter = null)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var query = companyContext.Equipment
                .Select(e => new DTOs.SummaryEquipmentDTO
                {
                    Id = e.Id,
                    Code = e.Code ?? string.Empty,
                    Name = e.Name ?? string.Empty,
                    ShortName = e.ShortName ?? string.Empty,
                    Branch = e.Branch ?? string.Empty,
                    Status = e.Status ?? string.Empty
                });

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var filterLower = filter.ToLower();
                query = query.Where(e =>
                    e.Code.ToLower().Contains(filterLower) ||
                    e.Name.ToLower().Contains(filterLower)
                );
            }

            var total = await query.CountAsync();
            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new { total, page, pageSize, data });
        }

        // GET: api/equipment/view/columns?companyCode=xxx
        [HttpGet("view/columns")]
        public async Task<ActionResult<object>> GetEquipmentViewColumns([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");

                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                using (var command = companyContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "EXEC sp_Equipment_view";
                    command.CommandType = System.Data.CommandType.Text;
                    
                    if (command.Connection.State != System.Data.ConnectionState.Open)
                        await command.Connection.OpenAsync();
                    
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        var columns = new List<object>();
                        for (int i = 0; i < result.FieldCount; i++)
                        {
                            columns.Add(new
                            {
                                Index = i,
                                Name = result.GetName(i),
                                DataType = result.GetDataTypeName(i)
                            });
                        }
                        
                        return Ok(new { 
                            totalColumns = result.FieldCount,
                            columns = columns,
                            message = "Columnas disponibles en sp_Equipment_view"
                        });
                    }
                }
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

        // GET: api/equipment/view?companyCode=xxx&page=1&pageSize=10&filter=xxx
        [HttpGet("view")]
        public async Task<ActionResult<object>> GetEquipmentView(
            [FromQuery] string companyCode,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string filter = null)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");

                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Ejecutar el procedimiento almacenado usando ADO.NET directo
                var equiposView = new List<EquipmentView>();
                
                using (var command = companyContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "EXEC sp_Equipment_view";
                    command.CommandType = System.Data.CommandType.Text;
                    
                    if (command.Connection.State != System.Data.ConnectionState.Open)
                        await command.Connection.OpenAsync();
                    
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        // Obtener los nombres de las columnas disponibles
                        var columnNames = new List<string>();
                        for (int i = 0; i < result.FieldCount; i++)
                        {
                            columnNames.Add(result.GetName(i));
                        }
                        
                        while (await result.ReadAsync())
                        {
                            try
                            {
                                equiposView.Add(new EquipmentView
                                {
                                    Codigo = GetStringValue(result, "Codigo"),
                                    Nombre = GetStringValue(result, "Nombre"),
                                    Estado = GetStringValue(result, "Estado"),
                                    Sede = GetStringValue(result, "Sede"),
                                    UltimoPedido = GetStringValue(result, "UltimoPedido"),
                                    FechaUltimaCirugia = GetDateTimeValue(result, "FechaUltimaCirugia"),
                                    EstadoPedido = GetStringValue(result, "EstadoPedido"),
                                    EstadoTrazabilidad = GetStringValue(result, "EstadoTrazabilidad"),
                                    FechaTrazabilidad = GetDateTimeValue(result, "FEchaTrazabilidad"),
                                    Institucion = GetStringValue(result, "Institucion"),
                                    DireccionEntrega = GetStringValue(result, "DireccíonEntrega"),
                                    TipoEquipo = GetStringValue(result, "TipoEquipo"),
                                    EquipoPrincipal = GetStringValue(result, "EquipoPrincipal"),
                                    NroCajas = GetIntValue(result, "NroCajas"),
                                    LineaProducto = GetStringValue(result, "LineaProducto"),
                                    Ciclocirugia = GetStringValue(result, "Ciclocirugia"),
                                    FechaInicio = GetDateTimeValue(result, "FechaInicio"),
                                    FechaRetiro = GetDateTimeValue(result, "FechaRetiro"),
                                    Proveedor = GetStringValue(result, "Proveedor"),
                                    Marca = GetStringValue(result, "Marca"),
                                    Modelo = GetStringValue(result, "Modelo"),
                                    Clasificacion = GetStringValue(result, "Clasificacion"),
                                    Alerta = GetStringValue(result, "Alerta")
                                });
                            }
                            catch (Exception ex)
                            {
                                // Log del error para debugging
                                Console.WriteLine($"Error mapeando fila: {ex.Message}");
                                Console.WriteLine($"Columnas disponibles: {string.Join(", ", columnNames)}");
                                throw;
                            }
                        }
                    }
                }

                // Aplicar filtro si se proporciona
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    var filterLower = filter.ToLower();
                    equiposView = equiposView.Where(e =>
                        e.Codigo.ToLower().Contains(filterLower) ||
                        e.Nombre.ToLower().Contains(filterLower) ||
                        e.Estado.ToLower().Contains(filterLower) ||
                        e.Sede.ToLower().Contains(filterLower)
                    ).ToList();
                }

                var total = equiposView.Count;
                var data = equiposView
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new { 
                    total, 
                    page, 
                    pageSize, 
                    data,
                    message = "Datos obtenidos del procedimiento almacenado sp_Equipment_view"
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

        // Métodos auxiliares para mapeo seguro de datos
        private string GetStringValue(System.Data.Common.DbDataReader reader, string columnName)
        {
            try
            {
                // Verificar si la columna existe
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        var value = reader[columnName];
                        return value?.ToString() ?? string.Empty;
                    }
                }
                
                // Si no encuentra la columna, buscar por nombre exacto
                var result = reader[columnName];
                return result?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error leyendo columna {columnName}: {ex.Message}");
                Console.WriteLine($"Columnas disponibles: {string.Join(", ", GetAvailableColumns(reader))}");
                return string.Empty;
            }
        }

        private List<string> GetAvailableColumns(System.Data.Common.DbDataReader reader)
        {
            var columns = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i));
            }
            return columns;
        }

        private DateTime? GetDateTimeValue(System.Data.Common.DbDataReader reader, string columnName)
        {
            try
            {
                // Verificar si la columna existe
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        var value = reader[columnName];
                        if (value == DBNull.Value || value == null || value.ToString().Trim() == "")
                            return null;
                        return Convert.ToDateTime(value);
                    }
                }
                
                // Si no encuentra la columna, buscar por nombre exacto
                var result = reader[columnName];
                if (result == DBNull.Value || result == null || result.ToString().Trim() == "")
                    return null;
                return Convert.ToDateTime(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error leyendo fecha {columnName}: {ex.Message}");
                Console.WriteLine($"Columnas disponibles: {string.Join(", ", GetAvailableColumns(reader))}");
                return null;
            }
        }

        private int? GetIntValue(System.Data.Common.DbDataReader reader, string columnName)
        {
            try
            {
                // Verificar si la columna existe
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        var value = reader[columnName];
                        if (value == DBNull.Value || value == null || value.ToString().Trim() == "")
                            return null;
                        return Convert.ToInt32(value);
                    }
                }
                
                // Si no encuentra la columna, buscar por nombre exacto
                var result = reader[columnName];
                if (result == DBNull.Value || result == null || result.ToString().Trim() == "")
                    return null;
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error leyendo entero {columnName}: {ex.Message}");
                Console.WriteLine($"Columnas disponibles: {string.Join(", ", GetAvailableColumns(reader))}");
                return null;
            }
        }

        // Método auxiliar para identificar el campo nulo
        private string IdentificarCampoNulo(Equipment equipo)
        {
            if (equipo == null) return "Equipo completo es null";
            if (equipo.LoanDate == null) return "LoanDate";
            if (equipo.ReturnDate == null) return "ReturnDate";
            if (equipo.EndDate == null) return "EndDate";
            if (equipo.LastPreventiveMaintenance == null) return "LastPreventiveMaintenance";
            if (equipo.LastMaintenance == null) return "LastMaintenance";
            // Para string, si quieres detectar null o vacío:
            if (string.IsNullOrEmpty(equipo.Alert)) return "Alert";
            if (string.IsNullOrEmpty(equipo.TransferStatus)) return "TransferStatus";
            return "Desconocido (todos los campos principales tienen valor)";
        }
    }
} 