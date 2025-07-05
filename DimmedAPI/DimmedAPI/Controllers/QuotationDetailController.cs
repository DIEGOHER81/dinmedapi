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
    public class QuotationDetailController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "quotationdetail";

        public QuotationDetailController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService,
            IOutputCacheStore outputCacheStore)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
            _outputCacheStore = outputCacheStore;
        }

        // GET: api/QuotationDetail
        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<QuotationDetail>>> GetAllQuotationDetails([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotationDetails = await companyContext.QuotationDetail
                    .Include(qd => qd.QuotationMaster)
                    .ToListAsync();

                return Ok(quotationDetails);
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

        // GET: api/QuotationDetail/{id}
        [HttpGet("{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<QuotationDetail>> GetQuotationDetailById(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotationDetail = await companyContext.QuotationDetail
                    .Include(qd => qd.QuotationMaster)
                    .FirstOrDefaultAsync(qd => qd.Id == id);

                if (quotationDetail == null)
                {
                    return NotFound($"No se encontró el detalle de cotización con ID {id}");
                }

                return Ok(quotationDetail);
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

        // GET: api/QuotationDetail/by-quotation/{quotationId}
        [HttpGet("by-quotation/{quotationId}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<QuotationDetail>>> GetQuotationDetailsByQuotation(int quotationId, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotationDetails = await companyContext.QuotationDetail
                    .Include(qd => qd.QuotationMaster)
                    .Where(qd => qd.Fk_IdQuotationMasterId == quotationId)
                    .ToListAsync();

                return Ok(quotationDetails);
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

        // GET: api/QuotationDetail/by-product/{codProduct}
        [HttpGet("by-product/{codProduct}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<QuotationDetail>>> GetQuotationDetailsByProduct(string codProduct, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (string.IsNullOrEmpty(codProduct))
                {
                    return BadRequest("El código de producto es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotationDetails = await companyContext.QuotationDetail
                    .Include(qd => qd.QuotationMaster)
                    .Where(qd => qd.CodProduct == codProduct)
                    .ToListAsync();

                return Ok(quotationDetails);
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

        // GET: api/QuotationDetail/by-product-type/{productType}
        [HttpGet("by-product-type/{productType}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<QuotationDetail>>> GetQuotationDetailsByProductType(char productType, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotationDetails = await companyContext.QuotationDetail
                    .Include(qd => qd.QuotationMaster)
                    .Where(qd => qd.ProductType == productType)
                    .ToListAsync();

                return Ok(quotationDetails);
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

        // GET: api/QuotationDetail/statistics
        [HttpGet("statistics")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<object>> GetQuotationDetailStatistics([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var statistics = new
                {
                    TotalQuotationDetails = await companyContext.QuotationDetail.CountAsync(),
                    TotalQuotations = await companyContext.QuotationDetail.Select(qd => qd.Fk_IdQuotationMasterId).Distinct().CountAsync(),
                    TotalProducts = await companyContext.QuotationDetail.Select(qd => qd.CodProduct).Distinct().CountAsync(),
                    ProductTypes = await companyContext.QuotationDetail.Select(qd => qd.ProductType).Distinct().ToListAsync(),
                    AveragePrice = await companyContext.QuotationDetail.Where(qd => qd.Price.HasValue).AverageAsync(qd => qd.Price),
                    AverageQuantity = await companyContext.QuotationDetail.Where(qd => qd.Quantity.HasValue).AverageAsync(qd => qd.Quantity),
                    AverageTaxPercentage = await companyContext.QuotationDetail.Where(qd => qd.PorcTax.HasValue).AverageAsync(qd => qd.PorcTax),
                    TotalValue = await companyContext.QuotationDetail
                        .Where(qd => qd.Price.HasValue && qd.Quantity.HasValue)
                        .SumAsync(qd => qd.Price.Value * qd.Quantity.Value),
                    TotalTaxValue = await companyContext.QuotationDetail
                        .Where(qd => qd.TaxValue.HasValue)
                        .SumAsync(qd => qd.TaxValue.Value)
                };

                return Ok(statistics);
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

        // POST: api/QuotationDetail
        [HttpPost]
        public async Task<ActionResult<QuotationDetail>> CreateQuotationDetail([FromBody] QuotationDetailCreateDTO quotationDetailDto, [FromQuery] string companyCode)
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
                var quotation = await companyContext.QuotationMaster.FindAsync(quotationDetailDto.Fk_IdQuotationMasterId);
                if (quotation == null)
                {
                    return BadRequest($"No se encontró la cotización con ID {quotationDetailDto.Fk_IdQuotationMasterId}");
                }

                // Mapear DTO a entidad
                var quotationDetail = new QuotationDetail
                {
                    Fk_IdQuotationMasterId = quotationDetailDto.Fk_IdQuotationMasterId,
                    ProductType = quotationDetailDto.ProductType,
                    CodProduct = quotationDetailDto.CodProduct,
                    Unit = quotationDetailDto.Unit,
                    Quantity = quotationDetailDto.Quantity,
                    Price = quotationDetailDto.Price,
                    PorcTax = quotationDetailDto.PorcTax,
                    TaxValue = quotationDetailDto.TaxValue,
                    ContractTime = quotationDetailDto.ContractTime,
                    WarrantyPeriod = quotationDetailDto.WarrantyPeriod
                };
                
                companyContext.QuotationDetail.Add(quotationDetail);
                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return CreatedAtAction(nameof(GetQuotationDetailById), new { id = quotationDetail.Id, companyCode }, quotationDetail);
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

        // PUT: api/QuotationDetail/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuotationDetail(int id, [FromBody] QuotationDetailUpdateDTO quotationDetailDto, [FromQuery] string companyCode)
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
                
                var existingQuotationDetail = await companyContext.QuotationDetail.FindAsync(id);
                if (existingQuotationDetail == null)
                {
                    return NotFound($"No se encontró el detalle de cotización con ID {id}");
                }

                // Verificar que la cotización existe si se está cambiando
                if (quotationDetailDto.Fk_IdQuotationMasterId != existingQuotationDetail.Fk_IdQuotationMasterId)
                {
                    var quotation = await companyContext.QuotationMaster.FindAsync(quotationDetailDto.Fk_IdQuotationMasterId);
                    if (quotation == null)
                    {
                        return BadRequest($"No se encontró la cotización con ID {quotationDetailDto.Fk_IdQuotationMasterId}");
                    }
                }

                // Actualizar propiedades
                existingQuotationDetail.Fk_IdQuotationMasterId = quotationDetailDto.Fk_IdQuotationMasterId;
                existingQuotationDetail.ProductType = quotationDetailDto.ProductType;
                existingQuotationDetail.CodProduct = quotationDetailDto.CodProduct;
                existingQuotationDetail.Unit = quotationDetailDto.Unit;
                existingQuotationDetail.Quantity = quotationDetailDto.Quantity;
                existingQuotationDetail.Price = quotationDetailDto.Price;
                existingQuotationDetail.PorcTax = quotationDetailDto.PorcTax;
                existingQuotationDetail.TaxValue = quotationDetailDto.TaxValue;
                existingQuotationDetail.ContractTime = quotationDetailDto.ContractTime;
                existingQuotationDetail.WarrantyPeriod = quotationDetailDto.WarrantyPeriod;

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

        // DELETE: api/QuotationDetail/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuotationDetail(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotationDetail = await companyContext.QuotationDetail.FindAsync(id);
                if (quotationDetail == null)
                {
                    return NotFound($"No se encontró el detalle de cotización con ID {id}");
                }

                companyContext.QuotationDetail.Remove(quotationDetail);
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

        // GET: api/QuotationDetail/calculate-tax/{id}
        [HttpGet("calculate-tax/{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<object>> CalculateTaxForQuotationDetail(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var quotationDetail = await companyContext.QuotationDetail.FindAsync(id);
                if (quotationDetail == null)
                {
                    return NotFound($"No se encontró el detalle de cotización con ID {id}");
                }

                var subtotal = (quotationDetail.Price ?? 0) * (quotationDetail.Quantity ?? 0);
                var taxAmount = subtotal * ((quotationDetail.PorcTax ?? 0) / 100);
                var total = subtotal + taxAmount;

                var calculation = new
                {
                    QuotationDetailId = quotationDetail.Id,
                    Price = quotationDetail.Price,
                    Quantity = quotationDetail.Quantity,
                    Subtotal = subtotal,
                    TaxPercentage = quotationDetail.PorcTax,
                    TaxAmount = taxAmount,
                    Total = total,
                    CurrentTaxValue = quotationDetail.TaxValue
                };

                return Ok(calculation);
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

        // GET: api/QuotationDetail/VerificarConfiguracionCompania
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
                    HasQuotationDetail = companyContext.QuotationDetail != null,
                    HasQuotationMaster = companyContext.QuotationMaster != null,
                    TotalQuotationDetails = await companyContext.QuotationDetail.CountAsync(),
                    TotalQuotations = await companyContext.QuotationMaster.CountAsync(),
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

        private async Task<bool> QuotationDetailExists(int id, ApplicationDBContext context)
        {
            return await context.QuotationDetail.AnyAsync(qd => qd.Id == id);
        }
    }
} 