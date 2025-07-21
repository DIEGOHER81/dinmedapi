using DimmedAPI.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DimmedAPI.Interfaces
{
    public interface ICustomerAddressBO
    {
        Task<CustomerAddress> UpdateAddFromBC(string systemID, int option);
        Task<CustomerAddress> getCustAddrBCAsync(string method, string systemID);
        Task<List<CustomerAddress>> GetAllAsync();
        Task<CustomerAddress> GetByIdAsync(int id);
        Task<List<CustomerAddress>> GetCustAddrListAsync(string method);
        Task<List<CustomerAddress>> SinCustAddressBCAsync();
        Task<List<CustomerAddress>> SyncAllFromBCAsync();
    }
} 