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
    public class OrderTypeController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "ordertype";

        public OrderTypeController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService,
            IOutputCacheStore outputCacheStore)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
            _outputCacheStore = outputCacheStore;
        }

        // GET: api/OrderType
        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<OrderTypeResponseDTO>>> GetAll([FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var orderTypes = await companyContext.OrderType
                .Select(ot => new OrderTypeResponseDTO
                {
                    Id = ot.Id,
                    Description = ot.Description,
                    IsActive = ot.IsActive
                })
                .ToListAsync();
            return Ok(orderTypes);
        }

        // GET: api/OrderType/active
        [HttpGet("active")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<OrderTypeResponseDTO>>> GetActive([FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var orderTypes = await companyContext.OrderType
                .Where(ot => ot.IsActive == true)
                .Select(ot => new OrderTypeResponseDTO
                {
                    Id = ot.Id,
                    Description = ot.Description,
                    IsActive = ot.IsActive
                })
                .ToListAsync();
            return Ok(orderTypes);
        }

        // GET: api/OrderType/{id}
        [HttpGet("{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<OrderTypeResponseDTO>> GetById(long id, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var orderType = await companyContext.OrderType.FindAsync(id);
            if (orderType == null)
                return NotFound($"No se encontró el tipo de orden con ID {id}");

            var response = new OrderTypeResponseDTO
            {
                Id = orderType.Id,
                Description = orderType.Description,
                IsActive = orderType.IsActive
            };
            return Ok(response);
        }

        // POST: api/OrderType
        [HttpPost]
        public async Task<ActionResult<OrderTypeResponseDTO>> Create([FromBody] OrderTypeCreateDTO createDto, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            
            var orderType = new OrderType
            {
                Description = createDto.Description,
                IsActive = createDto.IsActive
            };

            companyContext.OrderType.Add(orderType);
            await companyContext.SaveChangesAsync();

            var response = new OrderTypeResponseDTO
            {
                Id = orderType.Id,
                Description = orderType.Description,
                IsActive = orderType.IsActive
            };

            return CreatedAtAction(nameof(GetById), new { id = orderType.Id, companyCode }, response);
        }

        // PUT: api/OrderType/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] OrderTypeUpdateDTO updateDto, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var orderType = await companyContext.OrderType.FindAsync(id);
            if (orderType == null)
                return NotFound($"No se encontró el tipo de orden con ID {id}");

            orderType.Description = updateDto.Description;
            orderType.IsActive = updateDto.IsActive;

            companyContext.Entry(orderType).State = EntityState.Modified;
            try
            {
                await companyContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await OrderTypeExists(id, companyContext))
                    return NotFound($"No se encontró el tipo de orden con ID {id}");
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/OrderType/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var orderType = await companyContext.OrderType.FindAsync(id);
            if (orderType == null)
                return NotFound($"No se encontró el tipo de orden con ID {id}");

            companyContext.OrderType.Remove(orderType);
            await companyContext.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/OrderType/{id}/estado
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> UpdateEstado(long id, [FromQuery] string companyCode, [FromBody] bool isActive)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var orderType = await companyContext.OrderType.FindAsync(id);
            if (orderType == null)
                return NotFound($"No se encontró el tipo de orden con ID {id}");

            orderType.IsActive = isActive;
            await companyContext.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> OrderTypeExists(long id, ApplicationDBContext context)
        {
            return await context.OrderType.AnyAsync(e => e.Id == id);
        }
    }
} 