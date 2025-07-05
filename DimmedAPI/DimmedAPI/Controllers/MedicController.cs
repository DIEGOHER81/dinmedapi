using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace DimmedAPI.Controllers
{
    [Route("api/medic")]
    [ApiController]
    public class MedicController : ControllerBase
    {
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private const string cacheTag = "medic";

        public MedicController(
            IOutputCacheStore outputCacheStore, 
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService)
        {
            this._outputCacheStore = outputCacheStore;
            this.context = context;
            this._dynamicConnectionService = dynamicConnectionService;
        }

        [HttpGet("ObtenerMedicos")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<Medic>>> GetMedic([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var medicos = await companyContext.Medic
                    .ToListAsync();

                return Ok(medicos);
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

        [HttpGet("ObtenerMedicoPorId/{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<Medic>> GetMedicById(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var medic = await companyContext.Medic.FindAsync(id);
                if (medic == null)
                {
                    return NotFound($"No se encontró el médico con ID {id}");
                }
                return Ok(medic);
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

        [HttpGet("ObtenerMedicoPorIdentificacion/{identificacion}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<Medic>> GetMedicByIdentificacion(string identificacion, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var medic = await companyContext.Medic
                    .FirstOrDefaultAsync(m => m.Identification == identificacion);
                
                if (medic == null)
                {
                    return NotFound($"No se encontró el médico con identificación {identificacion}");
                }
                return Ok(medic);
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

        [HttpGet("ObtenerMedicoPorNombre/{nombre}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<Medic>>> GetMedicByNombre(string nombre, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var medicos = await companyContext.Medic
                    .Where(m => m.Name.Contains(nombre) || m.LastName.Contains(nombre))
                    .ToListAsync();
                
                if (!medicos.Any())
                {
                    return NotFound($"No se encontraron médicos con el nombre {nombre}");
                }
                return Ok(medicos);
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

        [HttpPost("CrearMedico")]
        public async Task<ActionResult<Medic>> CreateMedic([FromBody] Medic medic, [FromQuery] string companyCode)
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
                
                companyContext.Medic.Add(medic);
                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return CreatedAtAction(nameof(GetMedicById), new { id = medic.Id, companyCode }, medic);
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

        [HttpPut("ActualizarMedico/{id}")]
        public async Task<IActionResult> UpdateMedic(int id, [FromBody] Medic medic, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (id != medic.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del médico");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var existingMedic = await companyContext.Medic.FindAsync(id);
                if (existingMedic == null)
                {
                    return NotFound($"No se encontró el médico con ID {id}");
                }

                companyContext.Entry(existingMedic).CurrentValues.SetValues(medic);
                
                try
                {
                    await companyContext.SaveChangesAsync();
                    await _outputCacheStore.EvictByTagAsync(cacheTag, default);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await MedicExists(id, companyContext))
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

        [HttpDelete("EliminarMedico/{id}")]
        public async Task<IActionResult> DeleteMedic(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var medic = await companyContext.Medic.FindAsync(id);
                if (medic == null)
                {
                    return NotFound($"No se encontró el médico con ID {id}");
                }

                companyContext.Medic.Remove(medic);
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

        private async Task<bool> MedicExists(int id, ApplicationDBContext context)
        {
            return await context.Medic.AnyAsync(e => e.Id == id);
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
