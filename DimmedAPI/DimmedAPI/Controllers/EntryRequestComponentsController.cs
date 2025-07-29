using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Services;
using DimmedAPI.BO;
using DimmedAPI.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OutputCaching;
using System.Threading.Tasks;
using System.Linq;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntryRequestComponentsController : ControllerBase
    {
        private readonly IDynamicBCConnectionService _dynamicBCConnectionService;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "entryrequestcomponents";

        public EntryRequestComponentsController(
            IDynamicBCConnectionService dynamicBCConnectionService,
            IDynamicConnectionService dynamicConnectionService,
            IOutputCacheStore outputCacheStore)
        {
            _dynamicBCConnectionService = dynamicBCConnectionService;
            _dynamicConnectionService = dynamicConnectionService;
            _outputCacheStore = outputCacheStore;
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
        [OutputCache(Tags = [cacheTag], Duration = 300)] // Cache por 5 minutos
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

                // Invalidar caché después de sincronización
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

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
        [OutputCache(Tags = [cacheTag], Duration = 600)] // Cache por 10 minutos
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
        [OutputCache(Tags = [cacheTag], Duration = 300)] // Cache por 5 minutos
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
        [OutputCache(Tags = [cacheTag], Duration = 300)] // Cache por 5 minutos
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
        [OutputCache(Tags = [cacheTag], Duration = 600)] // Cache por 10 minutos
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

                // Invalidar caché después de eliminación
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

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
        [OutputCache(Tags = [cacheTag], Duration = 300)] // Cache por 5 minutos
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
        [OutputCache(Tags = [cacheTag], Duration = 1800)] // Cache por 30 minutos
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

        /// <summary>
        /// Consulta información detallada de cantidades y disponibilidad de componentes desde Business Central
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="location">Bodega/Ubicación</param>
        /// <param name="stock">Bodega de stock</param>
        /// <param name="salesCode">Código de venta</param>
        /// <param name="reference">Código de referencia del componente</param>
        /// <param name="page">Número de página (opcional, por defecto 1)</param>
        /// <param name="pageSize">Tamaño de página (opcional, por defecto 50, máximo 200)</param>
        /// <returns>Lista de componentes con información detallada de cantidades</returns>
        [HttpGet("consultar-cantidades-disponibilidad")]
        [OutputCache(Tags = [cacheTag], Duration = 120)] // Cache por 2 minutos
        public async Task<IActionResult> ConsultarCantidadesDisponibilidad(
            [FromQuery] string companyCode,
            [FromQuery] string? location = null,
            [FromQuery] string? stock = null,
            [FromQuery] string? salesCode = null,
            [FromQuery] string? reference = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Validar parámetros de paginación
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 50;
                if (pageSize > 200) pageSize = 200;

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EntryRequestComponentsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var componentsBO = new EntryRequestComponentsBO(companyContext, bcConn);
                
                // Obtener componentes con información de cantidades
                var componentes = await componentsBO.GetComponentsAsync(location, stock, salesCode);
                
                // Validar que componentes no sea null
                if (componentes == null)
                {
                    return Ok(new
                    {
                        TotalComponentes = 0,
                        ComponentesConStock = 0,
                        ComponentesSinStock = 0,
                        Componentes = new List<object>(),
                        Paginacion = new
                        {
                            PaginaActual = page,
                            TamañoPagina = pageSize,
                            TotalPaginas = 0,
                            TotalElementos = 0
                        },
                        Mensaje = "No se pudieron obtener componentes desde Business Central"
                    });
                }
                
                // Si se proporciona una referencia específica, filtrar por ella
                if (!string.IsNullOrEmpty(reference))
                {
                    componentes = componentes.Where(c => c.ItemNo == reference || c.ItemName.Contains(reference)).ToList();
                }

                // Aplicar paginación
                var totalElementos = componentes.Count;
                var totalPaginas = (int)Math.Ceiling((double)totalElementos / pageSize);
                var elementosPaginados = componentes
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Agregar información adicional de disponibilidad
                var componentesConDisponibilidad = elementosPaginados.Select(c => new
                {
                    c.Id,
                    c.ItemNo,
                    c.ItemName,
                    c.Warehouse,
                    c.Quantity,
                    c.UnitPrice,
                    c.SystemId,
                    c.Branch,
                    c.Lot,
                    c.status,
                    c.AssemblyNo,
                    c.TaxCode,
                    c.shortDesc,
                    c.Invima,
                    c.ExpirationDate,
                    c.TraceState,
                    c.RSFechaVencimiento,
                    c.RSClasifRegistro,
                    // Información adicional de disponibilidad
                    Disponibilidad = new
                    {
                        CantidadDisponible = c.Quantity ?? 0,
                        CantidadReservada = c.QuantityConsumed ?? 0,
                        CantidadNeta = (c.Quantity ?? 0) - (c.QuantityConsumed ?? 0),
                        TieneStock = (c.Quantity ?? 0) > 0,
                        RequiereReposicion = (c.Quantity ?? 0) <= 5, // Umbral configurable
                        EstadoInventario = GetEstadoInventario(c.Quantity ?? 0)
                    }
                }).ToList();

                return Ok(new
                {
                    TotalComponentes = totalElementos,
                    ComponentesConStock = componentes.Count(c => (c.Quantity ?? 0) > 0),
                    ComponentesSinStock = componentes.Count(c => (c.Quantity ?? 0) <= 0),
                    Componentes = componentesConDisponibilidad,
                    Paginacion = new
                    {
                        PaginaActual = page,
                        TamañoPagina = pageSize,
                        TotalPaginas = totalPaginas,
                        TotalElementos = totalElementos
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
                return BadRequest(new { mensaje = "Error al consultar cantidades y disponibilidad", detalle });
            }
        }

        /// <summary>
        /// Consulta información de cantidades por grupo de precios desde Business Central
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="location">Bodega/Ubicación</param>
        /// <param name="stock">Bodega de stock</param>
        /// <param name="priceGroup">Grupo de precios</param>
        /// <param name="reference">Código de referencia del componente</param>
        /// <param name="page">Número de página (opcional, por defecto 1)</param>
        /// <param name="pageSize">Tamaño de página (opcional, por defecto 50, máximo 200)</param>
        /// <returns>Lista de componentes con información de cantidades por grupo de precios</returns>
        [HttpGet("consultar-cantidades-por-grupo-precios")]
        [OutputCache(Tags = [cacheTag], Duration = 120)] // Cache por 2 minutos
        public async Task<IActionResult> ConsultarCantidadesPorGrupoPrecios(
            [FromQuery] string companyCode,
            [FromQuery] string? location = null,
            [FromQuery] string? stock = null,
            [FromQuery] string? priceGroup = null,
            [FromQuery] string? reference = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Validar parámetros de paginación
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 50;
                if (pageSize > 200) pageSize = 200;

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EntryRequestComponentsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var componentsBO = new EntryRequestComponentsBO(companyContext, bcConn);
                
                // Obtener componentes usando priceGroup como salesCode
                var componentes = await componentsBO.GetComponentsAsync(location, stock, priceGroup);
                
                // Validar que componentes no sea null
                if (componentes == null)
                {
                    return Ok(new
                    {
                        TotalComponentes = 0,
                        GrupoPrecios = priceGroup,
                        ComponentesConStock = 0,
                        ComponentesSinStock = 0,
                        ValorTotalInventario = 0,
                        Componentes = new List<object>(),
                        Paginacion = new
                        {
                            PaginaActual = page,
                            TamañoPagina = pageSize,
                            TotalPaginas = 0,
                            TotalElementos = 0
                        },
                        Mensaje = "No se pudieron obtener componentes desde Business Central"
                    });
                }
                
                // Si se proporciona una referencia específica, filtrar por ella
                if (!string.IsNullOrEmpty(reference))
                {
                    componentes = componentes.Where(c => c.ItemNo == reference || c.ItemName.Contains(reference)).ToList();
                }

                // Aplicar paginación
                var totalElementos = componentes.Count;
                var totalPaginas = (int)Math.Ceiling((double)totalElementos / pageSize);
                var elementosPaginados = componentes
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Agregar información adicional de precios y disponibilidad
                var componentesConPrecios = elementosPaginados.Select(c => new
                {
                    c.Id,
                    c.ItemNo,
                    c.ItemName,
                    c.Warehouse,
                    c.Quantity,
                    c.UnitPrice,
                    c.SystemId,
                    c.Branch,
                    c.Lot,
                    c.status,
                    c.AssemblyNo,
                    c.TaxCode,
                    c.shortDesc,
                    c.Invima,
                    c.ExpirationDate,
                    c.TraceState,
                    c.RSFechaVencimiento,
                    c.RSClasifRegistro,
                    // Información adicional de precios y disponibilidad
                    InformacionPrecios = new
                    {
                        GrupoPrecios = priceGroup,
                        PrecioUnitario = c.UnitPrice ?? 0,
                        CantidadDisponible = c.Quantity ?? 0,
                        CantidadReservada = c.QuantityConsumed ?? 0,
                        CantidadNeta = (c.Quantity ?? 0) - (c.QuantityConsumed ?? 0),
                        ValorTotalDisponible = (c.Quantity ?? 0) * (c.UnitPrice ?? 0),
                        TieneStock = (c.Quantity ?? 0) > 0,
                        RequiereReposicion = (c.Quantity ?? 0) <= 5, // Umbral configurable
                        EstadoInventario = GetEstadoInventario(c.Quantity ?? 0)
                    }
                }).ToList();

                return Ok(new
                {
                    TotalComponentes = totalElementos,
                    GrupoPrecios = priceGroup,
                    ComponentesConStock = componentes.Count(c => (c.Quantity ?? 0) > 0),
                    ComponentesSinStock = componentes.Count(c => (c.Quantity ?? 0) <= 0),
                    ValorTotalInventario = componentes.Sum(c => (c.Quantity ?? 0) * (c.UnitPrice ?? 0)),
                    Componentes = componentesConPrecios,
                    Paginacion = new
                    {
                        PaginaActual = page,
                        TamañoPagina = pageSize,
                        TotalPaginas = totalPaginas,
                        TotalElementos = totalElementos
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
                return BadRequest(new { mensaje = "Error al consultar cantidades por grupo de precios", detalle });
            }
        }

        /// <summary>
        /// Determina el estado del inventario basado en la cantidad disponible
        /// </summary>
        /// <param name="cantidad">Cantidad disponible</param>
        /// <returns>Estado del inventario</returns>
        private string GetEstadoInventario(decimal cantidad)
        {
            if (cantidad <= 0)
                return "SIN_STOCK";
            else if (cantidad <= 5)
                return "STOCK_BAJO";
            else if (cantidad <= 20)
                return "STOCK_MEDIO";
            else
                return "STOCK_ALTO";
        }
    }
} 