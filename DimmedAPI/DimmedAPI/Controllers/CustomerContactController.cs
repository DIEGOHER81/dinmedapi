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
        /// Sincroniza datos de contacto de cliente desde BC: si existe actualiza, si no existe crea el registro.
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="systemID">SystemId del registro en BC</param>
        /// <returns>Objeto CustomerContact y acción realizada (created, updated)</returns>
        [HttpGet("sync/{companyCode}")]
        public async Task<IActionResult> SyncCustomerContact(
            [FromRoute] string companyCode,
            [FromQuery] string systemID)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");
                if (string.IsNullOrEmpty(systemID))
                    return BadRequest("El SystemId es requerido");

                var bcConnection = await _bcConnectionService.GetBCConnectionAsync(companyCode);
                var dbContext = await _connectionService.GetCompanyDbContextAsync(companyCode);

                var existingContact = await dbContext.CustomerContact
                    .FirstOrDefaultAsync(c => c.systemIdBC == systemID);

                // Siempre usa el método fijo "lylcustcontact"
                var customerContact = await bcConnection.getCustContBCAsync("lylcustcontact", systemID);

                if (customerContact == null)
                {
                    return NotFound($"No se encontró el contacto del cliente con SystemId: {systemID}");
                }

                if (existingContact != null)
                {
                    // Actualizar
                    existingContact.Code = customerContact.Code;
                    existingContact.Name = customerContact.Name;
                    existingContact.Email = customerContact.Email;
                    existingContact.Phone = customerContact.Phone;
                    existingContact.CustomerName = customerContact.CustomerName;
                    existingContact.Identification = customerContact.Identification;
                    await dbContext.SaveChangesAsync();
                    return Ok(new { message = "Contacto actualizado", contact = existingContact, action = "updated" });
                }
                else
                {
                    // Crear
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
                    return Ok(new { message = "Contacto creado", contact = newContact, action = "created" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Sincroniza todos los contactos de clientes desde Business Central
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Lista de contactos sincronizados</returns>
        [HttpPost("sincronizar-bc")]
        public async Task<IActionResult> SincronizarDesdeBC([FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            var bcConnection = await _bcConnectionService.GetBCConnectionAsync(companyCode);
            var dbContext = await _connectionService.GetCompanyDbContextAsync(companyCode);

            var contactosBC = await bcConnection.GetCustContListAsync("lylcustcontact");
            if (contactosBC == null)
                return StatusCode(500, "No se pudieron obtener los contactos desde Business Central");

            var contactosActualizados = new List<CustomerContact>();
            foreach (var contactoBC in contactosBC)
            {
                var existing = await dbContext.CustomerContact.FirstOrDefaultAsync(c => c.systemIdBC == contactoBC.systemIdBC);
                if (existing == null)
                {
                    dbContext.CustomerContact.Add(contactoBC);
                    contactosActualizados.Add(contactoBC);
                }
                else
                {
                    existing.Code = contactoBC.Code;
                    existing.Name = contactoBC.Name;
                    existing.Identification = contactoBC.Identification;
                    existing.Phone = contactoBC.Phone;
                    existing.CustomerName = contactoBC.CustomerName;
                    existing.Email = contactoBC.Email;
                    contactosActualizados.Add(existing);
                }
            }
            await dbContext.SaveChangesAsync();
            return Ok(contactosActualizados);
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