using System.ComponentModel.DataAnnotations;

namespace DimmedAPI.DTOs
{
    public class CancelDetailsUpdateDTO
    {
        [Required(ErrorMessage = "La descripción es requerida")]
        [MaxLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string Description { get; set; }

        public int? IdRelation { get; set; }
    }
} 