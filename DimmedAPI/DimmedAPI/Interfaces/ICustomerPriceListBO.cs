using DimmedAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DimmedAPI.Interfaces
{
    public interface ICustomerPriceListBO
    {
        /// <summary>
        /// SUPLE TI: Consulta de listas de precio por id de cliente
        /// </summary>
        /// <param name="customerId">Id del cliente</param>
        /// <returns>Lista de precios asociadas al cliente</returns>
        Task<List<CustomerPriceListResponseDTO>> GetPriceListByCustomerIdAsync(int customerId);
    }
}
