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
    public class CommercialConditionController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "commercialcondition";

        public CommercialConditionController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService,
            IOutputCacheStore outputCacheStore)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
            _outputCacheStore = outputCacheStore;
        }

        // GET: api/CommercialCondition
        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<CommercialCondition>>> GetAllCommercialConditions([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var commercialConditions = await companyContext.CommercialCondition
                    .ToListAsync();

                return Ok(commercialConditions);
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

        // GET: api/CommercialCondition/active
        [HttpGet("active")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<CommercialCondition>>> GetActiveCommercialConditions([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var commercialConditions = await companyContext.CommercialCondition
                    .Where(cc => cc.IsActive)
                    .ToListAsync();

                return Ok(commercialConditions);
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

        // GET: api/CommercialCondition/with-quotations-count
        [HttpGet("with-quotations-count")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<CommercialConditionResponseDTO>>> GetCommercialConditionsWithQuotationsCount([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var commercialConditionsWithCount = await companyContext.CommercialCondition
                    .Select(cc => new CommercialConditionResponseDTO
                    {
                        Id = cc.Id,
                        Description = cc.Description,
                        CommercialText = cc.CommercialText,
                        IsActive = cc.IsActive,
                        QuotationsCount = companyContext.QuotationMaster.Count(q => q.FK_CommercialConditionId == cc.Id)
                    })
                    .ToListAsync();

                return Ok(commercialConditionsWithCount);
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

        // GET: api/CommercialCondition/{id}
        [HttpGet("{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<CommercialCondition>> GetCommercialConditionById(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var commercialCondition = await companyContext.CommercialCondition.FindAsync(id);
                if (commercialCondition == null)
                {
                    return NotFound($"No se encontró la condición comercial con ID {id}");
                }

                return Ok(commercialCondition);
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

        // GET: api/CommercialCondition/by-description/{description}
        [HttpGet("by-description/{description}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<CommercialCondition>>> GetCommercialConditionsByDescription(string description, [FromQuery] string companyCode)
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
                
                var commercialConditions = await companyContext.CommercialCondition
                    .Where(cc => cc.Description != null && cc.Description.Contains(description))
                    .ToListAsync();

                return Ok(commercialConditions);
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

        // GET: api/CommercialCondition/by-commercial-text/{commercialText}
        [HttpGet("by-commercial-text/{commercialText}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<CommercialCondition>>> GetCommercialConditionsByCommercialText(string commercialText, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (string.IsNullOrEmpty(commercialText))
                {
                    return BadRequest("El texto comercial es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var commercialConditions = await companyContext.CommercialCondition
                    .Where(cc => cc.CommercialText != null && cc.CommercialText.Contains(commercialText))
                    .ToListAsync();

                return Ok(commercialConditions);
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

        // POST: api/CommercialCondition
        [HttpPost]
        public async Task<ActionResult<CommercialCondition>> CreateCommercialCondition([FromBody] CommercialConditionCreateDTO commercialConditionDto, [FromQuery] string companyCode)
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
                
                // Verificar si ya existe una condición comercial con la misma descripción
                var existingCommercialCondition = await companyContext.CommercialCondition
                    .FirstOrDefaultAsync(cc => cc.Description == commercialConditionDto.Description);
                
                if (existingCommercialCondition != null)
                {
                    return BadRequest($"Ya existe una condición comercial con la descripción '{commercialConditionDto.Description}'");
                }

                // Mapear DTO a entidad
                var commercialCondition = new CommercialCondition
                {
                    Description = commercialConditionDto.Description,
                    CommercialText = commercialConditionDto.CommercialText,
                    IsActive = commercialConditionDto.IsActive
                };
                
                companyContext.CommercialCondition.Add(commercialCondition);
                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return CreatedAtAction(nameof(GetCommercialConditionById), new { id = commercialCondition.Id, companyCode }, commercialCondition);
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

        // PUT: api/CommercialCondition/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCommercialCondition(int id, [FromBody] CommercialConditionUpdateDTO commercialConditionDto, [FromQuery] string companyCode)
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
                
                var existingCommercialCondition = await companyContext.CommercialCondition.FindAsync(id);
                if (existingCommercialCondition == null)
                {
                    return NotFound($"No se encontró la condición comercial con ID {id}");
                }

                // Verificar si ya existe otra condición comercial con la misma descripción (excluyendo la actual)
                var duplicateCommercialCondition = await companyContext.CommercialCondition
                    .FirstOrDefaultAsync(cc => cc.Description == commercialConditionDto.Description && cc.Id != id);
                
                if (duplicateCommercialCondition != null)
                {
                    return BadRequest($"Ya existe otra condición comercial con la descripción '{commercialConditionDto.Description}'");
                }

                // Actualizar propiedades
                existingCommercialCondition.Description = commercialConditionDto.Description;
                existingCommercialCondition.CommercialText = commercialConditionDto.CommercialText;
                existingCommercialCondition.IsActive = commercialConditionDto.IsActive;

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

        // PATCH: api/CommercialCondition/{id}/toggle-status
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleCommercialConditionStatus(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var commercialCondition = await companyContext.CommercialCondition.FindAsync(id);
                if (commercialCondition == null)
                {
                    return NotFound($"No se encontró la condición comercial con ID {id}");
                }

                // Cambiar el estado
                commercialCondition.IsActive = !commercialCondition.IsActive;

                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return Ok(new { id = commercialCondition.Id, isActive = commercialCondition.IsActive });
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

        // DELETE: api/CommercialCondition/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommercialCondition(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var commercialCondition = await companyContext.CommercialCondition.FindAsync(id);
                if (commercialCondition == null)
                {
                    return NotFound($"No se encontró la condición comercial con ID {id}");
                }

                // Verificar si hay cotizaciones asociadas
                var hasQuotations = await companyContext.QuotationMaster.AnyAsync(q => q.FK_CommercialConditionId == id);
                if (hasQuotations)
                {
                    return BadRequest("No se puede eliminar la condición comercial porque tiene cotizaciones asociadas. Desactive la condición comercial en su lugar.");
                }

                companyContext.CommercialCondition.Remove(commercialCondition);
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

        // GET: api/CommercialCondition/{id}/quotations
        [HttpGet("{id}/quotations")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<object>>> GetCommercialConditionQuotations(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Verificar que la condición comercial existe
                var commercialCondition = await companyContext.CommercialCondition.FindAsync(id);
                if (commercialCondition == null)
                {
                    return NotFound($"No se encontró la condición comercial con ID {id}");
                }

                var quotations = await companyContext.QuotationMaster
                    .Where(q => q.FK_CommercialConditionId == id)
                    .Select(q => new
                    {
                        q.Id,
                        q.IdCustomer,
                        q.CreationDateTime,
                        q.DueDate,
                        q.FK_idEmployee,
                        q.TotalizingQuotation,
                        q.EquipmentRemains,
                        q.MonthlyConsumption,
                        q.PaymentTerm
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

        // GET: api/CommercialCondition/statistics
        [HttpGet("statistics")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<object>> GetCommercialConditionStatistics([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var statistics = await companyContext.CommercialCondition
                    .Select(cc => new
                    {
                        cc.Id,
                        cc.Description,
                        cc.CommercialText,
                        cc.IsActive,
                        QuotationsCount = companyContext.QuotationMaster.Count(q => q.FK_CommercialConditionId == cc.Id),
                        TotalizingQuotationsCount = companyContext.QuotationMaster.Count(q => q.FK_CommercialConditionId == cc.Id && q.TotalizingQuotation == true),
                        EquipmentRemainsCount = companyContext.QuotationMaster.Count(q => q.FK_CommercialConditionId == cc.Id && q.EquipmentRemains == true),
                        AveragePaymentTerm = companyContext.QuotationMaster
                            .Where(q => q.FK_CommercialConditionId == cc.Id && q.PaymentTerm.HasValue)
                            .Average(q => q.PaymentTerm)
                    })
                    .ToListAsync();

                var summary = new
                {
                    TotalCommercialConditions = statistics.Count,
                    ActiveCommercialConditions = statistics.Count(s => s.IsActive),
                    TotalQuotations = statistics.Sum(s => s.QuotationsCount),
                    TotalizingQuotations = statistics.Sum(s => s.TotalizingQuotationsCount),
                    EquipmentRemainsQuotations = statistics.Sum(s => s.EquipmentRemainsCount),
                    AveragePaymentTerm = statistics
                        .Where(s => s.AveragePaymentTerm.HasValue)
                        .Average(s => s.AveragePaymentTerm),
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

        // GET: api/CommercialCondition/VerificarConfiguracionCompania
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
                    HasCommercialCondition = companyContext.CommercialCondition != null,
                    TotalCommercialConditions = await companyContext.CommercialCondition.CountAsync(),
                    ActiveCommercialConditions = await companyContext.CommercialCondition.CountAsync(cc => cc.IsActive),
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

        private async Task<bool> CommercialConditionExists(int id, ApplicationDBContext context)
        {
            return await context.CommercialCondition.AnyAsync(cc => cc.Id == id);
        }
    }
} 