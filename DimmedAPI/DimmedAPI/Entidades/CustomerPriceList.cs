using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    [Table("CustomerPriceList")]
    public class CustomerPriceList
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? Identification { get; set; }
        public string? Name { get; set; }
        public string? InsurerType { get; set; }
        public string? PriceGroup { get; set; }
    }
}
