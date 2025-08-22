using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Services;
using DimmedAPI.BO;
using DimmedAPI.Entidades;
using DimmedAPI.DTOs;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LotsController : ControllerBase
    {
        private readonly IDynamicBCConnectionService _dynamicBCConnectionService;

        public LotsController(IDynamicBCConnectionService dynamicBCConnectionService)
        {
            _dynamicBCConnectionService = dynamicBCConnectionService;
        }

        /// <summary>
        /// Consultar lotes disponibles de referencia
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="itemCode">Código del artículo/referencia</param>
        /// <param name="locationCode">Código de la bodega</param>
        /// <returns>Lista de lotes disponibles</returns>
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<EntryRequestAssemblyResponseDTO>>> GetAvailableLots(
            [FromQuery] string companyCode,
            [FromQuery] string itemCode,
            [FromQuery] string locationCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");

                if (string.IsNullOrEmpty(itemCode))
                    return BadRequest("El código del artículo es requerido");

                if (string.IsNullOrEmpty(locationCode))
                    return BadRequest("El código de bodega es requerido");

                // Obtener conexión a Business Central para la compañía específica
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);

                // Consultar lotes disponibles
                var lots = await bcConn.GetLotsAdditionals("lyladditionals", itemCode, locationCode);

                if (lots == null || !lots.Any())
                {
                    return Ok(new List<EntryRequestAssemblyResponseDTO>());
                }

                // Mapear a DTO de respuesta
                var lotsResponse = lots.Select(lot => new EntryRequestAssemblyResponseDTO
                {
                    Id = lot.Id,
                    Code = lot.Code,
                    Description = lot.Description,
                    ShortDesc = lot.ShortDesc,
                    Invima = lot.Invima,
                    Lot = lot.Lot,
                    Quantity = lot.Quantity,
                    UnitPrice = lot.UnitPrice,
                    AssemblyNo = lot.AssemblyNo,
                    EntryRequestId = lot.EntryRequestId,
                    EntryRequestDetailId = lot.EntryRequestDetailId,
                    QuantityConsumed = lot.QuantityConsumed,
                    ExpirationDate = lot.ExpirationDate,
                    Name = lot.Name,
                    ReservedQuantity = lot.ReservedQuantity,
                    Location_Code_ile = lot.Location_Code_ile,
                    Classification = lot.Classification,
                    Status = lot.Status,
                    LineNo = lot.LineNo,
                    Position = lot.Position,
                    Quantity_ile = lot.Quantity_ile,
                    TaxCode = lot.TaxCode,
                    LowTurnover = lot.LowTurnover,
                    IsComponent = lot.IsComponent,
                    RSFechaVencimiento = lot.RSFechaVencimiento,
                    RSClasifRegistro = lot.RSClasifRegistro,
                    LocationCode = lot.LocationCode
                }).ToList();

                return Ok(lotsResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Verificar configuración de compañía para lotes
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Información de configuración de la compañía</returns>
        [HttpGet("config")]
        public async Task<ActionResult<object>> VerificarConfiguracionCompania([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                var bcConfig = await _dynamicBCConnectionService.GetBusinessCentralConfigAsync(companyCode);

                return Ok(new
                {
                    BusinessCentral = new
                    {
                        url = bcConfig.url,
                        company = bcConfig.company
                    }
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
