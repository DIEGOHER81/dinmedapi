using Microsoft.AspNetCore.Mvc;
using DimmedAPI.DTOs;
using DimmedAPI.Services;
using DimmedAPI.BO;
using Microsoft.EntityFrameworkCore;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentSchedulingController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;

        public EquipmentSchedulingController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
        }

        /// <summary>
        /// Valida si es permitido el agendamiento de un equipo en un rango de fechas específico y retorna todos los pedidos relacionados
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="request">Parámetros de validación (IdEquipment, DateIni, DateEnd)</param>
        /// <returns>Resultado de la validación de agendamiento con lista de pedidos relacionados</returns>
        [HttpPost("validate")]
        public async Task<ActionResult<EquipmentSchedulingValidationResponseDTO>> ValidateEquipmentScheduling(
            [FromQuery] string companyCode,
            [FromBody] EquipmentSchedulingValidationRequestDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");

                if (request == null)
                    return BadRequest("Los parámetros de validación son requeridos");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Validar que las fechas sean lógicas
                if (request.DateIni > request.DateEnd)
                    return BadRequest("La fecha inicial debe ser anterior a la fecha final");

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);

                // Crear el BO con el contexto de la compañía específica
                var equipmentSchedulingBO = new EquipmentSchedulingBO(companyContext);

                // Ejecutar la validación
                var result = await equipmentSchedulingBO.ValidateEquipmentSchedulingAsync(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new EquipmentSchedulingValidationResponseDTO
                {
                    IsAllowed = false,
                    Message = $"Error interno del servidor: {ex.Message}",
                    IdEquipment = request?.IdEquipment ?? 0,
                    DateIni = request?.DateIni ?? DateTime.MinValue,
                    DateEnd = request?.DateEnd ?? DateTime.MinValue
                });
            }
        }

        /// <summary>
        /// Valida si es permitido el agendamiento de un equipo usando parámetros de query string y retorna todos los pedidos relacionados
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="idEquipment">ID del equipo</param>
        /// <param name="dateIni">Fecha inicial (formato: yyyy-MM-dd)</param>
        /// <param name="dateEnd">Fecha final (formato: yyyy-MM-dd)</param>
        /// <param name="idEntryReq">ID del pedido a excluir de los relacionados (opcional)</param>
        /// <returns>Resultado de la validación de agendamiento con lista de pedidos relacionados</returns>
        [HttpGet("validate")]
        public async Task<ActionResult<EquipmentSchedulingValidationResponseDTO>> ValidateEquipmentSchedulingGet(
            [FromQuery] string companyCode,
            [FromQuery] int idEquipment,
            [FromQuery] string dateIni,
            [FromQuery] string dateEnd,
            [FromQuery] int? idEntryReq = null)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");

                if (idEquipment <= 0)
                    return BadRequest("El ID del equipo debe ser mayor a 0");

                if (string.IsNullOrEmpty(dateIni) || string.IsNullOrEmpty(dateEnd))
                    return BadRequest("Las fechas inicial y final son requeridas");

                // Parsear las fechas
                if (!DateTime.TryParse(dateIni, out DateTime parsedDateIni))
                    return BadRequest("La fecha inicial tiene un formato inválido. Use yyyy-MM-dd");

                if (!DateTime.TryParse(dateEnd, out DateTime parsedDateEnd))
                    return BadRequest("La fecha final tiene un formato inválido. Use yyyy-MM-dd");

                // Validar que las fechas sean lógicas
                if (parsedDateIni > parsedDateEnd)
                    return BadRequest("La fecha inicial debe ser anterior a la fecha final");

                // Crear el request DTO
                var request = new EquipmentSchedulingValidationRequestDTO
                {
                    IdEquipment = idEquipment,
                    DateIni = parsedDateIni,
                    DateEnd = parsedDateEnd,
                    IdEntryReq = idEntryReq
                };

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);

                // Crear el BO con el contexto de la compañía específica
                var equipmentSchedulingBO = new EquipmentSchedulingBO(companyContext);

                // Ejecutar la validación
                var result = await equipmentSchedulingBO.ValidateEquipmentSchedulingAsync(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new EquipmentSchedulingValidationResponseDTO
                {
                    IsAllowed = false,
                    Message = $"Error interno del servidor: {ex.Message}",
                    IdEquipment = idEquipment,
                    DateIni = DateTime.TryParse(dateIni, out DateTime parsedDateIni) ? parsedDateIni : DateTime.MinValue,
                    DateEnd = DateTime.TryParse(dateEnd, out DateTime parsedDateEnd) ? parsedDateEnd : DateTime.MinValue
                });
            }
        }
    }
}
