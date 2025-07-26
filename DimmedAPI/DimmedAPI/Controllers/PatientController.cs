using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;

        public PatientController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
        }

        // GET: api/Patient/basic
        [HttpGet("basic")]
        public async Task<ActionResult<object>> GetBasicPatients([FromQuery] string companyCode, int page = 1, int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var query = companyContext.Set<Patient>()
                    .Select(p => new { p.Id, p.Identification, p.Name, p.LastName });

                var total = await query.CountAsync();
                var result = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new { total, page, pageSize, data = result });
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

        // GET: api/Patient
        [HttpGet]
        public async Task<ActionResult<object>> GetAllPatients([FromQuery] string companyCode, int page = 1, int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var query = companyContext.Set<Patient>();
                var total = await query.CountAsync();
                var result = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new { total, page, pageSize, data = result });
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

        // POST: api/Patient
        [HttpPost]
        public async Task<ActionResult<Patient>> CreatePatient([FromBody] Patient patient, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                companyContext.Set<Patient>().Add(patient);
                await companyContext.SaveChangesAsync();
                return CreatedAtAction(nameof(GetAllPatients), new { id = patient.Id, companyCode }, patient);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Mostrar detalles de la inner exception para depuración
                var errorMsg = $"Error interno del servidor: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMsg += $" | InnerException: {ex.InnerException.Message}";
                }
                if (ex.InnerException?.InnerException != null)
                {
                    errorMsg += $" | InnerMostException: {ex.InnerException.InnerException.Message}";
                }
                return StatusCode(500, errorMsg);
            }
        }

        // PUT: api/Patient/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] Patient patient, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (id != patient.Id)
                    return BadRequest("El id de la URL no coincide con el del paciente.");

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                companyContext.Entry(patient).State = EntityState.Modified;
                try
                {
                    await companyContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await companyContext.Set<Patient>().AnyAsync(p => p.Id == id))
                        return NotFound();
                    else
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

        // DELETE: api/Patient/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var patient = await companyContext.Set<Patient>().FindAsync(id);
                if (patient == null)
                    return NotFound();

                companyContext.Set<Patient>().Remove(patient);
                await companyContext.SaveChangesAsync();
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

        // GET: api/Patient/search?companyCode=xxx&filter=xxx&page=1&pageSize=10
        [HttpGet("search")]
        public async Task<ActionResult<object>> SearchPatients(
            [FromQuery] string companyCode,
            [FromQuery] string filter = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;

                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                var query = companyContext.Set<Patient>().AsQueryable();

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    var filterLower = filter.ToLower();
                    query = query.Where(p =>
                        (!string.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(filterLower)) ||
                        (!string.IsNullOrEmpty(p.LastName) && p.LastName.ToLower().Contains(filterLower)) ||
                        (!string.IsNullOrEmpty(p.Identification) && p.Identification.ToLower().Contains(filterLower))
                    );
                }

                var total = await query.CountAsync();
                var result = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new { p.Id, p.Identification, p.Name, p.LastName })
                    .ToListAsync();

                return Ok(new { total, page, pageSize, data = result });
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