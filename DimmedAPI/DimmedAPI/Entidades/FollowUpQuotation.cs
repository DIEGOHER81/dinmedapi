#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    public class FollowUpQuotations
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Quotation))]
        public int Fk_IdQuotation { get; set; }

        [Required]
        [ForeignKey(nameof(Employee))]
        public int Fk_IdEmployee { get; set; }

        [Required]
        public string Observation { get; set; } = null!;

        public DateTime? CreateDateTime { get; set; }

        // Propiedades de navegación
        public QuotationMaster? Quotation { get; set; }

        public Employee? Employee { get; set; }
    }
}
