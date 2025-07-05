using DimmedAPI.BO;
using DimmedAPI.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DimmedAPI.Services
{
    public class DynamicBCConnectionService : IDynamicBCConnectionService
    {
        private readonly ApplicationDBContext _defaultContext;
        private readonly IConfiguration _configuration;

        public DynamicBCConnectionService(ApplicationDBContext defaultContext, IConfiguration configuration)
        {
            _defaultContext = defaultContext;
            _configuration = configuration;
        }

        public async Task<Companies?> GetCompanyByCodeAsync(string companyCode)
        {
            return await _defaultContext.Companies
                .FirstOrDefaultAsync(c => c.BCCodigoEmpresa == companyCode);
        }

        public async Task<IntBCConex> GetBCConnectionAsync(string companyCode)
        {
            var company = await GetCompanyByCodeAsync(companyCode);
            
            if (company == null)
            {
                throw new ArgumentException($"Compañía con código {companyCode} no encontrada");
            }

            // Crear una configuración dinámica para esta compañía
            var dynamicConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["BusinessCentral:url"] = company.BCURL,
                    ["BusinessCentral:company"] = company.BCCodigoEmpresa,
                    ["AzureAd:ClientId"] = _configuration["AzureAd:ClientId"],
                    ["AzureAd:ClientSecret"] = _configuration["AzureAd:ClientSecret"],
                    ["AzureAd:TenantId"] = _configuration["AzureAd:TenantId"]
                })
                .Build();

            return new bcConn(dynamicConfig);
        }

        public async Task<(string urlWS, string url, string company)> GetBusinessCentralConfigAsync(string companyCode)
        {
            var company = await GetCompanyByCodeAsync(companyCode);
            
            if (company == null)
            {
                throw new ArgumentException($"Compañía con código {companyCode} no encontrada");
            }

            return (company.BCURLWebService, company.BCURL, company.BCCodigoEmpresa);
        }
    }
} 