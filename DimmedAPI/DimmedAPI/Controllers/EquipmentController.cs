using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/equipment")]
    public class EquipmentController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;

        public EquipmentController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
        }

        // GET: api/equipment?companyCode=xxx
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Equipment>>> GetAll([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");

                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                var equipos = await companyContext.Equipment.ToListAsync();
                return Ok(equipos);
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

        // GET: api/equipment/{id}?companyCode=xxx
        [HttpGet("{id}")]
        public async Task<ActionResult<Equipment>> GetById(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");

                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                var equipo = await companyContext.Equipment.FindAsync(id);
                if (equipo == null)
                    return NotFound($"No se encontró el equipo con ID {id}");
                return Ok(equipo);
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

        // (Opcional) GET: api/equipment/by-code/{code}?companyCode=xxx
        [HttpGet("by-code/{code}")]
        public async Task<ActionResult<Equipment>> GetByCode(string code, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");

                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                var equipo = await companyContext.Equipment.FirstOrDefaultAsync(e => e.Code == code);
                if (equipo == null)
                    return NotFound($"No se encontró el equipo con código {code}");
                return Ok(equipo);
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
    }
} 