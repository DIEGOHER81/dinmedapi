using DimmedAPI.BO;
using DimmedAPI.Entidades;

namespace DimmedAPI.Services
{
    public interface IDynamicBCConnectionService
    {
        Task<IntBCConex> GetBCConnectionAsync(string companyCode);
        Task<(string urlWS, string url, string company)> GetBusinessCentralConfigAsync(string companyCode);
    }
} 