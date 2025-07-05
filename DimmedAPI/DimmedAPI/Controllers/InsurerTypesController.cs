using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.AspNetCore.OutputCaching;
using DimmedAPI.Migrations;
using Microsoft.EntityFrameworkCore;

namespace DimmedAPI.Controllers
{
    [Route("api/insurertype")]
    [ApiController]
    public class InsurerTypesController:ControllerBase
    {

        
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private const string cacheTag = "insurertype";



        public InsurerTypesController(
            IOutputCacheStore outputCacheStore, 
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService)
        {
            this._outputCacheStore = outputCacheStore;
            this.context = context;
            this._dynamicConnectionService = dynamicConnectionService;
        }

        [HttpGet("ObtenerTiposAseguradora")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<List<InsurerType>>> Get([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var tipos = await companyContext.InsurerType
                            //.Where(i => i.isActive) 
                            .ToListAsync();

                return Ok(tipos);
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


        [HttpGet("ObtenerTiposAseguradoraporCodigo/{code}")]
        public ActionResult<InsurerType> GetIdentificactionbyCode(String code)
        {
            throw new NotImplementedException();
        }


        //[HttpGet("ObtenerTiposporNombre/{name}")]
        //[OutputCache]
        //public async Task<ActionResult<InsurerType>> GetIdentificactionbyName(String name)
        //{
        //    throw new NotImplementedException();
        //}


        [HttpGet("{id:int}", Name="ObtenerTiposAseguradoraporId")]
        [OutputCache(Tags = [cacheTag])]
        public ActionResult<InsurerType> GetInsureTypeById(int id)
        {
            throw new NotImplementedException();
        }


        [HttpPost("CrearTipoAseguradora")]
        public async Task<IActionResult> Post([FromBody]InsurerType _insurerType, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                companyContext.Add(_insurerType);
                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);
                return CreatedAtRoute("ObtenerTiposAseguradoraporId", new { id = _insurerType.Id }, _insurerType);
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

        [HttpPut("ActualizarTipoAseguradora/{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<IActionResult> Put(int id, [FromBody] InsurerType updatedInsurerType, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (id != updatedInsurerType.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del cuerpo de la solicitud.");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var existingInsurerType = await companyContext.InsurerType.FindAsync(id);

                if (existingInsurerType == null)
                {
                    return NotFound("Tipo de aseguradora no encontrado.");
                }

                // Actualizar propiedades
                existingInsurerType.description = updatedInsurerType.description;
                existingInsurerType.isActive = updatedInsurerType.isActive;

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
