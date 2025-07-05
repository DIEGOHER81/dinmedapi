using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using DimmedAPI.DTOs;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuotationTypeController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "quotationtype";

        public QuotationTypeController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService,
            IOutputCacheStore outputCacheStore)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
            _outputCacheStore = outputCacheStore;
        }

        // GET: api/QuotationType
        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<QuotationType>>> GetAllQuotationTypes([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotationTypes = await companyContext.QuotationType
                    .ToListAsync();

                return Ok(quotationTypes);
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

        // GET: api/QuotationType/active
        [HttpGet("active")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<QuotationType>>> GetActiveQuotationTypes([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotationTypes = await companyContext.QuotationType
                    .Where(qt => qt.IsActive)
                    .ToListAsync();

                return Ok(quotationTypes);
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

        // GET: api/QuotationType/with-quotations-count
        [HttpGet("with-quotations-count")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<QuotationTypeResponseDTO>>> GetQuotationTypesWithQuotationsCount([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotationTypesWithCount = await companyContext.QuotationType
                    .Select(qt => new QuotationTypeResponseDTO
                    {
                        Id = qt.Id,
                        Description = qt.Description,
                        IsActive = qt.IsActive,
                        QuotationsCount = companyContext.QuotationMaster.Count(q => q.FK_QuotationTypeId == qt.Id)
                    })
                    .ToListAsync();

                return Ok(quotationTypesWithCount);
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

        // GET: api/QuotationType/{id}
        [HttpGet("{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<QuotationType>> GetQuotationTypeById(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotationType = await companyContext.QuotationType.FindAsync(id);
                if (quotationType == null)
                {
                    return NotFound($"No se encontró el tipo de cotización con ID {id}");
                }

                return Ok(quotationType);
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

        // GET: api/QuotationType/by-description/{description}
        [HttpGet("by-description/{description}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<QuotationType>>> GetQuotationTypesByDescription(string description, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (string.IsNullOrEmpty(description))
                {
                    return BadRequest("La descripción es requerida");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotationTypes = await companyContext.QuotationType
                    .Where(qt => qt.Description != null && qt.Description.Contains(description))
                    .ToListAsync();

                return Ok(quotationTypes);
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

        // POST: api/QuotationType
        [HttpPost]
        public async Task<ActionResult<QuotationType>> CreateQuotationType([FromBody] QuotationTypeCreateDTO quotationTypeDto, [FromQuery] string companyCode)
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
                
                // Verificar si ya existe un tipo de cotización con la misma descripción
                var existingQuotationType = await companyContext.QuotationType
                    .FirstOrDefaultAsync(qt => qt.Description == quotationTypeDto.Description);
                
                if (existingQuotationType != null)
                {
                    return BadRequest($"Ya existe un tipo de cotización con la descripción '{quotationTypeDto.Description}'");
                }

                // Mapear DTO a entidad
                var quotationType = new QuotationType
                {
                    Description = quotationTypeDto.Description,
                    IsActive = quotationTypeDto.IsActive
                };
                
                companyContext.QuotationType.Add(quotationType);
                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return CreatedAtAction(nameof(GetQuotationTypeById), new { id = quotationType.Id, companyCode }, quotationType);
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

        // PUT: api/QuotationType/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuotationType(int id, [FromBody] QuotationTypeUpdateDTO quotationTypeDto, [FromQuery] string companyCode)
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
                
                var existingQuotationType = await companyContext.QuotationType.FindAsync(id);
                if (existingQuotationType == null)
                {
                    return NotFound($"No se encontró el tipo de cotización con ID {id}");
                }

                // Verificar si ya existe otro tipo de cotización con la misma descripción (excluyendo el actual)
                var duplicateQuotationType = await companyContext.QuotationType
                    .FirstOrDefaultAsync(qt => qt.Description == quotationTypeDto.Description && qt.Id != id);
                
                if (duplicateQuotationType != null)
                {
                    return BadRequest($"Ya existe otro tipo de cotización con la descripción '{quotationTypeDto.Description}'");
                }

                // Actualizar propiedades
                existingQuotationType.Description = quotationTypeDto.Description;
                existingQuotationType.IsActive = quotationTypeDto.IsActive;

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

        // PATCH: api/QuotationType/{id}/toggle-status
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleQuotationTypeStatus(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotationType = await companyContext.QuotationType.FindAsync(id);
                if (quotationType == null)
                {
                    return NotFound($"No se encontró el tipo de cotización con ID {id}");
                }

                // Cambiar el estado
                quotationType.IsActive = !quotationType.IsActive;

                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return Ok(new { id = quotationType.Id, isActive = quotationType.IsActive });
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

        // DELETE: api/QuotationType/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuotationType(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotationType = await companyContext.QuotationType.FindAsync(id);
                if (quotationType == null)
                {
                    return NotFound($"No se encontró el tipo de cotización con ID {id}");
                }

                // Verificar si hay cotizaciones asociadas
                var hasQuotations = await companyContext.QuotationMaster.AnyAsync(q => q.FK_QuotationTypeId == id);
                if (hasQuotations)
                {
                    return BadRequest("No se puede eliminar el tipo de cotización porque tiene cotizaciones asociadas. Desactive el tipo de cotización en su lugar.");
                }

                companyContext.QuotationType.Remove(quotationType);
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

        // GET: api/QuotationType/{id}/quotations
        [HttpGet("{id}/quotations")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<object>>> GetQuotationTypeQuotations(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Verificar que el tipo de cotización existe
                var quotationType = await companyContext.QuotationType.FindAsync(id);
                if (quotationType == null)
                {
                    return NotFound($"No se encontró el tipo de cotización con ID {id}");
                }

                var quotations = await companyContext.QuotationMaster
                    .Where(q => q.FK_QuotationTypeId == id)
                    .Select(q => new
                    {
                        q.Id,
                        q.IdCustomer,
                        q.CreationDateTime,
                        q.DueDate,
                        q.FK_idEmployee,
                        q.TotalizingQuotation,
                        q.EquipmentRemains,
                        q.MonthlyConsumption
                    })
                    .ToListAsync();

                return Ok(quotations);
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

        // GET: api/QuotationType/statistics
        [HttpGet("statistics")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<object>> GetQuotationTypeStatistics([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var statistics = await companyContext.QuotationType
                    .Select(qt => new
                    {
                        qt.Id,
                        qt.Description,
                        qt.IsActive,
                        QuotationsCount = companyContext.QuotationMaster.Count(q => q.FK_QuotationTypeId == qt.Id),
                        TotalizingQuotationsCount = companyContext.QuotationMaster.Count(q => q.FK_QuotationTypeId == qt.Id && q.TotalizingQuotation == true),
                        EquipmentRemainsCount = companyContext.QuotationMaster.Count(q => q.FK_QuotationTypeId == qt.Id && q.EquipmentRemains == true)
                    })
                    .ToListAsync();

                var summary = new
                {
                    TotalQuotationTypes = statistics.Count,
                    ActiveQuotationTypes = statistics.Count(s => s.IsActive),
                    TotalQuotations = statistics.Sum(s => s.QuotationsCount),
                    TotalizingQuotations = statistics.Sum(s => s.TotalizingQuotationsCount),
                    EquipmentRemainsQuotations = statistics.Sum(s => s.EquipmentRemainsCount),
                    Details = statistics
                };

                return Ok(summary);
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

        // GET: api/QuotationType/VerificarConfiguracionCompania
        [HttpGet("VerificarConfiguracionCompania")]
        public async Task<ActionResult<object>> VerificarConfiguracionCompania([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var result = new
                {
                    CompanyCode = companyCode,
                    HasQuotationType = companyContext.QuotationType != null,
                    TotalQuotationTypes = await companyContext.QuotationType.CountAsync(),
                    ActiveQuotationTypes = await companyContext.QuotationType.CountAsync(qt => qt.IsActive),
                    Message = "Configuración verificada correctamente"
                };

                return Ok(result);
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

        private async Task<bool> QuotationTypeExists(int id, ApplicationDBContext context)
        {
            return await context.QuotationType.AnyAsync(qt => qt.Id == id);
        }
    }
} 