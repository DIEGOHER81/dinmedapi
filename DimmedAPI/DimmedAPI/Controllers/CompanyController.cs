using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.AspNetCore.OutputCaching;
using DimmedAPI.Migrations;
using Microsoft.EntityFrameworkCore;
using DimmedAPI.DTOs;


namespace DimmedAPI.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompanyController : ControllerBase
    {

        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private const string cacheTag = "companies";

        public CompanyController(
            IOutputCacheStore outputCacheStore, 
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService)
        {
            this._outputCacheStore = outputCacheStore;
            this.context = context;
            this._dynamicConnectionService = dynamicConnectionService;
        }


        [HttpGet("ObtenerCompanies")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<Companies>>> GetCompanies()
        {
            try
            {
                // Este endpoint usa la base de datos principal para obtener todas las compañías
                var companies = await context.Companies
                    .Include(c => c.IdentificationType)
                    .ToListAsync();

                return Ok(companies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("ObtenerCompanyPorCodigo")]
        public async Task<ActionResult<Companies>> GetCompanyByCode([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                var company = await _dynamicConnectionService.GetCompanyByCodeAsync(companyCode);
                if (company == null)
                {
                    return NotFound($"Compañía con código {companyCode} no encontrada");
                }

                return Ok(company);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("VerificarConfiguracionCompania")]
        public async Task<ActionResult<object>> VerificarConfiguracionCompania([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                var company = await _dynamicConnectionService.GetCompanyByCodeAsync(companyCode);
                if (company == null)
                {
                    return NotFound($"Compañía con código {companyCode} no encontrada");
                }

                var bcConfig = await _dynamicConnectionService.GetBusinessCentralConfigAsync(companyCode);

                return Ok(new
                {
                    Company = new
                    {
                        company.Id,
                        company.BusinessName,
                        company.BCCodigoEmpresa,
                        company.SqlConnectionString
                    },
                    BusinessCentral = new
                    {
                        urlWS = bcConfig.urlWS,
                        url = bcConfig.url,
                        company = bcConfig.company
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Companies> Get(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult Post([FromBody] Companies company)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Companies updated)
        {
            //var existing = _context.Companies.Find(id);
            //if (existing == null) return NotFound();

            //// Actualizar campos manualmente (puedes mejorar con AutoMapper)
            //existing.IdentificationTypeId = updated.IdentificationTypeId;
            //existing.IdentificationNumber = updated.IdentificationNumber;
            //existing.BusinessName = updated.BusinessName;
            //existing.TradeName = updated.TradeName;
            //existing.MainAddress = updated.MainAddress;
            //existing.Department = updated.Department;
            //existing.City = updated.City;
            //existing.LegalRepresentative = updated.LegalRepresentative;
            //existing.ContactEmail = updated.ContactEmail;
            //existing.ContactPhone = updated.ContactPhone;
            //existing.SqlConnectionString = updated.SqlConnectionString;
            //existing.ModifiedBy = updated.ModifiedBy;
            //existing.ModifiedAt = DateTime.UtcNow;

            //_context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            //var company = _context.Companies.Find(id);
            //if (company == null) return NotFound();

            //_context.Remove(company);
            //_context.SaveChanges();
            return NoContent();
        }
        
    }
}
