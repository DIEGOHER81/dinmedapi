#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    public class Options
    {
       [Key]
        public int Id { get; set; }

        public string? Text { get; set; }

        public bool? IsActive { get; set; }

        public string? Path { get; set; }

        public int? IOrder { get; set; }

        // Este valor representa el IOrder del padre
        public int? Parent { get; set; }

        public string? Icon { get; set; }

        public bool? IsParent { get; set; }

        // Navegación manual (no se puede usar ForeignKey en este caso)
        [NotMapped]
        public Options? ParentOption { get; set; }

        [NotMapped]
        public ICollection<Options>? Children { get; set; }
    }
}
