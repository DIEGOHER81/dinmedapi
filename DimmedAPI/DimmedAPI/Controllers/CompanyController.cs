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
        public async Task<ActionResult<Companies>> Get(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("El ID de la compañía debe ser mayor a 0");
                }

                var company = await context.Companies
                    .Include(c => c.IdentificationType)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (company == null)
                {
                    return NotFound($"Compañía con ID {id} no encontrada");
                }

                return Ok(company);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Companies>> Post([FromBody] CompanyCreateDTO createDto)
        {
            try
            {
                // Validar que el tipo de identificación existe
                if (createDto.IdentificationTypeId > 0)
                {
                    var identificationType = await context.IdentificationTypes.FindAsync(createDto.IdentificationTypeId);
                    if (identificationType == null)
                    {
                        return BadRequest($"Tipo de identificación con ID {createDto.IdentificationTypeId} no encontrado");
                    }
                }

                // Verificar si ya existe una compañía con el mismo número de identificación
                var existingCompany = await context.Companies
                    .FirstOrDefaultAsync(c => c.IdentificationNumber == createDto.IdentificationNumber);
                
                if (existingCompany != null)
        {
                    return BadRequest($"Ya existe una compañía con el número de identificación {createDto.IdentificationNumber}");
                }

                // Crear nueva compañía
                var newCompany = new Companies
                {
                    IdentificationTypeId = createDto.IdentificationTypeId,
                    IdentificationNumber = createDto.IdentificationNumber,
                    BusinessName = createDto.BusinessName,
                    TradeName = createDto.TradeName,
                    MainAddress = createDto.MainAddress,
                    Department = createDto.Department,
                    City = createDto.City,
                    LegalRepresentative = createDto.LegalRepresentative,
                    ContactEmail = createDto.ContactEmail,
                    ContactPhone = createDto.ContactPhone,
                    SqlConnectionString = createDto.SqlConnectionString,
                    BCURLWebService = createDto.BCURLWebService,
                    BCURL = createDto.BCURL,
                    BCCodigoEmpresa = createDto.BCCodigoEmpresa,
                    logoCompany = createDto.logoCompany,
                    instancia = createDto.instancia,
                    dominio = createDto.dominio,
                    clienteid = createDto.clienteid,
                    tenantid = createDto.tenantid,
                    clientsecret = createDto.clientsecret,
                    callbackpath = createDto.callbackpath,
                    correonotificacion = createDto.correonotificacion,
                    nombrenotificacion = createDto.nombrenotificacion,
                    pwdnotificacion = createDto.pwdnotificacion,
                    smtpserver = createDto.smtpserver,
                    puertosmtp = createDto.puertosmtp,
                    CreatedBy = createDto.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                context.Companies.Add(newCompany);
                await context.SaveChangesAsync();

                // Invalidar cache
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                // Retornar la compañía creada con el ID generado
                return CreatedAtAction(nameof(Get), new { id = newCompany.Id }, newCompany);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
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

        [HttpPost("upload-logo")]
        public async Task<ActionResult<LogoUploadResponseDTO>> UploadLogo(IFormFile file, [FromQuery] int companyId)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new LogoUploadResponseDTO
                    {
                        Success = false,
                        Message = "No se ha proporcionado ningún archivo"
                    });
                }

                // Validar que la compañía existe
                var company = await context.Companies.FindAsync(companyId);
                if (company == null)
                {
                    return NotFound(new LogoUploadResponseDTO
                    {
                        Success = false,
                        Message = $"Compañía con ID {companyId} no encontrada"
                    });
                }

                // Validar tipo de archivo
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new LogoUploadResponseDTO
                    {
                        Success = false,
                        Message = $"Tipo de archivo no permitido. Solo se permiten: {string.Join(", ", allowedExtensions)}"
                    });
                }

                // Validar tamaño del archivo (máximo 5MB)
                const long maxFileSize = 5 * 1024 * 1024; // 5MB
                if (file.Length > maxFileSize)
                {
                    return BadRequest(new LogoUploadResponseDTO
                    {
                        Success = false,
                        Message = "El archivo es demasiado grande. El tamaño máximo permitido es 5MB"
                    });
                }

                // Generar nombre único para el archivo
                var fileName = $"logo_{companyId}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "logos");
                
                // Crear directorio si no existe
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                // Guardar el archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Actualizar la ruta del logo en la base de datos
                var logoUrl = $"/uploads/logos/{fileName}";
                company.logoCompany = logoUrl;
                company.ModifiedAt = DateTime.UtcNow;
                
                await context.SaveChangesAsync();

                // Invalidar cache
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return Ok(new LogoUploadResponseDTO
                {
                    Success = true,
                    Message = "Logo subido exitosamente",
                    LogoUrl = logoUrl,
                    FileName = fileName,
                    FileSize = file.Length,
                    ContentType = file.ContentType
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new LogoUploadResponseDTO
                {
                    Success = false,
                    Message = $"Error interno del servidor: {ex.Message}"
                });
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
