using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DimmedAPI.Entidades;
using DimmedAPI.DTOs;
using DimmedAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DimmedAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntryRequestAssemblyController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDynamicConnectionService _dynamicConnectionService;

        public EntryRequestAssemblyController(
            ApplicationDBContext context,
            IDynamicConnectionService dynamicConnectionService)
        {
            _context = context;
            _dynamicConnectionService = dynamicConnectionService;
        }

        /// <summary>
        /// Obtiene todos los ensambles de solicitudes de entrada
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Lista de todos los ensambles</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntryRequestAssemblyResponseDTO>>> GetEntryRequestAssemblies([FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);

                var assemblies = await companyContext.EntryRequestAssembly
                    .AsNoTracking()
                    .Select(era => new EntryRequestAssemblyResponseDTO
                    {
                        Id = era.Id,
                        Code = era.Code,
                        Description = era.Description,
                        ShortDesc = era.ShortDesc,
                        Invima = era.Invima,
                        Lot = era.Lot,
                        Quantity = era.Quantity,
                        UnitPrice = era.UnitPrice,
                        AssemblyNo = era.AssemblyNo,
                        EntryRequestId = era.EntryRequestId,
                        EntryRequestDetailId = era.EntryRequestDetailId,
                        QuantityConsumed = era.QuantityConsumed,
                        ExpirationDate = era.ExpirationDate,
                        Name = era.Name,
                        ReservedQuantity = era.ReservedQuantity,
                        Location_Code_ile = era.Location_Code_ile,
                        Classification = era.Classification,
                        Status = era.Status,
                        LineNo = era.LineNo,
                        Position = era.Position,
                        Quantity_ile = era.Quantity_ile,
                        TaxCode = era.TaxCode,
                        LowTurnover = era.LowTurnover,
                        IsComponent = era.IsComponent,
                        RSFechaVencimiento = era.RSFechaVencimiento,
                        RSClasifRegistro = era.RSClasifRegistro,
                        LocationCode = era.LocationCode
                    })
                    .ToListAsync();

                return Ok(assemblies);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al obtener los ensambles", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un ensamble específico por ID
        /// </summary>
        /// <param name="id">ID del ensamble</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Ensemble específico</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<EntryRequestAssemblyResponseDTO>> GetEntryRequestAssembly(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);

                var assembly = await companyContext.EntryRequestAssembly
                    .AsNoTracking()
                    .Where(era => era.Id == id)
                    .Select(era => new EntryRequestAssemblyResponseDTO
                    {
                        Id = era.Id,
                        Code = era.Code,
                        Description = era.Description,
                        ShortDesc = era.ShortDesc,
                        Invima = era.Invima,
                        Lot = era.Lot,
                        Quantity = era.Quantity,
                        UnitPrice = era.UnitPrice,
                        AssemblyNo = era.AssemblyNo,
                        EntryRequestId = era.EntryRequestId,
                        EntryRequestDetailId = era.EntryRequestDetailId,
                        QuantityConsumed = era.QuantityConsumed,
                        ExpirationDate = era.ExpirationDate,
                        Name = era.Name,
                        ReservedQuantity = era.ReservedQuantity,
                        Location_Code_ile = era.Location_Code_ile,
                        Classification = era.Classification,
                        Status = era.Status,
                        LineNo = era.LineNo,
                        Position = era.Position,
                        Quantity_ile = era.Quantity_ile,
                        TaxCode = era.TaxCode,
                        LowTurnover = era.LowTurnover,
                        IsComponent = era.IsComponent,
                        RSFechaVencimiento = era.RSFechaVencimiento,
                        RSClasifRegistro = era.RSClasifRegistro,
                        LocationCode = era.LocationCode
                    })
                    .FirstOrDefaultAsync();

                if (assembly == null)
                    return NotFound("Ensemble no encontrado");

                return Ok(assembly);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al obtener el ensemble", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Filtra ensambles por múltiples criterios
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="id">ID del ensemble (opcional)</param>
        /// <param name="code">Código del ensemble (opcional)</param>
        /// <param name="lot">Lote (opcional)</param>
        /// <param name="assemblyNo">Número de ensamble (opcional)</param>
        /// <param name="entryRequestId">ID de la solicitud de entrada (opcional)</param>
        /// <param name="entryRequestDetailId">ID del detalle de la solicitud (opcional)</param>
        /// <returns>Lista filtrada de ensambles</returns>
        [HttpGet("filtro")]
        public async Task<ActionResult<IEnumerable<EntryRequestAssemblyResponseDTO>>> GetEntryRequestAssembliesFiltered(
            [FromQuery] string companyCode,
            [FromQuery] int? id = null,
            [FromQuery] string code = null,
            [FromQuery] string lot = null,
            [FromQuery] string assemblyNo = null,
            [FromQuery] int? entryRequestId = null,
            [FromQuery] int? entryRequestDetailId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);

                var query = companyContext.EntryRequestAssembly
                    .AsNoTracking()
                    .AsQueryable();

                // Aplicar filtros
                if (id.HasValue)
                    query = query.Where(era => era.Id == id.Value);

                if (!string.IsNullOrEmpty(code))
                    query = query.Where(era => era.Code.Contains(code));

                if (!string.IsNullOrEmpty(lot))
                    query = query.Where(era => era.Lot.Contains(lot));

                if (!string.IsNullOrEmpty(assemblyNo))
                    query = query.Where(era => era.AssemblyNo.Contains(assemblyNo));

                if (entryRequestId.HasValue)
                    query = query.Where(era => era.EntryRequestId == entryRequestId.Value);

                if (entryRequestDetailId.HasValue)
                    query = query.Where(era => era.EntryRequestDetailId == entryRequestDetailId.Value);

                var assemblies = await query
                    .Select(era => new EntryRequestAssemblyResponseDTO
                    {
                        Id = era.Id,
                        Code = era.Code,
                        Description = era.Description,
                        ShortDesc = era.ShortDesc,
                        Invima = era.Invima,
                        Lot = era.Lot,
                        Quantity = era.Quantity,
                        UnitPrice = era.UnitPrice,
                        AssemblyNo = era.AssemblyNo,
                        EntryRequestId = era.EntryRequestId,
                        EntryRequestDetailId = era.EntryRequestDetailId,
                        QuantityConsumed = era.QuantityConsumed,
                        ExpirationDate = era.ExpirationDate,
                        Name = era.Name,
                        ReservedQuantity = era.ReservedQuantity,
                        Location_Code_ile = era.Location_Code_ile,
                        Classification = era.Classification,
                        Status = era.Status,
                        LineNo = era.LineNo,
                        Position = era.Position,
                        Quantity_ile = era.Quantity_ile,
                        TaxCode = era.TaxCode,
                        LowTurnover = era.LowTurnover,
                        IsComponent = era.IsComponent,
                        RSFechaVencimiento = era.RSFechaVencimiento,
                        RSClasifRegistro = era.RSClasifRegistro,
                        LocationCode = era.LocationCode
                    })
                    .ToListAsync();

                return Ok(assemblies);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al filtrar los ensambles", detalle = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene ensambles con detalles de solicitud y equipos consumidos
        /// </summary>
        /// <param name="entryRequestId">ID de la solicitud de entrada</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="quantityConsumedFilter">Filtro de cantidad consumida: "greater" (>0), "zero" (=0), "all" (todos)</param>
        /// <returns>Lista de ensambles con detalles</returns>
        [HttpGet("entry-request/{entryRequestId}/with-details")]
        public async Task<ActionResult<IEnumerable<EntryRequestAssemblyDetailDTO>>> GetEntryRequestAssembliesWithDetails(
            int entryRequestId,
            [FromQuery] string companyCode,
            [FromQuery] string quantityConsumedFilter = "greater",
            [FromQuery] string assemblyNo = null)
        {
            try
            {
                Console.WriteLine($"=== INICIO GetEntryRequestAssembliesWithDetails ===");
                Console.WriteLine($"EntryRequestId: {entryRequestId}");
                Console.WriteLine($"CompanyCode: {companyCode}");
                Console.WriteLine($"QuantityConsumedFilter: {quantityConsumedFilter}");
                Console.WriteLine($"AssemblyNo: {assemblyNo}");

                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");

                // Verificar si existe la solicitud
                Console.WriteLine("Verificando existencia de la solicitud...");
                var entryRequestExists = await companyContext.EntryRequests
                    .AnyAsync(er => er.Id == entryRequestId);

                if (!entryRequestExists)
                {
                    Console.WriteLine($"No se encontró la solicitud de entrada con ID: {entryRequestId}");
                    return NotFound($"No se encontró la solicitud de entrada con ID: {entryRequestId}");
                }

                Console.WriteLine("Solicitud encontrada, ejecutando consulta con LEFT JOIN...");

                // Construir la consulta con LEFT JOIN desde EntryRequests hacia EntryRequestAssembly
                var query = from er in companyContext.EntryRequests
                           join era in companyContext.EntryRequestAssembly on er.Id equals era.EntryRequestId into eraGroup
                           from era in eraGroup.DefaultIfEmpty()
                           where er.Id == entryRequestId
                           select new { EntryRequest = er, Assembly = era };

                // Aplicar filtro de número de ensamble si se proporciona
                if (!string.IsNullOrEmpty(assemblyNo))
                {
                    query = query.Where(x => x.Assembly != null && x.Assembly.AssemblyNo == assemblyNo);
                }

                // Aplicar filtro de cantidad consumida solo si no es "all"
                if (quantityConsumedFilter.ToLower() != "all")
                {
                    switch (quantityConsumedFilter.ToLower())
                    {
                        case "greater":
                            query = query.Where(x => x.Assembly != null && (x.Assembly.QuantityConsumed ?? 0) > 0);
                            break;
                        case "zero":
                            query = query.Where(x => x.Assembly != null && (x.Assembly.QuantityConsumed ?? 0) == 0);
                            break;
                    }
                }

                                 // Ejecutar la consulta y mapear a DTO
                 var result = await query
                     .Where(x => x.Assembly != null) // Solo incluir registros que tengan ensambles
                     .Select(x => new EntryRequestAssemblyDetailDTO
                     {
                         Id = x.Assembly.Id,
                         Code = x.Assembly.Code,
                         Description = x.Assembly.Description,
                         Lot = x.Assembly.Lot,
                         AssemblyNo = x.Assembly.AssemblyNo,
                         Quantity = x.Assembly.Quantity,
                         QuantityConsumed = x.Assembly.QuantityConsumed ?? 0
                     })
                     .ToListAsync();

                Console.WriteLine($"Resultados obtenidos: {result.Count}");

                // Si no hay resultados, verificar si hay ensambles sin filtro
                if (!result.Any())
                {
                    Console.WriteLine("No hay resultados, verificando ensambles sin filtro...");
                    
                    // Verificar si hay ensambles sin el filtro usando la misma consulta LEFT JOIN
                    var allAssembliesQuery = from er in companyContext.EntryRequests
                                            join era in companyContext.EntryRequestAssembly on er.Id equals era.EntryRequestId into eraGroup
                                            from era in eraGroup.DefaultIfEmpty()
                                            where er.Id == entryRequestId && era != null
                                            select era;

                    // Aplicar filtro de número de ensamble si se proporciona
                    if (!string.IsNullOrEmpty(assemblyNo))
                    {
                        allAssembliesQuery = allAssembliesQuery.Where(era => era.AssemblyNo == assemblyNo);
                    }

                    var allAssemblies = await allAssembliesQuery.ToListAsync();

                    Console.WriteLine($"Ensambles totales encontrados: {allAssemblies.Count}");

                    if (!allAssemblies.Any())
                    {
                        var message = string.IsNullOrEmpty(assemblyNo) 
                            ? $"No se encontraron ensambles para la solicitud {entryRequestId}"
                            : $"No se encontraron ensambles con número '{assemblyNo}' para la solicitud {entryRequestId}";
                        
                        Console.WriteLine("No hay ensambles para esta solicitud");
                        return Ok(new { 
                            message = message,
                            entryRequestId = entryRequestId,
                            filter = quantityConsumedFilter,
                            assemblyNo = assemblyNo,
                            totalAssemblies = 0
                        });
                    }
                    else
                    {
                        var consumedCount = allAssemblies.Count(a => (a.QuantityConsumed ?? 0) > 0);
                        var zeroCount = allAssemblies.Count(a => (a.QuantityConsumed ?? 0) == 0);
                        
                        Console.WriteLine($"Ensambles con QuantityConsumed > 0: {consumedCount}");
                        Console.WriteLine($"Ensambles con QuantityConsumed = 0: {zeroCount}");
                        
                        var message = string.IsNullOrEmpty(assemblyNo)
                            ? $"No se encontraron ensambles con el filtro '{quantityConsumedFilter}' para la solicitud {entryRequestId}"
                            : $"No se encontraron ensambles con número '{assemblyNo}' y filtro '{quantityConsumedFilter}' para la solicitud {entryRequestId}";
                        
                        return Ok(new { 
                            message = message,
                            entryRequestId = entryRequestId,
                            filter = quantityConsumedFilter,
                            assemblyNo = assemblyNo,
                            totalAssemblies = allAssemblies.Count,
                            consumedGreaterThanZero = consumedCount,
                            consumedEqualToZero = zeroCount
                        });
                    }
                }

                Console.WriteLine("=== FIN GetEntryRequestAssembliesWithDetails ===");
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException en GetEntryRequestAssembliesWithDetails: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en GetEntryRequestAssembliesWithDetails ===");
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
        /// Obtiene componentes de solicitud de entrada
        /// </summary>
        /// <param name="entryRequestId">ID de la solicitud de entrada</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="quantityConsumedFilter">Filtro de cantidad consumida: "greater" (>0), "zero" (=0), "all" (todos)</param>
        /// <returns>Lista de componentes</returns>
        [HttpGet("entry-request/{entryRequestId}/components")]
        public async Task<ActionResult<IEnumerable<EntryRequestComponentDetailDTO>>> GetEntryRequestComponents(
            int entryRequestId,
            [FromQuery] string companyCode,
            [FromQuery] string quantityConsumedFilter = "greater",
            [FromQuery] string assemblyNo = null)
        {
            try
            {
                Console.WriteLine($"=== INICIO GetEntryRequestComponents ===");
                Console.WriteLine($"EntryRequestId: {entryRequestId}");
                Console.WriteLine($"CompanyCode: {companyCode}");
                Console.WriteLine($"QuantityConsumedFilter: {quantityConsumedFilter}");
                Console.WriteLine($"AssemblyNo: {assemblyNo}");

                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");

                // Verificar si existe la solicitud
                Console.WriteLine("Verificando existencia de la solicitud...");
                var entryRequestExists = await companyContext.EntryRequests
                    .AnyAsync(er => er.Id == entryRequestId);

                if (!entryRequestExists)
                {
                    Console.WriteLine($"No se encontró la solicitud de entrada con ID: {entryRequestId}");
                    return NotFound($"No se encontró la solicitud de entrada con ID: {entryRequestId}");
                }

                Console.WriteLine("Solicitud encontrada, ejecutando consulta con LEFT JOIN...");

                // Construir la consulta con LEFT JOIN desde EntryRequests hacia EntryRequestComponents
                var query = from er in companyContext.EntryRequests
                           join era in companyContext.EntryRequestComponents on er.Id equals era.IdEntryReq into eraGroup
                           from era in eraGroup.DefaultIfEmpty()
                           where er.Id == entryRequestId
                           select new { EntryRequest = er, Component = era };

                // Aplicar filtro de número de ensamble si se proporciona
                if (!string.IsNullOrEmpty(assemblyNo))
                {
                    query = query.Where(x => x.Component != null && x.Component.AssemblyNo == assemblyNo);
                }

                // Aplicar filtro de cantidad consumida solo si no es "all"
                if (quantityConsumedFilter.ToLower() != "all")
                {
                    switch (quantityConsumedFilter.ToLower())
                    {
                        case "greater":
                            query = query.Where(x => x.Component != null && (x.Component.QuantityConsumed ?? 0) > 0);
                            break;
                        case "zero":
                            query = query.Where(x => x.Component != null && (x.Component.QuantityConsumed ?? 0) == 0);
                            break;
                    }
                }

                                 // Ejecutar la consulta y mapear a DTO
                 var result = await query
                     .Where(x => x.Component != null) // Solo incluir registros que tengan componentes
                     .Select(x => new EntryRequestComponentDetailDTO
                     {
                         EntryRequestId = x.EntryRequest.Id,
                         RequestNumber = x.EntryRequest.Id.ToString(),
                         ComponentId = x.Component.Id,
                         ComponentCode = x.Component.ItemNo,
                         ComponentDescription = x.Component.ItemName,
                         Quantity = x.Component.Quantity ?? 0,
                         QuantityConsumed = x.Component.QuantityConsumed ?? 0,
                         IdEntryReq = x.Component.IdEntryReq,
                         Id = x.Component.Id
                     })
                     .ToListAsync();

                Console.WriteLine($"Resultados obtenidos: {result.Count}");

                // Si no hay resultados, verificar si hay componentes sin filtro
                if (!result.Any())
                {
                    Console.WriteLine("No hay resultados, verificando componentes sin filtro...");
                    
                    // Verificar si hay componentes sin el filtro usando la misma consulta LEFT JOIN
                    var allComponentsQuery = from er in companyContext.EntryRequests
                                           join era in companyContext.EntryRequestComponents on er.Id equals era.IdEntryReq into eraGroup
                                           from era in eraGroup.DefaultIfEmpty()
                                           where er.Id == entryRequestId && era != null
                                           select era;

                    // Aplicar filtro de número de ensamble si se proporciona
                    if (!string.IsNullOrEmpty(assemblyNo))
                    {
                        allComponentsQuery = allComponentsQuery.Where(era => era.AssemblyNo == assemblyNo);
                    }

                    var allComponents = await allComponentsQuery.ToListAsync();

                    Console.WriteLine($"Componentes totales encontrados: {allComponents.Count}");

                    if (!allComponents.Any())
                    {
                        var message = string.IsNullOrEmpty(assemblyNo)
                            ? $"No se encontraron componentes para la solicitud {entryRequestId}"
                            : $"No se encontraron componentes con número de ensamble '{assemblyNo}' para la solicitud {entryRequestId}";
                        
                        Console.WriteLine("No hay componentes para esta solicitud");
                        return Ok(new { 
                            message = message,
                            entryRequestId = entryRequestId,
                            filter = quantityConsumedFilter,
                            assemblyNo = assemblyNo,
                            totalComponents = 0
                        });
                    }
                    else
                    {
                        var consumedCount = allComponents.Count(a => (a.QuantityConsumed ?? 0) > 0);
                        var zeroCount = allComponents.Count(a => (a.QuantityConsumed ?? 0) == 0);
                        
                        Console.WriteLine($"Componentes con QuantityConsumed > 0: {consumedCount}");
                        Console.WriteLine($"Componentes con QuantityConsumed = 0: {zeroCount}");
                        
                        var message = string.IsNullOrEmpty(assemblyNo)
                            ? $"No se encontraron componentes con el filtro '{quantityConsumedFilter}' para la solicitud {entryRequestId}"
                            : $"No se encontraron componentes con número de ensamble '{assemblyNo}' y filtro '{quantityConsumedFilter}' para la solicitud {entryRequestId}";
                        
                        return Ok(new { 
                            message = message,
                            entryRequestId = entryRequestId,
                            filter = quantityConsumedFilter,
                            assemblyNo = assemblyNo,
                            totalComponents = allComponents.Count,
                            consumedGreaterThanZero = consumedCount,
                            consumedEqualToZero = zeroCount
                        });
                    }
                }

                Console.WriteLine("=== FIN GetEntryRequestComponents ===");
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException en GetEntryRequestComponents: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en GetEntryRequestComponents ===");
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
        /// Actualiza la cantidad consumida de un ensamble específico
        /// </summary>
        /// <param name="id">ID del ensamble a actualizar</param>
        /// <param name="quantityConsumed">Nueva cantidad consumida</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Resultado de la actualización</returns>
        [HttpPatch("{id}/quantity-consumed")]
        public async Task<ActionResult<UpdateResponseDTO>> UpdateQuantityConsumed(
            int id,
            [FromBody] decimal quantityConsumed,
            [FromQuery] string companyCode)
        {
            try
            {
                Console.WriteLine($"=== INICIO UpdateQuantityConsumed ===");
                Console.WriteLine($"ID del ensamble: {id}");
                Console.WriteLine($"Nueva cantidad consumida: {quantityConsumed}");
                Console.WriteLine($"CompanyCode: {companyCode}");

                if (string.IsNullOrEmpty(companyCode))
                {
                    Console.WriteLine("Error: CompanyCode está vacío");
                    return BadRequest("El código de compañía es requerido");
                }

                if (quantityConsumed < 0)
                {
                    Console.WriteLine("Error: La cantidad consumida no puede ser negativa");
                    return BadRequest("La cantidad consumida no puede ser negativa");
                }

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                Console.WriteLine("Contexto de base de datos creado exitosamente");

                // Buscar el ensamble por ID
                var assembly = await companyContext.EntryRequestAssembly
                    .FirstOrDefaultAsync(era => era.Id == id);

                if (assembly == null)
                {
                    Console.WriteLine($"No se encontró el ensamble con ID: {id}");
                    return NotFound($"No se encontró el ensamble con ID: {id}");
                }

                Console.WriteLine($"Ensemble encontrado: ID={assembly.Id}, Code={assembly.Code}, AssemblyNo={assembly.AssemblyNo}");
                Console.WriteLine($"Cantidad actual: {assembly.Quantity}, Cantidad consumida actual: {assembly.QuantityConsumed}");

                // Validar que la cantidad consumida no exceda la cantidad total
                if (quantityConsumed > assembly.Quantity)
                {
                    Console.WriteLine($"Error: La cantidad consumida ({quantityConsumed}) no puede exceder la cantidad total ({assembly.Quantity})");
                    return BadRequest(new { 
                        message = "La cantidad consumida no puede exceder la cantidad total",
                        currentQuantity = assembly.Quantity,
                        requestedQuantityConsumed = quantityConsumed
                    });
                }

                // Actualizar la cantidad consumida
                var previousQuantityConsumed = assembly.QuantityConsumed;
                assembly.QuantityConsumed = quantityConsumed;

                // Guardar los cambios
                await companyContext.SaveChangesAsync();

                Console.WriteLine($"Cantidad consumida actualizada exitosamente");
                Console.WriteLine($"Valor anterior: {previousQuantityConsumed}, Nuevo valor: {quantityConsumed}");

                var response = new UpdateResponseDTO
                {
                    Success = true,
                    Message = "Cantidad consumida actualizada exitosamente",
                    Id = assembly.Id,
                    PreviousValue = previousQuantityConsumed,
                    NewValue = quantityConsumed,
                    UpdatedAt = DateTime.UtcNow
                };

                Console.WriteLine("=== FIN UpdateQuantityConsumed ===");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR en UpdateQuantityConsumed ===");
                Console.WriteLine($"Mensaje de error: {ex.Message}");
                Console.WriteLine($"Tipo de excepción: {ex.GetType().Name}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Exception StackTrace: {ex.InnerException.StackTrace}");
                }
                
                Console.WriteLine("=== FIN ERROR ===");
                return StatusCode(500, new UpdateResponseDTO
                {
                    Success = false,
                    Message = $"Error interno del servidor: {ex.Message}",
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Crea un nuevo ensamble de solicitud de entrada
        /// </summary>
        /// <param name="assemblyDto">Datos del ensamble a crear</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Ensemble creado</returns>
        [HttpPost]
        public async Task<ActionResult<EntryRequestAssemblyResponseDTO>> CreateEntryRequestAssembly(
            [FromBody] EntryRequestAssemblyCreateDTO assemblyDto, 
            [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");

                // Obtener el contexto de la base de datos específica de la compañía
                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);

                // Validar que la solicitud de entrada existe
                var entryRequest = await companyContext.EntryRequests
                    .FirstOrDefaultAsync(er => er.Id == assemblyDto.EntryRequestId);

                if (entryRequest == null)
                    return BadRequest($"No se encontró la solicitud de entrada con ID {assemblyDto.EntryRequestId}");

                // Validar que el detalle de la solicitud existe si se proporciona (excluyendo ID 0)
                if (assemblyDto.EntryRequestDetailId.HasValue && assemblyDto.EntryRequestDetailId.Value > 0)
                {
                    var entryRequestDetail = await companyContext.EntryRequestDetails
                        .FirstOrDefaultAsync(erd => erd.Id == assemblyDto.EntryRequestDetailId.Value);

                    if (entryRequestDetail == null)
                        return BadRequest($"No se encontró el detalle de la solicitud con ID {assemblyDto.EntryRequestDetailId.Value}");
                    
                    // Verificar que el detalle pertenece a la misma solicitud de entrada
                    if (entryRequestDetail.IdEntryReq != assemblyDto.EntryRequestId)
                    {
                        return BadRequest($"El detalle de la solicitud con ID {assemblyDto.EntryRequestDetailId.Value} no pertenece a la solicitud de entrada {assemblyDto.EntryRequestId}");
                    }
                    
                    Console.WriteLine($"EntryRequestDetail encontrado: ID={entryRequestDetail.Id}, IdEntryReq={entryRequestDetail.IdEntryReq}");
                }

                // Crear el nuevo ensamble
                var newAssembly = new EntryRequestAssembly
                {
                    Code = assemblyDto.Code,
                    Description = assemblyDto.Description,
                    ShortDesc = assemblyDto.ShortDesc,
                    Invima = assemblyDto.Invima,
                    Lot = assemblyDto.Lot,
                    Quantity = assemblyDto.Quantity,
                    UnitPrice = assemblyDto.UnitPrice,
                    AssemblyNo = assemblyDto.AssemblyNo,
                    EntryRequestId = assemblyDto.EntryRequestId,
                    EntryRequestDetailId = assemblyDto.EntryRequestDetailId > 0 ? assemblyDto.EntryRequestDetailId : null,
                    QuantityConsumed = assemblyDto.QuantityConsumed ?? 0,
                    ExpirationDate = assemblyDto.ExpirationDate,
                    ReservedQuantity = assemblyDto.ReservedQuantity,
                    Location_Code_ile = assemblyDto.Location_Code_ile,
                    Classification = assemblyDto.Classification,
                    Status = assemblyDto.Status ?? "Activo",
                    LineNo = assemblyDto.LineNo,
                    Position = assemblyDto.Position,
                    Quantity_ile = assemblyDto.Quantity_ile,
                    TaxCode = assemblyDto.TaxCode,
                    LowTurnover = assemblyDto.LowTurnover,
                    IsComponent = assemblyDto.IsComponent,
                    RSFechaVencimiento = assemblyDto.RSFechaVencimiento,
                    RSClasifRegistro = assemblyDto.RSClasifRegistro
                };

                // Log de los datos que se van a guardar para depuración
                Console.WriteLine($"=== DATOS A GUARDAR ===");
                Console.WriteLine($"Code: {newAssembly.Code}");
                Console.WriteLine($"Description: {newAssembly.Description}");
                Console.WriteLine($"AssemblyNo: {newAssembly.AssemblyNo}");
                Console.WriteLine($"EntryRequestId: {newAssembly.EntryRequestId}");
                Console.WriteLine($"EntryRequestDetailId: {newAssembly.EntryRequestDetailId}");
                Console.WriteLine($"Quantity: {newAssembly.Quantity}");
                Console.WriteLine($"Status: {newAssembly.Status}");
                Console.WriteLine("=== FIN DATOS ===");

                // Agregar a la base de datos
                companyContext.EntryRequestAssembly.Add(newAssembly);
                
                // Intentar guardar con más información de depuración
                try
                {
                    await companyContext.SaveChangesAsync();
                    Console.WriteLine("Guardado exitoso en la base de datos");
                }
                catch (Exception saveEx)
                {
                    Console.WriteLine($"Error al guardar en SaveChangesAsync: {saveEx.Message}");
                    throw; // Re-lanzar la excepción para que sea capturada por el catch principal
                }

                // Crear la respuesta
                var response = new EntryRequestAssemblyResponseDTO
                {
                    Id = newAssembly.Id,
                    Code = newAssembly.Code,
                    Description = newAssembly.Description,
                    ShortDesc = newAssembly.ShortDesc,
                    Invima = newAssembly.Invima,
                    Lot = newAssembly.Lot,
                    Quantity = newAssembly.Quantity,
                    UnitPrice = newAssembly.UnitPrice,
                    AssemblyNo = newAssembly.AssemblyNo,
                    EntryRequestId = newAssembly.EntryRequestId,
                    EntryRequestDetailId = newAssembly.EntryRequestDetailId,
                    QuantityConsumed = newAssembly.QuantityConsumed,
                    ExpirationDate = newAssembly.ExpirationDate,
                    Name = newAssembly.Name,
                    ReservedQuantity = newAssembly.ReservedQuantity,
                    Location_Code_ile = newAssembly.Location_Code_ile,
                    Classification = newAssembly.Classification,
                    Status = newAssembly.Status,
                    LineNo = newAssembly.LineNo,
                    Position = newAssembly.Position,
                    Quantity_ile = newAssembly.Quantity_ile,
                    TaxCode = newAssembly.TaxCode,
                    LowTurnover = newAssembly.LowTurnover,
                    IsComponent = newAssembly.IsComponent,
                    RSFechaVencimiento = newAssembly.RSFechaVencimiento,
                    RSClasifRegistro = newAssembly.RSClasifRegistro,
                    LocationCode = newAssembly.LocationCode
                };

                return CreatedAtAction(nameof(GetEntryRequestAssembly), new { id = newAssembly.Id, companyCode }, response);
            }
            catch (Exception ex)
            {
                // Capturar detalles de la excepción interna para depuración
                var errorMsg = $"Error al crear el ensamble: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMsg += $" | InnerException: {ex.InnerException.Message}";
                }
                if (ex.InnerException?.InnerException != null)
                {
                    errorMsg += $" | InnerMostException: {ex.InnerException.InnerException.Message}";
                }
                
                // Log del error completo para depuración
                Console.WriteLine($"=== ERROR en CreateEntryRequestAssembly ===");
                Console.WriteLine($"Mensaje de error: {ex.Message}");
                Console.WriteLine($"Tipo de excepción: {ex.GetType().Name}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Exception StackTrace: {ex.InnerException.StackTrace}");
                }
                
                Console.WriteLine("=== FIN ERROR ===");
                
                return BadRequest(new { mensaje = "Error al crear el ensamble", detalle = errorMsg });
            }
        }

        /// <summary>
        /// Endpoint temporal para verificar la existencia de EntryRequestDetail
        /// </summary>
        /// <param name="id">ID del detalle a verificar</param>
        /// <param name="companyCode">Código de la compañía</param>
        /// <returns>Información del detalle</returns>
        [HttpGet("verify-detail/{id}")]
        public async Task<IActionResult> VerifyEntryRequestDetail(int id, [FromQuery] string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                    return BadRequest("El código de compañía es requerido");

                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);

                var detail = await companyContext.EntryRequestDetails
                    .Where(erd => erd.Id == id)
                    .Select(erd => new
                    {
                        erd.Id,
                        erd.IdEntryReq,
                        erd.IdEquipment,
                        erd.CreateAt,
                        erd.DateIni,
                        erd.DateEnd,
                        erd.status
                    })
                    .FirstOrDefaultAsync();

                if (detail == null)
                    return NotFound($"No se encontró el detalle con ID {id}");

                return Ok(detail);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al verificar el detalle", detalle = ex.Message });
            }
        }

        // Métodos helper para leer datos de forma segura
        private static string GetSafeString(IDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
            }
            catch
            {
                return null;
            }
        }

        private static decimal GetSafeDecimal(IDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
            }
            catch
            {
                return 0;
            }
        }

        private static int GetSafeInt32(IDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
            }
            catch
            {
                return 0;
            }
        }
    }
}
