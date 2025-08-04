using DimmedAPI.DTOs;
using DimmedAPI.Entidades;
using DimmedAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace DimmedAPI.BO
{
    public class EquipmentNotFoundException : Exception
    {
        public string EquipmentCode { get; }

        public EquipmentNotFoundException(string equipmentCode) 
            : base($"El equipo con código '{equipmentCode}' no existe en la base de datos")
        {
            EquipmentCode = equipmentCode;
        }
    }

    public class EquipmentBO : IEquipmentBO
    {
        private readonly ApplicationDBContext _context;
        private readonly IntBCConex _bcConn;

        public EquipmentBO(ApplicationDBContext context, IntBCConex bcConn)
        {
            _context = context;
            _bcConn = bcConn;
        }

        public async Task<List<Equipment>> SincronizeBCAsync()
        {
            try
            {
                // Obtener todos los equipos de Business Central
                Console.WriteLine("Obteniendo equipos desde Business Central...");
                List<Equipment> equipmentsBC = await _bcConn.getEquipmetListBCAsync("lylequipment");
                Console.WriteLine($"Total de equipos obtenidos desde BC: {equipmentsBC.Count}");
                
                List<Equipment> equipmentsUpdated = new List<Equipment>();
                int procesados = 0;
                int nuevos = 0;
                int actualizados = 0;

                foreach (var equipmentBC in equipmentsBC)
                {
                    procesados++;
                    if (procesados % 50 == 0) // Mostrar progreso cada 50 equipos
                    {
                        Console.WriteLine($"Procesando equipo {procesados}/{equipmentsBC.Count} - Nuevos: {nuevos}, Actualizados: {actualizados}");
                    }

                    // Buscar si el equipo ya existe usando consulta SQL raw que maneja valores NULL
                    var existingEquipment = await _context.Equipment
                        .FromSqlRaw(@"
                            SELECT 
                                Id,
                                ISNULL(Code, '') as Code,
                                ISNULL(Name, '') as Name,
                                ISNULL(ShortName, '') as ShortName,
                                ISNULL(Status, '') as Status,
                                ISNULL(ProductLine, '') as ProductLine,
                                ISNULL(Branch, '') as Branch,
                                ISNULL(EstimatedTime, '') as EstimatedTime,
                                ISNULL(Description, '') as Description,
                                IsActive,
                                ISNULL(TechSpec, '') as TechSpec,
                                ISNULL(DestinationBranch, '') as DestinationBranch,
                                LoanDate,
                                ReturnDate,
                                InitDate,
                                ISNULL(Vendor, '') as Vendor,
                                ISNULL(Brand, '') as Brand,
                                ISNULL(Model, '') as Model,
                                ISNULL(Abc, '') as Abc,
                                EndDate,
                                ISNULL(Type, '') as Type,
                                ISNULL(SystemIdBC, '') as SystemIdBC,
                                ISNULL(NoBoxes, 0) as NoBoxes,
                                ISNULL(LastPreventiveMaintenance, '1753-01-01') as LastPreventiveMaintenance,
                                ISNULL(LastMaintenance, '1753-01-01') as LastMaintenance,
                                ISNULL(Alert, '') as Alert,
                                ISNULL(LocationCode, '') as LocationCode,
                                ISNULL(TransferStatus, '') as TransferStatus
                            FROM Equipment 
                            WHERE SystemIdBC = {0}", equipmentBC.SystemIdBC ?? "")
                        .FirstOrDefaultAsync();

                    if (existingEquipment == null)
                    {
                        // Crear nuevo equipo
                        var newEquipment = new Equipment
                        {
                            Name = equipmentBC.Name ?? "",
                            Abc = equipmentBC.Abc ?? "",
                            Branch = equipmentBC.Branch ?? "",
                            Brand = equipmentBC.Brand ?? "",
                            Code = equipmentBC.Code ?? "",
                            Description = equipmentBC.Description ?? "",
                            DestinationBranch = equipmentBC.DestinationBranch ?? "",
                            EndDate = equipmentBC.EndDate,
                            EstimatedTime = equipmentBC.EstimatedTime ?? "",
                            InitDate = equipmentBC.InitDate,
                            SystemIdBC = equipmentBC.SystemIdBC ?? "",
                            IsActive = equipmentBC.IsActive,
                            LoanDate = equipmentBC.LoanDate,
                            Model = equipmentBC.Model ?? "",
                            ProductLine = equipmentBC.ProductLine ?? "",
                            ReturnDate = equipmentBC.ReturnDate,
                            ShortName = equipmentBC.ShortName ?? "",
                            Status = equipmentBC.Status ?? "",
                            TechSpec = equipmentBC.TechSpec ?? "",
                            Type = equipmentBC.Type ?? "",
                            Vendor = equipmentBC.Vendor ?? "",
                            NoBoxes = equipmentBC.NoBoxes,
                            LastPreventiveMaintenance = new DateTime(1753, 1, 1), // Valor por defecto para campos no nullable
                            LastMaintenance = new DateTime(1753, 1, 1), // Valor por defecto para campos no nullable
                            Alert = equipmentBC.Alert ?? "",
                            LocationCode = equipmentBC.LocationCode ?? "",
                            TransferStatus = equipmentBC.TransferStatus ?? ""
                        };

                        _context.Equipment.Add(newEquipment);
                        equipmentsUpdated.Add(newEquipment);
                        nuevos++;
                    }
                    else
                    {
                        // Actualizar equipo existente
                        existingEquipment.Name = equipmentBC.Name ?? "";
                        existingEquipment.Abc = equipmentBC.Abc ?? "";
                        existingEquipment.Branch = equipmentBC.Branch ?? "";
                        existingEquipment.Brand = equipmentBC.Brand ?? "";
                        existingEquipment.Code = equipmentBC.Code ?? "";
                        existingEquipment.Description = equipmentBC.Description ?? "";
                        existingEquipment.DestinationBranch = equipmentBC.DestinationBranch ?? "";
                        existingEquipment.EndDate = equipmentBC.EndDate;
                        existingEquipment.EstimatedTime = equipmentBC.EstimatedTime ?? "";
                        existingEquipment.InitDate = equipmentBC.InitDate;
                        existingEquipment.IsActive = equipmentBC.IsActive;
                        existingEquipment.LoanDate = equipmentBC.LoanDate;
                        existingEquipment.Model = equipmentBC.Model ?? "";
                        existingEquipment.ProductLine = equipmentBC.ProductLine ?? "";
                        existingEquipment.ReturnDate = equipmentBC.ReturnDate;
                        existingEquipment.ShortName = equipmentBC.ShortName ?? "";
                        existingEquipment.Status = equipmentBC.Status ?? "";
                        existingEquipment.TechSpec = equipmentBC.TechSpec ?? "";
                        existingEquipment.Type = equipmentBC.Type ?? "";
                        existingEquipment.Vendor = equipmentBC.Vendor ?? "";
                        existingEquipment.NoBoxes = equipmentBC.NoBoxes;
                        existingEquipment.Alert = equipmentBC.Alert ?? "";
                        existingEquipment.LocationCode = equipmentBC.LocationCode ?? "";
                        existingEquipment.TransferStatus = equipmentBC.TransferStatus ?? "";

                        equipmentsUpdated.Add(existingEquipment);
                        actualizados++;
                    }
                }

                Console.WriteLine($"Proceso completado. Total procesados: {procesados}, Nuevos: {nuevos}, Actualizados: {actualizados}");
                await _context.SaveChangesAsync();
                return equipmentsUpdated;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en SincronizeBCAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<object> SincronizarDesdeBC(EquipmentBCDTO dto)
        {
            //Console.WriteLine("=== INICIO SincronizarDesdeBC ===");
            //Console.WriteLine($"DTO recibido - Code: '{dto.Code}', Name: '{dto.Name}', SystemId: '{dto.SystemId}'");
            //Console.WriteLine($"DTO recibido - Status: '{dto.Status}', ProductLine: '{dto.ProductLine}', Branch: '{dto.Branch}'");
            
            // Validaciones de campos requeridos
            // Console.WriteLine("Iniciando validaciones...");
            
            if (string.IsNullOrWhiteSpace(dto.Code))
            {
                Console.WriteLine("ERROR: Code está vacío o nulo");
                throw new Exception("El campo 'Code' es requerido y viene vacío o nulo desde BC.");
            }
            Console.WriteLine("✓ Code válido");
            
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                Console.WriteLine("ERROR: Name está vacío o nulo");
                throw new Exception("El campo 'Name' es requerido y viene vacío o nulo desde BC.");
            }
            Console.WriteLine("✓ Name válido");
            
            if (string.IsNullOrWhiteSpace(dto.SystemId))
            {
                Console.WriteLine("ERROR: SystemId está vacío o nulo");
                throw new Exception("El campo 'SystemId' es requerido y viene vacío o nulo desde BC.");
            }
            Console.WriteLine("✓ SystemId válido");
            
            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                Console.WriteLine("ERROR: Status está vacío o nulo");
                throw new Exception("El campo 'Status' es requerido y viene vacío o nulo desde BC.");
            }
            Console.WriteLine("✓ Status válido");
            
            if (string.IsNullOrWhiteSpace(dto.ProductLine))
            {
                Console.WriteLine("ERROR: ProductLine está vacío o nulo");
                throw new Exception("El campo 'ProductLine' es requerido y viene vacío o nulo desde BC.");
            }
            Console.WriteLine("✓ ProductLine válido");
            
            if (string.IsNullOrWhiteSpace(dto.Branch))
            {
                Console.WriteLine("ERROR: Branch está vacío o nulo");
                throw new Exception("El campo 'Branch' es requerido y viene vacío o nulo desde BC.");
            }
            Console.WriteLine("✓ Branch válido");
            
            Console.WriteLine("Todas las validaciones pasaron correctamente.");

            // Buscar si el equipo ya existe
            Console.WriteLine("Iniciando búsqueda del equipo existente...");
            Console.WriteLine($"SystemId a buscar: '{dto.SystemId}'");

            // Consulta que maneja correctamente los valores NULL de la base de datos
            var existingEquipment = await _context.Equipment
                .FromSqlRaw(@"
                    SELECT 
                        Id,
                        ISNULL(Code, '') as Code,
                        ISNULL(Name, '') as Name,
                        ISNULL(ShortName, '') as ShortName,
                        ISNULL(Status, '') as Status,
                        ISNULL(ProductLine, '') as ProductLine,
                        ISNULL(Branch, '') as Branch,
                        ISNULL(EstimatedTime, '') as EstimatedTime,
                        ISNULL(Description, '') as Description,
                        IsActive,
                        ISNULL(TechSpec, '') as TechSpec,
                        ISNULL(DestinationBranch, '') as DestinationBranch,
                        LoanDate,
                        ReturnDate,
                        InitDate,
                        ISNULL(Vendor, '') as Vendor,
                        ISNULL(Brand, '') as Brand,
                        ISNULL(Model, '') as Model,
                        ISNULL(Abc, '') as Abc,
                        EndDate,
                        ISNULL(Type, '') as Type,
                        ISNULL(SystemIdBC, '') as SystemIdBC,
                        ISNULL(NoBoxes, 0) as NoBoxes,
                        ISNULL(LastPreventiveMaintenance, '1753-01-01') as LastPreventiveMaintenance,
                        ISNULL(LastMaintenance, '1753-01-01') as LastMaintenance,
                        ISNULL(Alert, '') as Alert,
                        ISNULL(LocationCode, '') as LocationCode,
                        ISNULL(TransferStatus, '') as TransferStatus
                    FROM Equipment 
                    WHERE SystemIdBC = {0}", dto.SystemId)
                .FirstOrDefaultAsync();

            Console.WriteLine("Búsqueda completada.");

            // Log de diagnóstico
            Console.WriteLine($"Buscando equipo con SystemIdBC: '{dto.SystemId}'");
            Console.WriteLine($"Equipo encontrado: {(existingEquipment != null ? "SÍ" : "NO")}");
            if (existingEquipment != null)
            {
                Console.WriteLine($"Equipo existente - ID: {existingEquipment.Id}, Code: {existingEquipment.Code}, SystemIdBC: '{existingEquipment.SystemIdBC}'");
            }
            else
            {
                // Verificar si hay equipos con SystemIdBC similar
                var equiposSimilares = _context.Equipment
                    .Where(e => e.SystemIdBC != null && e.SystemIdBC.Contains(dto.SystemId.Substring(0, 8)))
                    .Take(5)
                    .ToList();
                Console.WriteLine($"Equipos con SystemIdBC similar encontrados: {equiposSimilares.Count}");
                foreach (var eq in equiposSimilares)
                {
                    Console.WriteLine($"  - ID: {eq.Id}, Code: {eq.Code}, SystemIdBC: '{eq.SystemIdBC}'");
                }
            }

            if (existingEquipment == null)
            {
                // Crear nuevo
                var newEquipment = new Equipment
                {
                    Name = dto.Name ?? "",
                    Abc = dto.Abc ?? "",
                    Branch = dto.Branch ?? "",
                    Brand = dto.Brand ?? "",
                    Code = dto.Code ?? "",
                    Description = dto.Description ?? "",
                    DestinationBranch = dto.DestinationBranch ?? "",
                    EndDate = dto.EndDate,
                    EstimatedTime = dto.EstimatedTime ?? "",
                    InitDate = dto.InitDate,
                    SystemIdBC = dto.SystemId ?? "",
                    IsActive = dto.IsActive,
                    LoanDate = dto.LoanDate,
                    Model = dto.Model ?? "",
                    ProductLine = dto.ProductLine ?? "",
                    ReturnDate = dto.ReturnDate,
                    ShortName = dto.ShortName ?? "",
                    Status = dto.Status ?? "",
                    TechSpec = dto.TechSpec ?? "",
                    Type = dto.Type ?? "",
                    Vendor = dto.Vendor ?? "",
                    NoBoxes = dto.NoBoxes ?? 0,
                    LastPreventiveMaintenance = new DateTime(1753, 1, 1), // Valor por defecto para campos no nullable
                    LastMaintenance = new DateTime(1753, 1, 1), // Valor por defecto para campos no nullable
                    Alert = dto.Alert ?? "",
                    LocationCode = dto.LocationCode ?? "",
                    TransferStatus = dto.TransferStatus ?? ""
                };

                _context.Equipment.Add(newEquipment);
                await _context.SaveChangesAsync();
                return newEquipment;
            }
            else
            {
                // Actualizar
                existingEquipment.Name = dto.Name ?? "";
                existingEquipment.Abc = dto.Abc ?? "";
                existingEquipment.Branch = dto.Branch ?? "";
                existingEquipment.Brand = dto.Brand ?? "";
                existingEquipment.Code = dto.Code ?? "";
                existingEquipment.Description = dto.Description ?? "";
                existingEquipment.DestinationBranch = dto.DestinationBranch ?? "";
                existingEquipment.EndDate = dto.EndDate;
                existingEquipment.EstimatedTime = dto.EstimatedTime ?? "";
                existingEquipment.InitDate = dto.InitDate;
                existingEquipment.IsActive = dto.IsActive;
                existingEquipment.LoanDate = dto.LoanDate;
                existingEquipment.Model = dto.Model ?? "";
                existingEquipment.ProductLine = dto.ProductLine ?? "";
                existingEquipment.ReturnDate = dto.ReturnDate;
                existingEquipment.ShortName = dto.ShortName ?? "";
                existingEquipment.Status = dto.Status ?? "";
                existingEquipment.TechSpec = dto.TechSpec ?? "";
                existingEquipment.Type = dto.Type ?? "";
                existingEquipment.Vendor = dto.Vendor ?? "";
                existingEquipment.NoBoxes = dto.NoBoxes ?? 0;
                existingEquipment.Alert = dto.Alert ?? "";
                existingEquipment.LocationCode = dto.LocationCode ?? "";
                existingEquipment.TransferStatus = dto.TransferStatus ?? "";

                await _context.SaveChangesAsync();
                return existingEquipment;
            }
        }

        public async Task<List<Equipment>> GetEquipmentsFromBCAsync(int? take = null, string systemIdBc = null, string code = null)
        {
            try
            {
                var equipments = new List<Equipment>();
                if (!string.IsNullOrEmpty(systemIdBc))
                {
                    var equipment = await _bcConn.getEquipmentBCAsync("lylequipment", systemIdBc);
                    if (equipment != null)
                    {
                        equipments.Add(new Equipment
                        {
                            Name = equipment.Name ?? "",
                            Abc = equipment.Abc ?? "",
                            Branch = equipment.Branch ?? "",
                            Brand = equipment.Brand ?? "",
                            Code = equipment.Code ?? "",
                            Description = equipment.Description ?? "",
                            DestinationBranch = equipment.DestinationBranch ?? "",
                            EndDate = equipment.EndDate,
                            EstimatedTime = equipment.EstimatedTime ?? "",
                            InitDate = equipment.InitDate,
                            SystemIdBC = equipment.SystemId ?? "",
                            IsActive = equipment.IsActive,
                            LoanDate = equipment.LoanDate,
                            Model = equipment.Model ?? "",
                            ProductLine = equipment.ProductLine ?? "",
                            ReturnDate = equipment.ReturnDate,
                            ShortName = equipment.ShortName ?? "",
                            Status = equipment.Status ?? "",
                            TechSpec = equipment.TechSpec ?? "",
                            Type = equipment.Type ?? "",
                            Vendor = equipment.Vendor ?? "",
                            NoBoxes = equipment.NoBoxes ?? 0,
                            LastPreventiveMaintenance = equipment.LastPreventiveMaintenance ?? new DateTime(1753, 1, 1),
                            LastMaintenance = equipment.LastMaintenance ?? new DateTime(1753, 1, 1),
                            Alert = equipment.Alert ?? "",
                            LocationCode = equipment.LocationCode ?? "",
                            TransferStatus = equipment.TransferStatus ?? ""
                        });
                    }
                }
                else if (!string.IsNullOrEmpty(code))
                {
                    var allEquipments = await _bcConn.getEquipmetListBCAsync("lylequipment");
                    equipments = allEquipments
                        .Where(e => !string.IsNullOrEmpty(e.Code) && e.Code.Trim().Equals(code.Trim(), StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
                else
                {
                    var allEquipments = await _bcConn.getEquipmetListBCAsync("lylequipment");
                    if (take.HasValue && take.Value > 0)
                    {
                        equipments = allEquipments.Take(take.Value).ToList();
                    }
                    else
                    {
                        equipments = allEquipments;
                    }
                }
                return equipments;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en GetEquipmentsFromBCAsync: {ex.Message}", ex);
            }
        }

        private async Task<Equipment> GetByIdAsync(int id)
        {
            try
            {
                var equipment = await _context.Equipment
                    .FromSqlRaw(@"
                        SELECT 
                            Id,
                            ISNULL(Code, '') as Code,
                            ISNULL(Name, '') as Name,
                            ISNULL(ShortName, '') as ShortName,
                            ISNULL(Status, '') as Status,
                            ISNULL(ProductLine, '') as ProductLine,
                            ISNULL(Branch, '') as Branch,
                            ISNULL(EstimatedTime, '') as EstimatedTime,
                            ISNULL(Description, '') as Description,
                            IsActive,
                            ISNULL(TechSpec, '') as TechSpec,
                            ISNULL(DestinationBranch, '') as DestinationBranch,
                            LoanDate,
                            ReturnDate,
                            InitDate,
                            ISNULL(Vendor, '') as Vendor,
                            ISNULL(Brand, '') as Brand,
                            ISNULL(Model, '') as Model,
                            ISNULL(Abc, '') as Abc,
                            EndDate,
                            ISNULL(Type, '') as Type,
                            ISNULL(SystemIdBC, '') as SystemIdBC,
                            ISNULL(NoBoxes, 0) as NoBoxes,
                            ISNULL(LastPreventiveMaintenance, '1753-01-01') as LastPreventiveMaintenance,
                            ISNULL(LastMaintenance, '1753-01-01') as LastMaintenance,
                            ISNULL(Alert, '') as Alert,
                            ISNULL(LocationCode, '') as LocationCode,
                            ISNULL(TransferStatus, '') as TransferStatus
                        FROM Equipment 
                        WHERE Id = {0}", id)
                    .FirstOrDefaultAsync();

                if (equipment == null)
                {
                    throw new Exception($"No se encontró el equipo con ID {id}");
                }
                return equipment;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el equipo: {ex.Message}", ex);
            }
        }

        public async Task<Equipment> GetEquipmentByCode(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return null;
                }

                var equipment = await _context.Equipment
                    .FromSqlRaw(@"
                        SELECT 
                            Id,
                            ISNULL(Code, '') as Code,
                            ISNULL(Name, '') as Name,
                            ISNULL(ShortName, '') as ShortName,
                            ISNULL(Status, '') as Status,
                            ISNULL(ProductLine, '') as ProductLine,
                            ISNULL(Branch, '') as Branch,
                            ISNULL(EstimatedTime, '') as EstimatedTime,
                            ISNULL(Description, '') as Description,
                            IsActive,
                            ISNULL(TechSpec, '') as TechSpec,
                            ISNULL(DestinationBranch, '') as DestinationBranch,
                            LoanDate,
                            ReturnDate,
                            InitDate,
                            ISNULL(Vendor, '') as Vendor,
                            ISNULL(Brand, '') as Brand,
                            ISNULL(Model, '') as Model,
                            ISNULL(Abc, '') as Abc,
                            EndDate,
                            ISNULL(Type, '') as Type,
                            ISNULL(SystemIdBC, '') as SystemIdBC,
                            ISNULL(NoBoxes, 0) as NoBoxes,
                            ISNULL(LastPreventiveMaintenance, '1753-01-01') as LastPreventiveMaintenance,
                            ISNULL(LastMaintenance, '1753-01-01') as LastMaintenance,
                            ISNULL(Alert, '') as Alert,
                            ISNULL(LocationCode, '') as LocationCode,
                            ISNULL(TransferStatus, '') as TransferStatus
                        FROM Equipment 
                        WHERE Code = {0}", code)
                    .FirstOrDefaultAsync();

                return equipment;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el equipo: {ex.Message}", ex);
            }
        }

        private async Task<Equipment> GetByCodeAsync(string code)
        {
            try
            {
                var equipment = await GetEquipmentByCode(code);
                if (equipment == null)
                {
                    throw new EquipmentNotFoundException(code);
                }
                return equipment;
            }
            catch (EquipmentNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el equipo: {ex.Message}", ex);
            }
        }

        public async Task<List<EntryRequestAssembly>> getAInventory(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new ArgumentException("El código del equipo es obligatorio");
                }

                Equipment equipment = await GetByCodeAsync(code);
                List<EntryRequestAssembly> dataEquipment = new List<EntryRequestAssembly>();
                
                try
                {
                    // Try the original endpoints first
                    List<EntryRequestAssembly> dataEquipmentLines = await _bcConn.GetEntryReqAssembly("lylassemblyolines", equipment.Code, "");
                    dataEquipment = await _bcConn.GetEntryReqAssembly("lylassemblyeq", equipment.Code, "");
                    
                    if(dataEquipmentLines != null)
                        foreach (var eq in dataEquipmentLines)
                        {
                            if (!dataEquipment.Exists(x => x.Code == eq.Code))
                            {
                                dataEquipment.Add(eq);
                            }
                        }
                }
                catch (Exception ex)
                {
                    // If lylassemblyolines fails, try using lylassembly as fallback
                    if (ex.Message.Contains("NotFound") || ex.Message.Contains("BadRequest_NotFound"))
                    {
                        dataEquipment = await _bcConn.GetEntryReqAssembly("lylassembly", equipment.Code, "");
                    }
                    else
                    {
                        throw;
                    }
                }
                
                return dataEquipment;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (EquipmentNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el inventario del equipo '{code}': {ex.Message}", ex);
            }
        }

        public async Task<List<EntryRequestAssembly>> getAInventory(string code, string locationCode)
        {
            try
            {
                // Validar que ambos parámetros no sean nulos o vacíos
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new ArgumentException("El código del equipo es obligatorio");
                }

                if (string.IsNullOrWhiteSpace(locationCode))
                {
                    throw new ArgumentException("El código de ubicación es obligatorio");
                }

                Equipment equipment = await GetEquipmentByCode(code);

                List<EntryRequestAssembly> dataEquipment = new List<EntryRequestAssembly>();
                
                try
                {
                    // Try the original endpoints first
                    List<EntryRequestAssembly> dataEquipmentLines = await _bcConn.GetEntryReqAssembly("lylassemblyolines", equipment.Code, "");
                    dataEquipment = await _bcConn.GetEntryReqAssembly("lylassemblyeq", equipment.Code, "");
                    
                    if(dataEquipmentLines != null)
                        foreach (var eq in dataEquipmentLines)
                        {
                            if (!dataEquipment.Exists(x => x.Code == eq.Code))
                            {
                                dataEquipment.Add(eq);
                            }
                        }
                }
                catch (Exception ex)
                {
                    // If lylassemblyolines fails, try using lylassembly as fallback
                    if (ex.Message.Contains("NotFound") || ex.Message.Contains("BadRequest_NotFound"))
                    {
                        dataEquipment = await _bcConn.GetEntryReqAssembly("lylassembly", equipment.Code, "");
                    }
                    else
                    {
                        throw;
                    }
                }

                // Filtrar por locationCode
                var filteredInventory = dataEquipment
                    .Where(item => 
                        (!string.IsNullOrWhiteSpace(item.LocationCode) && 
                         string.Equals(item.LocationCode.Trim(), locationCode.Trim(), StringComparison.OrdinalIgnoreCase)) &&
                        (!string.IsNullOrWhiteSpace(item.Location_Code_ile) && 
                         string.Equals(item.Location_Code_ile.Trim(), locationCode.Trim(), StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                return filteredInventory;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<EntryRequestAssembly>> getAInventory(string code, string branchCode, string locationCode)
        {
            try
            {
                // Validar que todos los parámetros no sean nulos o vacíos
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new ArgumentException("El código del equipo es obligatorio");
                }
                
                if (string.IsNullOrWhiteSpace(branchCode))
                {
                    throw new ArgumentException("El código de sede es obligatorio");
                }

                if (string.IsNullOrWhiteSpace(locationCode))
                {
                    throw new ArgumentException("El código de ubicación es obligatorio");
                }

                Equipment equipment = await GetEquipmentByCode(code);
                
                // Verificar que el equipo pertenezca a la sede especificada
                if (!string.Equals(equipment.Branch?.Trim(), branchCode.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception($"El equipo con código {code} no pertenece a la sede {branchCode}. Sede actual: {equipment.Branch}");
                }

                List<EntryRequestAssembly> dataEquipment = new List<EntryRequestAssembly>();
                
                try
                {
                    // Try the original endpoints first
                    List<EntryRequestAssembly> dataEquipmentLines = await _bcConn.GetEntryReqAssembly("lylassemblyolines", equipment.Code, "");
                    dataEquipment = await _bcConn.GetEntryReqAssembly("lylassemblyeq", equipment.Code, "");
                    
                    if(dataEquipmentLines != null)
                        foreach (var eq in dataEquipmentLines)
                        {
                            if (!dataEquipment.Exists(x => x.Code == eq.Code))
                            {
                                dataEquipment.Add(eq);
                            }
                        }
                }
                catch (Exception ex)
                {
                    // If lylassemblyolines fails, try using lylassembly as fallback
                    if (ex.Message.Contains("NotFound") || ex.Message.Contains("BadRequest_NotFound"))
                    {
                        dataEquipment = await _bcConn.GetEntryReqAssembly("lylassembly", equipment.Code, "");
                    }
                    else
                    {
                        throw;
                    }
                }

                // Filtrar por locationCode
                var filteredInventory = dataEquipment
                    .Where(item => 
                        (!string.IsNullOrWhiteSpace(item.LocationCode) && 
                         string.Equals(item.LocationCode.Trim(), locationCode.Trim(), StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(item.Location_Code_ile) && 
                         string.Equals(item.Location_Code_ile.Trim(), locationCode.Trim(), StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                return filteredInventory;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<EntryRequestAssembly>> getAInventory(int equipmentId)
        {
            try
            {
                Equipment equipment = await GetByIdAsync(equipmentId);
                
                // Try using lylassembly first (which seems to be working based on other controllers)
                List<EntryRequestAssembly> dataEquipment = new List<EntryRequestAssembly>();
                
                try
                {
                    // Try the original endpoints first
                    List<EntryRequestAssembly> dataEquipmentLines = await _bcConn.GetEntryReqAssembly("lylassemblyolines", equipment.Code, "");
                    dataEquipment = await _bcConn.GetEntryReqAssembly("lylassemblyeq", equipment.Code, "");
                    
                    if(dataEquipmentLines != null)
                        foreach (var eq in dataEquipmentLines)
                        {
                            if (!dataEquipment.Exists(x => x.Code == eq.Code))
                            {
                                dataEquipment.Add(eq);
                            }
                        }
                }
                catch (Exception ex)
                {
                    // If lylassemblyolines fails, try using lylassembly as fallback
                    if (ex.Message.Contains("NotFound") || ex.Message.Contains("BadRequest_NotFound"))
                    {
                        dataEquipment = await _bcConn.GetEntryReqAssembly("lylassembly", equipment.Code, "");
                    }
                    else
                    {
                        throw;
                    }
                }
                
                return dataEquipment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Determina qué endpoint de ensamble está disponible
        /// </summary>
        /// <param name="equipmentCode">Código del equipo</param>
        /// <returns>El nombre del endpoint disponible</returns>
        private async Task<string> GetAvailableAssemblyEndpoint(string equipmentCode)
        {
            // Lista de endpoints a probar en orden de preferencia
            string[] endpointsToTry = { "lylassembly", "lylassemblyV2", "lylassemblyolines", "lylassemblyeq" };
            
            foreach (string endpoint in endpointsToTry)
            {
                try
                {
                    var testResult = await _bcConn.GetEntryReqAssembly(endpoint, equipmentCode, "");
                    if (testResult != null && testResult.Count > 0)
                    {
                        return endpoint;
                    }
                }
                catch (Exception ex)
                {
                    // Si el error es NotFound, continuar con el siguiente endpoint
                    if (!ex.Message.Contains("NotFound") && !ex.Message.Contains("BadRequest_NotFound"))
                    {
                        throw; // Re-lanzar otros tipos de errores
                    }
                }
            }
            
            // Si ningún endpoint funciona, usar lylassembly como fallback
            return "lylassembly";
        }

        public async Task<List<Equipment>> GetAllEquipmentCodes(int? take = null)
        {
            try
            {
                var query = _context.Equipment
                    .FromSqlRaw(@"
                        SELECT 
                            Id,
                            ISNULL(Code, '') as Code,
                            ISNULL(Name, '') as Name,
                            ISNULL(ShortName, '') as ShortName,
                            ISNULL(Status, '') as Status,
                            ISNULL(ProductLine, '') as ProductLine,
                            ISNULL(Branch, '') as Branch,
                            ISNULL(EstimatedTime, '') as EstimatedTime,
                            ISNULL(Description, '') as Description,
                            IsActive,
                            ISNULL(TechSpec, '') as TechSpec,
                            ISNULL(DestinationBranch, '') as DestinationBranch,
                            LoanDate,
                            ReturnDate,
                            InitDate,
                            ISNULL(Vendor, '') as Vendor,
                            ISNULL(Brand, '') as Brand,
                            ISNULL(Model, '') as Model,
                            ISNULL(Abc, '') as Abc,
                            EndDate,
                            ISNULL(Type, '') as Type,
                            ISNULL(SystemIdBC, '') as SystemIdBC,
                            ISNULL(NoBoxes, 0) as NoBoxes,
                            ISNULL(LastPreventiveMaintenance, '1753-01-01') as LastPreventiveMaintenance,
                            ISNULL(LastMaintenance, '1753-01-01') as LastMaintenance,
                            ISNULL(Alert, '') as Alert,
                            ISNULL(LocationCode, '') as LocationCode,
                            ISNULL(TransferStatus, '') as TransferStatus
                        FROM Equipment 
                        ORDER BY Code");

                if (take.HasValue)
                {
                    query = query.Take(take.Value);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la lista de equipos: {ex.Message}", ex);
            }
        }
    }
} 