using Microsoft.AspNetCore.Mvc;
using DimmedAPI.BO;
using DimmedAPI.DTOs;
using Microsoft.AspNetCore.OutputCaching;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntryRequestTraceController : ControllerBase
    {
        private readonly EntryRequestTraceBO _entryRequestTraceBO;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "entryrequesttrace";

        public EntryRequestTraceController(
            EntryRequestTraceBO entryRequestTraceBO,
            IOutputCacheStore outputCacheStore)
        {
            _entryRequestTraceBO = entryRequestTraceBO;
            _outputCacheStore = outputCacheStore;
        }

        /// <summary>
        /// Obtiene el trace de un EntryRequest usando el procedimiento almacenado GET_TRACE_RQ_2
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="rqId">ID del EntryRequest (opcional)</param>
        /// <param name="branchId">ID de la sucursal (opcional)</param>
        /// <param name="dateIni">Fecha inicial (opcional)</param>
        /// <param name="dateEnd">Fecha final (opcional)</param>
        /// <returns>Lista de traces del EntryRequest</returns>
        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<EntryRequestTraceDTO>>> GetEntryRequestTrace(
            [FromQuery] string companyCode,
            [FromQuery] int? rqId = null,
            [FromQuery] int? branchId = null,
            [FromQuery] DateTime? dateIni = null,
            [FromQuery] DateTime? dateEnd = null)
        {
            try
            {
                Console.WriteLine($"=== INICIO GetEntryRequestTrace ===");
                Console.WriteLine($"CompanyCode: {companyCode}, RQID: {rqId}, BranchId: {branchId}, DateIni: {dateIni}, DateEnd: {dateEnd}");

                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                if (rqId.HasValue && rqId.Value <= 0)
                {
                    Console.WriteLine("Error: RQID debe ser mayor a 0");
                    return BadRequest("El RQID debe ser mayor a 0");
                }

                var traces = await _entryRequestTraceBO.GetEntryRequestTraceAsync(
                    companyCode, rqId, branchId, dateIni, dateEnd);

                Console.WriteLine($"Total de traces encontrados: {traces.Count}");
                Console.WriteLine("=== FIN GetEntryRequestTrace ===");

                return Ok(traces);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error de validación: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error interno del servidor: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene el trace de un EntryRequest usando un DTO de filtro
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="filter">Filtro con los parámetros</param>
        /// <returns>Lista de traces del EntryRequest</returns>
        [HttpPost("filter")]
        public async Task<ActionResult<IEnumerable<EntryRequestTraceDTO>>> GetEntryRequestTraceWithFilter(
            [FromQuery] string companyCode,
            [FromBody] EntryRequestTraceFilterDTO filter)
        {
            try
            {
                Console.WriteLine($"=== INICIO GetEntryRequestTraceWithFilter ===");
                Console.WriteLine($"CompanyCode: {companyCode}");
                Console.WriteLine($"Filter - RQID: {filter?.RQID}, BranchId: {filter?.BranchId}, DateIni: {filter?.DateIni}, DateEnd: {filter?.DateEnd}");

                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                if (filter == null)
                {
                    Console.WriteLine("Error: Filter es nulo");
                    return BadRequest("El filtro es requerido");
                }

                if (filter.RQID.HasValue && filter.RQID.Value <= 0)
                {
                    Console.WriteLine("Error: RQID debe ser mayor a 0");
                    return BadRequest("El RQID debe ser mayor a 0");
                }

                var traces = await _entryRequestTraceBO.GetEntryRequestTraceWithFilterAsync(companyCode, filter);

                Console.WriteLine($"Total de traces encontrados: {traces.Count}");
                Console.WriteLine("=== FIN GetEntryRequestTraceWithFilter ===");

                return Ok(traces);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error de validación: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error interno del servidor: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Limpia el cache del trace de EntryRequest
        /// </summary>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("cache")]
        public async Task<IActionResult> ClearCache()
        {
            try
            {
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);
                return Ok("Cache limpiado exitosamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al limpiar cache: {ex.Message}");
                return StatusCode(500, $"Error al limpiar cache: {ex.Message}");
            }
        }
    }
} 