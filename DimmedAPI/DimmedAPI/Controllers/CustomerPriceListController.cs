using DimmedAPI.DTOs;
using DimmedAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerPriceListController : ControllerBase
    {
        private readonly ICustomerPriceListBO _customerPriceListBO;
        private readonly ILogger<CustomerPriceListController> _logger;

        public CustomerPriceListController(ICustomerPriceListBO customerPriceListBO, ILogger<CustomerPriceListController> logger)
        {
            _customerPriceListBO = customerPriceListBO;
            _logger = logger;
        }

        /// <summary>
        /// SUPLE TI: Obtiene las listas de precio asociadas a un cliente específico
        /// </summary>
        /// <param name="customerId">ID del cliente</param>
        /// <returns>Lista de precios asociadas al cliente</returns>
        /// <response code="200">Lista de precios obtenida exitosamente</response>
        /// <response code="400">ID de cliente inválido</response>
        /// <response code="404">Cliente no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(typeof(List<CustomerPriceListResponseDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetPriceListByCustomerId(int customerId)
        {
            try
            {
                if (customerId <= 0)
                {
                    _logger.LogWarning("Se intentó obtener listas de precio con ID de cliente inválido: {CustomerId}", customerId);
                    return BadRequest("El ID del cliente debe ser mayor a 0");
                }

                _logger.LogInformation("Obteniendo listas de precio para el cliente {CustomerId}", customerId);

                var priceLists = await _customerPriceListBO.GetPriceListByCustomerIdAsync(customerId);

                if (priceLists == null || !priceLists.Any())
                {
                    _logger.LogInformation("No se encontraron listas de precio para el cliente {CustomerId}", customerId);
                    return NotFound($"No se encontraron listas de precio para el cliente con ID {customerId}");
                }

                _logger.LogInformation("Se obtuvieron {Count} listas de precio para el cliente {CustomerId}", priceLists.Count, customerId);
                return Ok(priceLists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener listas de precio para el cliente {CustomerId}", customerId);
                return StatusCode((int)HttpStatusCode.InternalServerError, 
                    $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
