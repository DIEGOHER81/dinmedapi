using DimmedAPI.DTOs;
using DimmedAPI.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DimmedAPI.Interfaces
{
    public interface IEquipmentBO
    {
        Task<List<Equipment>> SincronizeBCAsync();
        Task<object> SincronizarDesdeBC(EquipmentBCDTO dto);
        Task<List<Equipment>> GetEquipmentsFromBCAsync(int? take = null, string systemIdBc = null, string code = null);
        Task<Equipment> GetEquipmentByCode(string code);
        Task<List<Equipment>> GetAllEquipmentCodes(int? take = null);
        Task<List<EntryRequestAssembly>> getAInventory(int equipmentId);
        Task<List<EntryRequestAssembly>> getAInventory(string code);
        Task<List<EntryRequestAssembly>> getAInventory(string code, string locationCode);
        Task<List<EntryRequestAssembly>> getAInventory(string code, string branchCode, string locationCode);
    }
} 