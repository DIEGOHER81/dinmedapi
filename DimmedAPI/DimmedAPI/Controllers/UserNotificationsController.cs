using DimmedAPI.DTOs;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserNotificationsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;

        public UserNotificationsController(ApplicationDBContext context, IDynamicConnectionService dynamicConnectionService)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
        }

        // GET: api/UserNotifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserNotificationsResponseDTO>>> GetAll([FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var notifications = await companyContext.UserNotifications
                .Include(u => u.Branch)
                .Include(u => u.Employee)
                .Select(n => new UserNotificationsResponseDTO
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    BranchId = n.BranchId,
                    NotificationCons = n.NotificationCons,
                    Name = n.Employee != null ? n.Employee.Name : null,
                    BranchName = n.Branch != null ? n.Branch.Name : null
                })
                .ToListAsync();
            return Ok(notifications);
        }

        // GET: api/UserNotifications/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserNotificationsResponseDTO>> GetById(int id, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var n = await companyContext.UserNotifications
                .Include(u => u.Branch)
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (n == null)
                return NotFound($"No se encontró la notificación con ID {id}");
            var dto = new UserNotificationsResponseDTO
            {
                Id = n.Id,
                UserId = n.UserId,
                BranchId = n.BranchId,
                NotificationCons = n.NotificationCons,
                Name = n.Employee != null ? n.Employee.Name : null,
                BranchName = n.Branch != null ? n.Branch.Name : null
            };
            return Ok(dto);
        }

        // POST: api/UserNotifications
        [HttpPost]
        public async Task<ActionResult<UserNotificationsResponseDTO>> Create([FromBody] UserNotificationsCreateDTO createDto, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var entity = new UserNotifications
            {
                UserId = createDto.UserId,
                BranchId = createDto.BranchId,
                NotificationCons = createDto.NotificationCons
            };
            companyContext.UserNotifications.Add(entity);
            await companyContext.SaveChangesAsync();
            var dto = new UserNotificationsResponseDTO
            {
                Id = entity.Id,
                UserId = entity.UserId,
                BranchId = entity.BranchId,
                NotificationCons = entity.NotificationCons,
                Name = null,
                BranchName = null
            };
            return CreatedAtAction(nameof(GetById), new { id = entity.Id, companyCode }, dto);
        }

        // PUT: api/UserNotifications/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserNotificationsUpdateDTO updateDto, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var entity = await companyContext.UserNotifications.FindAsync(id);
            if (entity == null)
                return NotFound($"No se encontró la notificación con ID {id}");
            entity.NotificationCons = updateDto.NotificationCons;
            await companyContext.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/UserNotifications/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var entity = await companyContext.UserNotifications.FindAsync(id);
            if (entity == null)
                return NotFound($"No se encontró la notificación con ID {id}");
            companyContext.UserNotifications.Remove(entity);
            await companyContext.SaveChangesAsync();
            return NoContent();
        }
    }
} 