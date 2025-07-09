using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Services;
using DimmedAPI.BO;
using DimmedAPI.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntryRequestComponentsController : ControllerBase
    {
        private readonly IDynamicBCConnectionService _dynamicBCConnectionService;
        private readonly IDynamicConnectionService _dynamicConnectionService;

        public EntryRequestComponentsController(
            IDynamicBCConnectionService dynamicBCConnectionService,
            IDynamicConnectionService dynamicConnectionService)
        {
            _dynamicBCConnectionService = dynamicBCConnectionService;
            _dynamicConnectionService = dynamicConnectionService;
        }

        /// <summary>
        /// Obtiene componentes de EntryRequest desde Business Central
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="location">Ubicación (opcional)</param>
        /// <param name="stock">Stock (opcional)</param>
        /// <param name="salesCode">Código de venta (opcional)</param>
        /// <returns>Lista de componentes</returns>
        [HttpGet("consultar-bc")]
        public async Task<IActionResult> ConsultarBC(
            [FromQuery] string companyCode,
            [FromQuery] string? location = null,
            [FromQuery] string? stock = null,
            [FromQuery] string? salesCode = null)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EntryRequestComponentsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var componentsBO = new EntryRequestComponentsBO(companyContext, bcConn);
                
                var componentes = await componentsBO.GetComponentsAsync(location, stock, salesCode);
                return Ok(componentes);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al consultar componentes en BC", detalle });
            }
        }

        /// <summary>
        /// Sincroniza componentes desde Business Central
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="location">Ubicación (opcional)</param>
        /// <param name="stock">Stock (opcional)</param>
        /// <param name="salesCode">Código de venta (opcional)</param>
        /// <returns>Resultado de la sincronización</returns>
        [HttpPost("sincronizar")]
        public async Task<IActionResult> Sincronizar(
            [FromQuery] string companyCode,
            [FromQuery] string? location = null,
            [FromQuery] string? stock = null,
            [FromQuery] string? salesCode = null)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EntryRequestComponentsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var componentsBO = new EntryRequestComponentsBO(companyContext, bcConn);
                
                var componentes = await componentsBO.SincronizarTodosDesdeBCAsync(location, stock, salesCode);
                
                // Obtener estadísticas
                var componentesNuevos = componentes.Count(c => c.Id == 0); // Los nuevos tendrán Id = 0
                var componentesActualizados = componentes.Count(c => c.Id > 0); // Los actualizados tendrán Id > 0

                var resultado = new
                {
                    totalProcesados = componentes.Count,
                    componentesNuevos = componentesNuevos,
                    componentesActualizados = componentesActualizados,
                    componentes = componentes
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
                return BadRequest(new { mensaje = "Error al sincronizar componentes", detalle });
            }
        }

        /// <summary>
        /// Obtiene componentes locales de la base de datos
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Lista de componentes locales</returns>
        [HttpGet("locales")]
        public async Task<IActionResult> GetLocales([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EntryRequestComponentsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var componentsBO = new EntryRequestComponentsBO(companyContext, bcConn);
                
                var componentes = await componentsBO.GetLocalComponentsAsync();
                return Ok(componentes);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al obtener componentes locales", detalle });
            }
        }

        /// <summary>
        /// Obtiene un componente específico por ItemNo
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="itemNo">Número del item</param>
        /// <returns>Componente encontrado</returns>
        [HttpGet("por-itemno")]
        public async Task<IActionResult> GetByItemNo(
            [FromQuery] string companyCode,
            [FromQuery] string itemNo)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (string.IsNullOrEmpty(itemNo))
                {
                    return BadRequest("El número del item es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EntryRequestComponentsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var componentsBO = new EntryRequestComponentsBO(companyContext, bcConn);
                
                var componente = await componentsBO.GetLocalComponentByItemNoAsync(itemNo);
                
                if (componente == null)
                {
                    return NotFound(new { mensaje = "Componente no encontrado" });
                }

                return Ok(componente);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al obtener componente por ItemNo", detalle });
            }
        }

        /// <summary>
        /// Obtiene componentes por EntryRequest ID
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="idEntryReq">ID del EntryRequest</param>
        /// <returns>Lista de componentes del EntryRequest</returns>
        [HttpGet("por-entryrequest")]
        public async Task<IActionResult> GetByEntryRequest(
            [FromQuery] string companyCode,
            [FromQuery] int idEntryReq)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EntryRequestComponentsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var componentsBO = new EntryRequestComponentsBO(companyContext, bcConn);
                
                var componentes = await componentsBO.GetComponentsByEntryRequestAsync(idEntryReq);
                return Ok(componentes);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al obtener componentes por EntryRequest", detalle });
            }
        }

        /// <summary>
        /// Obtiene estadísticas de sincronización
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Estadísticas de sincronización</returns>
        [HttpGet("estadisticas")]
        public async Task<IActionResult> GetEstadisticas([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EntryRequestComponentsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var componentsBO = new EntryRequestComponentsBO(companyContext, bcConn);
                
                var estadisticas = await componentsBO.GetSyncStatisticsAsync();
                return Ok(estadisticas);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al obtener estadísticas", detalle });
            }
        }

        /// <summary>
        /// Elimina un componente de la base de datos local
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="id">ID del componente a eliminar</param>
        /// <returns>Resultado de la eliminación</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            [FromQuery] string companyCode,
            [FromRoute] int id)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EntryRequestComponentsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var componentsBO = new EntryRequestComponentsBO(companyContext, bcConn);
                
                var resultado = await componentsBO.DeleteLocalComponentAsync(id);
                
                if (!resultado)
                {
                    return NotFound(new { mensaje = "Componente no encontrado" });
                }

                return Ok(new { mensaje = "Componente eliminado exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al eliminar componente", detalle });
            }
        }

        /// <summary>
        /// Obtiene líneas de ensamble desde Business Central
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="documentNo">Número de documento (opcional)</param>
        /// <returns>Lista de componentes de líneas de ensamble</returns>
        [HttpGet("assembly-lines")]
        public async Task<IActionResult> GetAssemblyLines(
            [FromQuery] string companyCode,
            [FromQuery] string? documentNo = null)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EntryRequestComponentsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var componentsBO = new EntryRequestComponentsBO(companyContext, bcConn);
                
                var componentes = await componentsBO.GetAssemblyLinesAsync(documentNo);
                return Ok(componentes);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al consultar líneas de ensamble en BC", detalle });
            }
        }

        /// <summary>
        /// Verifica la configuración de la compañía
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Configuración de la compañía</returns>
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