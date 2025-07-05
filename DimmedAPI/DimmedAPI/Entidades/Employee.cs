#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DimmedAPI.Entidades
{
    public class Employee
    {

        [Key]
        public int Id { get; set; }

        public string? Code { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Charge { get; set; }

        public string? SystemIdBC { get; set; }

        public bool? ATC { get; set; }

        public bool? MResponsible { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Branch { get; set; }

        // Relación inversa (opcional)
        public ICollection<QuotationMaster>? Quotations { get; set; }

    }
}
