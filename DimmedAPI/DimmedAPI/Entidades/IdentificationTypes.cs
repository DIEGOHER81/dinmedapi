using DimmedAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace DimmedAPI.Entidades
{
    public class IdentificationTypes
    {
        public int Id { get; set; }
        [Required (ErrorMessage = "El Campo {0} es requerido")]
        [StringLength(5, ErrorMessage ="El Campo {0} debe tener mínimo {1} caracteres o menos ")]
        public required string Code { get; set; }
        [PrimeraLetra]
        public required string Name { get; set; }

        /*
        public long? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public AppUser? CreatedByUser { get; set; }
        public AppUser? ModifiedByUser { get; set; }
        */

    }
}
