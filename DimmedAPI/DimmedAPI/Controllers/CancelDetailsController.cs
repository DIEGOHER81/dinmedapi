using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using DimmedAPI.DTOs;

namespace DimmedAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancelDetailsController : ControllerBase
    {
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private const string cacheTag = "CancelDetails";

        public CancelDetailsController(
            IOutputCacheStore outputCacheStore, 
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService)
        {
            this._outputCacheStore = outputCacheStore;
            this.context = context;
            this._dynamicConnectionService = dynamicConnectionService;
        }

        [HttpGet("ObtenerDetallesdeCancelacion")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<CancelDetails>>> GetCancelDetails([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var details = await companyContext.CancelDetails
                    .Include(c => c.CancellationReason)
                    .ToListAsync();

                return Ok(details);
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

        // GET: api/CancelDetails/{id}
        [HttpGet("{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<CancelDetailsResponseDTO>> GetCancelDetailById(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var cancelDetail = await companyContext.CancelDetails
                    .Include(c => c.CancellationReason)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cancelDetail == null)
                {
                    return NotFound($"No se encontró el detalle de cancelación con ID {id}");
                }

                var responseDto = new CancelDetailsResponseDTO
                {
                    Id = cancelDetail.Id,
                    Description = cancelDetail.Description,
                    IdRelation = cancelDetail.IdRelation,
                    CancellationReasonDescription = cancelDetail.CancellationReasonDescription
                };

                return Ok(responseDto);
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

        // POST: api/CancelDetails
        [HttpPost]
        public async Task<ActionResult<CancelDetailsResponseDTO>> CreateCancelDetail([FromBody] CancelDetailsCreateDTO createDto, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Verificar si ya existe un detalle de cancelación con la misma descripción
                var existingCancelDetail = await companyContext.CancelDetails
                    .FirstOrDefaultAsync(cd => cd.Description == createDto.Description);
                
                if (existingCancelDetail != null)
                {
                    return BadRequest($"Ya existe un detalle de cancelación con la descripción '{createDto.Description}'");
                }

                // Si se proporciona IdRelation, verificar que existe la razón de cancelación
                if (createDto.IdRelation.HasValue)
                {
                    var cancellationReason = await companyContext.CancellationReasons
                        .FirstOrDefaultAsync(cr => cr.Id == createDto.IdRelation.Value);
                    
                    if (cancellationReason == null)
                    {
                        return BadRequest($"No se encontró la razón de cancelación con ID {createDto.IdRelation.Value}");
                    }
                }

                // Mapear DTO a entidad
                var cancelDetail = new CancelDetails
                {
                    Description = createDto.Description,
                    IdRelation = createDto.IdRelation
                };
                
                companyContext.CancelDetails.Add(cancelDetail);
                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                // Crear respuesta
                var responseDto = new CancelDetailsResponseDTO
                {
                    Id = cancelDetail.Id,
                    Description = cancelDetail.Description,
                    IdRelation = cancelDetail.IdRelation,
                    CancellationReasonDescription = cancelDetail.CancellationReasonDescription
                };

                return CreatedAtAction(nameof(GetCancelDetailById), new { id = cancelDetail.Id, companyCode }, responseDto);
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

        // PUT: api/CancelDetails/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCancelDetail(int id, [FromBody] CancelDetailsUpdateDTO updateDto, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var existingCancelDetail = await companyContext.CancelDetails.FindAsync(id);
                if (existingCancelDetail == null)
                {
                    return NotFound($"No se encontró el detalle de cancelación con ID {id}");
                }

                // Verificar si ya existe otro detalle de cancelación con la misma descripción (excluyendo el actual)
                var duplicateCancelDetail = await companyContext.CancelDetails
                    .FirstOrDefaultAsync(cd => cd.Description == updateDto.Description && cd.Id != id);
                
                if (duplicateCancelDetail != null)
                {
                    return BadRequest($"Ya existe otro detalle de cancelación con la descripción '{updateDto.Description}'");
                }

                // Si se proporciona IdRelation, verificar que existe la razón de cancelación
                if (updateDto.IdRelation.HasValue)
                {
                    var cancellationReason = await companyContext.CancellationReasons
                        .FirstOrDefaultAsync(cr => cr.Id == updateDto.IdRelation.Value);
                    
                    if (cancellationReason == null)
                    {
                        return BadRequest($"No se encontró la razón de cancelación con ID {updateDto.IdRelation.Value}");
                    }
                }

                // Actualizar propiedades
                existingCancelDetail.Description = updateDto.Description;
                existingCancelDetail.IdRelation = updateDto.IdRelation;

                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return NoContent();
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

        // DELETE: api/CancelDetails/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCancelDetail(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var cancelDetail = await companyContext.CancelDetails.FindAsync(id);
                if (cancelDetail == null)
                {
                    return NotFound($"No se encontró el detalle de cancelación con ID {id}");
                }

                // Verificar si hay solicitudes de entrada asociadas
                var hasEntryRequests = await companyContext.EntryRequests.AnyAsync(er => er.IdCancelDetail == id);
                if (hasEntryRequests)
                {
                    return BadRequest("No se puede eliminar el detalle de cancelación porque tiene solicitudes de entrada asociadas.");
                }

                companyContext.CancelDetails.Remove(cancelDetail);
                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return NoContent();
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
