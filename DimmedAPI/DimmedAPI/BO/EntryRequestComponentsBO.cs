using DimmedAPI.Entidades;
using DimmedAPI.BO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DimmedAPI.BO
{
    public class EntryRequestComponentsBO
    {
        private readonly ApplicationDBContext _context;
        private readonly IntBCConex _bcConn;

        public EntryRequestComponentsBO(ApplicationDBContext context, IntBCConex bcConn)
        {
            _context = context;
            _bcConn = bcConn;
        }

        /// <summary>
        /// Obtiene componentes de EntryRequest desde Business Central
        /// </summary>
        /// <param name="location">Ubicación (opcional)</param>
        /// <param name="stock">Stock (opcional)</param>
        /// <param name="salesCode">Código de venta (opcional)</param>
        /// <returns>Lista de componentes</returns>
        public async Task<List<EntryRequestComponents>> GetComponentsAsync(string? location = null, string? stock = null, string? salesCode = null)
        {
            try
            {
                // Si no se proporcionan parámetros, devolver lista vacía o manejar según lógica de negocio
                if (string.IsNullOrEmpty(location) && string.IsNullOrEmpty(stock) && string.IsNullOrEmpty(salesCode))
                {
                    // Opción 1: Devolver lista vacía
                    return new List<EntryRequestComponents>();
                    
                    // Opción 2: Obtener todos los componentes sin filtros (si el método BC lo soporta)
                    // List<EntryRequestComponents> lData = await _bcConn.GetComponents("lylitems", "", "", "");
                    // return lData;
                }

                List<EntryRequestComponents> lData = await _bcConn.GetComponents("lylitems", location ?? "", stock ?? "", salesCode ?? "");
                
                // Si lData es null, devolver lista vacía en lugar de null
                return lData ?? new List<EntryRequestComponents>();
            }
            catch (Exception ex)
            {
                // Log del error para debugging
                Console.WriteLine($"Error en GetComponentsAsync: {ex.Message}");
                
                // Devolver lista vacía en lugar de lanzar excepción
                return new List<EntryRequestComponents>();
            }
        }

        /// <summary>
        /// Obtiene componentes de EntryRequest desde Business Central con parámetros opcionales
        /// </summary>
        /// <param name="method">Método de Business Central</param>
        /// <param name="location">Ubicación</param>
        /// <param name="stock">Stock</param>
        /// <param name="salesCode">Código de venta</param>
        /// <param name="take">Número máximo de registros a obtener</param>
        /// <returns>Lista de componentes</returns>
        public async Task<List<EntryRequestComponents>> GetComponentsFromBCAsync(string method, string location, string stock, string salesCode, int? take = null)
        {
            try
            {
                var components = await _bcConn.GetComponents(method, location, stock, salesCode);
                
                if (take.HasValue && take.Value > 0)
                {
                    components = components.Take(take.Value).ToList();
                }

                return components;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener componentes desde BC: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Sincroniza un componente específico desde Business Central
        /// </summary>
        /// <param name="componentBC">Componente de BC a sincronizar</param>
        /// <returns>Componente sincronizado</returns>
        public async Task<EntryRequestComponents> SincronizarDesdeBC(EntryRequestComponents componentBC)
        {
            try
            {
                if (componentBC == null)
                {
                    throw new ArgumentException("El componente no puede ser nulo");
                }

                // Buscar si el componente ya existe por ItemNo y SystemId
                var existingComponent = await _context.EntryRequestComponents
                    .FirstOrDefaultAsync(c => c.ItemNo == componentBC.ItemNo && c.SystemId == componentBC.SystemId);

                if (existingComponent == null)
                {
                    // Crear nuevo componente
                    var newComponent = new EntryRequestComponents
                    {
                        ItemNo = componentBC.ItemNo,
                        ItemName = componentBC.ItemName,
                        Warehouse = componentBC.Warehouse,
                        Quantity = componentBC.Quantity,
                        SystemId = componentBC.SystemId,
                        QuantityConsumed = componentBC.QuantityConsumed,
                        Branch = componentBC.Branch,
                        Lot = componentBC.Lot,
                        UnitPrice = componentBC.UnitPrice,
                        status = componentBC.status,
                        AssemblyNo = componentBC.AssemblyNo,
                        TaxCode = componentBC.TaxCode,
                        shortDesc = componentBC.shortDesc,
                        Invima = componentBC.Invima,
                        ExpirationDate = componentBC.ExpirationDate,
                        TraceState = componentBC.TraceState,
                        RSFechaVencimiento = componentBC.RSFechaVencimiento,
                        RSClasifRegistro = componentBC.RSClasifRegistro
                    };

                    _context.EntryRequestComponents.Add(newComponent);
                    await _context.SaveChangesAsync();
                    
                    return newComponent;
                }
                else
                {
                    // Actualizar componente existente
                    existingComponent.ItemName = componentBC.ItemName;
                    existingComponent.Warehouse = componentBC.Warehouse;
                    existingComponent.Quantity = componentBC.Quantity;
                    existingComponent.QuantityConsumed = componentBC.QuantityConsumed;
                    existingComponent.Branch = componentBC.Branch;
                    existingComponent.Lot = componentBC.Lot;
                    existingComponent.UnitPrice = componentBC.UnitPrice;
                    existingComponent.status = componentBC.status;
                    existingComponent.AssemblyNo = componentBC.AssemblyNo;
                    existingComponent.TaxCode = componentBC.TaxCode;
                    existingComponent.shortDesc = componentBC.shortDesc;
                    existingComponent.Invima = componentBC.Invima;
                    existingComponent.ExpirationDate = componentBC.ExpirationDate;
                    existingComponent.TraceState = componentBC.TraceState;
                    existingComponent.RSFechaVencimiento = componentBC.RSFechaVencimiento;
                    existingComponent.RSClasifRegistro = componentBC.RSClasifRegistro;

                    await _context.SaveChangesAsync();
                    return existingComponent;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al sincronizar componente: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Sincroniza todos los componentes desde Business Central
        /// </summary>
        /// <param name="location">Ubicación (opcional)</param>
        /// <param name="stock">Stock (opcional)</param>
        /// <param name="salesCode">Código de venta (opcional)</param>
        /// <returns>Lista de componentes sincronizados</returns>
        public async Task<List<EntryRequestComponents>> SincronizarTodosDesdeBCAsync(string? location = null, string? stock = null, string? salesCode = null)
        {
            try
            {
                var componentsBC = await GetComponentsAsync(location, stock, salesCode);
                var componentsSincronizados = new List<EntryRequestComponents>();

                foreach (var component in componentsBC)
                {
                    var componentSincronizado = await SincronizarDesdeBC(component);
                    componentsSincronizados.Add(componentSincronizado);
                }

                return componentsSincronizados;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al sincronizar componentes: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los componentes almacenados en la base de datos local
        /// </summary>
        /// <returns>Lista de componentes locales</returns>
        public async Task<List<EntryRequestComponents>> GetLocalComponentsAsync()
        {
            try
            {
                return await _context.EntryRequestComponents.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener componentes locales: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene un componente específico por ItemNo desde la base de datos local
        /// </summary>
        /// <param name="itemNo">Número del item</param>
        /// <returns>Componente encontrado o null</returns>
        public async Task<EntryRequestComponents> GetLocalComponentByItemNoAsync(string itemNo)
        {
            try
            {
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentException("El número del item no puede ser nulo o vacío");
                }

                return await _context.EntryRequestComponents
                    .FirstOrDefaultAsync(c => c.ItemNo == itemNo);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener componente por ItemNo: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene componentes por EntryRequest ID
        /// </summary>
        /// <param name="idEntryReq">ID del EntryRequest</param>
        /// <returns>Lista de componentes del EntryRequest</returns>
        public async Task<List<EntryRequestComponents>> GetComponentsByEntryRequestAsync(int idEntryReq)
        {
            try
            {
                return await _context.EntryRequestComponents
                    .Where(c => c.IdEntryReq == idEntryReq)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener componentes por EntryRequest: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina un componente de la base de datos local
        /// </summary>
        /// <param name="id">ID del componente a eliminar</param>
        /// <returns>True si se eliminó correctamente</returns>
        public async Task<bool> DeleteLocalComponentAsync(int id)
        {
            try
            {
                var component = await _context.EntryRequestComponents.FindAsync(id);
                if (component == null)
                {
                    return false;
                }

                _context.EntryRequestComponents.Remove(component);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar componente: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene líneas de ensamble desde Business Central
        /// </summary>
        /// <param name="documentNo">Número de documento (opcional)</param>
        /// <returns>Lista de componentes de líneas de ensamble</returns>
        public async Task<List<EntryRequestComponents>> GetAssemblyLinesAsync(string documentNo = null)
        {
            try
            {
                List<EntryRequestComponents> lData = await _bcConn.GetAssemblyLines("lylassemblylines", documentNo);
                return lData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene estadísticas de sincronización
        /// </summary>
        /// <returns>Estadísticas de sincronización</returns>
        public async Task<object> GetSyncStatisticsAsync()
        {
            try
            {
                var totalComponents = await _context.EntryRequestComponents.CountAsync();
                var componentsWithQuantity = await _context.EntryRequestComponents
                    .Where(c => c.Quantity > 0)
                    .CountAsync();
                var componentsWithExpiration = await _context.EntryRequestComponents
                    .Where(c => c.ExpirationDate.HasValue)
                    .CountAsync();

                return new
                {
                    totalComponents = totalComponents,
                    componentsWithQuantity = componentsWithQuantity,
                    componentsWithExpiration = componentsWithExpiration,
                    lastSync = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener estadísticas: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Crea un componente localmente con los datos proporcionados
        /// </summary>
        /// <param name="itemNo">Número del item</param>
        /// <param name="quantity">Cantidad</param>
        /// <param name="idEntryReq">ID del EntryRequest</param>
        /// <param name="assemblyNo">Número de ensamble</param>
        /// <param name="branch">ID de la sucursal</param>
        /// <returns>Componente creado</returns>
        public async Task<EntryRequestComponents> CreateLocalComponentAsync(string itemNo, decimal quantity, int idEntryReq, string assemblyNo, int branch)
        {
            try
            {
                // Validar parámetros requeridos
                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentException("El número del item es requerido");
                }

                if (quantity <= 0)
                {
                    throw new ArgumentException("La cantidad debe ser mayor a 0");
                }

                if (idEntryReq <= 0)
                {
                    throw new ArgumentException("El ID del EntryRequest debe ser mayor a 0");
                }

                if (string.IsNullOrEmpty(assemblyNo))
                {
                    throw new ArgumentException("El número de ensamble es requerido");
                }

                if (branch <= 0)
                {
                    throw new ArgumentException("El ID de la sucursal debe ser mayor a 0");
                }

                // Obtener datos del item desde ItemsBC
                var itemBC = await _context.ItemsBC
                    .FirstOrDefaultAsync(i => i.Code == itemNo);

                if (itemBC == null)
                {
                    throw new ArgumentException($"No se encontró el item con código: {itemNo}");
                }

                // Obtener datos de la sucursal
                var branchData = await _context.Branches
                    .FirstOrDefaultAsync(b => b.Id == branch);

                if (branchData == null)
                {
                    throw new ArgumentException($"No se encontró la sucursal con ID: {branch}");
                }

                // Crear el componente
                var componente = new EntryRequestComponents
                {
                    ItemNo = itemNo,
                    ItemName = itemBC.Name ?? "",
                    Warehouse = branchData.LocationCode ?? "",
                    Quantity = quantity,
                    IdEntryReq = idEntryReq,
                    //SystemId = Guid.NewGuid().ToString(),
                    SystemId = "",
                    QuantityConsumed = 0,
                    Branch = branchData.LocationCode ?? "",
                    Lot = "",
                    UnitPrice = 0,
                    status = "NUEVO",
                    AssemblyNo = assemblyNo,
                    TaxCode = itemBC.TaxCode ?? "",
                    shortDesc = itemBC.ShortDesc ?? "",
                    Invima = itemBC.Invima ?? "",
                    ExpirationDate = new DateTime(2999, 12, 31),
                    TraceState = "",
                    RSFechaVencimiento = null,
                    RSClasifRegistro = ""
                };

                _context.EntryRequestComponents.Add(componente);
                await _context.SaveChangesAsync();

                return componente;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear componente local: {ex.Message}", ex);
            }
        }
    }
} 