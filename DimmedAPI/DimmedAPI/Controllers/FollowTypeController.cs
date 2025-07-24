using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using DimmedAPI.DTOs;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FollowTypeController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "followtype";

        public FollowTypeController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService,
            IOutputCacheStore outputCacheStore)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
            _outputCacheStore = outputCacheStore;
        }

        // GET: api/FollowType
        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<FollowTypeResponseDTO>>> GetAll([FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var followTypes = await companyContext.FollowType
                .Select(ft => new FollowTypeResponseDTO
                {
                    Id = ft.Id,
                    Description = ft.Description,
                    IsActive = ft.IsActive
                })
                .ToListAsync();
            return Ok(followTypes);
        }

        // GET: api/FollowType/{id}
        [HttpGet("{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<FollowTypeResponseDTO>> GetById(int id, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var ft = await companyContext.FollowType.FindAsync(id);
            if (ft == null)
                return NotFound($"No se encontró el tipo de seguimiento con ID {id}");
            var dto = new FollowTypeResponseDTO
            {
                Id = ft.Id,
                Description = ft.Description,
                IsActive = ft.IsActive
            };
            return Ok(dto);
        }

        // POST: api/FollowType
        [HttpPost]
        public async Task<ActionResult<FollowTypeResponseDTO>> Create([FromBody] FollowTypeCreateDTO createDto, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var followType = new FollowType
            {
                Description = createDto.Description,
                IsActive = createDto.IsActive
            };
            companyContext.FollowType.Add(followType);
            await companyContext.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(cacheTag, default);
            var response = new FollowTypeResponseDTO
            {
                Id = followType.Id,
                Description = followType.Description,
                IsActive = followType.IsActive
            };
            return CreatedAtAction(nameof(GetById), new { id = followType.Id, companyCode }, response);
        }

        // PUT: api/FollowType/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FollowTypeUpdateDTO updateDto, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var followType = await companyContext.FollowType.FindAsync(id);
            if (followType == null)
                return NotFound($"No se encontró el tipo de seguimiento con ID {id}");

            followType.Description = updateDto.Description;
            followType.IsActive = updateDto.IsActive;
            await companyContext.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(cacheTag, default);
            return NoContent();
        }

        // DELETE: api/FollowType/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var followType = await companyContext.FollowType.FindAsync(id);
            if (followType == null)
                return NotFound($"No se encontró el tipo de seguimiento con ID {id}");

            companyContext.FollowType.Remove(followType);
            await companyContext.SaveChangesAsync();
            await _outputCacheStore.EvictByTagAsync(cacheTag, default);
            return NoContent();
        }
    }
} 