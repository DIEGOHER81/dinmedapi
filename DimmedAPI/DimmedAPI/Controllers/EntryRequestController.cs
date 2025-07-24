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
    public class EntryRequestController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "entryrequest";

        public EntryRequestController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService,
            IOutputCacheStore outputCacheStore)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
            _outputCacheStore = outputCacheStore;
        }

        // GET: api/EntryRequest
        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<EntryRequestResponseDTO>>> GetAllEntryRequests([FromQuery] string companyCode)
        {
            try
            {
                Console.WriteLine($"=== INICIO GetAllEntryRequests ===");
                Console.WriteLine($"CompanyCode recibido: {companyCode}");
                
                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                Console.WriteLine("Creando contexto de base de datos...");
                
                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");
                
                Console.WriteLine("Verificando conexión a la base de datos...");
                
                // Primero verificamos que podemos acceder a la tabla
                var totalCount = await companyContext.EntryRequests.CountAsync();
                Console.WriteLine($"Total de registros en EntryRequests: {totalCount}");
                
                if (totalCount == 0)
                {
                    Console.WriteLine("No hay registros en EntryRequests");
                    return Ok(new List<EntryRequestResponseDTO>());
                }
                
                Console.WriteLine("Iniciando consulta con mapeo...");
                
                // Intentamos obtener solo los primeros 5 registros para debug
                var entryRequests = await companyContext.EntryRequests
                    .Take(5)
                    .Select(er => new EntryRequestResponseDTO
                    {
                        Id = er.Id,
                        Date = er.Date,
                        Service = er.Service,
                        IdOrderType = er.IdOrderType,
                        DeliveryPriority = er.DeliveryPriority,
                        IdCustomer = er.IdCustomer,
                        InsurerType = er.InsurerType,
                        Insurer = er.Insurer,
                        IdMedic = er.IdMedic,
                        IdPatient = er.IdPatient,
                        Applicant = er.Applicant,
                        IdATC = er.IdATC,
                        LimbSide = er.LimbSide,
                        DeliveryDate = er.DeliveryDate,
                        OrderObs = er.OrderObs,
                        SurgeryTime = er.SurgeryTime,
                        SurgeryInit = er.SurgeryInit,
                        SurgeryEnd = er.SurgeryEnd,
                        Status = er.Status,
                        IdTraceStates = er.IdTraceStates,
                        BranchId = er.BranchId,
                        SurgeryInitTime = er.SurgeryInitTime,
                        SurgeryEndTime = er.SurgeryEndTime,
                        DeliveryAddress = er.DeliveryAddress,
                        PurchaseOrder = er.PurchaseOrder,
                        AtcConsumed = er.AtcConsumed,
                        IsSatisfied = er.IsSatisfied,
                        Observations = er.Observations,
                        obsMaint = er.obsMaint,
                        AuxLog = er.AuxLog,
                        IdCancelReason = er.IdCancelReason,
                        IdCancelDetail = er.IdCancelDetail,
                        CancelReason = er.CancelReason,
                        CancelDetail = er.CancelDetail,
                        Notification = er.Notification,
                        IsReplacement = er.IsReplacement,
                        AssemblyComponents = er.AssemblyComponents,
                        priceGroup = er.priceGroup,
                        // Propiedades de navegación se dejan como null para evitar problemas
                        InsurerNavigation = null,
                        InsurerTypeNavigation = null,
                        Branch = null,
                        IdCustomerNavigation = null,
                        IdMedicNavigation = null,
                        IdPatientNavigation = null,
                        IdTraceStatesNavigation = null,
                        IdAtcNavigation = null,
                        // Propiedades calculadas
                        CustomerName = null,
                        PatientName = null,
                        MedicName = null,
                        NoRequest = null,
                        sEquipments = null,
                        sAssembly = null
                    })
                    .ToListAsync();

                Console.WriteLine($"Consulta completada exitosamente. Registros obtenidos: {entryRequests.Count}");
                Console.WriteLine("=== FIN GetAllEntryRequests ===");
                
                return Ok(entryRequests);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException en GetAllEntryRequests: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en GetAllEntryRequests ===");
                Console.WriteLine($"Mensaje de error: {ex.Message}");
                Console.WriteLine($"Tipo de excepción: {ex.GetType().Name}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Exception StackTrace: {ex.InnerException.StackTrace}");
                }
                
                // Log adicional para errores específicos de Entity Framework
                if (ex.Message.Contains("Data is Null"))
                {
                    Console.WriteLine("ERROR ESPECÍFICO: Data is Null - Esto indica un problema con propiedades de navegación o campos null");
                }
                
                Console.WriteLine("=== FIN ERROR ===");
                
                // Mensaje de error más específico
                if (ex.InnerException != null)
                {
                    return StatusCode(500, $"Error al obtener las solicitudes de entrada: {ex.InnerException.Message}");
                }
                return StatusCode(500, $"Error al obtener las solicitudes de entrada: {ex.Message}");
            }
        }

        // GET: api/EntryRequest/{id}
        [HttpGet("{id}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<EntryRequests>> GetEntryRequestById(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var entryRequest = await companyContext.EntryRequests
                    .FirstOrDefaultAsync(er => er.Id == id);

                if (entryRequest == null)
                {
                    return NotFound($"No se encontró la solicitud de entrada con ID {id}");
                }

                return Ok(entryRequest);
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

        // GET: api/EntryRequest/by-customer/{customerId}
        [HttpGet("by-customer/{customerId}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<EntryRequests>>> GetEntryRequestsByCustomer(int customerId, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var entryRequests = await companyContext.EntryRequests
                    .Where(er => er.IdCustomer == customerId)
                    .ToListAsync();

                return Ok(entryRequests);
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

        // GET: api/EntryRequest/by-medic/{medicId}
        [HttpGet("by-medic/{medicId}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<EntryRequests>>> GetEntryRequestsByMedic(int medicId, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                var entryRequests = await companyContext.EntryRequests
                    .Where(er => er.IdMedic == medicId)
                    .ToListAsync();

                return Ok(entryRequests);
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

        // GET: api/EntryRequest/by-patient/{patientId}
        [HttpGet("by-patient/{patientId}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<EntryRequestResponseDTO>>> GetEntryRequestsByPatient(int patientId, [FromQuery] string companyCode)
        {
            try
            {
                Console.WriteLine($"=== INICIO GetEntryRequestsByPatient ===");
                Console.WriteLine($"PatientId: {patientId}, CompanyCode: {companyCode}");
                
                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");
                
                var entryRequests = await companyContext.EntryRequests
                    .Where(er => er.IdPatient == patientId)
                    .Select(er => new EntryRequestResponseDTO
                    {
                        Id = er.Id,
                        Date = er.Date,
                        Service = er.Service,
                        IdOrderType = er.IdOrderType,
                        DeliveryPriority = er.DeliveryPriority,
                        IdCustomer = er.IdCustomer,
                        InsurerType = er.InsurerType,
                        Insurer = er.Insurer,
                        IdMedic = er.IdMedic,
                        IdPatient = er.IdPatient,
                        Applicant = er.Applicant,
                        IdATC = er.IdATC,
                        LimbSide = er.LimbSide,
                        DeliveryDate = er.DeliveryDate,
                        OrderObs = er.OrderObs,
                        SurgeryTime = er.SurgeryTime,
                        SurgeryInit = er.SurgeryInit,
                        SurgeryEnd = er.SurgeryEnd,
                        Status = er.Status,
                        IdTraceStates = er.IdTraceStates,
                        BranchId = er.BranchId,
                        SurgeryInitTime = er.SurgeryInitTime,
                        SurgeryEndTime = er.SurgeryEndTime,
                        DeliveryAddress = er.DeliveryAddress,
                        PurchaseOrder = er.PurchaseOrder,
                        AtcConsumed = er.AtcConsumed,
                        IsSatisfied = er.IsSatisfied,
                        Observations = er.Observations,
                        obsMaint = er.obsMaint,
                        AuxLog = er.AuxLog,
                        IdCancelReason = er.IdCancelReason,
                        IdCancelDetail = er.IdCancelDetail,
                        CancelReason = er.CancelReason,
                        CancelDetail = er.CancelDetail,
                        Notification = er.Notification,
                        IsReplacement = er.IsReplacement,
                        AssemblyComponents = er.AssemblyComponents,
                        priceGroup = er.priceGroup,
                        // Propiedades de navegación se dejan como null para evitar problemas
                        InsurerNavigation = null,
                        InsurerTypeNavigation = null,
                        Branch = null,
                        IdCustomerNavigation = null,
                        IdMedicNavigation = null,
                        IdPatientNavigation = null,
                        IdTraceStatesNavigation = null,
                        IdAtcNavigation = null,
                        // Propiedades calculadas
                        CustomerName = null,
                        PatientName = null,
                        MedicName = null,
                        NoRequest = null,
                        sEquipments = null,
                        sAssembly = null
                    })
                    .ToListAsync();

                Console.WriteLine($"Consulta completada. Registros encontrados: {entryRequests.Count}");
                Console.WriteLine("=== FIN GetEntryRequestsByPatient ===");
                
                return Ok(entryRequests);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException en GetEntryRequestsByPatient: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en GetEntryRequestsByPatient ===");
                Console.WriteLine($"Mensaje de error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                Console.WriteLine("=== FIN ERROR ===");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/EntryRequest/by-status/{status}
        [HttpGet("by-status/{status}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<EntryRequestResponseDTO>>> GetEntryRequestsByStatus(string status, [FromQuery] string companyCode)
        {
            try
            {
                Console.WriteLine($"=== INICIO GetEntryRequestsByStatus ===");
                Console.WriteLine($"Status: {status}, CompanyCode: {companyCode}");
                
                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");
                
                var entryRequests = await companyContext.EntryRequests
                    .Where(er => er.Status == status)
                    .Select(er => new EntryRequestResponseDTO
                    {
                        Id = er.Id,
                        Date = er.Date,
                        Service = er.Service,
                        IdOrderType = er.IdOrderType,
                        DeliveryPriority = er.DeliveryPriority,
                        IdCustomer = er.IdCustomer,
                        InsurerType = er.InsurerType,
                        Insurer = er.Insurer,
                        IdMedic = er.IdMedic,
                        IdPatient = er.IdPatient,
                        Applicant = er.Applicant,
                        IdATC = er.IdATC,
                        LimbSide = er.LimbSide,
                        DeliveryDate = er.DeliveryDate,
                        OrderObs = er.OrderObs,
                        SurgeryTime = er.SurgeryTime,
                        SurgeryInit = er.SurgeryInit,
                        SurgeryEnd = er.SurgeryEnd,
                        Status = er.Status,
                        IdTraceStates = er.IdTraceStates,
                        BranchId = er.BranchId,
                        SurgeryInitTime = er.SurgeryInitTime,
                        SurgeryEndTime = er.SurgeryEndTime,
                        DeliveryAddress = er.DeliveryAddress,
                        PurchaseOrder = er.PurchaseOrder,
                        AtcConsumed = er.AtcConsumed,
                        IsSatisfied = er.IsSatisfied,
                        Observations = er.Observations,
                        obsMaint = er.obsMaint,
                        AuxLog = er.AuxLog,
                        IdCancelReason = er.IdCancelReason,
                        IdCancelDetail = er.IdCancelDetail,
                        CancelReason = er.CancelReason,
                        CancelDetail = er.CancelDetail,
                        Notification = er.Notification,
                        IsReplacement = er.IsReplacement,
                        AssemblyComponents = er.AssemblyComponents,
                        priceGroup = er.priceGroup,
                        // Propiedades de navegación se dejan como null para evitar problemas
                        InsurerNavigation = null,
                        InsurerTypeNavigation = null,
                        Branch = null,
                        IdCustomerNavigation = null,
                        IdMedicNavigation = null,
                        IdPatientNavigation = null,
                        IdTraceStatesNavigation = null,
                        IdAtcNavigation = null,
                        // Propiedades calculadas
                        CustomerName = null,
                        PatientName = null,
                        MedicName = null,
                        NoRequest = null,
                        sEquipments = null,
                        sAssembly = null
                    })
                    .ToListAsync();

                Console.WriteLine($"Consulta completada. Registros encontrados: {entryRequests.Count}");
                Console.WriteLine("=== FIN GetEntryRequestsByStatus ===");
                
                return Ok(entryRequests);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException en GetEntryRequestsByStatus: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en GetEntryRequestsByStatus ===");
                Console.WriteLine($"Mensaje de error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                Console.WriteLine("=== FIN ERROR ===");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/EntryRequest
        [HttpPost]
        public async Task<ActionResult<EntryRequests>> CreateEntryRequest([FromBody] EntryRequestCreateDTO entryRequestDto, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);

                // Validar que las entidades relacionadas existen
                var customer = await companyContext.Customer.FindAsync(entryRequestDto.IdCustomer);
                if (customer == null)
                {
                    return BadRequest($"Cliente con ID {entryRequestDto.IdCustomer} no encontrado");
                }

                // Validar médico solo si IdMedic tiene valor y es distinto de cero
                if (entryRequestDto.IdMedic != null && entryRequestDto.IdMedic != 0)
                {
                    var medic = await companyContext.Medic.FindAsync(entryRequestDto.IdMedic.Value);
                    if (medic == null)
                    {
                        return BadRequest($"Médico con ID {entryRequestDto.IdMedic} no encontrado");
                    }
                }

                // Validar paciente solo si IdPatient tiene valor y es distinto de cero
                if (entryRequestDto.IdPatient != null && entryRequestDto.IdPatient != 0)
                {
                    var patient = await companyContext.Patient.FindAsync(entryRequestDto.IdPatient.Value);
                    if (patient == null)
                    {
                        return BadRequest($"Paciente con ID {entryRequestDto.IdPatient} no encontrado");
                    }
                }

                // Crear nueva solicitud de entrada
                var newEntryRequest = new EntryRequests
                {
                    Date = entryRequestDto.Date,
                    Service = entryRequestDto.Service,
                    IdOrderType = entryRequestDto.IdOrderType,
                    DeliveryPriority = entryRequestDto.DeliveryPriority,
                    IdCustomer = entryRequestDto.IdCustomer,
                    InsurerType = entryRequestDto.InsurerType,
                    Insurer = entryRequestDto.Insurer,
                    IdMedic = entryRequestDto.IdMedic,
                    IdPatient = entryRequestDto.IdPatient,
                    Applicant = entryRequestDto.Applicant,
                    IdATC = entryRequestDto.IdATC,
                    LimbSide = entryRequestDto.LimbSide,
                    DeliveryDate = entryRequestDto.DeliveryDate,
                    OrderObs = entryRequestDto.OrderObs,
                    SurgeryTime = entryRequestDto.SurgeryTime,
                    SurgeryInit = entryRequestDto.SurgeryInit,
                    SurgeryEnd = entryRequestDto.SurgeryEnd,
                    Status = entryRequestDto.Status ?? "Pendiente",
                    IdTraceStates = entryRequestDto.IdTraceStates,
                    BranchId = entryRequestDto.BranchId,
                    SurgeryInitTime = entryRequestDto.SurgeryInitTime,
                    SurgeryEndTime = entryRequestDto.SurgeryEndTime,
                    DeliveryAddress = entryRequestDto.DeliveryAddress,
                    PurchaseOrder = entryRequestDto.PurchaseOrder,
                    AtcConsumed = entryRequestDto.AtcConsumed,
                    IsSatisfied = entryRequestDto.IsSatisfied,
                    Observations = entryRequestDto.Observations,
                    obsMaint = entryRequestDto.obsMaint,
                    AuxLog = entryRequestDto.AuxLog,
                    IdCancelReason = entryRequestDto.IdCancelReason,
                    IdCancelDetail = entryRequestDto.IdCancelDetail,
                    CancelReason = entryRequestDto.CancelReason,
                    CancelDetail = entryRequestDto.CancelDetail,
                    Notification = entryRequestDto.Notification,
                    IsReplacement = entryRequestDto.IsReplacement,
                    AssemblyComponents = entryRequestDto.AssemblyComponents,
                    priceGroup = entryRequestDto.priceGroup
                };

                companyContext.EntryRequests.Add(newEntryRequest);
                await companyContext.SaveChangesAsync();

                // Invalidar caché
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                return CreatedAtAction(nameof(GetEntryRequestById), new { id = newEntryRequest.Id, companyCode }, newEntryRequest);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Mostrar detalles de la inner exception para depuración
                var errorMsg = $"Error interno del servidor: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMsg += $" | InnerException: {ex.InnerException.Message}";
                }
                if (ex.InnerException?.InnerException != null)
                {
                    errorMsg += $" | InnerMostException: {ex.InnerException.InnerException.Message}";
                }
                return StatusCode(500, errorMsg);
            }
        }

        // PUT: api/EntryRequest/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEntryRequest(int id, [FromBody] EntryRequestUpdateDTO entryRequestDto, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);

                var existingEntryRequest = await companyContext.EntryRequests.FindAsync(id);
                if (existingEntryRequest == null)
                {
                    return NotFound($"No se encontró la solicitud de entrada con ID {id}");
                }

                // Validar que las entidades relacionadas existen
                var customer = await companyContext.Customer.FindAsync(entryRequestDto.IdCustomer);
                if (customer == null)
                {
                    return BadRequest($"Cliente con ID {entryRequestDto.IdCustomer} no encontrado");
                }

                // Validar médico solo si IdMedic tiene valor y es distinto de cero
                if (entryRequestDto.IdMedic != null && entryRequestDto.IdMedic != 0)
                {
                    var medic = await companyContext.Medic.FindAsync(entryRequestDto.IdMedic.Value);
                    if (medic == null)
                    {
                        return BadRequest($"Médico con ID {entryRequestDto.IdMedic} no encontrado");
                    }
                }

                // Validar paciente solo si IdPatient tiene valor y es distinto de cero
                if (entryRequestDto.IdPatient != null && entryRequestDto.IdPatient != 0)
                {
                    var patient = await companyContext.Patient.FindAsync(entryRequestDto.IdPatient.Value);
                    if (patient == null)
                    {
                        return BadRequest($"Paciente con ID {entryRequestDto.IdPatient} no encontrado");
                    }
                }

                // Actualizar propiedades
                existingEntryRequest.Date = entryRequestDto.Date;
                existingEntryRequest.Service = entryRequestDto.Service;
                existingEntryRequest.IdOrderType = entryRequestDto.IdOrderType;
                existingEntryRequest.DeliveryPriority = entryRequestDto.DeliveryPriority;
                existingEntryRequest.IdCustomer = entryRequestDto.IdCustomer;
                existingEntryRequest.InsurerType = entryRequestDto.InsurerType;
                existingEntryRequest.Insurer = entryRequestDto.Insurer;
                existingEntryRequest.IdMedic = entryRequestDto.IdMedic;
                existingEntryRequest.IdPatient = entryRequestDto.IdPatient;
                existingEntryRequest.Applicant = entryRequestDto.Applicant;
                existingEntryRequest.IdATC = entryRequestDto.IdATC;
                existingEntryRequest.LimbSide = entryRequestDto.LimbSide;
                existingEntryRequest.DeliveryDate = entryRequestDto.DeliveryDate;
                existingEntryRequest.OrderObs = entryRequestDto.OrderObs;
                existingEntryRequest.SurgeryTime = entryRequestDto.SurgeryTime;
                existingEntryRequest.SurgeryInit = entryRequestDto.SurgeryInit;
                existingEntryRequest.SurgeryEnd = entryRequestDto.SurgeryEnd;
                existingEntryRequest.Status = entryRequestDto.Status;
                existingEntryRequest.IdTraceStates = entryRequestDto.IdTraceStates;
                existingEntryRequest.BranchId = entryRequestDto.BranchId;
                existingEntryRequest.SurgeryInitTime = entryRequestDto.SurgeryInitTime;
                existingEntryRequest.SurgeryEndTime = entryRequestDto.SurgeryEndTime;
                existingEntryRequest.DeliveryAddress = entryRequestDto.DeliveryAddress;
                existingEntryRequest.PurchaseOrder = entryRequestDto.PurchaseOrder;
                existingEntryRequest.AtcConsumed = entryRequestDto.AtcConsumed;
                existingEntryRequest.IsSatisfied = entryRequestDto.IsSatisfied;
                existingEntryRequest.Observations = entryRequestDto.Observations;
                existingEntryRequest.obsMaint = entryRequestDto.obsMaint;
                existingEntryRequest.AuxLog = entryRequestDto.AuxLog;
                existingEntryRequest.IdCancelReason = entryRequestDto.IdCancelReason;
                existingEntryRequest.IdCancelDetail = entryRequestDto.IdCancelDetail;
                existingEntryRequest.CancelReason = entryRequestDto.CancelReason;
                existingEntryRequest.CancelDetail = entryRequestDto.CancelDetail;
                existingEntryRequest.Notification = entryRequestDto.Notification;
                existingEntryRequest.IsReplacement = entryRequestDto.IsReplacement;
                existingEntryRequest.AssemblyComponents = entryRequestDto.AssemblyComponents;
                existingEntryRequest.priceGroup = entryRequestDto.priceGroup;

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

        // DELETE: api/EntryRequest/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntryRequest(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);

                var entryRequest = await companyContext.EntryRequests.FindAsync(id);
                if (entryRequest == null)
                {
                    return NotFound($"No se encontró la solicitud de entrada con ID {id}");
                }

                companyContext.EntryRequests.Remove(entryRequest);
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

        // GET: api/EntryRequest/summary
        [HttpGet("summary")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<EntryRequestSummaryDTO>>> GetEntryRequestsSummary([FromQuery] string companyCode)
        {
            try
            {
                Console.WriteLine($"=== INICIO GetEntryRequestsSummary ===");
                Console.WriteLine($"CompanyCode: {companyCode}");
                
                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");
                
                var entryRequestsSummary = await companyContext.EntryRequests
                    .Include(er => er.IdCustomerNavigation) // Incluir la relación con Customer
                    .Select(er => new EntryRequestSummaryDTO
                    {
                        Id = er.Id,
                        SurgeryInit = er.SurgeryInit,
                        SurgeryEnd = er.SurgeryEnd,
                        Status = er.Status,
                        IdCustomer = er.IdCustomer,
                        IdATC = er.IdATC,
                        BranchId = er.BranchId,
                        Customer = er.IdCustomerNavigation
                    })
                    .ToListAsync();

                Console.WriteLine($"Consulta completada. Registros encontrados: {entryRequestsSummary.Count}");
                Console.WriteLine("=== FIN GetEntryRequestsSummary ===");
                
                return Ok(entryRequestsSummary);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException en GetEntryRequestsSummary: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en GetEntryRequestsSummary ===");
                Console.WriteLine($"Mensaje de error: {ex.Message}");
                Console.WriteLine($"Tipo de excepción: {ex.GetType().Name}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Exception StackTrace: {ex.InnerException.StackTrace}");
                }
                
                Console.WriteLine("=== FIN ERROR ===");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/EntryRequest/EntryRequestforATC
        [HttpGet("EntryRequestforATC")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<EntryRequestForATCResponseDTO>>> GetEntryRequestsForATC([FromQuery] string companyCode)
        {
            try
            {
                Console.WriteLine($"=== INICIO GetEntryRequestsForATC ===");
                Console.WriteLine($"CompanyCode: {companyCode}");
                
                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");
                
                var entryRequestsForATC = await companyContext.EntryRequests
                    .Where(er => er.Status != "CANCEL") // Excluir solicitudes canceladas
                    .Include(er => er.IdCustomerNavigation) // Incluir la relación con Customer
                    .OrderByDescending(er => er.Id) // Ordenar por ID descendente (más recientes primero)
                    .Select(er => new EntryRequestForATCResponseDTO
                    {
                        Id = er.Id,
                        SurgeryInit = er.SurgeryInit.HasValue ? er.SurgeryInit.Value.ToString("yyyy-MM-ddTHH:mm") : null,
                        SurgeryEnd = er.SurgeryEnd.HasValue ? er.SurgeryEnd.Value.ToString("yyyy-MM-ddTHH:mm") : null,
                        Status = er.Status,
                        IdCustomer = er.IdCustomer,
                        IdATC = er.IdATC,
                        BranchId = er.BranchId,
                        Customer = er.IdCustomerNavigation
                    })
                    .ToListAsync();

                Console.WriteLine($"Consulta completada. Registros encontrados (excluyendo cancelados): {entryRequestsForATC.Count}");
                Console.WriteLine("=== FIN GetEntryRequestsForATC ===");
                
                return Ok(entryRequestsForATC);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException en GetEntryRequestsForATC: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en GetEntryRequestsForATC ===");
                Console.WriteLine($"Mensaje de error: {ex.Message}");
                Console.WriteLine($"Tipo de excepción: {ex.GetType().Name}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Exception StackTrace: {ex.InnerException.StackTrace}");
                }
                
                Console.WriteLine("=== FIN ERROR ===");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/EntryRequest/filter
        [HttpGet("filter")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<EntryRequestFilteredResponseDTO>>> GetEntryRequestsFiltered(
            [FromQuery] string companyCode,
            [FromQuery] EntryRequestFilterDTO filter)
        {
            try
            {
                Console.WriteLine($"=== INICIO GetEntryRequestsFiltered ===");
                Console.WriteLine($"CompanyCode: {companyCode}");
                
                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");
                
                // Construir la consulta base
                var query = companyContext.EntryRequests
                    .Include(er => er.IdCustomerNavigation) // Incluir la relación con Customer
                    .AsQueryable();

                // Aplicar filtros dinámicamente
                if (filter.Id.HasValue)
                    query = query.Where(er => er.Id == filter.Id.Value);

                if (filter.SurgeryInitFrom.HasValue)
                    query = query.Where(er => er.SurgeryInit >= filter.SurgeryInitFrom.Value);

                if (filter.SurgeryInitTo.HasValue)
                    query = query.Where(er => er.SurgeryInit <= filter.SurgeryInitTo.Value);

                if (filter.SurgeryEndFrom.HasValue)
                    query = query.Where(er => er.SurgeryEnd >= filter.SurgeryEndFrom.Value);

                if (filter.SurgeryEndTo.HasValue)
                    query = query.Where(er => er.SurgeryEnd <= filter.SurgeryEndTo.Value);

                if (!string.IsNullOrEmpty(filter.Status))
                    query = query.Where(er => er.Status == filter.Status);

                if (filter.IdCustomer.HasValue)
                    query = query.Where(er => er.IdCustomer == filter.IdCustomer.Value);

                if (filter.IdATC.HasValue)
                    query = query.Where(er => er.IdATC == filter.IdATC.Value);

                if (filter.BranchId.HasValue)
                    query = query.Where(er => er.BranchId == filter.BranchId.Value);

                if (!string.IsNullOrEmpty(filter.Service))
                    query = query.Where(er => er.Service != null && er.Service.Contains(filter.Service));

                if (filter.IdOrderType.HasValue)
                    query = query.Where(er => er.IdOrderType == filter.IdOrderType.Value);

                if (!string.IsNullOrEmpty(filter.DeliveryPriority))
                    query = query.Where(er => er.DeliveryPriority == filter.DeliveryPriority);

                if (filter.InsurerType.HasValue)
                    query = query.Where(er => er.InsurerType == filter.InsurerType.Value);

                if (filter.Insurer.HasValue)
                    query = query.Where(er => er.Insurer == filter.Insurer.Value);

                if (filter.IdMedic.HasValue)
                    query = query.Where(er => er.IdMedic == filter.IdMedic.Value);

                if (filter.IdPatient.HasValue)
                    query = query.Where(er => er.IdPatient == filter.IdPatient.Value);

                if (!string.IsNullOrEmpty(filter.Applicant))
                    query = query.Where(er => er.Applicant != null && er.Applicant.Contains(filter.Applicant));

                if (!string.IsNullOrEmpty(filter.LimbSide))
                    query = query.Where(er => er.LimbSide == filter.LimbSide);

                if (filter.DeliveryDateFrom.HasValue)
                    query = query.Where(er => er.DeliveryDate >= filter.DeliveryDateFrom.Value);

                if (filter.DeliveryDateTo.HasValue)
                    query = query.Where(er => er.DeliveryDate <= filter.DeliveryDateTo.Value);

                if (!string.IsNullOrEmpty(filter.OrderObs))
                    query = query.Where(er => er.OrderObs != null && er.OrderObs.Contains(filter.OrderObs));

                if (filter.SurgeryTime.HasValue)
                    query = query.Where(er => er.SurgeryTime == filter.SurgeryTime.Value);

                if (filter.IdTraceStates.HasValue)
                    query = query.Where(er => er.IdTraceStates == filter.IdTraceStates.Value);

                if (filter.SurgeryInitTime.HasValue)
                    query = query.Where(er => er.SurgeryInitTime == filter.SurgeryInitTime.Value);

                if (filter.SurgeryEndTime.HasValue)
                    query = query.Where(er => er.SurgeryEndTime == filter.SurgeryEndTime.Value);

                if (!string.IsNullOrEmpty(filter.DeliveryAddress))
                    query = query.Where(er => er.DeliveryAddress != null && er.DeliveryAddress.Contains(filter.DeliveryAddress));

                if (!string.IsNullOrEmpty(filter.PurchaseOrder))
                    query = query.Where(er => er.PurchaseOrder != null && er.PurchaseOrder.Contains(filter.PurchaseOrder));

                if (filter.AtcConsumed.HasValue)
                    query = query.Where(er => er.AtcConsumed == filter.AtcConsumed.Value);

                if (filter.IsSatisfied.HasValue)
                    query = query.Where(er => er.IsSatisfied == filter.IsSatisfied.Value);

                if (!string.IsNullOrEmpty(filter.Observations))
                    query = query.Where(er => er.Observations != null && er.Observations.Contains(filter.Observations));

                if (!string.IsNullOrEmpty(filter.obsMaint))
                    query = query.Where(er => er.obsMaint != null && er.obsMaint.Contains(filter.obsMaint));

                if (filter.AuxLog.HasValue)
                    query = query.Where(er => er.AuxLog == filter.AuxLog.Value);

                if (filter.IdCancelReason.HasValue)
                    query = query.Where(er => er.IdCancelReason == filter.IdCancelReason.Value);

                if (filter.IdCancelDetail.HasValue)
                    query = query.Where(er => er.IdCancelDetail == filter.IdCancelDetail.Value);

                if (!string.IsNullOrEmpty(filter.CancelReason))
                    query = query.Where(er => er.CancelReason != null && er.CancelReason.Contains(filter.CancelReason));

                if (!string.IsNullOrEmpty(filter.CancelDetail))
                    query = query.Where(er => er.CancelDetail != null && er.CancelDetail.Contains(filter.CancelDetail));

                if (filter.Notification.HasValue)
                    query = query.Where(er => er.Notification == filter.Notification.Value);

                if (filter.IsReplacement.HasValue)
                    query = query.Where(er => er.IsReplacement == filter.IsReplacement.Value);

                if (filter.AssemblyComponents.HasValue)
                    query = query.Where(er => er.AssemblyComponents == filter.AssemblyComponents.Value);

                if (!string.IsNullOrEmpty(filter.priceGroup))
                    query = query.Where(er => er.priceGroup == filter.priceGroup);

                // Ordenar por ID descendente y ejecutar la consulta
                var entryRequestsFiltered = await query
                    .OrderByDescending(er => er.Id)
                    .Select(er => new EntryRequestFilteredResponseDTO
                    {
                        Id = er.Id,
                        Date = er.Date,
                        Service = er.Service,
                        IdOrderType = er.IdOrderType,
                        DeliveryPriority = er.DeliveryPriority,
                        IdCustomer = er.IdCustomer,
                        InsurerType = er.InsurerType,
                        Insurer = er.Insurer,
                        IdMedic = er.IdMedic,
                        IdPatient = er.IdPatient,
                        Applicant = er.Applicant,
                        IdATC = er.IdATC,
                        LimbSide = er.LimbSide,
                        DeliveryDate = er.DeliveryDate,
                        OrderObs = er.OrderObs,
                        SurgeryTime = er.SurgeryTime,
                        SurgeryInit = er.SurgeryInit,
                        SurgeryEnd = er.SurgeryEnd,
                        Status = er.Status,
                        IdTraceStates = er.IdTraceStates,
                        BranchId = er.BranchId,
                        SurgeryInitTime = er.SurgeryInitTime,
                        SurgeryEndTime = er.SurgeryEndTime,
                        DeliveryAddress = er.DeliveryAddress,
                        PurchaseOrder = er.PurchaseOrder,
                        AtcConsumed = er.AtcConsumed,
                        IsSatisfied = er.IsSatisfied,
                        Observations = er.Observations,
                        obsMaint = er.obsMaint,
                        AuxLog = er.AuxLog,
                        IdCancelReason = er.IdCancelReason,
                        IdCancelDetail = er.IdCancelDetail,
                        CancelReason = er.CancelReason,
                        CancelDetail = er.CancelDetail,
                        Notification = er.Notification,
                        IsReplacement = er.IsReplacement,
                        AssemblyComponents = er.AssemblyComponents,
                        priceGroup = er.priceGroup,
                        Customer = er.IdCustomerNavigation
                    })
                    .ToListAsync();

                Console.WriteLine($"Consulta completada. Registros encontrados: {entryRequestsFiltered.Count}");
                Console.WriteLine("=== FIN GetEntryRequestsFiltered ===");
                
                return Ok(entryRequestsFiltered);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException en GetEntryRequestsFiltered: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en GetEntryRequestsFiltered ===");
                Console.WriteLine($"Mensaje de error: {ex.Message}");
                Console.WriteLine($"Tipo de excepción: {ex.GetType().Name}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Exception StackTrace: {ex.InnerException.StackTrace}");
                }
                
                Console.WriteLine("=== FIN ERROR ===");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/EntryRequest/EntryRequestforATC/filter
        [HttpGet("EntryRequestforATC/filter")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<EntryRequestForATCResponseDTO>>> GetEntryRequestsForATCFiltered(
            [FromQuery] string companyCode,
            [FromQuery] EntryRequestForATCFilterDTO filter)
        {
            try
            {
                Console.WriteLine($"=== INICIO GetEntryRequestsForATCFiltered ===");
                Console.WriteLine($"CompanyCode: {companyCode}");
                
                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");
                
                // Construir la consulta base (excluyendo canceladas como EntryRequestforATC)
                var query = companyContext.EntryRequests
                    .Where(er => er.Status != "CANCEL") // Excluir solicitudes canceladas
                    .Include(er => er.IdCustomerNavigation) // Incluir la relación con Customer
                    .AsQueryable();

                // Aplicar filtros dinámicamente (solo los campos de EntryRequestforATC)
                if (filter.Id.HasValue)
                    query = query.Where(er => er.Id == filter.Id.Value);

                if (filter.SurgeryInit.HasValue)
                    query = query.Where(er => er.SurgeryInit.HasValue && er.SurgeryInit.Value.Date == filter.SurgeryInit.Value.Date);

                if (filter.SurgeryEnd.HasValue)
                    query = query.Where(er => er.SurgeryEnd.HasValue && er.SurgeryEnd.Value.Date == filter.SurgeryEnd.Value.Date);

                if (!string.IsNullOrEmpty(filter.Status))
                    query = query.Where(er => er.Status == filter.Status);

                if (filter.IdCustomer.HasValue)
                    query = query.Where(er => er.IdCustomer == filter.IdCustomer.Value);

                if (filter.IdATC.HasValue)
                    query = query.Where(er => er.IdATC == filter.IdATC.Value);

                if (filter.BranchId.HasValue)
                    query = query.Where(er => er.BranchId == filter.BranchId.Value);

                // Ordenar por ID descendente y ejecutar la consulta
                var entryRequestsForATCFiltered = await query
                    .OrderByDescending(er => er.Id)
                    .Select(er => new EntryRequestForATCResponseDTO
                    {
                        Id = er.Id,
                        SurgeryInit = er.SurgeryInit.HasValue ? er.SurgeryInit.Value.ToString("yyyy-MM-ddTHH:mm") : null,
                        SurgeryEnd = er.SurgeryEnd.HasValue ? er.SurgeryEnd.Value.ToString("yyyy-MM-ddTHH:mm") : null,
                        Status = er.Status,
                        IdCustomer = er.IdCustomer,
                        IdATC = er.IdATC,
                        BranchId = er.BranchId,
                        Customer = er.IdCustomerNavigation
                    })
                    .ToListAsync();

                Console.WriteLine($"Consulta completada. Registros encontrados (excluyendo cancelados): {entryRequestsForATCFiltered.Count}");
                Console.WriteLine("=== FIN GetEntryRequestsForATCFiltered ===");
                
                return Ok(entryRequestsForATCFiltered);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException en GetEntryRequestsForATCFiltered: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en GetEntryRequestsForATCFiltered ===");
                Console.WriteLine($"Mensaje de error: {ex.Message}");
                Console.WriteLine($"Tipo de excepción: {ex.GetType().Name}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Exception StackTrace: {ex.InnerException.StackTrace}");
                }
                
                Console.WriteLine("=== FIN ERROR ===");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/EntryRequest/VerificarConfiguracionCompania
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

        // PATCH: api/EntryRequest/{id}/idATC
        [HttpPatch("{id}/idATC")]
        public async Task<IActionResult> UpdateIdATC(int id, [FromQuery] string companyCode, [FromBody] int idATC)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var entryRequest = await companyContext.EntryRequests.FindAsync(id);
            if (entryRequest == null)
                return NotFound($"No se encontró la solicitud con ID {id}");

            entryRequest.IdATC = idATC;
            await companyContext.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/EntryRequest/remisiones?companyCode=xxx
        [HttpGet("remisiones")]
        public async Task<ActionResult<IEnumerable<DTOs.RemisionEquipoSummaryDTO>>> GetRemisionesEquipo([FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var remisiones = await companyContext.EntryRequests
                .Include(er => er.IdCustomerNavigation)
                .Include(er => er.Branch)
                .Join(companyContext.OrderType,
                    er => er.IdOrderType,
                    ot => ot.Id,
                    (er, ot) => new { er, ot })
                .Select(x => new DTOs.RemisionEquipoSummaryDTO
                {
                    Id = x.er.Id,
                    Date = x.er.Date,
                    IdOrderType = x.er.IdOrderType,
                    OrderTypeName = x.ot.Description,
                    IdCustomer = x.er.IdCustomer,
                    CustomerName = x.er.IdCustomerNavigation != null ? (x.er.IdCustomerNavigation.Name ?? "") : "",
                    Status = x.er.Status ?? string.Empty,
                    BranchId = x.er.BranchId,
                    BranchName = x.er.Branch != null ? x.er.Branch.Name : string.Empty
                })
                .ToListAsync();

            return Ok(remisiones);
        }

        // GET: api/EntryRequest/basic/{id}
        [HttpGet("basic/{id}")]
        public async Task<IActionResult> GetEntryRequestBasic(int id, [FromQuery] string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
                return BadRequest("El código de compañía es requerido");

            using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
            var entryRequest = await companyContext.EntryRequests
                .FirstOrDefaultAsync(er => er.Id == id);
            if (entryRequest == null)
                return NotFound($"No se encontró la solicitud de entrada con ID {id}");

            // Obtener nombres relacionados
            string? medicName = null;
            string? patientName = null;
            string? atcName = null;
            string? customerName = null;

            if (entryRequest.IdMedic.HasValue)
            {
                var medic = await companyContext.Medic.FindAsync(entryRequest.IdMedic.Value);
                medicName = medic?.Name;
            }
            if (entryRequest.IdPatient.HasValue)
            {
                var patient = await companyContext.Patient.FindAsync(entryRequest.IdPatient.Value);
                patientName = patient?.Name;
            }
            if (entryRequest.IdATC.HasValue)
            {
                var atc = await companyContext.Employee.FindAsync(entryRequest.IdATC.Value);
                atcName = atc?.Name;
            }
            if (entryRequest.IdCustomer != 0)
            {
                var customer = await companyContext.Customer.FindAsync(entryRequest.IdCustomer);
                customerName = customer?.Name;
            }

            return Ok(new {
                id = entryRequest.Id,
                date = entryRequest.Date,
                idCustomer = entryRequest.IdCustomer,
                customerName,
                idMedic = entryRequest.IdMedic,
                medicName,
                idPatient = entryRequest.IdPatient,
                patientName,
                idATC = entryRequest.IdATC,
                atcName,
                deliveryDate = entryRequest.DeliveryDate,
                surgeryInitTime = entryRequest.SurgeryInitTime,
                surgeryEndTime = entryRequest.SurgeryEndTime,
                surgeryInit = entryRequest.SurgeryInit,
                surgeryEnd = entryRequest.SurgeryEnd
            });
        }
    }
} 