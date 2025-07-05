using System.ComponentModel.DataAnnotations;

namespace DimmedAPI.DTOs
{
    public class QuotationDetailUpdateDTO
    {
        [Required]
        public int Fk_IdQuotationMasterId { get; set; }

        [Required]
        public char ProductType { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El código de producto no puede tener más de 50 caracteres")]
        public string CodProduct { get; set; } = null!;

        [Required]
        [StringLength(20, ErrorMessage = "La unidad no puede tener más de 20 caracteres")]
        public string Unit { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int? Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        public double? Price { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje de impuesto debe estar entre 0 y 100")]
        public double? PorcTax { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El valor del impuesto debe ser mayor o igual a 0")]
        public double? TaxValue { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El tiempo de contrato debe ser mayor a 0")]
        public int? ContractTime { get; set; }

        [StringLength(100, ErrorMessage = "El período de garantía no puede tener más de 100 caracteres")]
        public string? WarrantyPeriod { get; set; }
    }
} 