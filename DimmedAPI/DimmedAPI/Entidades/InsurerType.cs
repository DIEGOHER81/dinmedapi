using DimmedAPI.Validaciones;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    public class InsurerType
    {
        public int Id { get; set; }
        [Required (ErrorMessage = "El Campo {0} es requerido")]
        [StringLength(200, ErrorMessage ="El Campo {0} debe tener máximo {1} caracteres")]
        [PrimeraLetra]

        [Column("Name")] // Este es el nombre real en la base de datos
        public required string description { get; set; }
        public bool isActive { get; set; } = true;


    }
}
