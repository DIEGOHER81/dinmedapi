using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using DimmedAPI.DTOs;
using System.Collections.Generic;
using System.Data;

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
                    .OrderByDescending(er => er.Id) // Ordenar por Id descendente
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
                    .Include(er => er.IdPatientNavigation) // Incluir la relación con Patient
                    .Include(er => er.IdMedicNavigation) // Incluir la relación con Medic
                    .Include(er => er.IdCustomerNavigation.ShipAddress) // Incluir las direcciones del cliente
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
                        Customer = er.IdCustomerNavigation,
                        Patient = er.IdPatientNavigation,
                        Medic = er.IdMedicNavigation,
                        CustomerAddresses = er.IdCustomerNavigation != null ? er.IdCustomerNavigation.ShipAddress : null,
                        CustomerContacts = null // Se llenará después de la consulta principal
                    })
                    .ToListAsync();

                Console.WriteLine($"Consulta completada. Registros encontrados: {entryRequestsFiltered.Count}");
                
                // Obtener todos los contactos de clientes para esta compañía
                var customerContacts = await companyContext.CustomerContact.ToListAsync();
                
                // Asignar los contactos correspondientes a cada EntryRequest
                foreach (var entryRequest in entryRequestsFiltered)
                {
                    if (entryRequest.Customer != null)
                    {
                        // Buscar contactos que coincidan con el nombre del cliente
                        entryRequest.CustomerContacts = customerContacts
                            .Where(cc => cc.CustomerName == entryRequest.Customer.Name)
                            .ToList();
                    }
                }
                
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
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest(new UpdateResponseDTO
                    {
                        Success = false,
                        Message = "Error de validación",
                        ErrorDetails = "El código de compañía es requerido"
                    });
                }

                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                var entryRequest = await companyContext.EntryRequests.FindAsync(id);
                
                if (entryRequest == null)
                {
                    return NotFound(new UpdateResponseDTO
                    {
                        Success = false,
                        Message = "Recurso no encontrado",
                        ErrorDetails = $"No se encontró la solicitud con ID {id}"
                    });
                }

                entryRequest.IdATC = idATC;
                await companyContext.SaveChangesAsync();
                
                return Ok(new UpdateResponseDTO
                {
                    Success = true,
                    Message = "IdATC actualizado exitosamente",
                    UpdatedId = id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new UpdateResponseDTO
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    ErrorDetails = ex.Message
                });
            }
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

        // GET: api/EntryRequest/report
        [HttpGet("report")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<PaginatedResponseDTO<EntryRequestReportDTO>>> GetEntryRequestsReport(
            [FromQuery] string companyCode,
            [FromQuery] int? noEntryRequest = null,
            [FromQuery] DateTime? dateIni = null,
            [FromQuery] DateTime? dateEnd = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                Console.WriteLine($"=== INICIO GetEntryRequestsReport ===");
                Console.WriteLine($"CompanyCode recibido: {companyCode}");
                Console.WriteLine($"NoEntryRequest: {noEntryRequest}");
                Console.WriteLine($"DateIni: {dateIni}");
                Console.WriteLine($"DateEnd: {dateEnd}");
                Console.WriteLine($"PageNumber: {pageNumber}");
                Console.WriteLine($"PageSize: {pageSize}");

                // Validar parámetros de paginación
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 50;
                if (pageSize > 1000) pageSize = 1000; // Límite máximo

                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                Console.WriteLine("Creando contexto de base de datos...");
                
                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");

                // Ejecutar el procedimiento almacenado - siempre enviar todos los parámetros
                var parameters = new List<object>();
                var parameterNames = new List<string>();

                // Siempre enviar @NoEntryRequest (NULL si no se proporciona)
                parameters.Add(noEntryRequest ?? (object)DBNull.Value);
                parameterNames.Add("@NoEntryRequest");

                // Siempre enviar @DateIni (NULL si no se proporciona)
                parameters.Add(dateIni?.Date ?? (object)DBNull.Value);
                parameterNames.Add("@DateIni");

                // Siempre enviar @DateEnd (NULL si no se proporciona)
                parameters.Add(dateEnd?.Date ?? (object)DBNull.Value);
                parameterNames.Add("@DateEnd");

                // Construir la consulta SQL con todos los parámetros
                var sql = "EXEC sp_GetEntryRequestsReport " + string.Join(", ", parameterNames);

                Console.WriteLine($"Ejecutando SQL: {sql}");
                Console.WriteLine($"Parámetros: {string.Join(", ", parameters)}");
                Console.WriteLine($"Parámetros detallados:");
                Console.WriteLine($"  @NoEntryRequest: {noEntryRequest?.ToString() ?? "NULL"}");
                Console.WriteLine($"  @DateIni: {dateIni?.Date.ToString("yyyy-MM-dd") ?? "NULL"}");
                Console.WriteLine($"  @DateEnd: {dateEnd?.Date.ToString("yyyy-MM-dd") ?? "NULL"}");

                // Ejecutar el procedimiento almacenado usando DbCommand para mejor control de tipos
                var allResults = new List<EntryRequestReportDTO>();
                using var command = companyContext.Database.GetDbConnection().CreateCommand();
                command.CommandText = sql;
                command.CommandType = System.Data.CommandType.Text;
                
                // Agregar parámetros
                for (int i = 0; i < parameters.Count; i++)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = parameterNames[i];
                    parameter.Value = parameters[i];
                    command.Parameters.Add(parameter);
                }
                
                companyContext.Database.OpenConnection();
                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    // Debug: Log del valor original de Pedido
                    try
                    {
                        int pedidoIndex = reader.GetOrdinal("Pedido");
                        var pedidoValue = reader.GetValue(pedidoIndex);
                        Console.WriteLine($"DEBUG - Valor original de Pedido: '{pedidoValue}' (Tipo: {pedidoValue?.GetType().Name})");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"DEBUG - Error obteniendo Pedido: {ex.Message}");
                    }

                    var item = new EntryRequestReportDTO
                    {
                        Pedido = GetSafeInt32(reader, "Pedido"),
                        Consumo = GetSafeBoolean(reader, "Consumo"),
                        FechaCirugia = GetSafeDateTime(reader, "FechaCirugia"),
                        FechaSolicitud = GetSafeDateTime(reader, "FechaSolicitud"),
                        Estado = GetSafeString(reader, "Estado"),
                        EstadoTrazabilidad = GetSafeString(reader, "EstadoTrazabilidad"),
                        Cliente = GetSafeString(reader, "Cliente"),
                        Equipos = GetSafeString(reader, "Equipos"),
                        DireccionEntrega = GetSafeString(reader, "DireccionEntrega"),
                        PrioridadEntrega = GetSafeString(reader, "PrioridadEntrega"),
                        Observaciones = GetSafeString(reader, "Observaciones"),
                        ObservacionesComerciales = GetSafeString(reader, "ObservacionesComerciales"),
                        NombrePaciente = GetSafeString(reader, "NombrePaciente"),
                        NombreMedico = GetSafeString(reader, "NombreMedico"),
                        NombreAtc = GetSafeString(reader, "NombreAtc"),
                        Sede = GetSafeString(reader, "Sede"),
                        Servicio = GetSafeString(reader, "Servicio"),
                        TipodePedido = GetSafeString(reader, "TipodePedido"),
                        Causalesdenocirugia = GetSafeString(reader, "Causalesdenocirugia"),
                        Detallescausalesdenocirugia = GetSafeString(reader, "Detallescausalesdenocirugia"),
                        Aseguradora = GetSafeString(reader, "Aseguradora"),
                        Tipodeaseguradora = GetSafeString(reader, "Tipodeaseguradora"),
                        Solicitante = GetSafeString(reader, "Solicitante"),
                        LadoExtremidad = GetSafeString(reader, "LadoExtremidad"),
                        FechaTerminacion = GetSafeDateTime(reader, "FechaTerminacion"),
                        EsReposicion = GetSafeBoolean(reader, "EsReposicion"),
                        Imprimir = GetSafeBoolean(reader, "Imprimir"),
                        ReporteTrazabilidad = GetSafeString(reader, "ReporteTrazabilidad")
                    };
                    allResults.Add(item);
                }

                Console.WriteLine($"Resultados obtenidos: {allResults.Count} registros totales");

                // Aplicar paginación
                var totalRecords = allResults.Count;
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                var skip = (pageNumber - 1) * pageSize;
                var take = pageSize;

                var paginatedData = allResults
                    .Skip(skip)
                    .Take(take)
                    .ToList();

                var response = new PaginatedResponseDTO<EntryRequestReportDTO>
                {
                    Data = paginatedData,
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    HasPreviousPage = pageNumber > 1,
                    HasNextPage = pageNumber < totalPages
                };

                Console.WriteLine($"Paginación aplicada: Página {pageNumber} de {totalPages}, {paginatedData.Count} registros mostrados");

                return Ok(response);
            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine($"Error de conversión de tipos en GetEntryRequestsReport: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, $"Error de conversión de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetEntryRequestsReport: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // Métodos auxiliares para conversiones seguras
        private static int? GetSafeInt32(IDataReader reader, string columnName)
        {
            try
            {
                int columnIndex = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(columnIndex))
                {
                    Console.WriteLine($"DEBUG - {columnName} es NULL");
                    return null;
                }

                var value = reader.GetValue(columnIndex);
                Console.WriteLine($"DEBUG - {columnName}: '{value}' (Tipo: {value?.GetType().Name})");
                
                if (value is int intValue)
                {
                    Console.WriteLine($"DEBUG - {columnName} es int: {intValue}");
                    return intValue;
                }
                if (value is string stringValue)
                {
                    Console.WriteLine($"DEBUG - {columnName} es string: '{stringValue}'");
                    // Extraer números del string (ej: "P- 10168" -> 10168)
                    var numbers = new string(stringValue.Where(char.IsDigit).ToArray());
                    Console.WriteLine($"DEBUG - {columnName} números extraídos: '{numbers}'");
                    
                    if (!string.IsNullOrEmpty(numbers) && int.TryParse(numbers, out int parsedValue))
                    {
                        Console.WriteLine($"DEBUG - {columnName} parseado exitosamente: {parsedValue}");
                        return parsedValue;
                    }
                    
                    // Intentar parse directo si no hay caracteres especiales
                    if (int.TryParse(stringValue, out int directParsedValue))
                    {
                        Console.WriteLine($"DEBUG - {columnName} parse directo exitoso: {directParsedValue}");
                        return directParsedValue;
                    }
                    
                    Console.WriteLine($"DEBUG - {columnName} no se pudo parsear");
                }
                if (value is decimal decimalValue)
                {
                    Console.WriteLine($"DEBUG - {columnName} es decimal: {decimalValue}");
                    return (int)decimalValue;
                }
                if (value is double doubleValue)
                {
                    Console.WriteLine($"DEBUG - {columnName} es double: {doubleValue}");
                    return (int)doubleValue;
                }

                Console.WriteLine($"DEBUG - {columnName} tipo no manejado: {value?.GetType().Name}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG - Error en GetSafeInt32 para {columnName}: {ex.Message}");
                return null;
            }
        }

        private static DateTime? GetSafeDateTime(IDataReader reader, string columnName)
        {
            try
            {
                int columnIndex = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(columnIndex))
                    return null;

                var value = reader.GetValue(columnIndex);
                if (value is DateTime dateTimeValue)
                    return dateTimeValue;
                if (value is string stringValue && DateTime.TryParse(stringValue, out DateTime parsedValue))
                    return parsedValue;

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static string? GetSafeString(IDataReader reader, string columnName)
        {
            try
            {
                int columnIndex = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(columnIndex))
                    return null;

                var value = reader.GetValue(columnIndex);
                return value?.ToString();
            }
            catch
            {
                return null;
            }
        }

        private static bool? GetSafeBoolean(IDataReader reader, string columnName)
        {
            try
            {
                int columnIndex = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(columnIndex))
                    return null;

                var value = reader.GetValue(columnIndex);
                if (value is bool boolValue)
                    return boolValue;
                if (value is string stringValue)
                {
                    if (bool.TryParse(stringValue, out bool parsedValue))
                        return parsedValue;
                    if (stringValue.Equals("1", StringComparison.OrdinalIgnoreCase) || 
                        stringValue.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                        stringValue.Equals("si", StringComparison.OrdinalIgnoreCase) ||
                        stringValue.Equals("yes", StringComparison.OrdinalIgnoreCase))
                        return true;
                    if (stringValue.Equals("0", StringComparison.OrdinalIgnoreCase) || 
                        stringValue.Equals("false", StringComparison.OrdinalIgnoreCase) ||
                        stringValue.Equals("no", StringComparison.OrdinalIgnoreCase))
                        return false;
                }
                if (value is int intValue)
                    return intValue != 0;
                if (value is decimal decimalValue)
                    return decimalValue != 0;

                return null;
            }
            catch
            {
                return null;
            }
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

        // PATCH: api/EntryRequest/{id}/notification
        [HttpPatch("{id}/notification")]
        public async Task<ActionResult<NotificationUpdateResponseDTO>> UpdateNotification(
            int id, 
            [FromQuery] string companyCode, 
            [FromBody] bool notification)
        {
            try
            {
                Console.WriteLine($"=== INICIO UpdateNotification ===");
                Console.WriteLine($"Id: {id}, CompanyCode: {companyCode}, Notification: {notification}");
                
                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest(new NotificationUpdateResponseDTO
                    {
                        Success = false,
                        Message = "Error de validación",
                        ErrorDetails = "El código de compañía es requerido"
                    });
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");
                
                // Buscar el EntryRequest por ID
                var entryRequest = await companyContext.EntryRequests.FindAsync(id);
                
                if (entryRequest == null)
                {
                    Console.WriteLine($"No se encontró la solicitud con ID {id}");
                    return NotFound(new NotificationUpdateResponseDTO
                    {
                        Success = false,
                        Message = "Recurso no encontrado",
                        ErrorDetails = $"No se encontró la solicitud de entrada con ID {id}"
                    });
                }

                // Actualizar el campo Notification
                entryRequest.Notification = notification;
                
                // Guardar los cambios
                await companyContext.SaveChangesAsync();
                
                // Invalidar caché
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);
                
                Console.WriteLine($"Campo Notification actualizado exitosamente para ID {id}");
                Console.WriteLine("=== FIN UpdateNotification ===");
                
                return Ok(new NotificationUpdateResponseDTO
                {
                    Success = true,
                    Message = "Campo Notification actualizado exitosamente",
                    UpdatedId = id
                });
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException en UpdateNotification: {ex.Message}");
                return NotFound(new NotificationUpdateResponseDTO
                {
                    Success = false,
                    Message = "Error de configuración",
                    ErrorDetails = ex.Message
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en UpdateNotification ===");
                Console.WriteLine($"Mensaje de error: {ex.Message}");
                Console.WriteLine($"Tipo de excepción: {ex.GetType().Name}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Exception StackTrace: {ex.InnerException.StackTrace}");
                }
                
                Console.WriteLine("=== FIN ERROR ===");
                
                return StatusCode(500, new NotificationUpdateResponseDTO
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    ErrorDetails = ex.Message
                });
            }
        }

        // GET: api/EntryRequest/datosConfirmacion
        [HttpGet("datosConfirmacion")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<EntryRequestConfirmacionDTO>> GetDatosConfirmacion(
            [FromQuery] string companyCode,
            [FromQuery] int numeroPedido)
        {
            try
            {
                Console.WriteLine($"=== INICIO GetDatosConfirmacion ===");
                Console.WriteLine($"CompanyCode: {companyCode}");
                Console.WriteLine($"NumeroPedido: {numeroPedido}");
                
                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                if (numeroPedido <= 0)
                {
                    Console.WriteLine("Error: NumeroPedido debe ser mayor a 0");
                    return BadRequest("El número de pedido debe ser mayor a 0");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");
                
                // Ejecutar el procedimiento almacenado
                var sql = "EXEC sp_CorreoConfirmacion @NumeroPedido";
                Console.WriteLine($"Ejecutando SQL: {sql}");
                Console.WriteLine($"Parámetro @NumeroPedido: {numeroPedido}");

                // Ejecutar el procedimiento almacenado usando DbCommand
                using var command = companyContext.Database.GetDbConnection().CreateCommand();
                command.CommandText = sql;
                command.CommandType = System.Data.CommandType.Text;
                
                // Agregar parámetro
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@NumeroPedido";
                parameter.Value = numeroPedido;
                command.Parameters.Add(parameter);
                
                companyContext.Database.OpenConnection();
                using var reader = await command.ExecuteReaderAsync();
                
                // Leer el resultado (el procedimiento debería devolver una sola fila)
                if (await reader.ReadAsync())
                {
                    var resultado = new EntryRequestConfirmacionDTO
                    {
                        IdPedido = GetSafeInt32(reader, "idPedido"),
                        Saludo = GetSafeString(reader, "Saludo"),
                        HeaderCorreo = GetSafeString(reader, "HeaderCorreo"),
                        ListaEquipos = GetSafeString(reader, "ListaEquipos"),
                        FooterCorreo = GetSafeString(reader, "FooterCorreo"),
                        Destinatario = GetSafeString(reader, "destinatario")
                    };

                    Console.WriteLine($"Datos de confirmación obtenidos exitosamente");
                    Console.WriteLine($"IdPedido: {resultado.IdPedido}");
                    Console.WriteLine($"Saludo: {resultado.Saludo}");
                    Console.WriteLine($"HeaderCorreo: {resultado.HeaderCorreo}");
                    Console.WriteLine($"ListaEquipos: {resultado.ListaEquipos}");
                    Console.WriteLine($"FooterCorreo: {resultado.FooterCorreo}");
                    Console.WriteLine($"Destinatario: {resultado.Destinatario}");
                    Console.WriteLine("=== FIN GetDatosConfirmacion ===");
                    
                    return Ok(resultado);
                }
                else
                {
                    Console.WriteLine("No se encontraron datos de confirmación para el pedido especificado");
                    Console.WriteLine("=== FIN GetDatosConfirmacion ===");
                    return NotFound($"No se encontraron datos de confirmación para el pedido {numeroPedido}");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException en GetDatosConfirmacion: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en GetDatosConfirmacion ===");
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

        // GET: api/EntryRequest/ordenes-despacho
        [HttpGet("ordenes-despacho")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<EntryRequestOrdenesDespachoDTO>>> GetOrdenesDespacho(
            [FromQuery] string companyCode,
            [FromQuery] int? branchId = null,
            [FromQuery] DateTime? dateIni = null,
            [FromQuery] DateTime? dateEnd = null,
            [FromQuery] DateTime? deliveryDate = null,
            [FromQuery] string? filterText = null)
        {
            try
            {
                Console.WriteLine($"=== INICIO GetOrdenesDespacho ===");
                Console.WriteLine($"CompanyCode: {companyCode}");
                Console.WriteLine($"BranchId: {branchId}");
                Console.WriteLine($"DateIni: {dateIni}");
                Console.WriteLine($"DateEnd: {dateEnd}");
                Console.WriteLine($"DeliveryDate: {deliveryDate}");
                Console.WriteLine($"FilterText: {filterText}");
                
                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");
                
                // Ejecutar el procedimiento almacenado
                var sql = "EXEC GET_ENTRYREQUEST_3110 @BRANCHID, @DATEINI, @DATEEND, @DELIVERYDATE, @FILTERTEXT";
                Console.WriteLine($"Ejecutando SQL: {sql}");
                Console.WriteLine($"Parámetros:");
                Console.WriteLine($"  @BRANCHID: {branchId?.ToString() ?? "NULL"}");
                Console.WriteLine($"  @DATEINI: {dateIni?.Date.ToString("yyyy-MM-dd") ?? "NULL"}");
                Console.WriteLine($"  @DATEEND: {dateEnd?.Date.ToString("yyyy-MM-dd") ?? "NULL"}");
                Console.WriteLine($"  @DELIVERYDATE: {deliveryDate?.Date.ToString("yyyy-MM-dd") ?? "NULL"}");
                Console.WriteLine($"  @FILTERTEXT: {filterText ?? "NULL"}");

                // Ejecutar el procedimiento almacenado usando DbCommand
                using var command = companyContext.Database.GetDbConnection().CreateCommand();
                command.CommandText = sql;
                command.CommandType = System.Data.CommandType.Text;
                
                // Agregar parámetros
                var parameters = new[]
                {
                    new { Name = "@BRANCHID", Value = branchId ?? (object)DBNull.Value },
                    new { Name = "@DATEINI", Value = dateIni?.Date ?? (object)DBNull.Value },
                    new { Name = "@DATEEND", Value = dateEnd?.Date ?? (object)DBNull.Value },
                    new { Name = "@DELIVERYDATE", Value = deliveryDate?.Date ?? (object)DBNull.Value },
                    new { Name = "@FILTERTEXT", Value = filterText ?? (object)DBNull.Value }
                };
                
                foreach (var param in parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = param.Name;
                    parameter.Value = param.Value;
                    command.Parameters.Add(parameter);
                }
                
                companyContext.Database.OpenConnection();
                using var reader = await command.ExecuteReaderAsync();
                
                var resultados = new List<EntryRequestOrdenesDespachoDTO>();
                
                while (await reader.ReadAsync())
                {
                    var item = new EntryRequestOrdenesDespachoDTO
                    {
                        Id = GetSafeInt32(reader, "Id"),
                        Date = GetSafeDateTime(reader, "Date"),
                        Service = GetSafeString(reader, "Service"),
                        IdOrderType = GetSafeInt32(reader, "IdOrderType"),
                        DeliveryPriority = GetSafeString(reader, "DeliveryPriority"),
                        IdCustomer = GetSafeInt32(reader, "IdCustomer"),
                        InsurerType = GetSafeInt32(reader, "InsurerType"),
                        Insurer = GetSafeInt32(reader, "Insurer"),
                        IdMedic = GetSafeInt32(reader, "IdMedic"),
                        IdPatient = GetSafeInt32(reader, "IdPatient"),
                        Applicant = GetSafeString(reader, "Applicant"),
                        IdATC = GetSafeInt32(reader, "IdATC"),
                        LimbSide = GetSafeString(reader, "LimbSide"),
                        DeliveryDate = GetSafeDateTime(reader, "DeliveryDate"),
                        OrderObs = GetSafeString(reader, "OrderObs"),
                        SurgeryTime = GetSafeInt32(reader, "SurgeryTime"),
                        SurgeryInit = GetSafeDateTime(reader, "SurgeryInit"),
                        SurgeryEnd = GetSafeDateTime(reader, "SurgeryEnd"),
                        Status = GetSafeString(reader, "Status"),
                        IdTraceStates = GetSafeInt32(reader, "IdTraceStates"),
                        SurgeryInitTime = GetSafeInt32(reader, "SurgeryInitTime"),
                        SurgeryEndTime = GetSafeInt32(reader, "SurgeryEndTime"),
                        BranchId = GetSafeInt32(reader, "BranchId"),
                        DeliveryAddress = GetSafeString(reader, "DeliveryAddress"),
                        PurchaseOrder = GetSafeString(reader, "PurchaseOrder"),
                        AtcConsumed = GetSafeBoolean(reader, "AtcConsumed"),
                        IsSatisfied = GetSafeBoolean(reader, "IsSatisfied"),
                        Observations = GetSafeString(reader, "Observations"),
                        AuxLog = GetSafeInt32(reader, "AuxLog"),
                        Notification = GetSafeBoolean(reader, "Notification"),
                        IdCancelReason = GetSafeInt32(reader, "IdCancelReason"),
                        CancelReason = GetSafeString(reader, "CancelReason"),
                        IdCancelDetail = GetSafeInt32(reader, "IdCancelDetail"),
                        CancelDetail = GetSafeString(reader, "CancelDetail"),
                        IsReplacement = GetSafeBoolean(reader, "IsReplacement"),
                        AssemblyComponents = GetSafeBoolean(reader, "AssemblyComponents"),
                        obsMaint = GetSafeString(reader, "obsMaint"),
                        priceGroup = GetSafeString(reader, "priceGroup"),
                        traceState = GetSafeString(reader, "traceState"),
                        CustomerName = GetSafeString(reader, "CustomerName"),
                        Contact = GetSafeString(reader, "Contact"),
                        PatientName = GetSafeString(reader, "PatientName"),
                        MedicName = GetSafeString(reader, "MedicName"),
                        InsurerName = GetSafeString(reader, "InsurerName"),
                        InsurerTypeName = GetSafeString(reader, "InsurerTypeName"),
                        Branch = GetSafeString(reader, "Branch"),
                        IsRemLot = GetSafeBoolean(reader, "IsRemLot"),
                        ATCName = GetSafeString(reader, "ATCName"),
                        equipos = GetSafeString(reader, "equipos"),
                        Componentes = GetSafeString(reader, "Componentes"),
                        RemCustomer = GetSafeString(reader, "RemCustomer"),
                        ShortDesc = GetSafeString(reader, "ShortDesc")
                    };
                    resultados.Add(item);
                }

                Console.WriteLine($"Consulta completada. Registros encontrados: {resultados.Count}");
                Console.WriteLine("=== FIN GetOrdenesDespacho ===");
                
                return Ok(resultados);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException en GetOrdenesDespacho: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en GetOrdenesDespacho ===");
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
    }
} 