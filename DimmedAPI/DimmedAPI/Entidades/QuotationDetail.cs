using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DimmedAPI.Entidades
{
    public class QuotationDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(QuotationMaster))]
        public int Fk_IdQuotationMasterId { get; set; }

        [Required]
        public char ProductType { get; set; }

        [Required]
        public string CodProduct { get; set; } = null!;

        [Required]
        public string Unit { get; set; } = null!;

        public int? Quantity { get; set; }

        public double? Price { get; set; }

        public double? PorcTax { get; set; }

        public double? TaxValue { get; set; }

        public int? ContractTime { get; set; }

        public string? WarrantyPeriod { get; set; }

        // Propiedad de navegación
        public QuotationMaster? QuotationMaster { get; set; }
    }
}
