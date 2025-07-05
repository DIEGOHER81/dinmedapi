using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace DimmedAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteLeadController : ControllerBase
    {
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private const string cacheTag = "clientelead";

        public ClienteLeadController(
            IOutputCacheStore outputCacheStore,
            IDynamicConnectionService dynamicConnectionService)
        {
            _outputCacheStore = outputCacheStore;
            _dynamicConnectionService = dynamicConnectionService;
        }

        /// <summary>
        /// Obtiene todos los clientes lead
        /// </summary>
        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<ClienteLead>>> GetClienteLeads([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var clienteLeads = await companyContext.ClienteLead
                    .Include(c => c.IdentificationType)
                    .ToListAsync();

                return Ok(clienteLeads);
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

        /// <summary>
        /// Obtiene un cliente lead por ID
        /// </summary>
        [HttpGet("{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<ClienteLead>> GetClienteLead(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var clienteLead = await companyContext.ClienteLead
                    .Include(c => c.IdentificationType)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (clienteLead == null)
                {
                    return NotFound($"Cliente lead con ID {id} no encontrado");
                }

                return Ok(clienteLead);
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

        /// <summary>
        /// Obtiene un cliente lead por número de identificación
        /// </summary>
        [HttpGet("por-identificacion")]
        public async Task<ActionResult<ClienteLead>> GetClienteLeadPorIdentificacion(
            [FromQuery] string companyCode, 
            [FromQuery] string numeroIdentificacion)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (string.IsNullOrEmpty(numeroIdentificacion))
                {
                    return BadRequest("El número de identificación es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var clienteLead = await companyContext.ClienteLead
                    .Include(c => c.IdentificationType)
                    .FirstOrDefaultAsync(c => c.NumeroIdentificacion == numeroIdentificacion);

                if (clienteLead == null)
                {
                    return NotFound($"Cliente lead con identificación {numeroIdentificacion} no encontrado");
                }

                return Ok(clienteLead);
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

        /// <summary>
        /// Crea un nuevo cliente lead
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ClienteLead>> CreateClienteLead([FromBody] ClienteLead clienteLead, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Verificar si ya existe un cliente lead con el mismo número de identificación
                var existingClienteLead = await companyContext.ClienteLead
                    .FirstOrDefaultAsync(c => c.NumeroIdentificacion == clienteLead.NumeroIdentificacion);

                if (existingClienteLead != null)
                {
                    return BadRequest($"Ya existe un cliente lead con el número de identificación {clienteLead.NumeroIdentificacion}");
                }

                companyContext.ClienteLead.Add(clienteLead);
                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return CreatedAtAction(nameof(GetClienteLead), new { id = clienteLead.Id, companyCode }, clienteLead);
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

        /// <summary>
        /// Actualiza un cliente lead existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClienteLead(int id, [FromBody] ClienteLead clienteLead, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (id != clienteLead.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del cuerpo de la solicitud");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var existingClienteLead = await companyContext.ClienteLead.FindAsync(id);

                if (existingClienteLead == null)
                {
                    return NotFound($"Cliente lead con ID {id} no encontrado");
                }

                // Verificar si el nuevo número de identificación ya existe en otro registro
                var duplicateClienteLead = await companyContext.ClienteLead
                    .FirstOrDefaultAsync(c => c.NumeroIdentificacion == clienteLead.NumeroIdentificacion && c.Id != id);

                if (duplicateClienteLead != null)
                {
                    return BadRequest($"Ya existe otro cliente lead con el número de identificación {clienteLead.NumeroIdentificacion}");
                }

                // Actualizar propiedades
                existingClienteLead.TipoIdentificacion = clienteLead.TipoIdentificacion;
                existingClienteLead.NumeroIdentificacion = clienteLead.NumeroIdentificacion;
                existingClienteLead.RazonSocial = clienteLead.RazonSocial;
                existingClienteLead.Email = clienteLead.Email;
                existingClienteLead.Telefono = clienteLead.Telefono;
                existingClienteLead.Ciudad = clienteLead.Ciudad;
                existingClienteLead.Direccion = clienteLead.Direccion;
                existingClienteLead.RepresentanteComercial = clienteLead.RepresentanteComercial;
                existingClienteLead.PriceGroup = clienteLead.PriceGroup;
                existingClienteLead.Observaciones = clienteLead.Observaciones;
                existingClienteLead.Horario_LV = clienteLead.Horario_LV;
                existingClienteLead.Horario_SD = clienteLead.Horario_SD;

                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return NoContent();
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

        /// <summary>
        /// Elimina un cliente lead
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClienteLead(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var clienteLead = await companyContext.ClienteLead.FindAsync(id);

                if (clienteLead == null)
                {
                    return NotFound($"Cliente lead con ID {id} no encontrado");
                }

                companyContext.ClienteLead.Remove(clienteLead);
                await companyContext.SaveChangesAsync();
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return NoContent();
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

        /// <summary>
        /// Busca clientes lead por criterios
        /// </summary>
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<ClienteLead>>> BuscarClienteLeads(
            [FromQuery] string companyCode,
            [FromQuery] string? razonSocial = null,
            [FromQuery] string? email = null,
            [FromQuery] string? representanteComercial = null)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var query = companyContext.ClienteLead
                    .Include(c => c.IdentificationType)
                    .AsQueryable();

                // Aplicar filtros si se proporcionan
                if (!string.IsNullOrEmpty(razonSocial))
                {
                    query = query.Where(c => c.RazonSocial.Contains(razonSocial));
                }

                if (!string.IsNullOrEmpty(email))
                {
                    query = query.Where(c => c.Email.Contains(email));
                }

                if (!string.IsNullOrEmpty(representanteComercial))
                {
                    query = query.Where(c => c.RepresentanteComercial != null && c.RepresentanteComercial.Contains(representanteComercial));
                }

                var clienteLeads = await query.ToListAsync();

                return Ok(clienteLeads);
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

        /// <summary>
        /// Obtiene estadísticas de clientes lead
        /// </summary>
        [HttpGet("estadisticas")]
        public async Task<ActionResult<object>> GetEstadisticas([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var totalClientesLead = await companyContext.ClienteLead.CountAsync();
                var clientesPorCiudad = await companyContext.ClienteLead
                    .Where(c => !string.IsNullOrEmpty(c.Ciudad))
                    .GroupBy(c => c.Ciudad)
                    .Select(g => new { Ciudad = g.Key, Cantidad = g.Count() })
                    .ToListAsync();

                var estadisticas = new
                {
                    totalClientesLead = totalClientesLead,
                    clientesPorCiudad = clientesPorCiudad
                };

                return Ok(estadisticas);
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

        /// <summary>
        /// Verifica la configuración de la compañía
        /// </summary>
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