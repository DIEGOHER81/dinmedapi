using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.DTOs;
using DimmedAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntryRequestDetailsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;

        public EntryRequestDetailsController(ApplicationDBContext context, IDynamicConnectionService dynamicConnectionService)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
        }

        // GET: api/EntryRequestDetails?companyCode=xxx
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntryRequestDetailsResponseDTO>>> GetAll([FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var details = await companyContext.EntryRequestDetails
                .Include(d => d.IdEquipmentNavigation)
                .Select(d => new EntryRequestDetailsResponseDTO
                {
                    Id = d.Id,
                    IdEntryReq = d.IdEntryReq,
                    IdEquipment = d.IdEquipment,
                    CreateAt = d.CreateAt,
                    DateIni = d.DateIni,
                    DateEnd = d.DateEnd,
                    status = d.status,
                    DateLoadState = d.DateLoadState,
                    TraceState = d.TraceState,
                    IsComponent = d.IsComponent,
                    UserIdTraceState = d.UserIdTraceState,
                    sInformation = d.sInformation,
                    Name = d.Name,
                    EquipmentName = d.IdEquipmentNavigation != null ? d.IdEquipmentNavigation.Name : null,
                    EquipmentCode = d.IdEquipmentNavigation != null ? d.IdEquipmentNavigation.Code : null
                })
                .ToListAsync();
            return Ok(details);
        }

        // GET: api/EntryRequestDetails/{id}?companyCode=xxx
        [HttpGet("{id}")]
        public async Task<ActionResult<EntryRequestDetailsResponseDTO>> GetById(int id, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var d = await companyContext.EntryRequestDetails
                .Include(d => d.IdEquipmentNavigation)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (d == null)
                return NotFound();
            var dto = new EntryRequestDetailsResponseDTO
            {
                Id = d.Id,
                IdEntryReq = d.IdEntryReq,
                IdEquipment = d.IdEquipment,
                CreateAt = d.CreateAt,
                DateIni = d.DateIni,
                DateEnd = d.DateEnd,
                status = d.status,
                DateLoadState = d.DateLoadState,
                TraceState = d.TraceState,
                IsComponent = d.IsComponent,
                UserIdTraceState = d.UserIdTraceState,
                sInformation = d.sInformation,
                Name = d.Name,
                EquipmentName = d.IdEquipmentNavigation != null ? d.IdEquipmentNavigation.Name : null,
                EquipmentCode = d.IdEquipmentNavigation != null ? d.IdEquipmentNavigation.Code : null
            };
            return Ok(dto);
        }

        // GET: api/EntryRequestDetails/by-entryreq/{idEntryReq}?companyCode=xxx
        [HttpGet("by-entryreq/{idEntryReq}")]
        public async Task<ActionResult<IEnumerable<EntryRequestDetailsResponseDTO>>> GetByIdEntryReq(int idEntryReq, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var details = await companyContext.EntryRequestDetails
                .Where(d => d.IdEntryReq == idEntryReq)
                .Include(d => d.IdEquipmentNavigation)
                .Select(d => new EntryRequestDetailsResponseDTO
                {
                    Id = d.Id,
                    IdEntryReq = d.IdEntryReq,
                    IdEquipment = d.IdEquipment,
                    CreateAt = d.CreateAt,
                    DateIni = d.DateIni,
                    DateEnd = d.DateEnd,
                    status = d.status,
                    DateLoadState = d.DateLoadState,
                    TraceState = d.TraceState,
                    IsComponent = d.IsComponent,
                    UserIdTraceState = d.UserIdTraceState,
                    sInformation = d.sInformation,
                    Name = d.Name,
                    // Nuevo campo: nombre del equipo
                    EquipmentName = d.IdEquipmentNavigation != null ? d.IdEquipmentNavigation.Name : null,
                    // Nuevo campo: código del equipo
                    EquipmentCode = d.IdEquipmentNavigation != null ? d.IdEquipmentNavigation.Code : null
                })
                .ToListAsync();
            return Ok(details);
        }

        // POST: api/EntryRequestDetails?companyCode=xxx
        [HttpPost]
        public async Task<ActionResult<EntryRequestDetailsResponseDTO>> Create([FromBody] EntryRequestDetailsCreateDTO createDto, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            
            // Validación: verificar que no exista ya un registro con la misma combinación de IdEntryReq e IdEquipment
            var existingDetail = await companyContext.EntryRequestDetails
                .FirstOrDefaultAsync(d => d.IdEntryReq == createDto.IdEntryReq && d.IdEquipment == createDto.IdEquipment);
            
            if (existingDetail != null)
            {
                return BadRequest(new { 
                    message = "El equipo seleccionado ya se encuentra agendado en el pedido",
                    error = "DUPLICATE_EQUIPMENT_IN_ENTRY_REQUEST"
                });
            }
            
            var entity = new EntryRequestDetails
            {
                IdEntryReq = createDto.IdEntryReq,
                IdEquipment = createDto.IdEquipment,
                CreateAt = createDto.CreateAt,
                DateIni = createDto.DateIni,
                DateEnd = createDto.DateEnd,
                status = createDto.status,
                DateLoadState = createDto.DateLoadState,
                TraceState = createDto.TraceState,
                IsComponent = createDto.IsComponent,
                UserIdTraceState = createDto.UserIdTraceState,
                sInformation = createDto.sInformation,
                Name = createDto.Name
            };
            companyContext.EntryRequestDetails.Add(entity);
            await companyContext.SaveChangesAsync();
            
            // Load the equipment information for the response
            var equipment = await companyContext.Equipment.FindAsync(entity.IdEquipment);
            var dto = new EntryRequestDetailsResponseDTO
            {
                Id = entity.Id,
                IdEntryReq = entity.IdEntryReq,
                IdEquipment = entity.IdEquipment,
                CreateAt = entity.CreateAt,
                DateIni = entity.DateIni,
                DateEnd = entity.DateEnd,
                status = entity.status,
                DateLoadState = entity.DateLoadState,
                TraceState = entity.TraceState,
                IsComponent = entity.IsComponent,
                UserIdTraceState = entity.UserIdTraceState,
                sInformation = entity.sInformation,
                Name = entity.Name,
                EquipmentName = equipment?.Name,
                EquipmentCode = equipment?.Code
            };
            return CreatedAtAction(nameof(GetById), new { id = entity.Id, companyCode }, dto);
        }

        // PUT: api/EntryRequestDetails/{id}?companyCode=xxx
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EntryRequestDetailsCreateDTO updateDto, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var entity = await companyContext.EntryRequestDetails.FindAsync(id);
            if (entity == null)
                return NotFound();

            // Validación: verificar que no exista ya un registro con la misma combinación de IdEntryReq e IdEquipment
            // Excluir el registro actual que se está actualizando
            var existingDetail = await companyContext.EntryRequestDetails
                .FirstOrDefaultAsync(d => d.IdEntryReq == updateDto.IdEntryReq && 
                                        d.IdEquipment == updateDto.IdEquipment && 
                                        d.Id != id);
            
            if (existingDetail != null)
            {
                return BadRequest(new { 
                    message = "El equipo seleccionado ya se encuentra agendado en el pedido",
                    error = "DUPLICATE_EQUIPMENT_IN_ENTRY_REQUEST"
                });
            }

            entity.IdEntryReq = updateDto.IdEntryReq;
            entity.IdEquipment = updateDto.IdEquipment;
            entity.CreateAt = updateDto.CreateAt;
            entity.DateIni = updateDto.DateIni;
            entity.DateEnd = updateDto.DateEnd;
            entity.status = updateDto.status;
            entity.DateLoadState = updateDto.DateLoadState;
            entity.TraceState = updateDto.TraceState;
            entity.IsComponent = updateDto.IsComponent;
            entity.UserIdTraceState = updateDto.UserIdTraceState;
            entity.sInformation = updateDto.sInformation;
            entity.Name = updateDto.Name;

            await companyContext.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/EntryRequestDetails/{id}?companyCode=xxx
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var entity = await companyContext.EntryRequestDetails.FindAsync(id);
            if (entity == null)
                return NotFound();
            companyContext.EntryRequestDetails.Remove(entity);
            await companyContext.SaveChangesAsync();
            return NoContent();
        }
    }
} 