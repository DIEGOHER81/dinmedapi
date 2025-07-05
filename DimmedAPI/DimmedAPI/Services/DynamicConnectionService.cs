using DimmedAPI.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DimmedAPI.Services
{
    public class DynamicConnectionService : IDynamicConnectionService
    {
        private readonly ApplicationDBContext _defaultContext;
        private readonly IConfiguration _configuration;

        public DynamicConnectionService(ApplicationDBContext defaultContext, IConfiguration configuration)
        {
            _defaultContext = defaultContext;
            _configuration = configuration;
        }

        public async Task<Companies?> GetCompanyByCodeAsync(string companyCode)
        {
            return await _defaultContext.Companies
                .FirstOrDefaultAsync(c => c.BCCodigoEmpresa == companyCode);
        }

        public async Task<ApplicationDBContext> GetCompanyDbContextAsync(string companyCode)
        {
            var company = await GetCompanyByCodeAsync(companyCode);
            
            if (company == null)
            {
                throw new ArgumentException($"Compañía con código {companyCode} no encontrada");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDBContext>();
            optionsBuilder.UseSqlServer(company.SqlConnectionString);

            return new ApplicationDBContext(optionsBuilder.Options);
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