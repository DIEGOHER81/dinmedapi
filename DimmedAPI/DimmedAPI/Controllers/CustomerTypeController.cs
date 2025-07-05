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
    public class CustomerTypeController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "customertype";

        public CustomerTypeController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService,
            IOutputCacheStore outputCacheStore)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
            _outputCacheStore = outputCacheStore;
        }

        // GET: api/CustomerType
        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<CustomerType>>> GetAllCustomerTypes([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var customerTypes = await companyContext.CustomerType
                    .ToListAsync();

                return Ok(customerTypes);
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

        // GET: api/CustomerType/active
        [HttpGet("active")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<CustomerType>>> GetActiveCustomerTypes([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var customerTypes = await companyContext.CustomerType
                    .Where(ct => ct.IsActive)
                    .ToListAsync();

                return Ok(customerTypes);
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

        // GET: api/CustomerType/with-quotations-count
        [HttpGet("with-quotations-count")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<CustomerTypeResponseDTO>>> GetCustomerTypesWithQuotationsCount([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var customerTypesWithCount = await companyContext.CustomerType
                    .Select(ct => new CustomerTypeResponseDTO
                    {
                        Id = ct.Id,
                        Description = ct.Description,
                        IsActive = ct.IsActive,
                        QuotationsCount = companyContext.QuotationMaster.Count(q => q.FK_idCustomerType == ct.Id)
                    })
                    .ToListAsync();

                return Ok(customerTypesWithCount);
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

        // GET: api/CustomerType/{id}
        [HttpGet("{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<CustomerType>> GetCustomerTypeById(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var customerType = await companyContext.CustomerType.FindAsync(id);
                if (customerType == null)
                {
                    return NotFound($"No se encontró el tipo de cliente con ID {id}");
                }

                return Ok(customerType);
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

        // GET: api/CustomerType/by-description/{description}
        [HttpGet("by-description/{description}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<CustomerType>>> GetCustomerTypesByDescription(string description, [FromQuery] string companyCode)
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
                
                var customerTypes = await companyContext.CustomerType
                    .Where(ct => ct.Description != null && ct.Description.Contains(description))
                    .ToListAsync();

                return Ok(customerTypes);
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

        // POST: api/CustomerType
        [HttpPost]
        public async Task<ActionResult<CustomerType>> CreateCustomerType([FromBody] CustomerTypeCreateDTO customerTypeDto, [FromQuery] string companyCode)
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
                
                // Verificar si ya existe un tipo de cliente con la misma descripción
                var existingCustomerType = await companyContext.CustomerType
                    .FirstOrDefaultAsync(ct => ct.Description == customerTypeDto.Description);
                
                if (existingCustomerType != null)
                {
                    return BadRequest($"Ya existe un tipo de cliente con la descripción '{customerTypeDto.Description}'");
                }

                // Mapear DTO a entidad
                var customerType = new CustomerType
                {
                    Description = customerTypeDto.Description,
                    IsActive = customerTypeDto.IsActive
                };
                
                companyContext.CustomerType.Add(customerType);
                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return CreatedAtAction(nameof(GetCustomerTypeById), new { id = customerType.Id, companyCode }, customerType);
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

        // PUT: api/CustomerType/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomerType(int id, [FromBody] CustomerTypeUpdateDTO customerTypeDto, [FromQuery] string companyCode)
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
                
                var existingCustomerType = await companyContext.CustomerType.FindAsync(id);
                if (existingCustomerType == null)
                {
                    return NotFound($"No se encontró el tipo de cliente con ID {id}");
                }

                // Verificar si ya existe otro tipo de cliente con la misma descripción (excluyendo el actual)
                var duplicateCustomerType = await companyContext.CustomerType
                    .FirstOrDefaultAsync(ct => ct.Description == customerTypeDto.Description && ct.Id != id);
                
                if (duplicateCustomerType != null)
                {
                    return BadRequest($"Ya existe otro tipo de cliente con la descripción '{customerTypeDto.Description}'");
                }

                // Actualizar propiedades
                existingCustomerType.Description = customerTypeDto.Description;
                existingCustomerType.IsActive = customerTypeDto.IsActive;

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

        // PATCH: api/CustomerType/{id}/toggle-status
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleCustomerTypeStatus(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var customerType = await companyContext.CustomerType.FindAsync(id);
                if (customerType == null)
                {
                    return NotFound($"No se encontró el tipo de cliente con ID {id}");
                }

                // Cambiar el estado
                customerType.IsActive = !customerType.IsActive;

                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return Ok(new { id = customerType.Id, isActive = customerType.IsActive });
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

        // DELETE: api/CustomerType/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerType(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var customerType = await companyContext.CustomerType.FindAsync(id);
                if (customerType == null)
                {
                    return NotFound($"No se encontró el tipo de cliente con ID {id}");
                }

                // Verificar si hay cotizaciones asociadas
                var hasQuotations = await companyContext.QuotationMaster.AnyAsync(q => q.FK_idCustomerType == id);
                if (hasQuotations)
                {
                    return BadRequest("No se puede eliminar el tipo de cliente porque tiene cotizaciones asociadas. Desactive el tipo de cliente en su lugar.");
                }

                companyContext.CustomerType.Remove(customerType);
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

        // GET: api/CustomerType/{id}/quotations
        [HttpGet("{id}/quotations")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<object>>> GetCustomerTypeQuotations(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Verificar que el tipo de cliente existe
                var customerType = await companyContext.CustomerType.FindAsync(id);
                if (customerType == null)
                {
                    return NotFound($"No se encontró el tipo de cliente con ID {id}");
                }

                var quotations = await companyContext.QuotationMaster
                    .Where(q => q.FK_idCustomerType == id)
                    .Select(q => new
                    {
                        q.Id,
                        q.IdCustomer,
                        q.CreationDateTime,
                        q.DueDate,
                        q.FK_idEmployee,
                        q.TotalizingQuotation,
                        q.EquipmentRemains
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

        // GET: api/CustomerType/VerificarConfiguracionCompania
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
                    HasCustomerType = companyContext.CustomerType != null,
                    TotalCustomerTypes = await companyContext.CustomerType.CountAsync(),
                    ActiveCustomerTypes = await companyContext.CustomerType.CountAsync(ct => ct.IsActive),
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

        private async Task<bool> CustomerTypeExists(int id, ApplicationDBContext context)
        {
            return await context.CustomerType.AnyAsync(ct => ct.Id == id);
        }
    }
} 