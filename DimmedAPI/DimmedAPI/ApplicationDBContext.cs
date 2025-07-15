using DimmedAPI.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DimmedAPI
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<InsurerType> InsurerType { get; set; }
        public DbSet<Insurer> Insurer { get; set; }
        public DbSet<Companies> Companies { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Medic> Medic { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<CancelDetails> CancelDetails { get; set; }
        public DbSet<CancellationReasons> CancellationReasons { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<UserBranch> UserBranch { get; set; }

        public DbSet<Patient> Patient { get; set; }

        public DbSet<CustomerContact> CustomerContact { get; set; }

        public DbSet<ItemsBC> ItemsBC { get; set; }
        public DbSet<ItemsBCWithPriceList> ItemsBCWithPriceList { get; set; }

        public DbSet<ClienteLead> ClienteLead { get; set; }

        public DbSet<QuotationMaster> QuotationMaster { get; set; }
        public DbSet<QuotationDetail> QuotationDetail { get; set; }
        public DbSet<QuotationType> QuotationType { get; set; }
        public DbSet<CommercialCondition> CommercialCondition { get; set; }
        public DbSet<CustomerType> CustomerType { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<FollowUpQuotations> FollowUpQuotations { get; set; }

        public DbSet<PaymentTerm> PaymentTerm { get; set; }

        public DbSet<EntryRequestComponents> EntryRequestComponents  { get; set; }

        public DbSet<IdentificationTypes> IdentificationTypes { get; set; }

        public DbSet<Options> Options { get; set; }


    }
}
