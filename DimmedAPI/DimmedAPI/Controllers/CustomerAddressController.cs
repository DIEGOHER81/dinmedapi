using DimmedAPI.Entidades;
using DimmedAPI.Interfaces;
using DimmedAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerAddressController : ControllerBase
    {
        private readonly IDynamicBCConnectionService _bcConnectionService;
        private readonly IDynamicConnectionService _connectionService;

        public CustomerAddressController(IDynamicBCConnectionService bcConnectionService, IDynamicConnectionService connectionService)
        {
            _bcConnectionService = bcConnectionService;
            _connectionService = connectionService;
        }

        /// <summary>
        /// Obtiene todas las direcciones de clientes locales para una compañía
        /// </summary>
        [HttpGet("list/{companyCode}")]
        public async Task<ActionResult<List<CustomerAddress>>> GetAll([FromRoute] string companyCode)
        {
            var dbContext = await _connectionService.GetCompanyDbContextAsync(companyCode);
            var result = await dbContext.CustomerAddress.ToListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Sincroniza una dirección de cliente desde BC (crea o actualiza) para una compañía
        /// </summary>
        [HttpPost("sync/{companyCode}")]
        public async Task<ActionResult<CustomerAddress>> SyncFromBC([FromRoute] string companyCode, [FromQuery] string systemID, [FromQuery] int option)
        {
            var bcConnection = await _bcConnectionService.GetBCConnectionAsync(companyCode);
            var dbContext = await _connectionService.GetCompanyDbContextAsync(companyCode);
            // Aquí deberías mover la lógica de sincronización al BO, pero para mantener el patrón, puedes inyectar el contexto y la conexión dinámica
            // Si tienes un BO adaptado, llama al método correspondiente pasando el contexto y la conexión
            // Por simplicidad, aquí se muestra la lógica directa:
            var customerAddressBO = new BO.CustomerAddressBO(dbContext, bcConnection);
            var result = await customerAddressBO.UpdateAddFromBC(systemID, option);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Obtiene todas las direcciones de clientes desde Business Central (BC) para una compañía
        /// </summary>
        [HttpGet("bc/{companyCode}")]
        public async Task<ActionResult<List<CustomerAddress>>> GetAllFromBC([FromRoute] string companyCode)
        {
            var bcConnection = await _bcConnectionService.GetBCConnectionAsync(companyCode);
            var dbContext = await _connectionService.GetCompanyDbContextAsync(companyCode);
            var customerAddressBO = new BO.CustomerAddressBO(dbContext, bcConnection);
            var result = await customerAddressBO.SinCustAddressBCAsync();
            return Ok(result);
        }

        /// <summary>
        /// Sincroniza todas las direcciones de clientes desde Business Central (BC) y las almacena en la base local para una compañía
        /// </summary>
        [HttpPost("sync-all/{companyCode}")]
        public async Task<ActionResult<List<CustomerAddress>>> SyncAllFromBC([FromRoute] string companyCode)
        {
            var bcConnection = await _bcConnectionService.GetBCConnectionAsync(companyCode);
            var dbContext = await _connectionService.GetCompanyDbContextAsync(companyCode);
            var customerAddressBO = new BO.CustomerAddressBO(dbContext, bcConnection);
            var result = await customerAddressBO.SyncAllFromBCAsync();
            return Ok(result);
        }

        /// <summary>
        /// Obtiene todas las direcciones locales de un cliente específico en una compañía
        /// </summary>
        [HttpGet("detail/{companyCode}/{customerId}")]
        public async Task<ActionResult<List<CustomerAddress>>> GetByCustomerId([FromRoute] string companyCode, [FromRoute] int customerId)
        {
            var dbContext = await _connectionService.GetCompanyDbContextAsync(companyCode);
            var result = await dbContext.CustomerAddress.Where(x => x.CustomerId == customerId).ToListAsync();
            return Ok(result);
        }
    }
} 