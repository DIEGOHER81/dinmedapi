using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.AspNetCore.OutputCaching;
using DimmedAPI.Migrations;
using Microsoft.EntityFrameworkCore;
using DimmedAPI.DTOs;

namespace DimmedAPI.Controllers
{
    [Route("api/insurer")]
    [ApiController]
    public class InsurerController:ControllerBase
    {

        
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private const string cacheTag = "insurer";



        public InsurerController(
            IOutputCacheStore outputCacheStore, 
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService)
        {
            this._outputCacheStore = outputCacheStore;
            this.context = context;
            this._dynamicConnectionService = dynamicConnectionService;
        }


        [HttpGet("ObtenerAseguradoras")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<Insurer>>> GetInsurers([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var insurers = await companyContext.Insurer
                    .Include(i => i.InsurerType)
                    .ToListAsync();

                return Ok(insurers);
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



        [HttpGet("ObtenerAseguradoraporCodigo/{code}")]
        public ActionResult<InsurerType> GetIdentificactionbyCode(String code)
        {
            throw new NotImplementedException();
        }



        [HttpGet("{id:int}", Name="ObtenerAseguradoraporId")]
        [OutputCache(Tags = [cacheTag])]
        public ActionResult<InsurerType> GetInsureTypeById(int id)
        {
            throw new NotImplementedException();
        }


        [HttpPost("CrearAseguradora")]
        public async Task<IActionResult> Post([FromBody] InsurerCreateDTO dto, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var insurer = new Insurer
                {
                    Nit = dto.Nit,
                    Name = dto.Name,
                    InsurerTypeId = dto.InsurerTypeId
                };

                companyContext.Add(insurer);
                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return CreatedAtRoute("ObtenerAseguradoraporId", new { id = insurer.Id }, insurer);
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

        [HttpPut("ActualizarAseguradora/{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<IActionResult> Put(int id, [FromBody] Insurer updatedInsurer, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (id != updatedInsurer.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del cuerpo de la solicitud.");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var existingInsurer = await companyContext.Insurer.FindAsync(id);

                if (existingInsurer == null)
                {
                    return NotFound("Aseguradora no encontrado.");
                }

                // Actualizar propiedades
                existingInsurer.Nit = updatedInsurer.Nit;
                existingInsurer.Name = updatedInsurer.Name;
                existingInsurer.InsurerTypeId = updatedInsurer.InsurerTypeId;
                existingInsurer.IsActive = updatedInsurer.IsActive;

                // Guardar cambios
                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return NoContent(); // 204 OK sin contenido
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


        [HttpDelete]
        public void Delete()
        {

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

                return Ok(new
                {
                    Company = new
                    {
                        company.Id,
                        company.BusinessName,
                        company.BCCodigoEmpresa,
                        company.SqlConnectionString
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

    }
}
