using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    [Table("Customer")]
    public class Customer
    {
        public Customer()
        {
            ShipAddress = new HashSet<CustomerAddress>();
        }
        public int Id { get; set; }
        public string? Identification { get; set; }
        public int IdType { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool CertMant { get; set; }
        public bool RemCustomer { get; set; }
        public string? Observations { get; set; }
        public string? Name { get; set; }
        public string? SystemIdBc { get; set; }
        public string? SalesZone { get; set; }
        public string? TradeRepres { get; set; }
        public int? NoCopys { get; set; }
        public bool IsActive { get; set; }
        public string? Segment { get; set; }
        public string? No { get; set; }
        public string? FullName { get; set; }
        public string? PriceGroup { get; set; }
        public bool ShortDesc { get; set; }
        public bool ExIva { get; set; }
        public bool? IsSecondPriceList { get; set; }
        public string? SecondPriceGroup { get; set; }
        public string? InsurerType { get; set; }
        public bool? IsRemLot { get; set; }
        public string? LyLOpeningHours1 { get; set; }
        public string? LyLOpeningHours2 { get; set; }
        public string? PaymentMethodCode { get; set; }
        public string? PaymentTermsCode { get; set; }

        // Relación con CustomerAddress (uno-a-muchos)
        public virtual ICollection<CustomerAddress> ShipAddress { get; set; }

        // Propiedad no mapeada a la base de datos (uso en vista o lógica interna)
        //[NotMapped]
        // public ICollection<CustomerContact> CustomerContacts { get; set; }

        // Relación con EntryRequests (uno-a-muchos)
        //public virtual ICollection<EntryRequests> EntryRequests { get; set; }
    }
}
