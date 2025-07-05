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

        public DbSet<ClienteLead> ClienteLead { get; set; }



    }
}
