using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DimmedAPI.Entidades
{
    public class CancellationReasons
    {
        public CancellationReasons()
        {
            CancelDetails = new HashSet<CancelDetails>();
        }
        public int Id { get; set; }
        [Display(Name = "Descripción")]
        [MaxLength(200, ErrorMessage = "El atributo {0} solo puede contener {1} caracteres.")]
        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        public string Description { get; set; }
        [NotMapped]
        public string Name { get; set; }
        public virtual ICollection<CancelDetails> CancelDetails { get; set; }
    }
}
