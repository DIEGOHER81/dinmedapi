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
    public class EquipmentAPIController : ControllerBase
    {
        private readonly IEquipmentBO _equipmentBO;
        private readonly IDynamicBCConnectionService _dynamicBCConnectionService;
        private readonly IDynamicConnectionService _dynamicConnectionService;

        public EquipmentAPIController(
            IEquipmentBO equipmentBO,
            IDynamicBCConnectionService dynamicBCConnectionService,
            IDynamicConnectionService dynamicConnectionService)
        {
            _equipmentBO = equipmentBO;
            _dynamicBCConnectionService = dynamicBCConnectionService;
            _dynamicConnectionService = dynamicConnectionService;
        }

        [HttpPost("sincronizar")]
        public async Task<IActionResult> Sincronizar([FromBody] EquipmentBCDTO dto, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EquipmentBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var equipmentBO = new EquipmentBO(companyContext, bcConn);
                
                var result = await equipmentBO.SincronizarDesdeBC(dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al sincronizar equipo", detalle = ex.Message });
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
                
                // Crear un EquipmentBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var equipmentBO = new EquipmentBO(companyContext, bcConn);
                
                var equipos = await equipmentBO.SincronizeBCAsync();
                
                // Obtener estadísticas
                var equiposNuevos = equipos.Count(e => e.Id == 0); // Los nuevos tendrán Id = 0
                var equiposActualizados = equipos.Count(e => e.Id > 0); // Los actualizados tendrán Id > 0

                var resultado = new
                {
                    totalProcesados = equipos.Count,
                    equiposNuevos = equiposNuevos,
                    equiposActualizados = equiposActualizados,
                    equipos = equipos
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
                return BadRequest(new { mensaje = "Error al sincronizar equipos", detalle });
            }
        }

        [HttpGet("consultar-bc")]
        public async Task<IActionResult> ConsultarBC([FromQuery] string companyCode, [FromQuery] int? take = null, [FromQuery] string systemIdBc = null, [FromQuery] string code = null)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EquipmentBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var equipmentBO = new EquipmentBO(companyContext, bcConn);
                
                var equipos = await equipmentBO.GetEquipmentsFromBCAsync(take, systemIdBc, code);
                return Ok(equipos);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al consultar equipos en BC", detalle });
            }
        }

        [HttpPost("sincronizar-uno")]
        public async Task<IActionResult> SincronizarUno([FromQuery] string companyCode, [FromQuery] string systemId = null, [FromQuery] string code = null)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EquipmentBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var equipmentBO = new EquipmentBO(companyContext, bcConn);
                
                // Buscar el equipo en Business Central por systemId, code, o ambos
                var equipos = await equipmentBO.GetEquipmentsFromBCAsync(null, systemId, code);
                var equipoBC = equipos.FirstOrDefault(e =>
                    (string.IsNullOrEmpty(systemId) || (!string.IsNullOrEmpty(e.SystemIdBC) && e.SystemIdBC.Equals(systemId, StringComparison.OrdinalIgnoreCase))) &&
                    (string.IsNullOrEmpty(code) || (!string.IsNullOrEmpty(e.Code) && e.Code.Trim().Equals(code.Trim(), StringComparison.OrdinalIgnoreCase)))
                );
                if (equipoBC == null)
                {
                    return NotFound(new { mensaje = "Equipo no encontrado en Business Central" });
                }

                // Sincronizar el equipo encontrado
                var result = await equipmentBO.SincronizarDesdeBC(new EquipmentBCDTO
                {
                    Name = equipoBC.Name,
                    Abc = equipoBC.Abc,
                    Branch = equipoBC.Branch,
                    Brand = equipoBC.Brand,
                    Code = equipoBC.Code,
                    Description = equipoBC.Description,
                    DestinationBranch = equipoBC.DestinationBranch,
                    EndDate = equipoBC.EndDate,
                    EstimatedTime = equipoBC.EstimatedTime,
                    InitDate = equipoBC.InitDate,
                    SystemId = equipoBC.SystemIdBC,
                    IsActive = equipoBC.IsActive,
                    LoanDate = equipoBC.LoanDate,
                    Model = equipoBC.Model,
                    ProductLine = equipoBC.ProductLine,
                    ReturnDate = equipoBC.ReturnDate,
                    ShortName = equipoBC.ShortName,
                    Status = equipoBC.Status,
                    TechSpec = equipoBC.TechSpec,
                    Type = equipoBC.Type,
                    Vendor = equipoBC.Vendor,
                    NoBoxes = equipoBC.NoBoxes,
                    Alert = equipoBC.Alert,
                    LocationCode = equipoBC.LocationCode,
                    TransferStatus = equipoBC.TransferStatus
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
                return BadRequest(new { mensaje = "Error al sincronizar equipo específico", detalle });
            }
        }

        [HttpGet("inventory/{equipmentId}")]
        public async Task<IActionResult> GetInventory(int equipmentId, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EquipmentBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var equipmentBO = new EquipmentBO(companyContext, bcConn);
                
                var inventory = await equipmentBO.getAInventory(equipmentId);
                return Ok(inventory);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al obtener el inventario del equipo", detalle });
            }
        }

        [HttpGet("inventory/code/{code}")]
        public async Task<IActionResult> GetInventoryByCode(string code, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Validar que el código no sea nulo o vacío
                if (string.IsNullOrWhiteSpace(code))
                {
                    return BadRequest(new { mensaje = "El código del equipo es obligatorio" });
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EquipmentBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var equipmentBO = new EquipmentBO(companyContext, bcConn);
                
                var inventory = await equipmentBO.getAInventory(code);
                return Ok(inventory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (EquipmentNotFoundException ex)
            {
                return NotFound(new { 
                    mensaje = ex.Message,
                    detalle = "Para solucionar este problema, puede:",
                    soluciones = new string[] {
                        "1. Verificar que el código del equipo sea correcto",
                        "2. Sincronizar el equipo desde Business Central usando: POST /api/EquipmentAPI/sincronizar-uno?companyCode=" + companyCode + "&code=" + code,
                        "3. Consultar la lista de equipos disponibles usando: GET /api/EquipmentAPI/list-codes?companyCode=" + companyCode,
                        "4. Verificar si el equipo existe usando: GET /api/EquipmentAPI/exists/code/" + code + "?companyCode=" + companyCode
                    },
                    codigoError = "EQUIPO_NO_ENCONTRADO",
                    equipoSolicitado = code
                });
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al obtener el inventario del equipo", detalle });
            }
        }

        [HttpGet("inventory/code/{code}/location/{locationCode}")]
        public async Task<IActionResult> GetInventoryByCodeAndLocation(string code, string locationCode, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EquipmentBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var equipmentBO = new EquipmentBO(companyContext, bcConn);
                
                var inventory = await equipmentBO.getAInventory(code, locationCode);
                return Ok(inventory);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al obtener el inventario del equipo", detalle });
            }
        }

        [HttpGet("exists/code/{code}")]
        public async Task<IActionResult> CheckEquipmentExists(string code, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (string.IsNullOrWhiteSpace(code))
                {
                    return BadRequest(new { mensaje = "El código del equipo es obligatorio" });
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EquipmentBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var equipmentBO = new EquipmentBO(companyContext, bcConn);

                // Intentar obtener el equipo para verificar si existe
                var equipment = await equipmentBO.GetEquipmentByCode(code);
                if (equipment == null)
                {
                    return NotFound(new { mensaje = $"No se encontró el equipo con código '{code}' en la base de datos" });
                }

                return Ok(new { 
                    mensaje = "Equipo encontrado", 
                    equipment = new { 
                        id = equipment.Id, 
                        code = equipment.Code, 
                        name = equipment.Name,
                        branch = equipment.Branch,
                        status = equipment.Status,
                        isActive = equipment.IsActive
                    } 
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al verificar la existencia del equipo", detalle });
            }
        }

        [HttpGet("list-codes")]
        public async Task<IActionResult> ListEquipmentCodes([FromQuery] string companyCode, [FromQuery] int? take = 50)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EquipmentBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var equipmentBO = new EquipmentBO(companyContext, bcConn);
                
                var equipments = await equipmentBO.GetAllEquipmentCodes(take);
                return Ok(new { 
                    mensaje = $"Se encontraron {equipments.Count} equipos",
                    equipments = equipments.Select(e => new { 
                        id = e.Id, 
                        code = e.Code, 
                        name = e.Name,
                        branch = e.Branch,
                        status = e.Status,
                        isActive = e.IsActive
                    })
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al obtener la lista de equipos", detalle });
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