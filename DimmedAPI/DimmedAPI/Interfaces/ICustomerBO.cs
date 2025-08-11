using DimmedAPI.DTOs;
using DimmedAPI.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DimmedAPI.Interfaces
{
    public interface ICustomerBO
    {
        Task<List<Customer>> SincronizeBCAsync();
        Task<object> SincronizarDesdeBC(CustomerBCDTO dto);
        Task<List<Customer>> GetCustomersFromBCAsync(int? take = null, string systemIdBc = null);
        Task<Customer?> GetByIdAsync(int id);
    }
}
