using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    [Table("Insurer")]
    public class Insurer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(20)]
        public string Nit { get; set; } = null!;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [Column("InsurerType")] // Este es el nombre real en la base de datos
        public int? InsurerTypeId { get; set; }

        [ForeignKey("InsurerTypeId")]
        public virtual InsurerType? InsurerType { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
