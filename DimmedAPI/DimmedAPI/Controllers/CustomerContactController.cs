using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Services;
using DimmedAPI.BO;
using DimmedAPI.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerContactController : ControllerBase
    {
        private readonly IDynamicBCConnectionService _bcConnectionService;
        private readonly IDynamicConnectionService _connectionService;

        public CustomerContactController(
            IDynamicBCConnectionService bcConnectionService,
            IDynamicConnectionService connectionService)
        {
            _bcConnectionService = bcConnectionService;
            _connectionService = connectionService;
        }

        /// <summary>
        /// Sincroniza un contacto de cliente desde Business Central
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="method">Método de Business Central</param>
        /// <param name="systemID">SystemId del registro en Business Central</param>
        /// <returns>Información del contacto del cliente</returns>
        [HttpGet("sync/{companyCode}")]
        public async Task<IActionResult> SyncCustomerContact(
            [FromRoute] string companyCode,
            [FromQuery] string method,
            [FromQuery] string systemID)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (string.IsNullOrEmpty(method))
                {
                    return BadRequest("El método de Business Central es requerido");
                }

                if (string.IsNullOrEmpty(systemID))
                {
                    return BadRequest("El SystemId es requerido");
                }

                // Obtener la conexión a Business Central para la compañía específica
                var bcConnection = await _bcConnectionService.GetBCConnectionAsync(companyCode);

                // Consultar el contacto del cliente en Business Central
                var customerContact = await bcConnection.getCustContBCAsync(method, systemID);

                if (customerContact == null)
                {
                    return NotFound($"No se encontró el contacto del cliente con SystemId: {systemID}");
                }

                // Obtener el contexto de base de datos para la compañía específica
                var dbContext = await _connectionService.GetCompanyDbContextAsync(companyCode);

                // Verificar si el contacto ya existe en la base de datos local
                var existingContact = await dbContext.CustomerContact
                    .FirstOrDefaultAsync(c => c.systemIdBC == customerContact.systemIdBC);

                if (existingContact != null)
                {
                    // Actualizar el contacto existente
                    existingContact.Code = customerContact.Code;
                    existingContact.Name = customerContact.Name;
                    existingContact.Email = customerContact.Email;
                    existingContact.Phone = customerContact.Phone;
                    existingContact.CustomerName = customerContact.CustomerName;
                    existingContact.Identification = customerContact.Identification;

                    await dbContext.SaveChangesAsync();

                    return Ok(new
                    {
                        message = "Contacto del cliente actualizado exitosamente",
                        contact = existingContact,
                        action = "updated"
                    });
                }
                else
                {
                    // Crear nuevo contacto
                    var newContact = new CustomerContact
                    {
                        Code = customerContact.Code,
                        Name = customerContact.Name,
                        Email = customerContact.Email,
                        systemIdBC = customerContact.systemIdBC,
                        Phone = customerContact.Phone,
                        CustomerName = customerContact.CustomerName,
                        Identification = customerContact.Identification
                    };

                    dbContext.CustomerContact.Add(newContact);
                    await dbContext.SaveChangesAsync();

                    return Ok(new
                    {
                        message = "Contacto del cliente sincronizado exitosamente",
                        contact = newContact,
                        action = "created"
                    });
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene todos los contactos de clientes de una compañía
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Lista de contactos de clientes</returns>
        [HttpGet("list/{companyCode}")]
        public async Task<IActionResult> GetCustomerContacts([FromRoute] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                var dbContext = await _connectionService.GetCompanyDbContextAsync(companyCode);
                var contacts = await dbContext.CustomerContact.ToListAsync();

                return Ok(contacts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene un contacto de cliente específico por ID
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="id">ID del contacto</param>
        /// <returns>Contacto del cliente</returns>
        [HttpGet("detail/{companyCode}/{id}")]
        public async Task<IActionResult> GetCustomerContact([FromRoute] string companyCode, [FromRoute] int id)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                var dbContext = await _connectionService.GetCompanyDbContextAsync(companyCode);
                var contact = await dbContext.CustomerContact.FindAsync(id);

                if (contact == null)
                {
                    return NotFound($"No se encontró el contacto con ID: {id}");
                }

                return Ok(contact);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Busca contactos de clientes por nombre o código
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="searchTerm">Término de búsqueda</param>
        /// <returns>Lista de contactos que coinciden con la búsqueda</returns>
        [HttpGet("search/{companyCode}")]
        public async Task<IActionResult> SearchCustomerContacts(
            [FromRoute] string companyCode,
            [FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (string.IsNullOrEmpty(searchTerm))
                {
                    return BadRequest("El término de búsqueda es requerido");
                }

                var dbContext = await _connectionService.GetCompanyDbContextAsync(companyCode);
                var contacts = await dbContext.CustomerContact
                    .Where(c => c.Name.Contains(searchTerm) || c.Code.Contains(searchTerm))
                    .ToListAsync();

                return Ok(contacts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Busca contactos de clientes por nombre del cliente (customerName)
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="customerName">Nombre del cliente a buscar</param>
        /// <returns>Lista de contactos que coinciden con el nombre del cliente</returns>
        [HttpGet("searchByCustomerName/{companyCode}")]
        public async Task<IActionResult> SearchCustomerContactsByCustomerName(
            [FromRoute] string companyCode,
            [FromQuery] string customerName)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (string.IsNullOrEmpty(customerName))
                {
                    return BadRequest("El nombre del cliente es requerido");
                }

                var dbContext = await _connectionService.GetCompanyDbContextAsync(companyCode);
                var contacts = await dbContext.CustomerContact
                    .Where(c => c.CustomerName.Contains(customerName))
                    .ToListAsync();

                return Ok(contacts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina un contacto de cliente
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="id">ID del contacto a eliminar</param>
        /// <returns>Resultado de la eliminación</returns>
        [HttpDelete("delete/{companyCode}/{id}")]
        public async Task<IActionResult> DeleteCustomerContact([FromRoute] string companyCode, [FromRoute] int id)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                var dbContext = await _connectionService.GetCompanyDbContextAsync(companyCode);
                var contact = await dbContext.CustomerContact.FindAsync(id);

                if (contact == null)
                {
                    return NotFound($"No se encontró el contacto con ID: {id}");
                }

                dbContext.CustomerContact.Remove(contact);
                await dbContext.SaveChangesAsync();

                return Ok(new { message = "Contacto eliminado exitosamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
} 