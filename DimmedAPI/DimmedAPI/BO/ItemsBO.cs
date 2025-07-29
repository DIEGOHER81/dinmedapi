using DimmedAPI.Entidades;
using DimmedAPI.BO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DimmedAPI.DTOs;

namespace DimmedAPI.BO
{
    public class ItemsBO
    {
        private readonly ApplicationDBContext _context;
        private readonly IntBCConex _bcConn;

        public ItemsBO(ApplicationDBContext context, IntBCConex bcConn)
        {
            _context = context;
            _bcConn = bcConn;
        }

        /// <summary>
        /// Obtiene todos los artículos desde Business Central
        /// </summary>
        /// <returns>Lista de artículos de BC</returns>
        public async Task<List<ItemsBC>> getItems()
        {
            try
            {
                List<ItemsBC> lData = await _bcConn.GetItems("lylitemsall");
                return lData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene artículos desde Business Central con parámetros opcionales
        /// </summary>
        /// <param name="take">Número máximo de registros a obtener</param>
        /// <param name="code">Código específico del artículo</param>
        /// <returns>Lista de artículos de BC</returns>
        public async Task<List<ItemsBC>> GetItemsFromBCAsync(int? take = null, string code = null)
        {
            try
            {
                var items = await _bcConn.GetItems("lylitemsall");
                
                if (!string.IsNullOrEmpty(code))
                {
                    items = items.Where(i => i.Code == code).ToList();
                }

                if (take.HasValue && take.Value > 0)
                {
                    items = items.Take(take.Value).ToList();
                }

                return items;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener artículos desde BC: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Sincroniza un artículo específico desde Business Central
        /// </summary>
        /// <param name="itemBC">Artículo de BC a sincronizar</param>
        /// <returns>Artículo sincronizado</returns>
        public async Task<ItemsBC> SincronizarDesdeBC(ItemsBC itemBC)
        {
            try
            {
                if (itemBC == null)
                {
                    throw new ArgumentException("El artículo no puede ser nulo");
                }

                // Buscar si el artículo ya existe por código
                var existingItem = await _context.ItemsBC
                    .FirstOrDefaultAsync(i => i.Code == itemBC.Code);

                if (existingItem == null)
                {
                    // Crear nuevo artículo
                    var newItem = new ItemsBC
                    {
                        Code = itemBC.Code,
                        Name = itemBC.Name,
                        PriceList = itemBC.PriceList,
                        TaxCode = itemBC.TaxCode,
                        ShortDesc = itemBC.ShortDesc,
                        Invima = itemBC.Invima
                    };

                    _context.ItemsBC.Add(newItem);
                    await _context.SaveChangesAsync();
                    
                    // Asignar el ID generado
                    newItem.Id = newItem.Id;
                    return newItem;
                }
                else
                {
                    // Actualizar artículo existente
                    existingItem.Name = itemBC.Name;
                    existingItem.PriceList = itemBC.PriceList;
                    existingItem.TaxCode = itemBC.TaxCode;
                    existingItem.ShortDesc = itemBC.ShortDesc;
                    existingItem.Invima = itemBC.Invima;

                    await _context.SaveChangesAsync();
                    return existingItem;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al sincronizar artículo: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Sincroniza todos los artículos desde Business Central
        /// </summary>
        /// <returns>Lista de artículos sincronizados</returns>
        public async Task<List<ItemsBC>> SincronizeBCAsync()
        {
            try
            {
                var itemsBC = await getItems();
                var itemsSincronizados = new List<ItemsBC>();

                foreach (var item in itemsBC)
                {
                    var itemSincronizado = await SincronizarDesdeBC(item);
                    itemsSincronizados.Add(itemSincronizado);
                }

                return itemsSincronizados;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al sincronizar artículos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los artículos almacenados en la base de datos local
        /// </summary>
        /// <returns>Lista de artículos locales</returns>
        public async Task<List<ItemsBC>> GetLocalItemsAsync()
        {
            try
            {
                return await _context.ItemsBC.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener artículos locales: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene un artículo específico por código desde la base de datos local
        /// </summary>
        /// <param name="code">Código del artículo</param>
        /// <returns>Artículo encontrado o null</returns>
        public async Task<ItemsBC> GetLocalItemByCodeAsync(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    throw new ArgumentException("El código del artículo no puede ser nulo o vacío");
                }

                return await _context.ItemsBC
                    .FirstOrDefaultAsync(i => i.Code == code);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener artículo por código: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina un artículo de la base de datos local
        /// </summary>
        /// <param name="code">Código del artículo a eliminar</param>
        /// <returns>True si se eliminó correctamente</returns>
        public async Task<bool> DeleteLocalItemAsync(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    throw new ArgumentException("El código del artículo no puede ser nulo o vacío");
                }

                var item = await _context.ItemsBC
                    .FirstOrDefaultAsync(i => i.Code == code);

                if (item == null)
                {
                    return false;
                }

                _context.ItemsBC.Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar artículo: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene estadísticas de sincronización
        /// </summary>
        /// <returns>Estadísticas de artículos</returns>
        public async Task<object> GetSyncStatisticsAsync()
        {
            try
            {
                var totalLocal = await _context.ItemsBC.CountAsync();
                var itemsBC = await getItems();
                var totalBC = itemsBC.Count;

                return new
                {
                    totalLocal = totalLocal,
                    totalBC = totalBC,
                    diferencia = totalBC - totalLocal
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener estadísticas: {ex.Message}", ex);
            }
        }

        public async Task<List<ItemsBCWithPriceListDTO>> getItemsWithPriceList(int? take = null)
        {
            try
            {
                var lData = await _bcConn.GetItemsWithPriceList(take);
                return lData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ItemsBCWithPriceList>> SincronizarItemsBCWithPriceListAsync(int? take = null, bool limpiarTabla = true)
        {
            // Traer los datos desde BC
            var itemsFromBC = await _bcConn.GetItemsWithPriceList(take);
            if (itemsFromBC == null)
                return new List<ItemsBCWithPriceList>();

            // Limpiar la tabla local solo si se solicita
            if (limpiarTabla)
            {
                var allLocal = _context.ItemsBCWithPriceList.ToList();
                if (allLocal.Any())
                {
                    _context.ItemsBCWithPriceList.RemoveRange(allLocal);
                    await _context.SaveChangesAsync();
                }
            }

            // Mapear y guardar los nuevos registros
            var entities = itemsFromBC.Select(dto => new ItemsBCWithPriceList
            {
                No = dto.No,
                Description = dto.Description,
                BaseUnitMeasure = dto.BaseUnitMeasure,
                UnitCost = dto.UnitCost,
                PriceIncludesVAT = dto.PriceIncludesVAT,
                SalesCode = dto.SalesCode,
                UnitPrice = dto.UnitPrice,
                UnitMeasureCode = dto.UnitMeasureCode,
                AuxiliaryIndex1 = dto.AuxiliaryIndex1,
                AuxiliaryIndex2 = dto.AuxiliaryIndex2,
                AuxiliaryIndex3 = dto.AuxiliaryIndex3
            }).ToList();

            _context.ItemsBCWithPriceList.AddRange(entities);
            await _context.SaveChangesAsync();
            return entities;
        }
    }
} 