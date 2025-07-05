using Microsoft.AspNetCore.Mvc;
using DimmedAPI.BO;
using DimmedAPI.Entidades;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DimmedAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentAssemblyAPIController : ControllerBase
    {
        private readonly IntBCConex _bcConn;

        public EquipmentAssemblyAPIController(IntBCConex bcConn)
        {
            _bcConn = bcConn;
        }

        /// <summary>
        /// Obtiene el ensamble de un equipo específico
        /// </summary>
        /// <param name="equipmentCode">Código del equipo</param>
        /// <param name="salesPrice">Código de precio de venta (opcional)</param>
        /// <returns>Lista de componentes del ensamble</returns>
        [HttpGet("assembly/{equipmentCode}")]
        public async Task<IActionResult> GetAssembly(string equipmentCode, [FromQuery] string salesPrice = "")
        {
            try
            {
                var assembly = await _bcConn.GetEntryReqAssembly("lylassembly", equipmentCode, salesPrice);
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
        /// <returns>Lista de componentes del ensamble</returns>
        [HttpGet("assembly-v2/{equipmentCode}")]
        public async Task<IActionResult> GetAssemblyV2(string equipmentCode, [FromQuery] string salesPrice = "")
        {
            try
            {
                var assembly = await _bcConn.GetEntryReqAssembly("lylassemblyV2", equipmentCode, salesPrice);
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
        /// <returns>Lista de líneas de ensamble</returns>
        [HttpGet("assembly-lines/{equipmentCode}")]
        public async Task<IActionResult> GetAssemblyLines(string equipmentCode)
        {
            try
            {
                var assemblyLines = await _bcConn.GetEntryReqAssembly("lylassemblyolines", equipmentCode, "");
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
        /// <returns>Lista de componentes del ensamble de equipo</returns>
        [HttpGet("equipment-assembly/{equipmentCode}")]
        public async Task<IActionResult> GetEquipmentAssembly(string equipmentCode)
        {
            try
            {
                var equipmentAssembly = await _bcConn.GetEntryReqAssembly("lylassemblyeq", equipmentCode, "");
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