using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.AspNetCore.OutputCaching;
using DimmedAPI.Migrations;
using Microsoft.EntityFrameworkCore;
using DimmedAPI.DTOs;


namespace DimmedAPI.Controllers
{

    [Route("api/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private const string cacheTag = "customer";

        public CustomerController(
            IOutputCacheStore outputCacheStore, 
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService)
        {
            this._outputCacheStore = outputCacheStore;
            this.context = context;
            this._dynamicConnectionService = dynamicConnectionService;
        }

        [HttpGet("ObtenerClientes")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var customers = await companyContext.Customer
                    .ToListAsync();

                return Ok(customers);
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

        [HttpGet("test")]
        public ActionResult<string> Test()
        {
            return Ok("CustomerController está funcionando correctamente");
        }

        [HttpGet("ping")]
        public ActionResult<string> Ping()
        {
            return Ok("Pong desde CustomerController");
        }
    }
}
