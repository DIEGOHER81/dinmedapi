#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    public class CustomerType
    {
        [Key]
        public int Id { get; set; }

        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        // Relación inversa (opcional)
        public ICollection<QuotationMaster>? Quotations { get; set; }
    }
}
