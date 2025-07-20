using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using DimmedAPI.DTOs;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntryrequestServiceController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "entryrequestservice";

        public EntryrequestServiceController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService,
            IOutputCacheStore outputCacheStore)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
            _outputCacheStore = outputCacheStore;
        }

        // GET: api/EntryrequestService
        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<EntryrequestServiceResponseDTO>>> GetAll([FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var services = await companyContext.EntryrequestService
                .Select(s => new EntryrequestServiceResponseDTO
                {
                    Id = s.Id,
                    Description = s.Description,
                    IsActive = s.IsActive
                })
                .ToListAsync();
            return Ok(services);
        }

        // GET: api/EntryrequestService/active
        [HttpGet("active")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<EntryrequestServiceResponseDTO>>> GetActive([FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var services = await companyContext.EntryrequestService
                .Where(s => s.IsActive == true)
                .Select(s => new EntryrequestServiceResponseDTO
                {
                    Id = s.Id,
                    Description = s.Description,
                    IsActive = s.IsActive
                })
                .ToListAsync();
            return Ok(services);
        }

        // GET: api/EntryrequestService/{id}
        [HttpGet("{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<EntryrequestServiceResponseDTO>> GetById(long id, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var service = await companyContext.EntryrequestService.FindAsync(id);
            if (service == null)
                return NotFound($"No se encontró el servicio con ID {id}");

            var response = new EntryrequestServiceResponseDTO
            {
                Id = service.Id,
                Description = service.Description,
                IsActive = service.IsActive
            };
            return Ok(response);
        }

        // POST: api/EntryrequestService
        [HttpPost]
        public async Task<ActionResult<EntryrequestServiceResponseDTO>> Create([FromBody] EntryrequestServiceCreateDTO createDto, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            
            var service = new EntryrequestService
            {
                Description = createDto.Description,
                IsActive = createDto.IsActive
            };

            companyContext.EntryrequestService.Add(service);
            await companyContext.SaveChangesAsync();

            var response = new EntryrequestServiceResponseDTO
            {
                Id = service.Id,
                Description = service.Description,
                IsActive = service.IsActive
            };

            return CreatedAtAction(nameof(GetById), new { id = service.Id, companyCode }, response);
        }

        // PUT: api/EntryrequestService/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] EntryrequestServiceUpdateDTO updateDto, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var service = await companyContext.EntryrequestService.FindAsync(id);
            if (service == null)
                return NotFound($"No se encontró el servicio con ID {id}");

            service.Description = updateDto.Description;
            service.IsActive = updateDto.IsActive;

            companyContext.Entry(service).State = EntityState.Modified;
            try
            {
                await companyContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ServiceExists(id, companyContext))
                    return NotFound($"No se encontró el servicio con ID {id}");
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/EntryrequestService/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var service = await companyContext.EntryrequestService.FindAsync(id);
            if (service == null)
                return NotFound($"No se encontró el servicio con ID {id}");

            companyContext.EntryrequestService.Remove(service);
            await companyContext.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/EntryrequestService/{id}/estado
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> UpdateEstado(long id, [FromQuery] string companyCode, [FromBody] bool isActive)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var service = await companyContext.EntryrequestService.FindAsync(id);
            if (service == null)
                return NotFound($"No se encontró el servicio con ID {id}");

            service.IsActive = isActive;
            await companyContext.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> ServiceExists(long id, ApplicationDBContext context)
        {
            return await context.EntryrequestService.AnyAsync(e => e.Id == id);
        }
    }
} 