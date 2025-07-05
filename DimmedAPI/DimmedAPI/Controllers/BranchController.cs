using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "branch";

        public BranchController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService,
            IOutputCacheStore outputCacheStore)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
            _outputCacheStore = outputCacheStore;
        }

        // GET: api/Branch
        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<Branch>>> GetAllBranches([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var branches = await companyContext.Branches
                    .ToListAsync();

                return Ok(branches);
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

        // GET: api/Branch/{id}
        [HttpGet("{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<Branch>> GetBranchById(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var branch = await companyContext.Branches.FindAsync(id);
                if (branch == null)
                {
                    return NotFound($"No se encontró la sucursal con ID {id}");
                }

                return Ok(branch);
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

        // GET: api/Branch/location-codes
        [HttpGet("location-codes")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<object>>> GetLocationCodesAndNames([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var result = await companyContext.Branches
                    .Select(b => new { b.LocationCode, b.Name })
                    .ToListAsync();
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

        // GET: api/Branch/by-location-code/{locationCode}
        [HttpGet("by-location-code/{locationCode}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<Branch>> GetBranchByLocationCode(string locationCode, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (string.IsNullOrEmpty(locationCode))
                {
                    return BadRequest("El código de ubicación es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var branch = await companyContext.Branches
                    .FirstOrDefaultAsync(b => b.LocationCode == locationCode);

                if (branch == null)
                {
                    return NotFound($"No se encontró la sucursal con código de ubicación {locationCode}");
                }

                return Ok(branch);
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

        // POST: api/Branch
        [HttpPost]
        public async Task<ActionResult<Branch>> CreateBranch([FromBody] Branch branch, [FromQuery] string companyCode)
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
                
                companyContext.Branches.Add(branch);
                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return CreatedAtAction(nameof(GetBranchById), new { id = branch.Id, companyCode }, branch);
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

        // PUT: api/Branch/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody] Branch branch, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (id != branch.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID de la sucursal");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var existingBranch = await companyContext.Branches.FindAsync(id);
                if (existingBranch == null)
                {
                    return NotFound($"No se encontró la sucursal con ID {id}");
                }

                companyContext.Entry(existingBranch).CurrentValues.SetValues(branch);
                
                try
                {
                    await companyContext.SaveChangesAsync();
                    await _outputCacheStore.EvictByTagAsync(cacheTag, default);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await BranchExists(id, companyContext))
                    {
                        return NotFound();
                    }
                    throw;
                }

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

        // DELETE: api/Branch/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var branch = await companyContext.Branches.FindAsync(id);
                if (branch == null)
                {
                    return NotFound($"No se encontró la sucursal con ID {id}");
                }

                // Primero eliminar los registros relacionados en UserBranch
                var userBranches = await companyContext.UserBranch
                    .Where(ub => ub.IdBranch == id)
                    .ToListAsync();

                if (userBranches.Any())
                {
                    companyContext.UserBranch.RemoveRange(userBranches);
                    await companyContext.SaveChangesAsync();
                }

                // Luego eliminar la sucursal
                companyContext.Branches.Remove(branch);
                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return Ok(new 
                { 
                    mensaje = "Sucursal eliminada correctamente",
                    registrosRelacionadosEliminados = userBranches.Count,
                    sucursalEliminada = new { id = branch.Id, nombre = branch.Name }
                });
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

        private async Task<bool> BranchExists(int id, ApplicationDBContext context)
        {
            return await context.Branches.AnyAsync(e => e.Id == id);
        }

        [HttpGet("VerificarConfiguracionCompania")]
        public async Task<ActionResult<object>> VerificarConfiguracionCompania([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                var company = await _dynamicConnectionService.GetCompanyByCodeAsync(companyCode);
                if (company == null)
                {
                    return NotFound($"Compañía con código {companyCode} no encontrada");
                }

                return Ok(new
                {
                    Company = new
                    {
                        company.Id,
                        company.BusinessName,
                        company.BCCodigoEmpresa,
                        company.SqlConnectionString
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
} 