using Microsoft.AspNetCore.Mvc;
using DimmedAPI.BO;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DimmedAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentAssemblyAPIController : ControllerBase
    {
        private readonly IDynamicBCConnectionService _dynamicBCConnectionService;

        public EquipmentAssemblyAPIController(IDynamicBCConnectionService dynamicBCConnectionService)
        {
            _dynamicBCConnectionService = dynamicBCConnectionService;
        }

        /// <summary>
        /// Obtiene el ensamble de un equipo específico
        /// </summary>
        /// <param name="equipmentCode">Código del equipo</param>
        /// <param name="salesPrice">Código de precio de venta (opcional)</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Lista de componentes del ensamble</returns>
        [HttpGet("assembly/{equipmentCode}")]
        public async Task<IActionResult> GetAssembly(string equipmentCode, [FromQuery] string companyCode, [FromQuery] string salesPrice = "")
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var assembly = await bcConn.GetEntryReqAssembly("lylassembly", equipmentCode, salesPrice);
                return Ok(assembly);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al obtener el ensamble del equipo", detalle });
            }
        }

        /// <summary>
        /// Obtiene el ensamble de un equipo específico (versión 2)
        /// </summary>
        /// <param name="equipmentCode">Código del equipo</param>
        /// <param name="salesPrice">Código de precio de venta (opcional)</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Lista de componentes del ensamble</returns>
        [HttpGet("assembly-v2/{equipmentCode}")]
        public async Task<IActionResult> GetAssemblyV2(string equipmentCode, [FromQuery] string companyCode, [FromQuery] string salesPrice = "")
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var assembly = await bcConn.GetEntryReqAssembly("lylassemblyV2", equipmentCode, salesPrice);
                return Ok(assembly);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al obtener el ensamble del equipo (V2)", detalle });
            }
        }

        /// <summary>
        /// Obtiene las líneas de ensamble de un equipo específico
        /// </summary>
        /// <param name="equipmentCode">Código del equipo</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Lista de líneas de ensamble</returns>
        [HttpGet("assembly-lines/{equipmentCode}")]
        public async Task<IActionResult> GetAssemblyLines(string equipmentCode, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var assemblyLines = await bcConn.GetEntryReqAssembly("lylassemblyolines", equipmentCode, "");
                return Ok(assemblyLines);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al obtener las líneas de ensamble del equipo", detalle });
            }
        }

        /// <summary>
        /// Obtiene el ensamble de equipo específico
        /// </summary>
        /// <param name="equipmentCode">Código del equipo</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Lista de componentes del ensamble de equipo</returns>
        [HttpGet("equipment-assembly/{equipmentCode}")]
        public async Task<IActionResult> GetEquipmentAssembly(string equipmentCode, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var equipmentAssembly = await bcConn.GetEntryReqAssembly("lylassemblyeq", equipmentCode, "");
                return Ok(equipmentAssembly);
            }
            catch (Exception ex)
            {
                var detalle = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = "Error al obtener el ensamble de equipo", detalle });
            }
        }
    }
} 