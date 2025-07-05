using System.ComponentModel.DataAnnotations;

namespace DimmedAPI.DTOs
{
    public class CustomerTypeCreateDTO
    {
        [Required]
        [StringLength(200, ErrorMessage = "La descripción no puede tener más de 200 caracteres")]
        public string Description { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
} 