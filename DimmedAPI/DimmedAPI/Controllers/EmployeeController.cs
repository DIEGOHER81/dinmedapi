using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using DimmedAPI.DTOs;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "employee";

        public EmployeeController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService,
            IOutputCacheStore outputCacheStore)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
            _outputCacheStore = outputCacheStore;
        }

        // GET: api/Employee
        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var employees = await companyContext.Employee
                    .ToListAsync();

                return Ok(employees);
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

        // GET: api/Employee/with-quotations-count
        [HttpGet("with-quotations-count")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<EmployeeResponseDTO>>> GetEmployeesWithQuotationsCount([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var employeesWithCount = await companyContext.Employee
                    .Select(e => new EmployeeResponseDTO
                    {
                        Id = e.Id,
                        Code = e.Code,
                        Name = e.Name,
                        Charge = e.Charge,
                        SystemIdBC = e.SystemIdBC,
                        ATC = e.ATC,
                        MResponsible = e.MResponsible,
                        Phone = e.Phone,
                        Email = e.Email,
                        Branch = e.Branch,
                        QuotationsCount = companyContext.QuotationMaster.Count(q => q.FK_idEmployee == e.Id)
                    })
                    .ToListAsync();

                return Ok(employeesWithCount);
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

        // GET: api/Employee/statistics
        [HttpGet("statistics")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<EmployeeStatisticsDTO>>> GetEmployeesStatistics([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var employeesStats = await companyContext.Employee
                    .Select(e => new EmployeeStatisticsDTO
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Charge = e.Charge,
                        Branch = e.Branch,
                        TotalQuotations = companyContext.QuotationMaster.Count(q => q.FK_idEmployee == e.Id),
                        TotalQuotationValue = (decimal)companyContext.QuotationMaster
                            .Where(q => q.FK_idEmployee == e.Id)
                            .Sum(q => q.Total ?? 0),
                        AverageQuotationValue = (decimal)companyContext.QuotationMaster
                            .Where(q => q.FK_idEmployee == e.Id)
                            .Average(q => q.Total ?? 0),
                        LastQuotationDate = companyContext.QuotationMaster
                            .Where(q => q.FK_idEmployee == e.Id)
                            .Max(q => q.CreationDateTime)
                    })
                    .ToListAsync();

                return Ok(employeesStats);
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

        // GET: api/Employee/{id}
        [HttpGet("{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var employee = await companyContext.Employee.FindAsync(id);
                if (employee == null)
                {
                    return NotFound($"No se encontró el empleado con ID {id}");
                }

                return Ok(employee);
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

        // GET: api/Employee/by-name/{name}
        [HttpGet("by-name/{name}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByName(string name, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest("El nombre es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var employees = await companyContext.Employee
                    .Where(e => e.Name.Contains(name))
                    .ToListAsync();

                return Ok(employees);
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

        // GET: api/Employee/by-branch/{branch}
        [HttpGet("by-branch/{branch}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByBranch(string branch, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (string.IsNullOrEmpty(branch))
                {
                    return BadRequest("La sucursal es requerida");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var employees = await companyContext.Employee
                    .Where(e => e.Branch != null && e.Branch.Contains(branch))
                    .ToListAsync();

                return Ok(employees);
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

        // GET: api/Employee/atc-employees
        [HttpGet("atc-employees")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<Employee>>> GetATCEmployees([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var atcEmployees = await companyContext.Employee
                    .Where(e => e.ATC == true)
                    .ToListAsync();

                return Ok(atcEmployees);
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

        // GET: api/Employee/m-responsible-employees
        [HttpGet("m-responsible-employees")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<Employee>>> GetMResponsibleEmployees([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var mResponsibleEmployees = await companyContext.Employee
                    .Where(e => e.MResponsible == true)
                    .ToListAsync();

                return Ok(mResponsibleEmployees);
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

        // POST: api/Employee
        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee([FromBody] EmployeeCreateDTO employeeDto, [FromQuery] string companyCode)
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

                // Verificar si ya existe un empleado con el mismo código (si se proporciona)
                if (!string.IsNullOrEmpty(employeeDto.Code))
                {
                    var existingEmployee = await companyContext.Employee
                        .FirstOrDefaultAsync(e => e.Code == employeeDto.Code);
                    
                    if (existingEmployee != null)
                    {
                        return BadRequest($"Ya existe un empleado con el código {employeeDto.Code}");
                    }
                }

                // Verificar si ya existe un empleado con el mismo SystemIdBC (si se proporciona)
                if (!string.IsNullOrEmpty(employeeDto.SystemIdBC))
                {
                    var existingEmployee = await companyContext.Employee
                        .FirstOrDefaultAsync(e => e.SystemIdBC == employeeDto.SystemIdBC);
                    
                    if (existingEmployee != null)
                    {
                        return BadRequest($"Ya existe un empleado con el SystemIdBC {employeeDto.SystemIdBC}");
                    }
                }

                var employee = new Employee
                {
                    Code = employeeDto.Code,
                    Name = employeeDto.Name,
                    Charge = employeeDto.Charge,
                    SystemIdBC = employeeDto.SystemIdBC,
                    ATC = employeeDto.ATC,
                    MResponsible = employeeDto.MResponsible,
                    Phone = employeeDto.Phone,
                    Email = employeeDto.Email,
                    Branch = employeeDto.Branch
                };

                companyContext.Employee.Add(employee);
                await companyContext.SaveChangesAsync();

                // Invalidar caché
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id, companyCode }, employee);
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

        // PUT: api/Employee/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeUpdateDTO employeeDto, [FromQuery] string companyCode)
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
                
                var employee = await companyContext.Employee.FindAsync(id);
                if (employee == null)
                {
                    return NotFound($"No se encontró el empleado con ID {id}");
                }

                // Verificar si ya existe otro empleado con el mismo código (si se proporciona)
                if (!string.IsNullOrEmpty(employeeDto.Code) && employeeDto.Code != employee.Code)
                {
                    var existingEmployee = await companyContext.Employee
                        .FirstOrDefaultAsync(e => e.Code == employeeDto.Code && e.Id != id);
                    
                    if (existingEmployee != null)
                    {
                        return BadRequest($"Ya existe un empleado con el código {employeeDto.Code}");
                    }
                }

                // Verificar si ya existe otro empleado con el mismo SystemIdBC (si se proporciona)
                if (!string.IsNullOrEmpty(employeeDto.SystemIdBC) && employeeDto.SystemIdBC != employee.SystemIdBC)
                {
                    var existingEmployee = await companyContext.Employee
                        .FirstOrDefaultAsync(e => e.SystemIdBC == employeeDto.SystemIdBC && e.Id != id);
                    
                    if (existingEmployee != null)
                    {
                        return BadRequest($"Ya existe un empleado con el SystemIdBC {employeeDto.SystemIdBC}");
                    }
                }

                // Actualizar propiedades
                employee.Code = employeeDto.Code;
                employee.Name = employeeDto.Name;
                employee.Charge = employeeDto.Charge;
                employee.SystemIdBC = employeeDto.SystemIdBC;
                employee.ATC = employeeDto.ATC;
                employee.MResponsible = employeeDto.MResponsible;
                employee.Phone = employeeDto.Phone;
                employee.Email = employeeDto.Email;
                employee.Branch = employeeDto.Branch;

                await companyContext.SaveChangesAsync();

                // Invalidar caché
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

        // DELETE: api/Employee/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var employee = await companyContext.Employee.FindAsync(id);
                if (employee == null)
                {
                    return NotFound($"No se encontró el empleado con ID {id}");
                }

                // Verificar si el empleado tiene cotizaciones asociadas
                var hasQuotations = await companyContext.QuotationMaster
                    .AnyAsync(q => q.FK_idEmployee == id);

                if (hasQuotations)
                {
                    return BadRequest("No se puede eliminar el empleado porque tiene cotizaciones asociadas");
                }

                companyContext.Employee.Remove(employee);
                await companyContext.SaveChangesAsync();

                // Invalidar caché
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

        // GET: api/Employee/{id}/quotations
        [HttpGet("{id}/quotations")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<object>>> GetEmployeeQuotations(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var employee = await companyContext.Employee.FindAsync(id);
                if (employee == null)
                {
                    return NotFound($"No se encontró el empleado con ID {id}");
                }

                var quotations = await companyContext.QuotationMaster
                    .Where(q => q.FK_idEmployee == id)
                    .Select(q => new
                    {
                        q.Id,
                        q.IdCustomer,
                        q.CreationDateTime,
                        q.DueDate,
                        q.TotalizingQuotation,
                        q.EquipmentRemains
                    })
                    .ToListAsync();

                return Ok(quotations);
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

        // GET: api/Employee/{id}/active-quotations
        [HttpGet("{id}/active-quotations")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<object>>> GetEmployeeActiveQuotations(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var employee = await companyContext.Employee.FindAsync(id);
                if (employee == null)
                {
                    return NotFound($"No se encontró el empleado con ID {id}");
                }

                var activeQuotations = await companyContext.QuotationMaster
                    .Where(q => q.FK_idEmployee == id)
                    .Select(q => new
                    {
                        q.Id,
                        q.IdCustomer,
                        q.CreationDateTime,
                        q.DueDate,
                        q.TotalizingQuotation,
                        q.EquipmentRemains
                    })
                    .ToListAsync();

                return Ok(activeQuotations);
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

        // GET: api/Employee/{id}/quotations-with-details
        [HttpGet("{id}/quotations-with-details")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<EmployeeWithQuotationsDTO>> GetEmployeeWithQuotations(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var employee = await companyContext.Employee.FindAsync(id);
                if (employee == null)
                {
                    return NotFound($"No se encontró el empleado con ID {id}");
                }

                var quotations = await companyContext.QuotationMaster
                    .Where(q => q.FK_idEmployee == id)
                    .Select(q => new QuotationSummaryDTO
                    {
                        Id = q.Id,
                        IdCustomer = q.IdCustomer,
                        CreationDateTime = q.CreationDateTime,
                        DueDate = q.DueDate,
                        FK_idEmployee = q.FK_idEmployee,
                        TotalizingQuotation = q.TotalizingQuotation,
                        EquipmentRemains = q.EquipmentRemains
                    })
                    .ToListAsync();

                var result = new EmployeeWithQuotationsDTO
                {
                    Id = employee.Id,
                    Code = employee.Code,
                    Name = employee.Name,
                    Charge = employee.Charge,
                    SystemIdBC = employee.SystemIdBC,
                    ATC = employee.ATC,
                    MResponsible = employee.MResponsible,
                    Phone = employee.Phone,
                    Email = employee.Email,
                    Branch = employee.Branch,
                    Quotations = quotations
                };

                return Ok(result);
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

        // GET: api/Employee/VerificarConfiguracionCompania
        [HttpGet("VerificarConfiguracionCompania")]
        public async Task<ActionResult<object>> VerificarConfiguracionCompania([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var employeeCount = await companyContext.Employee.CountAsync();
                var atcEmployeeCount = await companyContext.Employee.CountAsync(e => e.ATC == true);
                var mResponsibleEmployeeCount = await companyContext.Employee.CountAsync(e => e.MResponsible == true);

                return Ok(new
                {
                    TotalEmployees = employeeCount,
                    ATCEmployees = atcEmployeeCount,
                    MResponsibleEmployees = mResponsibleEmployeeCount,
                    ConfigurationStatus = employeeCount > 0 ? "Configurado" : "No configurado"
                });
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

        private async Task<bool> EmployeeExists(int id, ApplicationDBContext context)
        {
            return await context.Employee.AnyAsync(e => e.Id == id);
        }
    }
} 