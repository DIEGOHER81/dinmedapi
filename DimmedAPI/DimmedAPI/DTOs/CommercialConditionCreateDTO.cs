using System.ComponentModel.DataAnnotations;

namespace DimmedAPI.DTOs
{
    public class CommercialConditionCreateDTO
    {
        [Required]
        [StringLength(200, ErrorMessage = "La descripción no puede tener más de 200 caracteres")]
        public string Description { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "El texto comercial no puede tener más de 1000 caracteres")]
        public string? CommercialText { get; set; }

        public bool IsActive { get; set; } = true;
    }
} 