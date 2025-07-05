using DimmedAPI.Entidades;

namespace DimmedAPI.Services
{
    public interface IDynamicConnectionService
    {
        Task<ApplicationDBContext> GetCompanyDbContextAsync(string companyCode);
        Task<(string urlWS, string url, string company)> GetBusinessCentralConfigAsync(string companyCode);
        Task<Companies?> GetCompanyByCodeAsync(string companyCode);
    }
} 