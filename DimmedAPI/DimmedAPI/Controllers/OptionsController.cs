using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OptionsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "options";

        public OptionsController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService,
            IOutputCacheStore outputCacheStore)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
            _outputCacheStore = outputCacheStore;
        }

        // GET: api/Options
        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<Options>>> GetAll([FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var options = await companyContext.Options.ToListAsync();
            return Ok(options);
        }

        // GET: api/Options/{id}
        [HttpGet("{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<Options>> GetById(int id, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var option = await companyContext.Options.FindAsync(id);
            if (option == null)
                return NotFound($"No se encontró la opción con ID {id}");
            return Ok(option);
        }

        // POST: api/Options
        [HttpPost]
        public async Task<ActionResult<Options>> Create([FromBody] Options option, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            companyContext.Options.Add(option);
            await companyContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = option.Id, companyCode }, option);
        }

        // PUT: api/Options/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Options option, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");
            if (id != option.Id)
                return BadRequest("El ID de la opción no coincide");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            companyContext.Entry(option).State = EntityState.Modified;
            try
            {
                await companyContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await OptionExists(id, companyContext))
                    return NotFound($"No se encontró la opción con ID {id}");
                else
                    throw;
            }
            return NoContent();
        }

        // DELETE: api/Options/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var option = await companyContext.Options.FindAsync(id);
            if (option == null)
                return NotFound($"No se encontró la opción con ID {id}");
            companyContext.Options.Remove(option);
            await companyContext.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/Options/{id}/estado
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> UpdateEstado(int id, [FromQuery] string companyCode, [FromBody] bool isActive)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var option = await companyContext.Options.FindAsync(id);
            if (option == null)
                return NotFound($"No se encontró la opción con ID {id}");
            option.IsActive = isActive;
            await companyContext.SaveChangesAsync();
            return NoContent();
        }

        private async Task<bool> OptionExists(int id, ApplicationDBContext context)
        {
            return await context.Options.AnyAsync(e => e.Id == id);
        }
    }
} 