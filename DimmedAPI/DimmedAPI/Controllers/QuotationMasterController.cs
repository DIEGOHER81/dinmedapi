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
    public class QuotationMasterController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "quotationmaster";

        public QuotationMasterController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService,
            IOutputCacheStore outputCacheStore)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
            _outputCacheStore = outputCacheStore;
        }

        // GET: api/QuotationMaster
        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<QuotationMaster>>> GetAllQuotations([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotations = await companyContext.QuotationMaster
                    .Include(q => q.Branch)
                    .Include(q => q.CustomerType)
                    .Include(q => q.Employee)
                    .Include(q => q.QuotationType)
                    .Include(q => q.CommercialCondition)
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

        // GET: api/QuotationMaster/{id}
        [HttpGet("{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<QuotationMaster>> GetQuotationById(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotation = await companyContext.QuotationMaster
                    .Include(q => q.Branch)
                    .Include(q => q.CustomerType)
                    .Include(q => q.Employee)
                    .Include(q => q.QuotationType)
                    .Include(q => q.CommercialCondition)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (quotation == null)
                {
                    return NotFound($"No se encontró la cotización con ID {id}");
                }

                return Ok(quotation);
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

        // GET: api/QuotationMaster/by-customer/{customerId}
        [HttpGet("by-customer/{customerId}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<QuotationMaster>>> GetQuotationsByCustomer(int customerId, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotations = await companyContext.QuotationMaster
                    .Include(q => q.Branch)
                    .Include(q => q.CustomerType)
                    .Include(q => q.Employee)
                    .Include(q => q.QuotationType)
                    .Include(q => q.CommercialCondition)
                    .Where(q => q.IdCustomer == customerId)
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

        // GET: api/QuotationMaster/by-branch/{branchId}
        [HttpGet("by-branch/{branchId}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<QuotationMaster>>> GetQuotationsByBranch(int branchId, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotations = await companyContext.QuotationMaster
                    .Include(q => q.Branch)
                    .Include(q => q.CustomerType)
                    .Include(q => q.Employee)
                    .Include(q => q.QuotationType)
                    .Include(q => q.CommercialCondition)
                    .Where(q => q.FK_idBranch == branchId)
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

        // GET: api/QuotationMaster/by-employee/{employeeId}
        [HttpGet("by-employee/{employeeId}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<QuotationMaster>>> GetQuotationsByEmployee(int employeeId, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotations = await companyContext.QuotationMaster
                    .Include(q => q.Branch)
                    .Include(q => q.CustomerType)
                    .Include(q => q.Employee)
                    .Include(q => q.QuotationType)
                    .Include(q => q.CommercialCondition)
                    .Where(q => q.FK_idEmployee == employeeId)
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

        // POST: api/QuotationMaster
        [HttpPost]
        public async Task<ActionResult<QuotationMaster>> CreateQuotation([FromBody] QuotationMasterCreateDTO quotationDto, [FromQuery] string companyCode)
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
                
                // Mapear DTO a entidad
                var quotation = new QuotationMaster
                {
                    FK_idBranch = quotationDto.FK_idBranch,
                    CustomerOrigin = quotationDto.CustomerOrigin,
                    FK_idCustomerType = quotationDto.FK_idCustomerType,
                    IdCustomer = quotationDto.IdCustomer,
                    CreationDateTime = DateTime.Now,
                    DueDate = quotationDto.DueDate,
                    FK_idEmployee = quotationDto.FK_idEmployee,
                    FK_QuotationTypeId = quotationDto.FK_QuotationTypeId,
                    PaymentTerm = quotationDto.PaymentTerm,
                    FK_CommercialConditionId = quotationDto.FK_CommercialConditionId,
                    TotalizingQuotation = quotationDto.TotalizingQuotation,
                    EquipmentRemains = quotationDto.EquipmentRemains,
                    MonthlyConsumption = quotationDto.MonthlyConsumption
                };
                
                companyContext.QuotationMaster.Add(quotation);
                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return CreatedAtAction(nameof(GetQuotationById), new { id = quotation.Id, companyCode }, quotation);
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

        // PUT: api/QuotationMaster/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuotation(int id, [FromBody] QuotationMasterUpdateDTO quotationDto, [FromQuery] string companyCode)
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
                
                var existingQuotation = await companyContext.QuotationMaster.FindAsync(id);
                if (existingQuotation == null)
                {
                    return NotFound($"No se encontró la cotización con ID {id}");
                }

                // Actualizar propiedades
                existingQuotation.FK_idBranch = quotationDto.FK_idBranch;
                existingQuotation.CustomerOrigin = quotationDto.CustomerOrigin;
                existingQuotation.FK_idCustomerType = quotationDto.FK_idCustomerType;
                existingQuotation.IdCustomer = quotationDto.IdCustomer;
                existingQuotation.DueDate = quotationDto.DueDate;
                existingQuotation.FK_idEmployee = quotationDto.FK_idEmployee;
                existingQuotation.FK_QuotationTypeId = quotationDto.FK_QuotationTypeId;
                existingQuotation.PaymentTerm = quotationDto.PaymentTerm;
                existingQuotation.FK_CommercialConditionId = quotationDto.FK_CommercialConditionId;
                existingQuotation.TotalizingQuotation = quotationDto.TotalizingQuotation;
                existingQuotation.EquipmentRemains = quotationDto.EquipmentRemains;
                existingQuotation.MonthlyConsumption = quotationDto.MonthlyConsumption;

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

        // DELETE: api/QuotationMaster/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuotation(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotation = await companyContext.QuotationMaster.FindAsync(id);
                if (quotation == null)
                {
                    return NotFound($"No se encontró la cotización con ID {id}");
                }

                // Verificar si hay detalles de cotización asociados
                var hasDetails = await companyContext.QuotationDetail.AnyAsync(qd => qd.Fk_IdQuotationMasterId == id);
                if (hasDetails)
                {
                    return BadRequest("No se puede eliminar la cotización porque tiene detalles asociados. Elimine los detalles primero.");
                }

                companyContext.QuotationMaster.Remove(quotation);
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

        // GET: api/QuotationMaster/{id}/details
        [HttpGet("{id}/details")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<QuotationDetail>>> GetQuotationDetails(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var details = await companyContext.QuotationDetail
                    .Where(qd => qd.Fk_IdQuotationMasterId == id)
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

        // POST: api/QuotationMaster/{id}/details
        [HttpPost("{id}/details")]
        public async Task<ActionResult<QuotationDetail>> AddQuotationDetail(int id, [FromBody] QuotationDetailCreateDTO detailDto, [FromQuery] string companyCode)
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
                
                // Verificar que la cotización existe
                var quotation = await companyContext.QuotationMaster.FindAsync(id);
                if (quotation == null)
                {
                    return NotFound($"No se encontró la cotización con ID {id}");
                }

                // Mapear DTO a entidad
                var detail = new QuotationDetail
                {
                    Fk_IdQuotationMasterId = id,
                    ProductType = detailDto.ProductType,
                    CodProduct = detailDto.CodProduct,
                    Unit = detailDto.Unit,
                    Quantity = detailDto.Quantity,
                    Price = detailDto.Price,
                    PorcTax = detailDto.PorcTax,
                    TaxValue = detailDto.TaxValue,
                    ContractTime = detailDto.ContractTime,
                    WarrantyPeriod = detailDto.WarrantyPeriod
                };
                
                companyContext.QuotationDetail.Add(detail);
                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return CreatedAtAction(nameof(GetQuotationDetails), new { id, companyCode }, detail);
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

        // DELETE: api/QuotationMaster/{quotationId}/details/{detailId}
        [HttpDelete("{quotationId}/details/{detailId}")]
        public async Task<IActionResult> DeleteQuotationDetail(int quotationId, int detailId, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var detail = await companyContext.QuotationDetail
                    .FirstOrDefaultAsync(qd => qd.Id == detailId && qd.Fk_IdQuotationMasterId == quotationId);

                if (detail == null)
                {
                    return NotFound($"No se encontró el detalle con ID {detailId} para la cotización {quotationId}");
                }

                companyContext.QuotationDetail.Remove(detail);
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

        // GET: api/QuotationMaster/with-details/{id}
        [HttpGet("with-details/{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<QuotationMasterResponseDTO>> GetQuotationWithDetails(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotation = await companyContext.QuotationMaster
                    .Include(q => q.Branch)
                    .Include(q => q.CustomerType)
                    .Include(q => q.Employee)
                    .Include(q => q.QuotationType)
                    .Include(q => q.CommercialCondition)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (quotation == null)
                {
                    return NotFound($"No se encontró la cotización con ID {id}");
                }

                // Obtener detalles
                var details = await companyContext.QuotationDetail
                    .Where(qd => qd.Fk_IdQuotationMasterId == id)
                    .ToListAsync();

                // Mapear a DTO de respuesta
                var response = new QuotationMasterResponseDTO
                {
                    Id = quotation.Id,
                    FK_idBranch = quotation.FK_idBranch,
                    CustomerOrigin = quotation.CustomerOrigin,
                    FK_idCustomerType = quotation.FK_idCustomerType,
                    IdCustomer = quotation.IdCustomer,
                    CreationDateTime = quotation.CreationDateTime,
                    DueDate = quotation.DueDate,
                    FK_idEmployee = quotation.FK_idEmployee,
                    FK_QuotationTypeId = quotation.FK_QuotationTypeId,
                    PaymentTerm = quotation.PaymentTerm,
                    FK_CommercialConditionId = quotation.FK_CommercialConditionId,
                    TotalizingQuotation = quotation.TotalizingQuotation,
                    EquipmentRemains = quotation.EquipmentRemains,
                    MonthlyConsumption = quotation.MonthlyConsumption,
                    Branch = quotation.Branch != null ? new BranchInfo
                    {
                        Id = quotation.Branch.Id,
                        Name = quotation.Branch.Name,
                        SystemId = quotation.Branch.SystemId,
                        LocationCode = quotation.Branch.LocationCode
                    } : null,
                    CustomerType = quotation.CustomerType != null ? new CustomerTypeInfo
                    {
                        Id = quotation.CustomerType.Id,
                        Description = quotation.CustomerType.Description,
                        IsActive = quotation.CustomerType.IsActive
                    } : null,
                    Employee = quotation.Employee != null ? new EmployeeInfo
                    {
                        Id = quotation.Employee.Id,
                        Code = quotation.Employee.Code,
                        Name = quotation.Employee.Name,
                        Charge = quotation.Employee.Charge,
                        Phone = quotation.Employee.Phone,
                        Email = quotation.Employee.Email
                    } : null,
                    QuotationType = quotation.QuotationType != null ? new QuotationTypeInfo
                    {
                        Id = quotation.QuotationType.Id,
                        Description = quotation.QuotationType.Description,
                        IsActive = quotation.QuotationType.IsActive
                    } : null,
                    CommercialCondition = quotation.CommercialCondition != null ? new CommercialConditionInfo
                    {
                        Id = quotation.CommercialCondition.Id,
                        Description = quotation.CommercialCondition.Description,
                        CommercialText = quotation.CommercialCondition.CommercialText,
                        IsActive = quotation.CommercialCondition.IsActive
                    } : null,
                    Details = details.Select(d => new QuotationDetailInfo
                    {
                        Id = d.Id,
                        ProductType = d.ProductType,
                        CodProduct = d.CodProduct,
                        Unit = d.Unit,
                        Quantity = d.Quantity,
                        Price = d.Price,
                        PorcTax = d.PorcTax,
                        TaxValue = d.TaxValue,
                        ContractTime = d.ContractTime,
                        WarrantyPeriod = d.WarrantyPeriod
                    }).ToList()
                };

                return Ok(response);
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

        // GET: api/QuotationMaster/VerificarConfiguracionCompania
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
                    HasQuotationMaster = companyContext.QuotationMaster != null,
                    HasQuotationDetail = companyContext.QuotationDetail != null,
                    HasQuotationType = companyContext.QuotationType != null,
                    HasCommercialCondition = companyContext.CommercialCondition != null,
                    HasCustomerType = companyContext.CustomerType != null,
                    HasEmployee = companyContext.Employee != null,
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

        private async Task<bool> QuotationExists(int id, ApplicationDBContext context)
        {
            return await context.QuotationMaster.AnyAsync(q => q.Id == id);
        }
    }
} 