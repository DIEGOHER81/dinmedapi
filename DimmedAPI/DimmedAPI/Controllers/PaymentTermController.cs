using DimmedAPI.BO;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentTermController : ControllerBase
    {
        private readonly IDynamicBCConnectionService _dynamicBCConnectionService;
        private readonly IDynamicConnectionService _dynamicConnectionService;

        public PaymentTermController(
            IDynamicBCConnectionService dynamicBCConnectionService,
            IDynamicConnectionService dynamicConnectionService)
        {
            _dynamicBCConnectionService = dynamicBCConnectionService;
            _dynamicConnectionService = dynamicConnectionService;
        }

        // Sincroniza desde BC a local
        [HttpPost("sincronizar-bc")]
        public async Task<IActionResult> SincronizarDesdeBC([FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
            var paymentTermBO = new PaymentTermBO(companyContext, bcConn);

            var result = await paymentTermBO.SincronizarDesdeBCAsync();
            return Ok(result);
        }

        // Consulta local
        [HttpGet]
        public async Task<ActionResult<List<PaymentTerm>>> GetAll([FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var paymentTermBO = new PaymentTermBO(companyContext, null);

            var result = await paymentTermBO.GetAllAsync();
            return Ok(result);
        }
    }
} 