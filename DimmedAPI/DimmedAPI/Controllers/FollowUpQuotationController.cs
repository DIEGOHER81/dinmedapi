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
    public class FollowUpQuotationController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "followupquotation";

        public FollowUpQuotationController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService,
            IOutputCacheStore outputCacheStore)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
            _outputCacheStore = outputCacheStore;
        }

        // GET: api/FollowUpQuotation
        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<FollowUpQuotations>>> GetAllFollowUpQuotations([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var followUpQuotations = await companyContext.FollowUpQuotations
                    .Include(f => f.Quotation)
                    .Include(f => f.Employee)
                    .OrderByDescending(f => f.CreateDateTime)
                    .ToListAsync();

                return Ok(followUpQuotations);
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

        // GET: api/FollowUpQuotation/{id}
        [HttpGet("{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<FollowUpQuotations>> GetFollowUpQuotationById(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var followUpQuotation = await companyContext.FollowUpQuotations
                    .Include(f => f.Quotation)
                    .Include(f => f.Employee)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (followUpQuotation == null)
                {
                    return NotFound($"No se encontró el seguimiento de cotización con ID {id}");
                }

                return Ok(followUpQuotation);
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

        // GET: api/FollowUpQuotation/by-quotation/{quotationId}
        [HttpGet("by-quotation/{quotationId}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<FollowUpQuotations>>> GetFollowUpQuotationsByQuotation(int quotationId, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var followUpQuotations = await companyContext.FollowUpQuotations
                    .Include(f => f.Quotation)
                    .Include(f => f.Employee)
                    .Where(f => f.Fk_IdQuotation == quotationId)
                    .OrderByDescending(f => f.CreateDateTime)
                    .ToListAsync();

                return Ok(followUpQuotations);
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

        // GET: api/FollowUpQuotation/by-employee/{employeeId}
        [HttpGet("by-employee/{employeeId}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<FollowUpQuotations>>> GetFollowUpQuotationsByEmployee(int employeeId, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var followUpQuotations = await companyContext.FollowUpQuotations
                    .Include(f => f.Quotation)
                    .Include(f => f.Employee)
                    .Where(f => f.Fk_IdEmployee == employeeId)
                    .OrderByDescending(f => f.CreateDateTime)
                    .ToListAsync();

                return Ok(followUpQuotations);
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

        // POST: api/FollowUpQuotation
        [HttpPost]
        public async Task<ActionResult<FollowUpQuotations>> CreateFollowUpQuotation([FromBody] FollowUpQuotationCreateDTO followUpQuotationDto, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);

                // Verificar que la cotización existe
                var quotationExists = await companyContext.QuotationMaster.AnyAsync(q => q.Id == followUpQuotationDto.Fk_IdQuotation);
                if (!quotationExists)
                {
                    return BadRequest($"No se encontró la cotización con ID {followUpQuotationDto.Fk_IdQuotation}");
                }

                // Verificar que el empleado existe
                var employeeExists = await companyContext.Employee.AnyAsync(e => e.Id == followUpQuotationDto.Fk_IdEmployee);
                if (!employeeExists)
                {
                    return BadRequest($"No se encontró el empleado con ID {followUpQuotationDto.Fk_IdEmployee}");
                }

                var followUpQuotation = new FollowUpQuotations
                {
                    Fk_IdQuotation = followUpQuotationDto.Fk_IdQuotation,
                    Fk_IdEmployee = followUpQuotationDto.Fk_IdEmployee,
                    idconceptoseguimiento = followUpQuotationDto.idconceptoseguimiento,
                    Observation = followUpQuotationDto.Observation,
                    CreateDateTime = DateTime.Now
                };

                companyContext.FollowUpQuotations.Add(followUpQuotation);
                await companyContext.SaveChangesAsync();

                // Invalidar caché
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return CreatedAtAction(nameof(GetFollowUpQuotationById), new { id = followUpQuotation.Id, companyCode }, followUpQuotation);
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

        // PUT: api/FollowUpQuotation/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFollowUpQuotation(int id, [FromBody] FollowUpQuotationUpdateDTO followUpQuotationDto, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);

                var followUpQuotation = await companyContext.FollowUpQuotations.FindAsync(id);
                if (followUpQuotation == null)
                {
                    return NotFound($"No se encontró el seguimiento de cotización con ID {id}");
                }

                // Verificar que la cotización existe
                var quotationExists = await companyContext.QuotationMaster.AnyAsync(q => q.Id == followUpQuotationDto.Fk_IdQuotation);
                if (!quotationExists)
                {
                    return BadRequest($"No se encontró la cotización con ID {followUpQuotationDto.Fk_IdQuotation}");
                }

                // Verificar que el empleado existe
                var employeeExists = await companyContext.Employee.AnyAsync(e => e.Id == followUpQuotationDto.Fk_IdEmployee);
                if (!employeeExists)
                {
                    return BadRequest($"No se encontró el empleado con ID {followUpQuotationDto.Fk_IdEmployee}");
                }

                followUpQuotation.Fk_IdQuotation = followUpQuotationDto.Fk_IdQuotation;
                followUpQuotation.Fk_IdEmployee = followUpQuotationDto.Fk_IdEmployee;
                followUpQuotation.idconceptoseguimiento = followUpQuotationDto.idconceptoseguimiento;
                followUpQuotation.Observation = followUpQuotationDto.Observation;

                await companyContext.SaveChangesAsync();

                // Invalidar caché
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

        // DELETE: api/FollowUpQuotation/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFollowUpQuotation(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);

                var followUpQuotation = await companyContext.FollowUpQuotations.FindAsync(id);
                if (followUpQuotation == null)
                {
                    return NotFound($"No se encontró el seguimiento de cotización con ID {id}");
                }

                companyContext.FollowUpQuotations.Remove(followUpQuotation);
                await companyContext.SaveChangesAsync();

                // Invalidar caché
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

        // GET: api/FollowUpQuotation/with-details/{id}
        [HttpGet("with-details/{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<FollowUpQuotationResponseDTO>> GetFollowUpQuotationWithDetails(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var followUpQuotation = await companyContext.FollowUpQuotations
                    .Include(f => f.Quotation)
                    .Include(f => f.Employee)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (followUpQuotation == null)
                {
                    return NotFound($"No se encontró el seguimiento de cotización con ID {id}");
                }

                var responseDto = new FollowUpQuotationResponseDTO
                {
                    Id = followUpQuotation.Id,
                    Fk_IdQuotation = followUpQuotation.Fk_IdQuotation,
                    Fk_IdEmployee = followUpQuotation.Fk_IdEmployee,
                    idconceptoseguimiento = followUpQuotation.idconceptoseguimiento,
                    Observation = followUpQuotation.Observation,
                    CreateDateTime = followUpQuotation.CreateDateTime,
                    Quotation = followUpQuotation.Quotation != null ? new QuotationMasterInfo
                    {
                        Id = followUpQuotation.Quotation.Id,
                        FK_idBranch = followUpQuotation.Quotation.FK_idBranch,
                        CustomerOrigin = followUpQuotation.Quotation.CustomerOrigin,
                        FK_idCustomerType = followUpQuotation.Quotation.FK_idCustomerType,
                        IdCustomer = followUpQuotation.Quotation.IdCustomer,
                        CreationDateTime = followUpQuotation.Quotation.CreationDateTime,
                        DueDate = followUpQuotation.Quotation.DueDate,
                        FK_idEmployee = followUpQuotation.Quotation.FK_idEmployee,
                        FK_QuotationTypeId = followUpQuotation.Quotation.FK_QuotationTypeId,
                        PaymentTerm = followUpQuotation.Quotation.PaymentTerm,
                        FK_CommercialConditionId = followUpQuotation.Quotation.FK_CommercialConditionId,
                        TotalizingQuotation = followUpQuotation.Quotation.TotalizingQuotation,
                        Total = followUpQuotation.Quotation.Total,
                        EquipmentRemains = followUpQuotation.Quotation.EquipmentRemains,
                        MonthlyConsumption = followUpQuotation.Quotation.MonthlyConsumption
                    } : null,
                    Employee = followUpQuotation.Employee != null ? new FollowUpEmployeeInfo
                    {
                        Id = followUpQuotation.Employee.Id,
                        Code = followUpQuotation.Employee.Code,
                        Name = followUpQuotation.Employee.Name,
                        Charge = followUpQuotation.Employee.Charge,
                        Phone = followUpQuotation.Employee.Phone,
                        Email = followUpQuotation.Employee.Email
                    } : null
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
    }
} 