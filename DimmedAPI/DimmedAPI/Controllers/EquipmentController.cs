using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;
using DimmedAPI.DTOs;

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
        public async Task<ActionResult<IEnumerable<EquipmentResponseDTO>>> GetAll([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");

                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                var equipos = await companyContext.Equipment.ToListAsync();

                var equiposDto = equipos.Select(e => new EquipmentResponseDTO
                {
                    Id = e.Id,
                    Code = e.Code ?? "",
                    Name = e.Name ?? "",
                    ShortName = e.ShortName ?? "",
                    Status = e.Status ?? "",
                    ProductLine = e.ProductLine ?? "",
                    Branch = e.Branch ?? "",
                    EstimatedTime = e.EstimatedTime ?? "",
                    Description = e.Description ?? "",
                    IsActive = e.IsActive,
                    TechSpec = e.TechSpec ?? "",
                    DestinationBranch = e.DestinationBranch ?? "",
                    LoanDate = e.LoanDate,
                    ReturnDate = e.ReturnDate,
                    InitDate = e.InitDate,
                    Vendor = e.Vendor ?? "",
                    Brand = e.Brand ?? "",
                    Model = e.Model ?? "",
                    Abc = e.Abc ?? "",
                    EndDate = e.EndDate,
                    Type = e.Type ?? "",
                    SystemIdBC = e.SystemIdBC ?? "",
                    NoBoxes = e.NoBoxes,
                    LastPreventiveMaintenance = e.LastPreventiveMaintenance,
                    LastMaintenance = e.LastMaintenance,
                    Alert = e.Alert ?? "",
                    LocationCode = e.LocationCode ?? "",
                    TransferStatus = e.TransferStatus ?? ""
                }).ToList();

                return Ok(equiposDto);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (System.Data.SqlTypes.SqlNullValueException ex)
            {
                return StatusCode(500, $"Error de datos nulos en la base de datos (SqlNullValueException): {ex.Message}. Verifique que los campos en la base de datos permitan nulos o que el modelo los acepte como nullable.");
            }
            catch (NullReferenceException ex)
            {
                return StatusCode(500, $"Error de datos nulos: {ex.Message}. Es posible que algún campo en la base de datos esté en NULL y el modelo no lo permite. Verifique los campos de fechas y cadenas nulas.");
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"Error de operación inválida: {ex.Message}. Es posible que algún campo en la base de datos esté en NULL y el modelo no lo permite. Verifique los campos de fechas y cadenas nulas.");
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

        // Método auxiliar para identificar el campo nulo
        private string IdentificarCampoNulo(Equipment equipo)
        {
            if (equipo == null) return "Equipo completo es null";
            if (equipo.LoanDate == null) return "LoanDate";
            if (equipo.ReturnDate == null) return "ReturnDate";
            if (equipo.EndDate == null) return "EndDate";
            if (equipo.LastPreventiveMaintenance == null) return "LastPreventiveMaintenance";
            if (equipo.LastMaintenance == null) return "LastMaintenance";
            // Para string, si quieres detectar null o vacío:
            if (string.IsNullOrEmpty(equipo.Alert)) return "Alert";
            if (string.IsNullOrEmpty(equipo.TransferStatus)) return "TransferStatus";
            return "Desconocido (todos los campos principales tienen valor)";
        }
    }
} 