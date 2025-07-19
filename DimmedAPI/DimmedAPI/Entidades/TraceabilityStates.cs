using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    public class TraceabilityStates
    {
        public TraceabilityStates()
        {
            EntryRequests = new HashSet<EntryRequests>();
        }
        public int Id { get; set; }
        [Display(Name = "Descripción")]
        [MaxLength(200, ErrorMessage = "El atributo {0} solo puede contener {1} caracteres.")]
        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        public string Description { get; set; }
        public string Type { get; set; }
        [NotMapped]
        public string Name { get; set; }
        public virtual ICollection<EntryRequests> EntryRequests { get; set; }
        public virtual ICollection<EntryRequestTraceStates> EntryRequestTraceStates { get; set; }
    }
}
