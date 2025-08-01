using DimmedAPI.BO;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsAPIController : ControllerBase
    {
        private readonly IDynamicBCConnectionService _dynamicBCConnectionService;
        private readonly IDynamicConnectionService _dynamicConnectionService;

        public ItemsAPIController(
            IDynamicBCConnectionService dynamicBCConnectionService,
            IDynamicConnectionService dynamicConnectionService)
        {
            _dynamicBCConnectionService = dynamicBCConnectionService;
            _dynamicConnectionService = dynamicConnectionService;
        }

        [HttpPost("sincronizar")]
        public async Task<IActionResult> Sincronizar([FromBody] ItemsBC dto, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un ItemsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var itemsBO = new ItemsBO(companyContext, bcConn);
                
                var result = await itemsBO.SincronizarDesdeBC(dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al sincronizar artículo", detalle = ex.Message });
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
                
                // Crear un ItemsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var itemsBO = new ItemsBO(companyContext, bcConn);
                
                var articulos = await itemsBO.SincronizeBCAsync();
                
                // Obtener estadísticas
                var articulosNuevos = articulos.Count(a => a.Id == 0); // Los nuevos tendrán Id = 0
                var articulosActualizados = articulos.Count(a => a.Id > 0); // Los actualizados tendrán Id > 0

                var resultado = new
                {
                    totalProcesados = articulos.Count,
                    articulosNuevos = articulosNuevos,
                    articulosActualizados = articulosActualizados,
                    articulos = articulos
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
                return BadRequest(new { mensaje = "Error al sincronizar artículos", detalle });
            }
        }

        [HttpGet("consultar-bc")]
        public async Task<IActionResult> ConsultarBC([FromQuery] string companyCode, [FromQuery] int? take = null, [FromQuery] string code = null)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un ItemsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var itemsBO = new ItemsBO(companyContext, bcConn);
                
                var articulos = await itemsBO.GetItemsFromBCAsync(take, code);
                return Ok(articulos);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al consultar artículos en BC", detalle });
            }
        }

        [HttpPost("sincronizar-uno")]
        public async Task<IActionResult> SincronizarUno([FromQuery] string companyCode, [FromQuery] string code = null)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (string.IsNullOrEmpty(code))
                {
                    return BadRequest("El código del artículo es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un ItemsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var itemsBO = new ItemsBO(companyContext, bcConn);
                
                var articulos = await itemsBO.GetItemsFromBCAsync(null, code);
                
                if (!articulos.Any())
                {
                    return NotFound($"No se encontró el artículo con código {code}");
                }

                var articulo = articulos.First();
                var resultado = await itemsBO.SincronizarDesdeBC(articulo);
                
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al sincronizar artículo", detalle });
            }
        }

        [HttpGet("obtener-todos")]
        public async Task<IActionResult> ObtenerTodos([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un ItemsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var itemsBO = new ItemsBO(companyContext, bcConn);
                
                var articulos = await itemsBO.getItems();
                return Ok(articulos);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al obtener artículos", detalle });
            }
        }

        [HttpGet("obtener-locales")]
        public async Task<IActionResult> ObtenerLocales([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un ItemsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var itemsBO = new ItemsBO(companyContext, bcConn);
                
                var articulos = await itemsBO.GetLocalItemsAsync();
                return Ok(articulos);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al obtener artículos locales", detalle });
            }
        }

        [HttpGet("obtener-locales-basico")]
        public async Task<IActionResult> ObtenerLocalesBasico(
            [FromQuery] string companyCode,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string filter = null)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un ItemsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var itemsBO = new ItemsBO(companyContext, bcConn);
                
                var articulos = await itemsBO.GetLocalItemsAsync();
                var query = articulos.AsQueryable();
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    var filterLower = filter.ToLower();
                    query = query.Where(a =>
                        (!string.IsNullOrEmpty(a.Code) && a.Code.ToLower().Contains(filterLower)) ||
                        (!string.IsNullOrEmpty(a.Name) && a.Name.ToLower().Contains(filterLower))
                    );
                }
                var total = query.Count();
                var data = query
                    .OrderBy(a => a.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new { a.Id, a.Code, a.Name })
                    .ToList();
                return Ok(new { total, page, pageSize, data });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al obtener artículos locales básicos", detalle });
            }
        }

        [HttpGet("obtener-por-codigo")]
        public async Task<IActionResult> ObtenerPorCodigo([FromQuery] string companyCode, [FromQuery] string code)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (string.IsNullOrEmpty(code))
                {
                    return BadRequest("El código del artículo es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un ItemsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var itemsBO = new ItemsBO(companyContext, bcConn);
                
                var articulo = await itemsBO.GetLocalItemByCodeAsync(code);
                
                if (articulo == null)
                {
                    return NotFound($"No se encontró el artículo con código {code}");
                }

                return Ok(articulo);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al obtener artículo por código", detalle });
            }
        }

        [HttpDelete("eliminar-por-codigo")]
        public async Task<IActionResult> EliminarPorCodigo([FromQuery] string companyCode, [FromQuery] string code)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (string.IsNullOrEmpty(code))
                {
                    return BadRequest("El código del artículo es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un ItemsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var itemsBO = new ItemsBO(companyContext, bcConn);
                
                var eliminado = await itemsBO.DeleteLocalItemAsync(code);
                
                if (!eliminado)
                {
                    return NotFound($"No se encontró el artículo con código {code} para eliminar");
                }

                return Ok(new { mensaje = "Artículo eliminado correctamente" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al eliminar artículo", detalle });
            }
        }

        [HttpGet("estadisticas")]
        public async Task<IActionResult> ObtenerEstadisticas([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un ItemsBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var itemsBO = new ItemsBO(companyContext, bcConn);
                
                var estadisticas = await itemsBO.GetSyncStatisticsAsync();
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

        [HttpGet("VerificarConfiguracionCompania")]
        public async Task<ActionResult<object>> VerificarConfiguracionCompania([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                var company = await _dynamicConnectionService.GetCompanyByCodeAsync(companyCode);
                if (company == null)
                {
                    return NotFound($"Compañía con código {companyCode} no encontrada");
                }

                var bcConfig = await _dynamicConnectionService.GetBusinessCentralConfigAsync(companyCode);

                return Ok(new
                {
                    Company = new
                    {
                        company.Id,
                        company.BusinessName,
                        company.BCCodigoEmpresa,
                        company.SqlConnectionString
                    },
                    BusinessCentral = new
                    {
                        urlWS = bcConfig.urlWS,
                        url = bcConfig.url,
                        company = bcConfig.company
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("itmsBCwithpricelyst")]
        public async Task<IActionResult> GetItemsWithPriceList([FromQuery] string companyCode, [FromQuery] int? take = null)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var itemsBO = new ItemsBO(companyContext, bcConn);

                var articulos = await itemsBO.getItemsWithPriceList(take);
                return Ok(articulos);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al consultar artículos con lista de precios en BC", detalle });
            }
        }

        [HttpPost("sincronizar-items-bc-pricelist")]
        public async Task<IActionResult> SincronizarItemsBCWithPriceList([FromQuery] string companyCode, [FromQuery] int? take = null)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var itemsBO = new ItemsBO(companyContext, bcConn);

                // Si take está vacío, procesar en bloques de 100
                if (!take.HasValue || take.Value <= 0)
                {
                    return await SincronizarEnBloques(itemsBO, companyContext);
                }

                // Si take tiene valor, procesar normalmente
                var resultado = await itemsBO.SincronizarItemsBCWithPriceListAsync(take, true);
                return Ok(new {
                    totalSincronizados = resultado.Count,
                    mensaje = $"Sincronización completada. Total de items procesados: {resultado.Count}"
                });
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al sincronizar items con price list", detalle });
            }
        }

        /// <summary>
        /// Sincroniza items en bloques de 100 hasta completar toda la sincronización
        /// </summary>
        private async Task<IActionResult> SincronizarEnBloques(ItemsBO itemsBO, ApplicationDBContext companyContext)
        {
            const int TAMANO_BLOQUE = 100;
            int totalProcesados = 0;
            int bloqueActual = 1;
            var resultados = new List<object>();

            try
            {
                // Limpiar la tabla local antes de comenzar
                var allLocal = companyContext.ItemsBCWithPriceList.ToList();
                if (allLocal.Any())
                {
                    companyContext.ItemsBCWithPriceList.RemoveRange(allLocal);
                    await companyContext.SaveChangesAsync();
                }

                while (true)
                {
                    // Procesar bloque actual
                    var bloqueResultado = await itemsBO.SincronizarItemsBCWithPriceListAsync(TAMANO_BLOQUE, false);
                    
                    if (bloqueResultado.Count == 0)
                    {
                        // No hay más items para procesar
                        break;
                    }

                    totalProcesados += bloqueResultado.Count;
                    
                    // Agregar información del bloque procesado
                    resultados.Add(new
                    {
                        bloque = bloqueActual,
                        itemsEnBloque = bloqueResultado.Count,
                        totalAcumulado = totalProcesados,
                        mensaje = $"Bloque {bloqueActual} procesado: {bloqueResultado.Count} items"
                    });

                    bloqueActual++;

                    // Si el bloque tiene menos de 100 items, significa que es el último
                    if (bloqueResultado.Count < TAMANO_BLOQUE)
                    {
                        break;
                    }
                }

                return Ok(new
                {
                    totalSincronizados = totalProcesados,
                    totalBloques = bloqueActual - 1,
                    mensaje = $"Sincronización completada exitosamente. Total de items procesados: {totalProcesados} en {bloqueActual - 1} bloques",
                    detalles = resultados
                });
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { 
                    mensaje = "Error durante la sincronización en bloques", 
                    detalle,
                    totalProcesados,
                    bloqueActual,
                    resultados
                });
            }
        }

        /// <summary>
        /// Sincroniza items con progreso en tiempo real usando Server-Sent Events
        /// </summary>
        [HttpPost("sincronizar-items-bc-pricelist-progreso")]
        public async Task SincronizarItemsBCWithPriceListProgreso([FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("El código de compañía es requerido");
                return;
            }

            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            try
            {
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var itemsBO = new ItemsBO(companyContext, bcConn);

                const int TAMANO_BLOQUE = 100;
                int totalProcesados = 0;
                int bloqueActual = 1;

                // Enviar mensaje de inicio
                await Response.WriteAsync($"data: {{\"tipo\":\"inicio\",\"mensaje\":\"Iniciando sincronización de items con price list...\"}}\n\n");
                await Response.Body.FlushAsync();

                // Limpiar la tabla local antes de comenzar
                var allLocal = companyContext.ItemsBCWithPriceList.ToList();
                if (allLocal.Any())
                {
                    await Response.WriteAsync($"data: {{\"tipo\":\"limpieza\",\"mensaje\":\"Limpiando tabla local...\"}}\n\n");
                    await Response.Body.FlushAsync();
                    
                    companyContext.ItemsBCWithPriceList.RemoveRange(allLocal);
                    await companyContext.SaveChangesAsync();
                    
                    await Response.WriteAsync($"data: {{\"tipo\":\"limpieza_completada\",\"mensaje\":\"Tabla local limpiada exitosamente\"}}\n\n");
                    await Response.Body.FlushAsync();
                }

                while (true)
                {
                    // Enviar progreso del bloque actual
                    await Response.WriteAsync($"data: {{\"tipo\":\"procesando_bloque\",\"bloque\":{bloqueActual},\"mensaje\":\"Procesando bloque {bloqueActual}...\"}}\n\n");
                    await Response.Body.FlushAsync();

                    // Procesar bloque actual
                    var bloqueResultado = await itemsBO.SincronizarItemsBCWithPriceListAsync(TAMANO_BLOQUE, false);
                    
                    if (bloqueResultado.Count == 0)
                    {
                        // No hay más items para procesar
                        await Response.WriteAsync($"data: {{\"tipo\":\"sin_mas_items\",\"mensaje\":\"No hay más items para procesar\"}}\n\n");
                        await Response.Body.FlushAsync();
                        break;
                    }

                    totalProcesados += bloqueResultado.Count;
                    
                    // Enviar resultado del bloque
                    await Response.WriteAsync($"data: {{\"tipo\":\"bloque_completado\",\"bloque\":{bloqueActual},\"itemsEnBloque\":{bloqueResultado.Count},\"totalAcumulado\":{totalProcesados},\"mensaje\":\"Bloque {bloqueActual} completado: {bloqueResultado.Count} items procesados\"}}\n\n");
                    await Response.Body.FlushAsync();

                    bloqueActual++;

                    // Si el bloque tiene menos de 100 items, significa que es el último
                    if (bloqueResultado.Count < TAMANO_BLOQUE)
                    {
                        await Response.WriteAsync($"data: {{\"tipo\":\"ultimo_bloque\",\"mensaje\":\"Último bloque procesado\"}}\n\n");
                        await Response.Body.FlushAsync();
                        break;
                    }
                }

                // Enviar mensaje de finalización
                await Response.WriteAsync($"data: {{\"tipo\":\"completado\",\"totalSincronizados\":{totalProcesados},\"totalBloques\":{bloqueActual - 1},\"mensaje\":\"Sincronización completada exitosamente. Total de items procesados: {totalProcesados} en {bloqueActual - 1} bloques\"}}\n\n");
                await Response.Body.FlushAsync();
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                await Response.WriteAsync($"data: {{\"tipo\":\"error\",\"mensaje\":\"Error durante la sincronización\",\"detalle\":\"{detalle}\"}}\n\n");
                await Response.Body.FlushAsync();
            }
        }

        [HttpGet("items-bc-pricelist-local")]
        public async Task<IActionResult> GetItemsBCWithPriceListLocal([FromQuery] string companyCode, [FromQuery] int? take = null)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                var items = companyContext.ItemsBCWithPriceList
                    .OrderBy(i => i.Id);
                if (take.HasValue && take.Value > 0)
                {
                    return Ok(await items.Take(take.Value).ToListAsync());
                }
                return Ok(await items.ToListAsync());
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al consultar items locales con price list", detalle });
            }
        }

        [HttpGet("items-bc-pricelist-descriptions")]
        public async Task<IActionResult> GetItemsBCWithPriceListDescriptions([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                var result = await companyContext.ItemsBCWithPriceList
                    .Select(i => new { i.Description, i.UnitMeasureCode })
                    .Distinct()
                    .OrderBy(i => i.Description)
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al consultar descripciones de items", detalle });
            }
        }
    }
} 