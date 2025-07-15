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
        public async Task<ActionResult> Put(int id, [FromBody] CompanyUpdateDTO updateDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("El ID de la compañía debe ser mayor a 0");
                }

                var existing = await context.Companies.FindAsync(id);
                if (existing == null)
                {
                    return NotFound($"Compañía con ID {id} no encontrada");
                }

                // Validar que el tipo de identificación existe
                if (updateDto.IdentificationTypeId > 0)
                {
                    var identificationType = await context.IdentificationTypes.FindAsync(updateDto.IdentificationTypeId);
                    if (identificationType == null)
                    {
                        return BadRequest($"Tipo de identificación con ID {updateDto.IdentificationTypeId} no encontrado");
                    }
                }

                // Actualizar campos
                existing.IdentificationTypeId = updateDto.IdentificationTypeId;
                existing.IdentificationNumber = updateDto.IdentificationNumber;
                existing.BusinessName = updateDto.BusinessName;
                existing.TradeName = updateDto.TradeName;
                existing.MainAddress = updateDto.MainAddress;
                existing.Department = updateDto.Department;
                existing.City = updateDto.City;
                existing.LegalRepresentative = updateDto.LegalRepresentative;
                existing.ContactEmail = updateDto.ContactEmail;
                existing.ContactPhone = updateDto.ContactPhone;
                existing.SqlConnectionString = updateDto.SqlConnectionString;
                existing.BCURLWebService = updateDto.BCURLWebService;
                existing.BCURL = updateDto.BCURL;
                existing.BCCodigoEmpresa = updateDto.BCCodigoEmpresa;
                existing.logoCompany = updateDto.logoCompany;
                existing.instancia = updateDto.instancia;
                existing.dominio = updateDto.dominio;
                existing.clienteid = updateDto.clienteid;
                existing.tenantid = updateDto.tenantid;
                existing.clientsecret = updateDto.clientsecret;
                existing.callbackpath = updateDto.callbackpath;
                existing.correonotificacion = updateDto.correonotificacion;
                existing.nombrenotificacion = updateDto.nombrenotificacion;
                existing.pwdnotificacion = updateDto.pwdnotificacion;
                existing.smtpserver = updateDto.smtpserver;
                existing.puertosmtp = updateDto.puertosmtp?.ToString();
                existing.ModifiedBy = updateDto.ModifiedBy;
                existing.ModifiedAt = DateTime.UtcNow;

                await context.SaveChangesAsync();

                // Invalidar cache
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return Ok(new { message = "Compañía actualizada exitosamente", company = existing });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
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
