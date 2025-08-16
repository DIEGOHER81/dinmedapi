using Microsoft.AspNetCore.Mvc;
using DimmedAPI.Entidades;
using DimmedAPI.Services;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using DimmedAPI.DTOs;
using DimmedAPI.Interfaces;
using System.Collections.Generic;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace DimmedAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntryRequestController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;
        private readonly IDynamicBCConnectionService _dynamicBCConnectionService;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly ICustomerPriceListBO _customerPriceListBO;
        private const string cacheTag = "entryrequest";

        public EntryRequestController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService,
            IDynamicBCConnectionService dynamicBCConnectionService,
            IOutputCacheStore outputCacheStore,
            ICustomerPriceListBO customerPriceListBO)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
            _dynamicBCConnectionService = dynamicBCConnectionService;
            _outputCacheStore = outputCacheStore;
            _customerPriceListBO = customerPriceListBO;
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

        /// <summary>
        /// Consulta básica de pedido por id con todas sus relaciones
        /// </summary>
        /// <param name="id">Id del registro</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Objeto EntryRequests con todas sus relaciones</returns>
        // GET: api/EntryRequest/{id}/with-details
        [HttpGet("{id}/with-details")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<EntryRequests>> GetByIdWithDetails(int id, [FromQuery] string companyCode)
        {
            try
            {
                Console.WriteLine($"=== GetByIdWithDetails INICIO ===");
                Console.WriteLine($"ID: {id}, CompanyCode: {companyCode}");
                
                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("ERROR: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                if (id == 0)
                {
                    Console.WriteLine("ERROR: ID es 0");
                    return BadRequest("El ID no puede ser 0");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de compañía obtenido correctamente");
                
                // Obtener la EntryRequest básica
                var dataEntry = await companyContext.EntryRequests
                    .Include(er => er.IdCustomerNavigation)
                    .Include(er => er.IdMedicNavigation)
                    .Include(er => er.IdPatientNavigation)
                    .Include(er => er.IdAtcNavigation)
                    .Include(er => er.Branch)
                    .Include(er => er.InsurerNavigation)
                    .Include(er => er.InsurerTypeNavigation)
                    .Include(er => er.IdTraceStatesNavigation)
                    .FirstOrDefaultAsync(er => er.Id == id);

                Console.WriteLine($"EntryRequest encontrada: {dataEntry != null}");
                if (dataEntry != null)
                {
                    Console.WriteLine($"EntryRequest ID: {dataEntry.Id}");
                }

                if (dataEntry == null)
                {
                    Console.WriteLine($"ERROR: No se encontró EntryRequest con ID {id}");
                    return NotFound($"No se encontró la solicitud de entrada con ID {id}");
                }

                // Obtener los detalles de la EntryRequest
                var details = await companyContext.EntryRequestDetails
                    .Include(erd => erd.IdEquipmentNavigation)
                    .Where(x => x.IdEntryReq == id)
                    .ToListAsync();

                Console.WriteLine($"Details encontrados: {details?.Count ?? 0}");

                if (details != null && details.Any())
                {
                    dataEntry.EntryRequestDetails = details;
                    Console.WriteLine($"Details asignados a EntryRequest: {dataEntry.EntryRequestDetails.Count}");
                    
                    // Obtener los ensambles de la EntryRequest
                    var dataAssembly = await companyContext.EntryRequestAssembly
                        .Where(x => x.EntryRequestId == id)
                        .OrderByDescending(x => x.QuantityConsumed)
                        .ToListAsync();

                    Console.WriteLine($"Assemblies encontrados: {dataAssembly?.Count ?? 0}");

                    if (dataAssembly != null && dataAssembly.Any())
                    {
                        dataEntry.EntryRequestAssembly = dataAssembly;
                        Console.WriteLine($"Assemblies asignados a EntryRequest: {dataEntry.EntryRequestAssembly.Count}");

                        // Asignar los ensambles a cada detalle correspondiente
                        foreach (var detail in dataEntry.EntryRequestDetails)
                        {
                            detail.EntryRequestAssembly = dataAssembly
                                .Where(y => y.EntryRequestDetailId == detail.Id)
                                .ToList();
                        }
                        Console.WriteLine("Assemblies asignados a cada detalle");
                    }
                }

                // Obtener los componentes de la EntryRequest
                var dataComponents = await companyContext.EntryRequestComponents
                    .Where(x => x.IdEntryReq == id)
                    .ToListAsync();

                Console.WriteLine($"Components encontrados: {dataComponents?.Count ?? 0}");

                if (dataComponents != null && dataComponents.Any())
                {
                    dataEntry.EntryRequestComponents = dataComponents;
                    Console.WriteLine($"Components asignados a EntryRequest: {dataEntry.EntryRequestComponents.Count}");
                }

                Console.WriteLine($"=== GetByIdWithDetails FINAL ===");
                Console.WriteLine($"EntryRequest final - Details: {dataEntry.EntryRequestDetails?.Count ?? 0}");
                Console.WriteLine($"EntryRequest final - Assemblies: {dataEntry.EntryRequestAssembly?.Count ?? 0}");
                Console.WriteLine($"EntryRequest final - Components: {dataEntry.EntryRequestComponents?.Count ?? 0}");

                return Ok(dataEntry);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException en GetByIdWithDetails: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR en GetByIdWithDetails: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene todas las Entry Requests con todas sus relaciones
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Lista de EntryRequests con todas sus relaciones</returns>
        // GET: api/EntryRequest/with-details
        [HttpGet("with-details")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<IEnumerable<EntryRequests>>> GetAllEntryRequestsWithDetails([FromQuery] string companyCode)
        {
            try
            {
                Console.WriteLine($"=== INICIO GetAllEntryRequestsWithDetails ===");
                Console.WriteLine($"CompanyCode recibido: {companyCode}");
                
                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");
                
                // Obtener todas las EntryRequests con sus relaciones básicas
                var entryRequests = await companyContext.EntryRequests
                    .Include(er => er.IdCustomerNavigation)
                    .Include(er => er.IdMedicNavigation)
                    .Include(er => er.IdPatientNavigation)
                    .Include(er => er.IdAtcNavigation)
                    .Include(er => er.Branch)
                    .Include(er => er.InsurerNavigation)
                    .Include(er => er.InsurerTypeNavigation)
                    .Include(er => er.IdTraceStatesNavigation)
                    .ToListAsync();

                Console.WriteLine($"Total de EntryRequests obtenidas: {entryRequests.Count}");

                // Para cada EntryRequest, obtener sus detalles, ensambles y componentes
                foreach (var entryRequest in entryRequests)
                {
                    // Obtener los detalles de la EntryRequest
                    var details = await companyContext.EntryRequestDetails
                        .Include(erd => erd.IdEquipmentNavigation)
                        .Where(x => x.IdEntryReq == entryRequest.Id)
                        .ToListAsync();

                    if (details != null && details.Any())
                    {
                        entryRequest.EntryRequestDetails = details;

                        // Obtener los ensambles de la EntryRequest
                        var dataAssembly = await companyContext.EntryRequestAssembly
                            .Where(x => x.EntryRequestId == entryRequest.Id)
                            .OrderByDescending(x => x.QuantityConsumed)
                            .ToListAsync();

                        if (dataAssembly != null && dataAssembly.Any())
                        {
                            entryRequest.EntryRequestAssembly = dataAssembly;

                            // Asignar los ensambles a cada detalle correspondiente
                            foreach (var detail in entryRequest.EntryRequestDetails)
                            {
                                detail.EntryRequestAssembly = dataAssembly
                                    .Where(y => y.EntryRequestDetailId == detail.Id)
                                    .ToList();
                            }
                        }
                    }

                    // Obtener los componentes de la EntryRequest
                    var dataComponents = await companyContext.EntryRequestComponents
                        .Where(x => x.IdEntryReq == entryRequest.Id)
                        .ToListAsync();

                    if (dataComponents != null && dataComponents.Any())
                    {
                        entryRequest.EntryRequestComponents = dataComponents;
                    }
                }

                Console.WriteLine("=== FIN GetAllEntryRequestsWithDetails ===");
                
                return Ok(entryRequests);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException en GetAllEntryRequestsWithDetails: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en GetAllEntryRequestsWithDetails ===");
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
                    .Include(er => er.IdAtcNavigation) // Incluir la relación con Employee/ATC
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
                        Employee = er.IdAtcNavigation,
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

        /// <summary>
        /// Consulta si un pedido contiene faltantes
        /// </summary>
        /// <param name="IdEntryReq">Id del registro del pedido</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>ValidDispatchResponseDTO Response</returns>
        [HttpGet("valid-dispatch")]
        public async Task<ActionResult<ValidDispatchResponseDTO>> ValidDispatch([FromQuery] int IdEntryReq, [FromQuery] string companyCode)
        {
            try
            {
                Console.WriteLine($"=== ValidDispatch INICIO ===");
                Console.WriteLine($"IdEntryReq: {IdEntryReq}, CompanyCode: {companyCode}");
                
                var response = new ValidDispatchResponseDTO
                {
                    IsSuccess = true,
                    Result = null,
                    Message = "SAVE"
                };

                if (IdEntryReq == 0)
                {
                    Console.WriteLine("ERROR: IdEntryReq es 0");
                    response.IsSuccess = false;
                    response.Message = "MODEL_NOT_VALID";
                    return BadRequest(response);
                }

                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("ERROR: CompanyCode está vacío");
                    response.IsSuccess = false;
                    response.Message = "El código de compañía es requerido";
                    return BadRequest(response);
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de compañía obtenido correctamente");
                
                // Crear un EquipmentBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var equipmentBO = new BO.EquipmentBO(companyContext, bcConn);
                Console.WriteLine("EquipmentBO creado correctamente");

                // Obtener la Entry Request con todos sus detalles
                Console.WriteLine("Llamando a GetByIdWithDetails...");
                var dataResult = await GetByIdWithDetails(IdEntryReq, companyCode);
                Console.WriteLine($"GetByIdWithDetails completado. Result: {dataResult.Result}");
                
                // Verificar si el resultado es exitoso
                if (dataResult.Result is OkObjectResult okResult)
                {
                    var entryRequest = okResult.Value as EntryRequests;
                    Console.WriteLine($"EntryRequest extraída del OkObjectResult: {entryRequest != null}");
                    
                    if (entryRequest == null)
                    {
                        Console.WriteLine("ERROR: No se pudo extraer EntryRequest del OkObjectResult");
                        response.IsSuccess = false;
                        response.Message = "Entry Request no encontrada";
                        return NotFound(response);
                    }
                    
                    Console.WriteLine($"EntryRequest ID: {entryRequest.Id}");
                    Console.WriteLine($"EntryRequest Details Count: {entryRequest.EntryRequestDetails?.Count ?? 0}");
                    
                    if (entryRequest?.EntryRequestDetails != null && entryRequest.EntryRequestDetails.Count > 0)
                    {
                        Console.WriteLine($"Procesando {entryRequest.EntryRequestDetails.Count} detalles...");
                        
                        foreach (var detail in entryRequest.EntryRequestDetails)
                        {
                            Console.WriteLine($"Procesando detalle ID: {detail.Id}, Equipment ID: {detail.IdEquipment}");
                            
                            List<EntryRequestAssembly> inventory = await equipmentBO.getAInventory(detail.IdEquipment);
                            Console.WriteLine($"Inventory obtenido: {inventory?.Count ?? 0} elementos");
                            
                            if (inventory != null)
                            {
                                bool vld = inventory.Exists(x => x.LocationCode == x.Location_Code_ile && ((x.Quantity - x.ReservedQuantity) > 0));
                                Console.WriteLine($"Validación de inventario: {vld}");
                                
                                if (vld)
                                {
                                    response.Message = "FALTANTES";
                                    Console.WriteLine("Se encontraron faltantes");
                                }
                            }
                            else
                            {
                                response.Message = "FALTANTES";
                                Console.WriteLine("Inventory es null - faltantes");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No hay detalles en la EntryRequest");
                    }
                }
                else
                {
                    Console.WriteLine($"ERROR: Result no es OkObjectResult, es: {dataResult.Result?.GetType().Name}");
                    response.IsSuccess = false;
                    response.Message = "Error al obtener Entry Request";
                    return BadRequest(response);
                }

                Console.WriteLine($"=== ValidDispatch FINAL ===");
                Console.WriteLine($"Response Message: {response.Message}");
                Console.WriteLine($"Response IsSuccess: {response.IsSuccess}");

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR en ValidDispatch: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                var response = new ValidDispatchResponseDTO
                {
                    IsSuccess = false,
                    Result = null,
                    Message = ex.Message,
                    ErrorDetails = ex.StackTrace?.Length > 200 ? ex.StackTrace.Substring(0, 200) : ex.StackTrace
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Método de sincronización de productos de un pedido a CRM
        /// </summary>
        /// <param name="entryRequestId">ID del Entry Request a sincronizar</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>SincronizarProductosCRMResponseDTO Response</returns>
        [HttpPost("sincronizar-productos-crm")]
        public async Task<ActionResult<SincronizarProductosCRMResponseDTO>> SincronizarProductosCRM([FromBody] int entryRequestId, [FromQuery] string companyCode)
        {
            try
            {
                var response = new SincronizarProductosCRMResponseDTO
                {
                    IsSuccess = true,
                    Result = null,
                    Message = "SAVE",
                    EntryRequestId = entryRequestId
                };

                if (string.IsNullOrEmpty(companyCode))
                {
                    response.IsSuccess = false;
                    response.Message = "El código de compañía es requerido";
                    return BadRequest(response);
                }

                if (entryRequestId == 0)
                {
                    response.IsSuccess = false;
                    response.Message = "El ID del Entry Request es requerido";
                    return BadRequest(response);
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                
                // Crear un EquipmentBO con el contexto específico de la compañía
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                var equipmentBO = new BO.EquipmentBO(companyContext, bcConn);

                // Obtener la Entry Request con todos sus detalles
                var dataPedidoResult = await GetByIdWithDetails(entryRequestId, companyCode);
                
                if (dataPedidoResult.Result is not OkObjectResult okResult)
                {
                    response.IsSuccess = false;
                    response.Message = "Error al obtener Entry Request";
                    return BadRequest(response);
                }

                var entryRequest = okResult.Value as EntryRequests;
                
                if (entryRequest == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Entry Request no encontrada";
                    return NotFound(response);
                }
                
                // Sincronizar productos a CRM
                string result = await equipmentBO.CRMProductsSyncronize(entryRequest);
                
                response.Message = result;
                response.TotalProductsSynchronized = entryRequest?.EntryRequestDetails?.Count;
                
                // Agregar información de la Entry Request
                response.EntryRequestInfo = $"Entry Request #{entryRequest.Id} - {entryRequest.Service} - {entryRequest.DeliveryPriority}";
                
                // Agregar detalles de los productos sincronizados
                if (entryRequest?.EntryRequestDetails != null && entryRequest.EntryRequestDetails.Any())
                {
                    response.ProductosSincronizados = new List<ProductoSincronizadoDTO>();
                    
                    foreach (var detail in entryRequest.EntryRequestDetails)
                    {
                        if (detail.IdEquipmentNavigation != null)
                        {
                            var producto = new ProductoSincronizadoDTO
                            {
                                Id = detail.IdEquipmentNavigation.Id,
                                Code = detail.IdEquipmentNavigation.Code,
                                Name = detail.IdEquipmentNavigation.Name,
                                ShortName = detail.IdEquipmentNavigation.ShortName,
                                Status = detail.IdEquipmentNavigation.Status,
                                ProductLine = detail.IdEquipmentNavigation.ProductLine,
                                Branch = detail.IdEquipmentNavigation.Branch,
                                Brand = detail.IdEquipmentNavigation.Brand,
                                Model = detail.IdEquipmentNavigation.Model,
                                Type = detail.IdEquipmentNavigation.Type,
                                SystemIdBC = detail.IdEquipmentNavigation.SystemIdBC
                            };
                            
                            response.ProductosSincronizados.Add(producto);
                        }
                    }
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new SincronizarProductosCRMResponseDTO
                {
                    IsSuccess = false,
                    Result = null,
                    Message = ex.Message,
                    ErrorDetails = ex.StackTrace?.Length > 200 ? ex.StackTrace.Substring(0, 200) : ex.StackTrace,
                    EntryRequestId = entryRequestId
                };
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Generar excel de remisión
        /// </summary>
        /// <param name="IdEntryReq">Id del pedido</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>FileResult</returns>
        [HttpGet("generate-xlsx")]
        public async Task<FileResult> GenerateXLSXAsync([FromQuery] int IdEntryReq, [FromQuery] string companyCode)
        {
            try
            {
                Console.WriteLine($"=== INICIO GenerateXLSXAsync ===");
                Console.WriteLine($"IdEntryReq: {IdEntryReq}");
                Console.WriteLine($"CompanyCode: {companyCode}");

                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    throw new ArgumentException("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");

                // Obtener la solicitud de entrada con sus relaciones básicas
                var dataInformation = await companyContext.EntryRequests
                    .Include(er => er.IdCustomerNavigation)
                    .Include(er => er.IdPatientNavigation)
                    .Include(er => er.IdMedicNavigation)
                    .Include(er => er.IdAtcNavigation)
                    .Include(er => er.EntryRequestDetails)
                        .ThenInclude(erd => erd.IdEquipmentNavigation)
                    .Include(er => er.EntryRequestComponents)
                    .FirstOrDefaultAsync(er => er.Id == IdEntryReq);

                if (dataInformation == null)
                {
                    Console.WriteLine($"No se encontró la solicitud de entrada con ID: {IdEntryReq}");
                    throw new ArgumentException($"No se encontró la solicitud de entrada con ID: {IdEntryReq}");
                }

                // Obtener los ensambles de la EntryRequest
                var dataAssembly = await companyContext.EntryRequestAssembly
                    .Where(x => x.EntryRequestId == IdEntryReq)
                    .OrderByDescending(x => x.QuantityConsumed)
                    .ToListAsync();

                Console.WriteLine($"Assemblies encontrados: {dataAssembly?.Count ?? 0}");

                if (dataAssembly != null && dataAssembly.Any())
                {
                    dataInformation.EntryRequestAssembly = dataAssembly;
                    Console.WriteLine($"Assemblies asignados a EntryRequest: {dataInformation.EntryRequestAssembly.Count}");

                    // Asignar los ensambles a cada detalle correspondiente
                    foreach (var detail in dataInformation.EntryRequestDetails)
                    {
                        detail.EntryRequestAssembly = dataAssembly
                            .Where(y => y.EntryRequestDetailId == detail.Id)
                            .ToList();
                    }
                    Console.WriteLine("Assemblies asignados a cada detalle");
                }

                Console.WriteLine($"Solicitud encontrada: {dataInformation.Id}");
                Console.WriteLine($"Cliente: {dataInformation.IdCustomerNavigation?.Name}");
                Console.WriteLine($"Paciente: {dataInformation.IdPatientNavigation?.Name}");

                // Configurar EPPlus para uso no comercial
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                int index = 17;
                decimal totalQuantity;
                var workbook = new ExcelPackage();
                var worksheet = workbook.Workbook.Worksheets.Add("Sheet1");

                string deliveryAddress = dataInformation.DeliveryAddress ?? string.Empty;
                List<string> details = new List<string>();
                List<string> detailsZero = new List<string>();

                // Configurar estilos del encabezado
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Size = 30;
                worksheet.Cells[2, 1].Value = "";
                worksheet.Cells[2, 1].Style.Font.Bold = true;
                worksheet.Cells[2, 1].Style.Font.Size = 30;
                worksheet.Cells[1, 5].Value = "REMISIÓN P-" + IdEntryReq.ToString();
                worksheet.Cells[1, 5].Style.Font.Bold = true;

                // Información del cliente y paciente
                worksheet.Cells[5, 1].Value = "Cliente Pagador:";
                worksheet.Cells[5, 1].Style.Font.Bold = true;
                worksheet.Cells[6, 1].Value = "Nit:";
                worksheet.Cells[6, 1].Style.Font.Bold = true;
                worksheet.Cells[7, 1].Value = "Direccion:";
                worksheet.Cells[7, 1].Style.Font.Bold = true;
                worksheet.Cells[8, 1].Value = "Telefono";
                worksheet.Cells[8, 1].Style.Font.Bold = true;
                worksheet.Cells[9, 1].Value = "Ciudad";
                worksheet.Cells[9, 1].Style.Font.Bold = true;
                worksheet.Cells[5, 4].Value = "Paciente:";
                worksheet.Cells[5, 4].Style.Font.Bold = true;
                worksheet.Cells[6, 4].Value = "Cedula Paciente:";
                worksheet.Cells[6, 4].Style.Font.Bold = true;
                worksheet.Cells[7, 4].Value = "Dir. Entrega:";
                worksheet.Cells[7, 4].Style.Font.Bold = true;
                worksheet.Cells[8, 4].Value = "Nombre Solicitante";
                worksheet.Cells[8, 4].Style.Font.Bold = true;
                worksheet.Cells[9, 4].Value = "Doctor";
                worksheet.Cells[9, 4].Style.Font.Bold = true;
                worksheet.Cells[10, 4].Value = "Oden de Compra";
                worksheet.Cells[10, 4].Style.Font.Bold = true;
                worksheet.Cells[11, 4].Value = "Historia Clinica";
                worksheet.Cells[11, 4].Style.Font.Bold = true;

                // Datos del cliente
                worksheet.Cells[5, 2].Value = dataInformation.IdCustomerNavigation?.Name ?? "";
                worksheet.Cells[5, 2].Style.Font.Bold = true;
                worksheet.Cells[6, 2].Value = dataInformation.IdCustomerNavigation?.Identification ?? "";
                worksheet.Cells[7, 2].Value = dataInformation.IdCustomerNavigation?.Address ?? "";
                worksheet.Cells[8, 2].Value = dataInformation.IdCustomerNavigation?.Phone ?? "";
                worksheet.Cells[9, 2].Value = dataInformation.IdCustomerNavigation?.City ?? "";

                // Datos del paciente
                worksheet.Cells[5, 5].Value = $"{dataInformation.IdPatientNavigation?.Name ?? ""} {dataInformation.IdPatientNavigation?.LastName ?? ""}";
                worksheet.Cells[6, 5].Value = dataInformation.IdPatientNavigation?.Identification ?? "";
                worksheet.Cells[7, 5].Value = deliveryAddress;
                worksheet.Cells[8, 5].Value = dataInformation.Applicant ?? "";
                worksheet.Cells[9, 5].Value = $"{dataInformation.IdMedicNavigation?.Name ?? ""} {dataInformation.IdMedicNavigation?.LastName ?? ""}";
                worksheet.Cells[10, 5].Value = dataInformation.PurchaseOrder ?? "";
                worksheet.Cells[11, 5].Value = dataInformation.IdPatientNavigation?.MedicalRecord ?? "";

                // Información del asesor técnico
                worksheet.Cells[13, 1].Value = "Asesor Tecnico";
                worksheet.Cells[13, 1].Style.Font.Bold = true;
                worksheet.Cells[13, 2].Value = "Fecha de Entrega";
                worksheet.Cells[13, 2].Style.Font.Bold = true;
                worksheet.Cells[13, 3].Value = "Fecha de Cirugia";
                worksheet.Cells[13, 3].Style.Font.Bold = true;
                worksheet.Cells[13, 4].Value = "Hora de Cirugía";
                worksheet.Cells[13, 4].Style.Font.Bold = true;
                worksheet.Cells[13, 5].Value = "Fecha de Recogida";
                worksheet.Cells[13, 5].Style.Font.Bold = true;

                index = index + 1;
                if (dataInformation.IdAtcNavigation != null && dataInformation.IdAtcNavigation.Name != null)
                {
                    worksheet.Cells[14, 1].Value = dataInformation.IdAtcNavigation.Name;
                }
                else
                {
                    worksheet.Cells[14, 1].Value = " ";
                }
                worksheet.Cells[14, 2].Value = dataInformation.DeliveryDate.ToString("dd/MM/yyyy");
                worksheet.Cells[14, 3].Value = dataInformation.SurgeryInit?.ToString("dd/MM/yyyy") ?? "";
                worksheet.Cells[14, 4].Value = dataInformation.SurgeryInit?.ToString("hh:mm tt") ?? "";
                worksheet.Cells[14, 5].Value = dataInformation.SurgeryEnd?.ToString("dd/MM/yyyy") ?? "";

                // Componentes adicionales
                if (dataInformation.EntryRequestComponents != null && dataInformation.EntryRequestComponents.Any())
                {
                    index = index + 1;
                    worksheet.Cells[index, 1].Value = "COMPONENTES ADICIONALES";
                    worksheet.Cells[index, 1].Style.Font.Bold = true;

                    index = index + 1;
                    worksheet.Cells[index, 1].Value = "REF";
                    worksheet.Cells[index, 1].Style.Font.Bold = true;
                    worksheet.Cells[index, 2].Value = "Descripción";
                    worksheet.Cells[index, 2].Style.Font.Bold = true;
                    worksheet.Cells[index, 3].Value = "INVIMA";
                    worksheet.Cells[index, 3].Style.Font.Bold = true;
                    worksheet.Cells[index, 4].Value = "LOTE";
                    worksheet.Cells[index, 4].Style.Font.Bold = true;
                    worksheet.Cells[index, 5].Value = "Cant. LT";
                    worksheet.Cells[index, 5].Style.Font.Bold = true;
                    worksheet.Cells[index, 6].Value = "Cant. Total";
                    worksheet.Cells[index, 6].Style.Font.Bold = true;
                    worksheet.Cells[index, 7].Value = "Precio U.";
                    worksheet.Cells[index, 7].Style.Font.Bold = true;
                    worksheet.Cells[index, 8].Value = "IVA";
                    worksheet.Cells[index, 8].Style.Font.Bold = true;
                    worksheet.Cells[index, 9].Value = "Gasto";
                    worksheet.Cells[index, 9].Style.Font.Bold = true;

                    index = index + 1;

                    foreach (var CompoD in dataInformation.EntryRequestComponents)
                    {
                        decimal totalQuantityAd = dataInformation.EntryRequestComponents.Where(x => x.ItemNo == CompoD.ItemNo).Sum(x => (decimal)(x.Quantity ?? 0));

                        worksheet.Cells[index, 1].Value = CompoD.ItemNo;
                        worksheet.Cells[index, 2].Value = CompoD.ItemName;
                        worksheet.Cells[index, 3].Value = CompoD.Invima;
                        worksheet.Cells[index, 6].Value = totalQuantityAd;
                        index = index + 1;

                        foreach (var CompoDdub in dataInformation.EntryRequestComponents)
                        {
                            if (CompoDdub.ItemNo == CompoD.ItemNo)
                            {
                                worksheet.Cells[index, 1].Value = "";
                                worksheet.Cells[index, 2].Value = "";
                                worksheet.Cells[index, 3].Value = "";
                                worksheet.Cells[index, 4].Value = CompoD.Lot;
                                worksheet.Cells[index, 5].Value = CompoD.Quantity;
                                if (CompoD.UnitPrice != null)
                                {
                                    decimal UP = (decimal)CompoD.UnitPrice;
                                    worksheet.Cells[index, 7].Value = string.Format("{0:n0}", UP);
                                }
                                else
                                    worksheet.Cells[index, 7].Value = "0";
                                if (CompoD.UnitPrice != null && CompoD.UnitPrice != 0 && CompoD.TaxCode != "V_ARTVENTAEXC" && dataInformation.IdCustomerNavigation?.ExIva == true)
                                {
                                    decimal IVA = ((decimal)CompoD.UnitPrice) * (decimal)0.19;
                                    worksheet.Cells[index, 8].Value = string.Format("{0:n0}", IVA);
                                }
                                else
                                {
                                    worksheet.Cells[index, 8].Value = "0";
                                }
                                worksheet.Cells[index, 9].Value = "0";
                                index = index + 1;
                            }
                        }
                    }
                }

                // Equipos y sus componentes
                foreach (var eq in dataInformation.EntryRequestDetails)
                {
                    worksheet.Cells[index, 1].Value = eq.IdEquipmentNavigation?.Code ?? "";
                    worksheet.Cells[index, 1].Style.Font.Bold = true;
                    worksheet.Cells[index, 2].Value = eq.IdEquipmentNavigation?.Name ?? "";
                    worksheet.Cells[index, 2].Style.Font.Bold = true;
                    worksheet.Cells[index, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[index, 8].Value = "# Cajas";
                    worksheet.Cells[index, 8].Style.Font.Bold = true;
                    worksheet.Cells[index, 9].Value = eq.IdEquipmentNavigation?.NoBoxes ?? 0;
                    index += 1;

                    if (eq.EntryRequestAssembly != null)
                    {
                        worksheet.Cells[index, 1].Value = "REF";
                        worksheet.Cells[index, 1].Style.Font.Bold = true;
                        worksheet.Cells[index, 2].Value = "Descripción";
                        worksheet.Cells[index, 2].Style.Font.Bold = true;
                        worksheet.Cells[index, 3].Value = "INVIMA";
                        worksheet.Cells[index, 3].Style.Font.Bold = true;
                        worksheet.Cells[index, 4].Value = "LOTE";
                        worksheet.Cells[index, 4].Style.Font.Bold = true;
                        worksheet.Cells[index, 5].Value = "Cant. LT";
                        worksheet.Cells[index, 5].Style.Font.Bold = true;
                        worksheet.Cells[index, 6].Value = "Cant. Total";
                        worksheet.Cells[index, 6].Style.Font.Bold = true;
                        worksheet.Cells[index, 7].Value = "Precio U.";
                        worksheet.Cells[index, 7].Style.Font.Bold = true;
                        worksheet.Cells[index, 8].Value = "IVA";
                        worksheet.Cells[index, 8].Style.Font.Bold = true;
                        worksheet.Cells[index, 9].Value = "Gasto";
                        worksheet.Cells[index, 9].Style.Font.Bold = true;

                        if (eq.EntryRequestAssembly != null && eq.EntryRequestAssembly.Count > 0)
                        {
                            var validq = new List<EntryRequestAssembly>();
                            if (dataInformation.Id > 9597)
                                validq = eq.EntryRequestAssembly.OrderBy(x => x.Position).ToList();
                            else
                                validq = eq.EntryRequestAssembly.OrderBy(x => x.LineNo).ToList();

                            foreach (var subDetail in validq)
                            {
                                totalQuantity = (decimal)validq.Where(x => x.Code == subDetail.Code).Sum(x => x.ReservedQuantity ?? 0);

                                if (!details.Contains(subDetail.Code))
                                {
                                    details.Add(subDetail.Code);
                                    if (totalQuantity > 0)
                                    {
                                        index += 1;
                                        worksheet.Cells[index, 1].Value = subDetail.Code;
                                        worksheet.Cells[index, 2].Value = subDetail.Description;
                                        worksheet.Cells[index, 6].Value = totalQuantity;
                                        worksheet.Cells[index, 8].Value = "";
                                        index = index + 1;

                                        foreach (var equ in validq)
                                        {
                                            if (equ.Code == subDetail.Code)
                                            {
                                                if (!string.IsNullOrEmpty(equ.Lot))
                                                {
                                                    worksheet.Cells[index, 1].Value = "";
                                                    worksheet.Cells[index, 2].Value = "";
                                                    worksheet.Cells[index, 3].Value = equ.Invima;
                                                    worksheet.Cells[index, 4].Value = equ.Lot;
                                                    worksheet.Cells[index, 5].Value = equ.ReservedQuantity;
                                                    if (equ.UnitPrice != null)
                                                    {
                                                        decimal UP = (decimal)equ.UnitPrice;
                                                        worksheet.Cells[index, 7].Value = string.Format("{0:n0}", UP);
                                                    }
                                                    else
                                                        worksheet.Cells[index, 7].Value = equ.UnitPrice;

                                                    if (equ.UnitPrice != null && equ.UnitPrice != 0 && equ.TaxCode != "V_ARTVENTAEXC" && dataInformation.IdCustomerNavigation?.ExIva == true)
                                                    {
                                                        decimal IVA = ((decimal)equ.UnitPrice) * (decimal)0.19;
                                                        worksheet.Cells[index, 8].Value = string.Format("{0:n0}", IVA);
                                                    }
                                                    else
                                                    {
                                                        worksheet.Cells[index, 8].Value = "0";
                                                    }
                                                    worksheet.Cells[index, 9].Value = "0";
                                                    index = index + 1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                index = index + 2;
                worksheet.Cells[index, 1].Value = "Lista de Equipos";
                worksheet.Cells[index, 1].Style.Font.Bold = true;
                index = index + 2;

                foreach (var eq in dataInformation.EntryRequestDetails)
                {
                    worksheet.Cells[index, 1].Value = "id Master";
                    worksheet.Cells[index, 1].Style.Font.Bold = true;
                    worksheet.Cells[index, 2].Value = "Nombre del Master";
                    worksheet.Cells[index, 2].Style.Font.Bold = true;
                    worksheet.Cells[index, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    index = index + 1;

                    worksheet.Cells[index, 1].Value = eq.IdEquipmentNavigation?.Id ?? 0;
                    worksheet.Cells[index, 2].Value = eq.IdEquipmentNavigation?.Name ?? "";
                    worksheet.Cells[index, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                index = index + 2;
                worksheet.Cells[index, 1].Value = "RECIBIDO Y CONFORME POR";
                worksheet.Cells[index, 1].Style.Font.Bold = true;
                worksheet.Cells[index, 4].Value = "AUXILIAR SUPLEMEDICOS";
                worksheet.Cells[index, 4].Style.Font.Bold = true;
                worksheet.Cells[index, 7].Value = "ASESOR TECNICO";
                worksheet.Cells[index, 7].Style.Font.Bold = true;

                index = index + 1;
                worksheet.Cells[index, 1].Value = "(FIRMA Y SELLO DEL CLIENTE)";
                worksheet.Cells[index, 1].Style.Font.Bold = true;
                worksheet.Cells[index, 4].Value = "QUE ENTREGA";
                worksheet.Cells[index, 4].Style.Font.Bold = true;
                worksheet.Cells[index, 7].Value = "ASESOR TECNICO";
                worksheet.Cells[index, 7].Style.Font.Bold = true;

                index = index + 2;
                worksheet.Cells[index, 1].Value = "RECIBIDO Y CONFORME POR";
                worksheet.Cells[index, 1].Style.Font.Bold = true;
                worksheet.Cells[index, 4].Value = "AUXILIAR SUPLEMEDICOS";
                worksheet.Cells[index, 4].Style.Font.Bold = true;
                worksheet.Cells[index, 7].Value = "ASESOR TECNICO";
                worksheet.Cells[index, 7].Style.Font.Bold = true;

                index = index + 2;
                worksheet.Cells[index, 1].Value = "Area de Distribución";
                worksheet.Cells[index, 1].Style.Font.Bold = true;
                worksheet.Cells[index, 5].Value = "Area de Lavado";
                worksheet.Cells[index, 5].Style.Font.Bold = true;

                index = index + 1;
                worksheet.Cells[index, 3].Value = "SI";
                worksheet.Cells[index, 4].Value = "NO";
                worksheet.Cells[index, 8].Value = "SI";
                worksheet.Cells[index, 9].Value = "NO";

                index = index + 1;
                worksheet.Cells[index, 1].Value = "Lleva brocas";
                worksheet.Cells[index, 5].Value = "Lleva sustitutos oseos";
                index = index + 1;
                worksheet.Cells[index, 1].Value = "Recibe brocas";
                worksheet.Cells[index, 5].Value = "Recibe sustitutos oseos";
                index = index + 1;
                worksheet.Cells[index, 1].Value = "Lleva baterias";
                worksheet.Cells[index, 5].Value = "Recibe baterias";
                index = index + 1;
                worksheet.Cells[index, 1].Value = "Lleva manometro";
                worksheet.Cells[index, 5].Value = "Recibe manometro";
                index = index + 1;
                worksheet.Cells[index, 1].Value = "Cuantas cajas entrega";
                worksheet.Cells[index, 5].Value = "Cuantos equipos recibe";

                index = index + 1;
                worksheet.Cells[index, 1].Value = "Responsable";
                worksheet.Cells[index, 5].Value = "Responsable";

                index = index + 1;
                worksheet.Cells[index, 1].Value = "Observaciones";
                worksheet.Cells[index, 5].Value = "Observaciones";

                index = index + 2;
                worksheet.Cells[index, 1].Value = "METODOS DE ESTERILIZACION";
                worksheet.Cells[index, 1].Style.Font.Bold = true;
                worksheet.Cells[index, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                index = index + 1;
                worksheet.Cells[index, 1].Value = "MARCA";
                worksheet.Cells[index, 2].Value = "TIPO DE PIEZAS";
                worksheet.Cells[index, 3].Value = "METODO DE ESTERILIZACIÓN";
                worksheet.Cells[index, 4].Value = "TEMPERATURA DE ESTERILIZACIÓN";
                worksheet.Cells[index, 5].Value = "TIEMPO DE CICLO";
                index = index + 1;
                worksheet.Cells[index, 1].Value = "IMEDICOM";
                worksheet.Cells[index, 2].Value = "NEUMATICO";
                worksheet.Cells[index, 3].Value = "VAPOR / BAJA TEMPERATURA";
                worksheet.Cells[index, 4].Value = "134° /50°";
                worksheet.Cells[index, 5].Value = "1 HORA";
                index = index + 1;
                worksheet.Cells[index, 1].Value = "DESOUTTER";
                worksheet.Cells[index, 2].Value = "NEUMATICO";
                worksheet.Cells[index, 3].Value = "VAPOR / BAJA TEMPERATURA";
                worksheet.Cells[index, 4].Value = "134° /50°";
                worksheet.Cells[index, 5].Value = "1 HORA";
                index = index + 1;
                worksheet.Cells[index, 1].Value = "MICROAIRE";
                worksheet.Cells[index, 2].Value = "NEUMATICO";
                worksheet.Cells[index, 3].Value = "VAPOR / BAJA TEMPERATURA";
                worksheet.Cells[index, 4].Value = "134° /50°";
                worksheet.Cells[index, 5].Value = "1 HORA";
                index = index + 1;
                worksheet.Cells[index, 1].Value = "ZIMMER";
                worksheet.Cells[index, 2].Value = "ELECTRICO";
                worksheet.Cells[index, 3].Value = "VAPOR";
                worksheet.Cells[index, 4].Value = "134° /50°";
                worksheet.Cells[index, 5].Value = "1 HORA";
                index = index + 1;
                worksheet.Cells[index, 1].Value = "SURCIC AP";
                worksheet.Cells[index, 2].Value = "ELECTRICO";
                worksheet.Cells[index, 3].Value = "BAJA TEMPERTURA";
                worksheet.Cells[index, 4].Value = "50° ";
                worksheet.Cells[index, 5].Value = "1 HORA";
                index = index + 1;
                worksheet.Cells[index, 1].Value = "MICRO AIRE ELECTRICA";
                worksheet.Cells[index, 2].Value = "ELECTRICO";
                worksheet.Cells[index, 3].Value = "VAPOR / BAJA TEMPERATURA";
                worksheet.Cells[index, 4].Value = "134° /50°";
                worksheet.Cells[index, 5].Value = "1 HORA";
                index = index + 1;
                worksheet.Cells[index, 1].Value = "BATERIA DE MICRO AIRE ";
                worksheet.Cells[index, 2].Value = "ELECTRICO";
                worksheet.Cells[index, 3].Value = "BAJA TEMPERTURA";
                worksheet.Cells[index, 4].Value = "50°";
                worksheet.Cells[index, 5].Value = "CICLO RAPIDO";
                index = index + 1;
                worksheet.Cells[index, 1].Value = "BATERIA DE ZIMMER";
                worksheet.Cells[index, 2].Value = "ELECTRICO";
                worksheet.Cells[index, 3].Value = "NO ESTERILIZAR";
                worksheet.Cells[index, 4].Value = "N/A";
                worksheet.Cells[index, 5].Value = "N/A";

                index = index + 2;
                worksheet.Cells[index, 1].Value = "CANTIDADES EN 0";
                worksheet.Cells[index, 1].Style.Font.Bold = true;
                worksheet.Cells[index, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                index = index + 1;

                foreach (var equ in dataInformation.EntryRequestDetails)
                {
                    if (equ.EntryRequestAssembly != null && equ.EntryRequestAssembly.Count > 0)
                    {
                        foreach (var eq in equ.EntryRequestAssembly)
                        {
                            decimal totalQuantityZero = (decimal)equ.EntryRequestAssembly.Where(x => x.Code == eq.Code).Sum(x => x.ReservedQuantity ?? 0);
                            if (totalQuantityZero <= 0)
                            {
                                if (!detailsZero.Contains(equ.IdEquipmentNavigation?.Code ?? ""))
                                {
                                    index = index + 1;
                                    detailsZero.Add(equ.IdEquipmentNavigation?.Code ?? "");
                                    worksheet.Cells[index, 1].Value = equ.IdEquipmentNavigation?.Code ?? "";
                                    worksheet.Cells[index, 1].Style.Font.Bold = true;
                                    worksheet.Cells[index, 2].Value = equ.IdEquipmentNavigation?.Name ?? "";
                                    worksheet.Cells[index, 2].Style.Font.Bold = true;
                                    worksheet.Cells[index, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                                    index = index + 1;
                                    worksheet.Cells[index, 1].Value = "REF";
                                    worksheet.Cells[index, 1].Style.Font.Bold = true;
                                    worksheet.Cells[index, 2].Value = "Descripción";
                                    worksheet.Cells[index, 2].Style.Font.Bold = true;
                                    worksheet.Cells[index, 3].Value = "INVIMA";
                                    worksheet.Cells[index, 3].Style.Font.Bold = true;
                                    worksheet.Cells[index, 4].Value = "LOTE";
                                    worksheet.Cells[index, 4].Style.Font.Bold = true;
                                    worksheet.Cells[index, 5].Value = "Cant. LT";
                                    worksheet.Cells[index, 5].Style.Font.Bold = true;
                                    worksheet.Cells[index, 6].Value = "Precio U.";
                                    worksheet.Cells[index, 6].Style.Font.Bold = true;
                                    worksheet.Cells[index, 7].Value = "IVA";
                                    worksheet.Cells[index, 7].Style.Font.Bold = true;
                                }
                                if (eq.EntryRequestDetailId == equ.Id)
                                {
                                    index = index + 1;
                                    worksheet.Cells[index, 1].Value = eq.Code;
                                    worksheet.Cells[index, 2].Value = eq.Description;
                                    worksheet.Cells[index, 3].Value = eq.Invima;
                                    worksheet.Cells[index, 4].Value = eq.Lot;
                                    worksheet.Cells[index, 5].Value = eq.ReservedQuantity;
                                    if (eq.UnitPrice != null)
                                    {
                                        decimal UP = (decimal)eq.UnitPrice;
                                        worksheet.Cells[index, 7].Value = string.Format("{0:n0}", UP);
                                    }
                                    else
                                        worksheet.Cells[index, 7].Value = "0";
                                    if (eq.UnitPrice != null && eq.UnitPrice != 0 && eq.TaxCode != "V_ARTVENTAEXC" && dataInformation.IdCustomerNavigation?.ExIva == true)
                                    {
                                        decimal IVA = ((decimal)eq.UnitPrice) * (decimal)0.19;
                                        worksheet.Cells[index, 8].Value = string.Format("{0:n0}", IVA);
                                    }
                                    else
                                    {
                                        worksheet.Cells[index, 8].Value = "0";
                                    }
                                    worksheet.Cells[index, 9].Value = "";
                                }
                            }
                        }
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    Console.WriteLine("=== FIN GenerateXLSXAsync ===");
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"remision_{IdEntryReq}.xlsx");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en GenerateXLSXAsync ===");
                Console.WriteLine($"Mensaje de error: {ex.Message}");
                Console.WriteLine($"Tipo de excepción: {ex.GetType().Name}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                Console.WriteLine("=== FIN ERROR ===");
                throw;
            }
        }

        /// <summary>
        /// Marcar como despachado un pedido
        /// </summary>
        /// <param name="IdEntryReq">Id del registro de pedido</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>ActionResult Response</returns>
        [HttpPost("dispatch")]
        public async Task<ActionResult<object>> Dispatch(int IdEntryReq, [FromQuery] string companyCode)
        {
            try
            {
                Console.WriteLine($"=== INICIO Dispatch ===");
                Console.WriteLine($"IdEntryReq: {IdEntryReq}, CompanyCode: {companyCode}");

                var response = new
                {
                    IsSuccess = true,
                    Result = (object)null,
                    Message = "SAVE"
                };

                var ordersMessage = "";

                if (IdEntryReq == 0)
                {
                    Console.WriteLine("ERROR: IdEntryReq es 0");
                    return BadRequest(new
                    {
                        IsSuccess = false,
                        Result = (object)null,
                        Message = "MODEL_NOT_VALID"
                    });
                }

                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("ERROR: CompanyCode está vacío");
                    return BadRequest(new
                    {
                        IsSuccess = false,
                        Result = (object)null,
                        Message = "El código de compañía es requerido"
                    });
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de compañía obtenido correctamente");

                // Obtener la Entry Request con todos sus detalles
                var data = await companyContext.EntryRequests
                    .Include(er => er.EntryRequestComponents)
                    .Include(er => er.EntryRequestDetails)
                        .ThenInclude(erd => erd.IdEquipmentNavigation)
                    .FirstOrDefaultAsync(er => er.Id == IdEntryReq);

                if (data == null)
                {
                    Console.WriteLine("ERROR: No se encontró la Entry Request");
                    return NotFound(new
                    {
                        IsSuccess = false,
                        Result = (object)null,
                        Message = "No se encontró el pedido especificado"
                    });
                }

                // Validar que todos los componentes tengan lote asignado
                if (data.EntryRequestComponents != null && data.EntryRequestComponents.Count > 0)
                {
                    foreach (var component in data.EntryRequestComponents)
                    {
                        if (string.IsNullOrEmpty(component.Lot) || component.Lot == "0")
                        {
                            Console.WriteLine("ERROR: Componente sin lote asignado");
                            return BadRequest(new
                            {
                                IsSuccess = false,
                                Result = (object)null,
                                Message = "No puede despachar, existen componentes que no tienen asignado lote"
                            });
                        }
                    }

                    // Validar que se haya generado pedido de ensamble
                    if (data.AssemblyComponents != true)
                    {
                        Console.WriteLine("ERROR: No se ha generado pedido de ensamble");
                        return BadRequest(new
                        {
                            IsSuccess = false,
                            Result = (object)null,
                            Message = "No puede despachar, debe generar pedido de ensamble para los componentes"
                        });
                    }
                }

                // Validar que no haya conflictos con otros pedidos
                var validation = await companyContext.EntryRequestDetails
                    .Include(erd => erd.IdEntryReqNavigation)
                    .Include(erd => erd.IdEquipmentNavigation)
                    .Where(x => (x.IdEntryReqNavigation.Status == "DESPACHADO" || 
                                x.IdEntryReqNavigation.Status == "ATC_CONS" || 
                                x.IdEntryReqNavigation.Status == "DISPATCH_PAR") && 
                               x.IdEntryReqNavigation.Id != IdEntryReq && 
                               x.IsComponent == false)
                    .ToListAsync();

                if (validation.Any())
                {
                    var validationList = validation.ToList();
                    foreach (var detail in data.EntryRequestDetails)
                    {
                        var eqValidation = validationList.Find(x => x.IdEquipment == detail.IdEquipment && 
                                                                   x.TraceState != "PROCESADO" && 
                                                                   x.TraceState != "PEDIDO PROCESADO");
                        if (eqValidation != null)
                        {
                            ordersMessage += $"El equipo {eqValidation.IdEquipmentNavigation.Code} ya se encuentra en el pedido {eqValidation.IdEntryReqNavigation.Id} en estado {eqValidation.IdEntryReqNavigation.Status}; ";
                        }
                    }

                    if (!string.IsNullOrEmpty(ordersMessage))
                    {
                        Console.WriteLine($"ERROR: Conflictos encontrados - {ordersMessage}");
                        return BadRequest(new
                        {
                            IsSuccess = false,
                            Result = (object)null,
                            Message = ordersMessage
                        });
                    }
                }

                // Actualizar estados de los detalles
                foreach (var detail in data.EntryRequestDetails)
                {
                    if (detail.status == "DISPATCH_PAR" || detail.status == "Pendiente")
                    {
                        detail.status = "DESPACHADO";
                        detail.DateLoadState = DateTime.Now;
                        companyContext.EntryRequestDetails.Update(detail);
                    }
                }

                // Actualizar estados de los componentes
                if (data.EntryRequestComponents != null && data.EntryRequestComponents.Count > 0)
                {
                    foreach (var component in data.EntryRequestComponents)
                    {
                        if (component.status == "DISPATCH_PAR" || component.status == "Pendiente")
                        {
                            component.status = "DESPACHADO";
                            companyContext.EntryRequestComponents.Update(component);
                        }
                    }
                }

                // Actualizar estado del pedido principal
                data.Status = "DESPACHADO";
                companyContext.EntryRequests.Update(data);

                // Guardar cambios
                await companyContext.SaveChangesAsync();

                // Crear registro en el historial
                var historyEntry = new EntryRequestHistory
                {
                    IdEntryRequest = data.Id,
                    Description = "Pedido Despachado.",
                    Information = $"Pedido: {data.Id}",
                    DateLoad = DateTime.Now,
                    UserId = 1, // TODO: Obtener el ID del usuario actual
                    Location = "WEB"
                };

                companyContext.EntryRequestHistory.Add(historyEntry);
                await companyContext.SaveChangesAsync();

                Console.WriteLine("=== FIN Dispatch ===");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en Dispatch ===");
                Console.WriteLine($"Mensaje de error: {ex.Message}");
                Console.WriteLine($"Tipo de excepción: {ex.GetType().Name}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                Console.WriteLine("=== FIN ERROR ===");

                return StatusCode(500, new
                {
                    IsSuccess = false,
                    Result = (object)null,
                    Message = ex.Message
                });
            }
        }

        // PATCH: api/EntryRequest/{id}/cancel
        [HttpPatch("{id}/cancel")]
        public async Task<ActionResult<EntryRequestCancelUpdateResponseDTO>> UpdateEntryRequestCancel(int id, [FromBody] EntryRequestCancelUpdateDTO cancelUpdateDto, [FromQuery] string companyCode)
        {
            try
            {
                Console.WriteLine($"=== INICIO UpdateEntryRequestCancel ===");
                Console.WriteLine($"EntryRequest ID: {id}");
                Console.WriteLine($"CompanyCode: {companyCode}");

                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest(new EntryRequestCancelUpdateResponseDTO
                    {
                        Success = false,
                        Message = "El código de compañía es requerido",
                        EntryRequestId = id,
                        UpdatedAt = DateTime.Now
                    });
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");

                // Buscar la EntryRequest
                var existingEntryRequest = await companyContext.EntryRequests.FindAsync(id);
                if (existingEntryRequest == null)
                {
                    Console.WriteLine($"Error: EntryRequest con ID {id} no encontrada");
                    return NotFound(new EntryRequestCancelUpdateResponseDTO
                    {
                        Success = false,
                        Message = $"No se encontró la solicitud de entrada con ID {id}",
                        EntryRequestId = id,
                        UpdatedAt = DateTime.Now
                    });
                }

                Console.WriteLine($"EntryRequest encontrada. Status actual: {existingEntryRequest.Status}");

                // Actualizar los campos de cancelación
                existingEntryRequest.Status = cancelUpdateDto.Status;
                existingEntryRequest.IdCancelReason = cancelUpdateDto.IdCancelReason;
                existingEntryRequest.CancelReason = cancelUpdateDto.CancelReason;
                existingEntryRequest.IdCancelDetail = cancelUpdateDto.IdCancelDetail;
                existingEntryRequest.CancelDetail = cancelUpdateDto.CancelDetail;

                // Marcar la entidad como modificada
                companyContext.EntryRequests.Update(existingEntryRequest);

                // Guardar cambios
                await companyContext.SaveChangesAsync();
                Console.WriteLine("Cambios guardados exitosamente");

                // Invalidar caché
                await _outputCacheStore.EvictByTagAsync(cacheTag, default);

                // Crear respuesta exitosa
                var response = new EntryRequestCancelUpdateResponseDTO
                {
                    Success = true,
                    Message = "EntryRequest actualizada exitosamente",
                    EntryRequestId = id,
                    Status = existingEntryRequest.Status,
                    IdCancelReason = existingEntryRequest.IdCancelReason,
                    CancelReason = existingEntryRequest.CancelReason,
                    IdCancelDetail = existingEntryRequest.IdCancelDetail,
                    CancelDetail = existingEntryRequest.CancelDetail,
                    UpdatedAt = DateTime.Now
                };

                Console.WriteLine("=== FIN UpdateEntryRequestCancel ===");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en UpdateEntryRequestCancel ===");
                Console.WriteLine($"Mensaje de error: {ex.Message}");
                Console.WriteLine($"Tipo de excepción: {ex.GetType().Name}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                Console.WriteLine("=== FIN ERROR ===");

                return StatusCode(500, new EntryRequestCancelUpdateResponseDTO
                {
                    Success = false,
                    Message = $"Error interno del servidor: {ex.Message}",
                    EntryRequestId = id,
                    UpdatedAt = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Validar ensamble de referencias de pedido al momento de despachar 
        /// </summary>
        /// <param name="entryRequestId">Id del pedido</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Texto para identificar si genera error por validaciones</returns>
        [HttpGet("{entryRequestId}/reloadAssemblyDis")]
        public async Task<ActionResult<ReloadAssemblyDisResponseDTO>> ReloadAssemblyDis(int entryRequestId, [FromQuery] string companyCode)
        {
            try
            {
                Console.WriteLine($"=== INICIO ReloadAssemblyDis ===");
                Console.WriteLine($"EntryRequest ID: {entryRequestId}");
                Console.WriteLine($"CompanyCode: {companyCode}");

                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest(new ReloadAssemblyDisResponseDTO
                    {
                        Success = false,
                        Message = "El código de compañía es requerido",
                        EntryRequestId = entryRequestId,
                        ValidationMessages = "",
                        ProcessedAt = DateTime.Now
                    });
                }

                if (entryRequestId <= 0)
                {
                    Console.WriteLine("Error: EntryRequestId debe ser mayor a 0");
                    return BadRequest(new ReloadAssemblyDisResponseDTO
                    {
                        Success = false,
                        Message = "El ID del pedido debe ser mayor a 0",
                        EntryRequestId = entryRequestId,
                        ValidationMessages = "",
                        ProcessedAt = DateTime.Now
                    });
                }

                string sUpdate = string.Empty;

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");

                // Obtener la EntryRequest con todos sus detalles usando el endpoint existente
                var entry = await companyContext.EntryRequests
                    .Include(er => er.IdCustomerNavigation)
                    .Include(er => er.InsurerTypeNavigation)
                    .Include(er => er.EntryRequestDetails)
                        .ThenInclude(erd => erd.IdEquipmentNavigation)
                    .Include(er => er.EntryRequestAssembly)
                    .FirstOrDefaultAsync(er => er.Id == entryRequestId);

                if (entry == null)
                {
                    Console.WriteLine($"Error: EntryRequest con ID {entryRequestId} no encontrada");
                    return NotFound(new ReloadAssemblyDisResponseDTO
                    {
                        Success = false,
                        Message = $"No se encontró la solicitud de entrada con ID {entryRequestId}",
                        EntryRequestId = entryRequestId,
                        ValidationMessages = "",
                        ProcessedAt = DateTime.Now
                    });
                }

                Console.WriteLine($"EntryRequest encontrada. Customer: {entry.IdCustomerNavigation?.Name}");

                // Obtener conexión a Business Central
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                Console.WriteLine("Conexión a Business Central establecida");

                ICollection<EntryRequestDetails> entryRequestDetails = entry.EntryRequestDetails;
                List<EntryRequestAssembly> dataEquipment = new List<EntryRequestAssembly>();
                List<EntryRequestAssembly> dataEquipmentAll = new List<EntryRequestAssembly>();

                string sPriceList = "";
                if (entry.priceGroup != null && entry.priceGroup != "")
                {
                    sPriceList = entry.priceGroup;
                }

                foreach (var detail in entryRequestDetails)
                {
                    if (detail.IsComponent == null || !(bool)detail.IsComponent)
                    {
                        Console.WriteLine($"Procesando detalle para equipo: {detail.IdEquipmentNavigation?.Code}");

                        if (sPriceList == "")
                        {
                            if (entry.IdCustomerNavigation.IsSecondPriceList == true)
                            {
                                // Obtener listas de precio del cliente usando el servicio directamente
                                var priceLists = await _customerPriceListBO.GetPriceListByCustomerIdAsync(entry.IdCustomerNavigation.Id);
                                
                                var value = priceLists?.Where(list => list.InsurerType == entry.InsurerTypeNavigation?.description);
                                
                                if (value != null && value.Any())
                                {
                                    var finalList = value.FirstOrDefault();
                                    sPriceList = finalList.PriceGroup;
                                }
                                else
                                {
                                    sPriceList = entry.IdCustomerNavigation.PriceGroup;
                                }
                            }
                            else
                            {
                                sPriceList = entry.IdCustomerNavigation.PriceGroup;
                            }
                        }

                        // Obtener ensamble V2 usando el endpoint existente
                        var assemblyV2Response = await bcConn.GetEntryReqAssembly("lylassemblyV2", detail.IdEquipmentNavigation.Code, sPriceList);
                        dataEquipmentAll = assemblyV2Response ?? new List<EntryRequestAssembly>();

                        if (dataEquipmentAll != null && dataEquipmentAll.Any())
                        {
                            bool insertNew = false;
                            foreach (var e in dataEquipmentAll)
                            {
                                if (entry.EntryRequestAssembly != null && entry.EntryRequestAssembly.Any())
                                {
                                    var assembly = entry.EntryRequestAssembly.ToList();
                                    if (assembly != null && assembly.Count() > 0)
                                    {
                                        var dataAs = assembly.Find(x => x.Code == e.Code && x.EntryRequestDetailId == detail.Id);
                                        if (dataAs == null)
                                        {
                                            // Componente no encontrado en el ensamble actual
                                        }
                                    }
                                    else
                                        insertNew = true;
                                }
                                else
                                    insertNew = true;

                                if (insertNew)
                                {
                                    // Nuevo componente encontrado
                                }
                            }
                        }

                        string PriceList = "";
                        if (entry.priceGroup != null && entry.priceGroup != "")
                        {
                            PriceList = entry.priceGroup;
                        }
                        else if (entry.IdCustomerNavigation.IsSecondPriceList == true)
                        {
                            // Obtener listas de precio del cliente usando el servicio directamente
                            var priceLists = await _customerPriceListBO.GetPriceListByCustomerIdAsync(entry.IdCustomerNavigation.Id);
                            
                            var value = priceLists?.Where(list => list.InsurerType == entry.InsurerTypeNavigation?.description);
                            
                            if (value != null && value.Any())
                            {
                                var finalList = value.FirstOrDefault();
                                PriceList = finalList.PriceGroup;
                            }
                            else
                            {
                                PriceList = entry.IdCustomerNavigation.PriceGroup;
                            }
                        }
                        else
                        {
                            PriceList = entry.IdCustomerNavigation.PriceGroup;
                        }

                        // Obtener ensamble usando el método existente
                        dataEquipment = await bcConn.GetEntryReqAssembly("lylassembly", detail.IdEquipmentNavigation.Code, PriceList);

                        if (dataEquipment != null && dataEquipment.Count() == 0)
                        {
                            sUpdate += $"El Equipo no tiene registros con reservas: {detail.IdEquipmentNavigation.Code}; ";
                        }

                        if (dataEquipment != null)
                        {
                            if (entry.EntryRequestAssembly != null && entry.EntryRequestAssembly.Any())
                            {
                                var assembly = entry.EntryRequestAssembly.ToList().FindAll(x => x.EntryRequestDetailId == detail.Id);
                                foreach (var line in assembly)
                                {
                                    var exist = dataEquipment.Find(x => x.Code == line.Code && x.Lot == line.Lot);
                                    if (exist == null)
                                    {
                                        if (line.QuantityConsumed > 0)
                                            sUpdate += $"Para el Equipo: {detail.IdEquipmentNavigation.Code}, Componente: {line.Code}, Lote: {line.Lot} no existe cantidad reservada; ";
                                    }
                                }
                            }

                            foreach (var e in dataEquipment)
                            {
                                try
                                {
                                    var assembly = entry.EntryRequestAssembly.First(x => x.Code == e.Code && x.Lot == e.Lot && x.EntryRequestDetailId == detail.Id);

                                    if (assembly != null && assembly.QuantityConsumed > 0 && assembly.QuantityConsumed > e.ReservedQuantity)
                                    {
                                        sUpdate += $"Equipo: {detail.IdEquipmentNavigation.Code} Componente: {assembly.Code}; {e.ToString()}; ";
                                    }
                                }
                                catch (Exception)
                                {
                                    // Componente no encontrado en el ensamble actual
                                }
                            }
                        }
                    }
                }

                Console.WriteLine($"=== FIN ReloadAssemblyDis ===");
                Console.WriteLine($"Mensajes de validación: {sUpdate}");

                var response = new ReloadAssemblyDisResponseDTO
                {
                    Success = true,
                    Message = "Validación de ensamble completada exitosamente",
                    EntryRequestId = entryRequestId,
                    ValidationMessages = sUpdate,
                    ProcessedAt = DateTime.Now
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en ReloadAssemblyDis ===");
                Console.WriteLine($"Mensaje de error: {ex.Message}");
                Console.WriteLine($"Tipo de excepción: {ex.GetType().Name}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                Console.WriteLine("=== FIN ERROR ===");

                return StatusCode(500, new ReloadAssemblyDisResponseDTO
                {
                    Success = false,
                    Message = $"Error interno del servidor: {ex.Message}",
                    EntryRequestId = entryRequestId,
                    ValidationMessages = "",
                    ProcessedAt = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Actualizar cantidades de referencias de un pedido
        /// </summary>
        /// <param name="entryRequestId">Id del registro del pedido</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>True/False</returns>
        [HttpGet("{entryRequestId}/reloadAssemblyToBC")]
        public async Task<ActionResult<bool>> ReloadAssemblyToBC(int entryRequestId, [FromQuery] string companyCode)
        {
            try
            {
                Console.WriteLine($"=== INICIO ReloadAssemblyToBC ===");
                Console.WriteLine($"EntryRequest ID: {entryRequestId}");
                Console.WriteLine($"CompanyCode: {companyCode}");

                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                if (entryRequestId <= 0)
                {
                    Console.WriteLine("Error: EntryRequestId debe ser mayor a 0");
                    return BadRequest("El ID del pedido debe ser mayor a 0");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");

                // Obtener la EntryRequest con todos sus detalles usando el endpoint existente
                var entry = await companyContext.EntryRequests
                    .Include(er => er.IdCustomerNavigation)
                    .Include(er => er.InsurerTypeNavigation)
                    .Include(er => er.EntryRequestDetails)
                        .ThenInclude(erd => erd.IdEquipmentNavigation)
                    .Include(er => er.EntryRequestAssembly)
                    .FirstOrDefaultAsync(er => er.Id == entryRequestId);

                if (entry == null)
                {
                    Console.WriteLine($"Error: EntryRequest con ID {entryRequestId} no encontrada");
                    return NotFound($"No se encontró la solicitud de entrada con ID {entryRequestId}");
                }

                Console.WriteLine($"EntryRequest encontrada. Customer: {entry.IdCustomerNavigation?.Name}");

                // Obtener conexión a Business Central
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                Console.WriteLine("Conexión a Business Central establecida");

                ICollection<EntryRequestDetails> entryRequestDetails = entry.EntryRequestDetails;
                List<EntryRequestAssembly> dataEquipment = new List<EntryRequestAssembly>();
                List<EntryRequestAssembly> dataEquipmentAll = new List<EntryRequestAssembly>();

                string sPriceList = string.Empty;
                if (entry.priceGroup != null && entry.priceGroup != "")
                {
                    sPriceList = entry.priceGroup;
                }
                else if (entry.IdCustomerNavigation.IsSecondPriceList == true) //SUPLE TI: Valido si el cliente tiene mas de una lista de precios 
                {
                    CustomerPriceListResponseDTO finalList;
                    List<CustomerPriceListResponseDTO> dataPriceList = await _customerPriceListBO.GetPriceListByCustomerIdAsync(entry.IdCustomerNavigation.Id);
                    var value = dataPriceList.Where(list => list.InsurerType == entry.InsurerTypeNavigation?.description);
                    if (value.Any())
                    {
                        finalList = value.FirstOrDefault();
                        sPriceList = finalList.PriceGroup;
                    }
                    else
                    {
                        sPriceList = entry.IdCustomerNavigation.PriceGroup;
                    }
                }
                else
                {
                    sPriceList = entry.IdCustomerNavigation.PriceGroup;
                }

                foreach (var detail in entryRequestDetails)
                {
                    List<EntryRequestAssembly> dataToDelete = new List<EntryRequestAssembly>();
                    
                    // Obtener ensamble V2 usando el endpoint existente
                    dataEquipmentAll = await bcConn.GetEntryReqAssembly("lylassemblyV2", detail.IdEquipmentNavigation.Code, sPriceList);

                    if (dataEquipmentAll != null)
                    {
                        bool insertNew = false;
                        foreach (var e in dataEquipmentAll)
                        {
                            if (entry.EntryRequestAssembly != null && entry.EntryRequestAssembly.Any())
                            {
                                var assembly = entry.EntryRequestAssembly.ToList();
                                if (assembly != null && assembly.Count() > 0)
                                {
                                    var dataAs = assembly.Find(x => x.Code == e.Code && x.EntryRequestDetailId == detail.Id && x.LineNo == e.LineNo);
                                    if (assembly != null && dataAs != null)
                                    {
                                        if (e.ReservedQuantity != null)
                                        {
                                            dataAs.Quantity = e.Quantity;
                                            dataAs.UnitPrice = e.UnitPrice;
                                            dataAs.ReservedQuantity = e.ReservedQuantity;
                                            dataAs.LowTurnover = e.LowTurnover;

                                            companyContext.EntryRequestAssembly.Update(dataAs);
                                        }
                                    }
                                    else
                                        insertNew = true;
                                }
                                else
                                    insertNew = true;
                            }
                            else
                                insertNew = true;

                            if (insertNew)
                            {
                                // Nuevo componente encontrado - aquí se podría implementar la lógica para insertar
                                Console.WriteLine($"Nuevo componente encontrado: {e.Code}");
                            }
                        }
                    }

                    // Obtener ensamble usando el método existente
                    dataEquipment = await bcConn.GetEntryReqAssembly("lylassembly", detail.IdEquipmentNavigation.Code, sPriceList);

                    if (dataEquipment != null)
                    {
                        if (entry.EntryRequestAssembly != null && entry.EntryRequestAssembly.Any())
                        {
                            var assembly = entry.EntryRequestAssembly.ToList().FindAll(x => x.EntryRequestDetailId == detail.Id);
                            foreach (var line in assembly)
                            {
                                var exist = dataEquipment.Find(x => x.Code == line.Code && x.Lot == line.Lot && x.LineNo == line.LineNo);
                                if (exist != null)
                                {
                                    // Componente existe en el ensamble
                                }
                                else
                                {
                                    // Componente no existe - se podría marcar para eliminación
                                    // dataToDelete.Add(line);
                                }
                            }
                        }

                        foreach (var e in dataEquipment)
                        {
                            try
                            {
                                var assembly = entry.EntryRequestAssembly.First(x => x.Code == e.Code && x.Lot == e.Lot && x.EntryRequestDetailId == detail.Id && x.LineNo == e.LineNo);
                                if (assembly != null)
                                {
                                    assembly.ReservedQuantity = e.Quantity;
                                    assembly.ShortDesc = e.ShortDesc;
                                    assembly.Quantity = e.Quantity;
                                    assembly.Quantity_ile = e.Quantity_ile;
                                    assembly.Location_Code_ile = e.Location_Code_ile;
                                    assembly.Invima = e.Invima;
                                    assembly.UnitPrice = e.UnitPrice;
                                    assembly.ExpirationDate = e.ExpirationDate;
                                    assembly.LowTurnover = e.LowTurnover;

                                    companyContext.EntryRequestAssembly.Update(assembly);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error actualizando ensamble {e.Code}: {ex.Message}");
                            }
                        }
                    }
                }

                // Guardar cambios en la base de datos
                await companyContext.SaveChangesAsync();
                Console.WriteLine("Cambios guardados en la base de datos");

                Console.WriteLine($"=== FIN ReloadAssemblyToBC ===");
                return Ok(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en ReloadAssemblyToBC ===");
                Console.WriteLine($"Mensaje de error: {ex.Message}");
                Console.WriteLine($"Tipo de excepción: {ex.GetType().Name}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                Console.WriteLine("=== FIN ERROR ===");

                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Reportar consumo de pedido a BC, genera pedido de venta en BC
        /// </summary>
        /// <param name="data">Datos del registro con cantidades actualizadas</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>JsonResult Response</returns>
        [HttpPost("update-quantities")]
        public async Task<ActionResult<UpdateQuantitysResponseDTO>> UpdateQuantitys([FromBody] UpdateQuantitysRequestDTO data, [FromQuery] string companyCode)
        {
            try
            {
                Console.WriteLine($"=== INICIO UpdateQuantitys ===");
                Console.WriteLine($"EntryRequest ID: {data.Id}");
                Console.WriteLine($"CompanyCode: {companyCode}");

                var response = new UpdateQuantitysResponseDTO
                {
                    IsSuccess = true,
                    Result = null,
                    Message = "Cantidades actualizadas exitosamente"
                };

                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest(new UpdateQuantitysResponseDTO
                    {
                        IsSuccess = false,
                        Message = "El código de compañía es requerido"
                    });
                }

                if (data == null || data.Id <= 0)
                {
                    Console.WriteLine("Error: Datos inválidos");
                    return BadRequest(new UpdateQuantitysResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Los datos del pedido son requeridos y el ID debe ser mayor a 0"
                    });
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");

                // Validar que existe el EntryRequest
                var entryRequest = await companyContext.EntryRequests
                    .Include(er => er.IdCustomerNavigation)
                    .Include(er => er.InsurerTypeNavigation)
                    .Include(er => er.EntryRequestDetails)
                        .ThenInclude(erd => erd.IdEquipmentNavigation)
                    .Include(er => er.EntryRequestAssembly)
                    .Include(er => er.EntryRequestComponents)
                    .FirstOrDefaultAsync(er => er.Id == data.Id);

                if (entryRequest == null)
                {
                    Console.WriteLine($"Error: EntryRequest con ID {data.Id} no encontrada");
                    return NotFound(new UpdateQuantitysResponseDTO
                    {
                        IsSuccess = false,
                        Message = $"No se encontró la solicitud de entrada con ID {data.Id}"
                    });
                }

                Console.WriteLine($"EntryRequest encontrada. Customer: {entryRequest.IdCustomerNavigation?.Name}");

                // 1. Validar cantidades usando el método existente
                Console.WriteLine("Validando cantidades del pedido...");
                var validationResult = await ReloadAssemblyDis(data.Id, companyCode);
                
                if (validationResult.Result is not OkObjectResult validationOkResult)
                {
                    response.IsSuccess = false;
                    response.Message = "Error al validar cantidades del pedido";
                    return BadRequest(response);
                }

                var validationResponse = validationOkResult.Value as ReloadAssemblyDisResponseDTO;
                if (validationResponse == null || !validationResponse.Success)
                {
                    response.IsSuccess = false;
                    response.Message = validationResponse?.Message ?? "Error en validación";
                    return BadRequest(response);
                }

                // Si hay mensajes de validación, significa que hay problemas
                if (!string.IsNullOrEmpty(validationResponse.ValidationMessages))
                {
                    response.IsSuccess = false;
                    response.Message = "Las Cantidades del pedido no coinciden. " + validationResponse.ValidationMessages;
                    
                    // Registrar en el historial
                    await companyContext.EntryRequestHistory.AddAsync(new EntryRequestHistory
                    {
                        Description = "Cantidades no coinciden",
                        Information = validationResponse.ValidationMessages.Length > 500 ? 
                            validationResponse.ValidationMessages.Substring(0, 500) : 
                            validationResponse.ValidationMessages,
                        DateLoad = DateTime.Now,
                        UserId = 1, // TODO: Obtener del contexto de autenticación
                        Location = "WEB",
                        IdEntryRequest = data.Id
                    });
                    
                    await companyContext.SaveChangesAsync();
                    return BadRequest(response);
                }

                // 2. Actualizar cantidades de ensambles si se proporcionan
                if (data.EntryRequestAssembly != null && data.EntryRequestAssembly.Count > 0)
                {
                    Console.WriteLine("Actualizando cantidades de ensambles...");
                    foreach (var assemblyUpdate in data.EntryRequestAssembly)
                    {
                        var assembly = entryRequest.EntryRequestAssembly?.FirstOrDefault(x => x.Id == assemblyUpdate.Id);
                        if (assembly != null && assembly.QuantityConsumed != assemblyUpdate.QuantityConsumed)
                        {
                            assembly.QuantityConsumed = assemblyUpdate.QuantityConsumed;
                            companyContext.EntryRequestAssembly.Update(assembly);
                            
                            Console.WriteLine($"Ensemble {assembly.Code} actualizado: {assembly.QuantityConsumed}");
                        }
                    }
                }

                // 3. Actualizar cantidades de componentes si se proporcionan
                if (data.EntryRequestComponents != null && data.EntryRequestComponents.Count > 0)
                {
                    Console.WriteLine("Actualizando cantidades de componentes...");
                    foreach (var componentUpdate in data.EntryRequestComponents)
                    {
                        var component = entryRequest.EntryRequestComponents?.FirstOrDefault(x => x.Id == componentUpdate.Id);
                        if (component != null && component.QuantityConsumed != componentUpdate.QuantityConsumed)
                        {
                            component.QuantityConsumed = componentUpdate.QuantityConsumed;
                            companyContext.EntryRequestComponents.Update(component);
                            
                            Console.WriteLine($"Componente {component.ItemNo} actualizado: {component.QuantityConsumed}");
                        }
                    }
                }

                // Guardar cambios en la base de datos
                await companyContext.SaveChangesAsync();
                Console.WriteLine("Cambios guardados en la base de datos");

                // 4. Actualizar cantidades desde BC usando el método existente
                Console.WriteLine("Actualizando cantidades desde Business Central...");
                var reloadResult = await ReloadAssemblyToBC(data.Id, companyCode);
                
                if (reloadResult.Result is not OkObjectResult reloadOkResult)
                {
                    response.IsSuccess = false;
                    response.Message = "Error al actualizar cantidades desde Business Central";
                    return BadRequest(response);
                }

                var reloadSuccess = reloadOkResult.Value as bool?;
                if (reloadSuccess != true)
                {
                    response.IsSuccess = false;
                    response.Message = "Error al actualizar cantidades desde Business Central";
                    return BadRequest(response);
                }

                // 5. Obtener el EntryRequest actualizado con todos los detalles
                var updatedEntryRequest = await GetByIdWithDetails(data.Id, companyCode);
                
                if (updatedEntryRequest.Result is not OkObjectResult updatedOkResult)
                {
                    response.IsSuccess = false;
                    response.Message = "Error al obtener datos actualizados del pedido";
                    return BadRequest(response);
                }

                var finalEntryRequest = updatedOkResult.Value as EntryRequests;
                if (finalEntryRequest == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Error al obtener datos actualizados del pedido";
                    return BadRequest(response);
                }

                // 6. Enviar a Business Central
                Console.WriteLine("Enviando pedido a Business Central...");
                string result = "";
                
                if (finalEntryRequest.IdCancelReason == null || finalEntryRequest.IdCancelReason == 0)
                {
                    // Eliminar pedido de venta existente en BC
                    await DeleteSalesHeaderFromBC(finalEntryRequest.Id, companyCode);
                    
                    // Enviar nuevo pedido a BC
                    result = await SendEntryRequestToBC(finalEntryRequest, companyCode);
                }
                else
                {
                    result = "PEDIDO REGISTRADO";
                    await DeleteSalesHeaderFromBC(finalEntryRequest.Id, companyCode);
                }

                if (result != "PEDIDO REGISTRADO" && !result.Contains("PEDIDO ACTUALIZADO"))
                {
                    response.IsSuccess = false;
                    response.Message = result;
                    
                    // Registrar error en el historial
                    await companyContext.EntryRequestHistory.AddAsync(new EntryRequestHistory
                    {
                        Description = "Error al enviar a Business Central",
                        Information = result.Length > 500 ? result.Substring(0, 500) : result,
                        DateLoad = DateTime.Now,
                        UserId = 1, // TODO: Obtener del contexto de autenticación
                        Location = "WEB",
                        IdEntryRequest = data.Id
                    });
                    
                    await companyContext.SaveChangesAsync();
                    return BadRequest(response);
                }

                // 7. Actualizar estado del pedido
                finalEntryRequest.Status = "CREPORTED"; // EntryReqStates.CREPORTED
                companyContext.EntryRequests.Update(finalEntryRequest);
                await companyContext.SaveChangesAsync();

                // 8. Registrar en el historial
                await companyContext.EntryRequestHistory.AddAsync(new EntryRequestHistory
                {
                    Description = "Reportar consumo Pedido",
                    Information = result.Length > 500 ? result.Substring(0, 500) : result,
                    DateLoad = DateTime.Now,
                    UserId = 1, // TODO: Obtener del contexto de autenticación
                    Location = "WEB",
                    IdEntryRequest = data.Id
                });

                // 9. Registrar en el log de eventos
                await companyContext.EventLog.AddAsync(new EventLog
                {
                    Description = "Reportar consumo Pedido",
                    Information = $"Pedido: {data.Id}",
                    DateLoad = DateTime.Now,
                    UserId = 1, // TODO: Obtener del contexto de autenticación
                    IdModule = 14
                });

                await companyContext.SaveChangesAsync();

                response.Result = new
                {
                    EntryRequestId = data.Id,
                    Status = finalEntryRequest.Status,
                    Result = result,
                    UpdatedAt = DateTime.Now
                };

                Console.WriteLine($"=== FIN UpdateQuantitys ===");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en UpdateQuantitys ===");
                Console.WriteLine($"Mensaje de error: {ex.Message}");
                Console.WriteLine($"Tipo de excepción: {ex.GetType().Name}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                Console.WriteLine("=== FIN ERROR ===");

                return StatusCode(500, new UpdateQuantitysResponseDTO
                {
                    IsSuccess = false,
                    Message = $"Error interno del servidor: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Eliminar pedido de venta de Business Central
        /// </summary>
        /// <param name="entryRequestId">ID del pedido</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>True si se eliminó correctamente</returns>
        private async Task<bool> DeleteSalesHeaderFromBC(int entryRequestId, string companyCode)
        {
            try
            {
                Console.WriteLine($"Eliminando pedido de venta de BC: P-{entryRequestId}");
                
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                string dataJson = $"{{\"inputJson\":\"{{\\\"No\\\":\\\"P-{entryRequestId}\\\"}}\"}}";
                
                var response = await bcConn.BCRQ_postDeletePD("LyLSupleDeleteSalesOrder_DeleteSalesOrder", "", dataJson);
                
                if (!response.IsSuccessful)
                {
                    Console.WriteLine($"Error eliminando pedido de BC: {response.StatusCode} - {response.Content}");
                    return false;
                }
                
                Console.WriteLine("Pedido eliminado de BC correctamente");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteSalesHeaderFromBC: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Enviar pedido a Business Central
        /// </summary>
        /// <param name="entry">Objeto EntryRequests con los datos del pedido</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Texto, PEDIDO REGISTRADO|ERROR</returns>
        private async Task<string> SendEntryRequestToBC(EntryRequests entry, string companyCode)
        {
            try
            {
                Console.WriteLine($"Enviando pedido {entry.Id} a Business Central...");
                
                var bcConn = await _dynamicBCConnectionService.GetBCConnectionAsync(companyCode);
                
                // Construir objeto para BC
                var entryRequestApiBC_Header = new EntryRequestApiBC_Header
                {
                    documentNo = $"P-{entry.Id}",
                    customerNo = entry.IdCustomerNavigation?.No ?? "",
                    branch = entry.Branch?.Name ?? "",
                    address = entry.DeliveryAddress ?? "",
                    dateIni = entry.DeliveryDate.ToString("yyyy-MM-dd"),
                    dateSrg = entry.SurgeryInit?.ToString("yyyy-MM-dd") ?? "",
                    dateEnd = entry.SurgeryEnd?.ToString("yyyy-MM-dd") ?? "",
                    idMedic = entry.IdMedicNavigation?.Identification ?? "",
                    idPatient = entry.IdPatientNavigation?.Identification ?? "",
                    orderNo = entry.PurchaseOrder ?? "",
                    insurer = entry.InsurerNavigation?.Name ?? "",
                    insurerType = entry.InsurerTypeNavigation?.description ?? "",
                    listPrice = entry.priceGroup ?? entry.IdCustomerNavigation?.PriceGroup ?? "",
                    patientName = $"{entry.IdPatientNavigation?.Name} {entry.IdPatientNavigation?.LastName}",
                    medicalrecordPatient = entry.IdPatientNavigation?.MedicalRecord ?? "",
                    medicName = $"{entry.IdMedicNavigation?.Name} {entry.IdMedicNavigation?.LastName}",
                    salesline = new List<EntryRequestApiBC_Line>(),
                    salesassembly = new List<EntryRequestApiBC_Assembly>()
                };

                // Procesar ensambles
                if (entry.EntryRequestAssembly != null && entry.EntryRequestAssembly.Any())
                {
                    foreach (var assembly in entry.EntryRequestAssembly.Where(a => a.QuantityConsumed > 0))
                    {
                        entryRequestApiBC_Header.salesassembly.Add(new EntryRequestApiBC_Assembly
                        {
                            documentNo = $"P-{entry.Id}",
                            itemNo = assembly.Code,
                            lot = assembly.Lot ?? "",
                            quantityConsumed = assembly.QuantityConsumed ?? 0,
                            lineNo = assembly.LineNo ?? 0,
                            assemblyNo = assembly.AssemblyNo,
                            idWeb = assembly.Id
                        });
                    }
                }

                // Procesar componentes
                if (entry.EntryRequestComponents != null && entry.EntryRequestComponents.Any())
                {
                    foreach (var component in entry.EntryRequestComponents.Where(c => c.QuantityConsumed > 0))
                    {
                        entryRequestApiBC_Header.salesline.Add(new EntryRequestApiBC_Line
                        {
                            documentNo = $"P-{entry.Id}",
                            itemNo = component.ItemNo,
                            quantity = (int)(component.Quantity ?? 0),
                            price = component.UnitPrice ?? 0,
                            assemblyNo = component.AssemblyNo ?? "",
                            quantityConsumed = component.QuantityConsumed ?? 0,
                            locationCode = component.Warehouse ?? "",
                            idWeb = component.Id,
                            rowType = 1
                        });

                        entryRequestApiBC_Header.salesassembly.Add(new EntryRequestApiBC_Assembly
                        {
                            documentNo = $"P-{entry.Id}",
                            itemNo = component.ItemNo,
                            lot = component.Lot ?? "",
                            quantityConsumed = component.QuantityConsumed ?? 0,
                            idWeb = component.Id
                        });
                    }
                }

                // Verificar si hay consumos
                if (!entryRequestApiBC_Header.salesline.Any() && !entryRequestApiBC_Header.salesassembly.Any())
                {
                    return "ERROR: El pedido no tiene consumos";
                }

                // Enviar a BC
                var jsonEntry = System.Text.Json.JsonSerializer.Serialize(entryRequestApiBC_Header);
                var response = await bcConn.BCRQ_post("lylsuplesalesheader?$expand=salesline,salesassembly", "", jsonEntry);
                
                if (!response.IsSuccessful)
                {
                    return $"ERROR: {response.Content}";
                }
                
                Console.WriteLine("Pedido enviado a BC correctamente");
                return "PEDIDO REGISTRADO";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en SendEntryRequestToBC: {ex.Message}");
                return $"ERROR: {ex.Message}";
            }
        }
    }
} 