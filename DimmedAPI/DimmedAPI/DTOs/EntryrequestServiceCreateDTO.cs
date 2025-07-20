using System.ComponentModel.DataAnnotations;

namespace DimmedAPI.DTOs
{
    public class EntryrequestServiceCreateDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "La descripción no puede tener más de 100 caracteres")]
        public string Description { get; set; } = null!;

        public bool? IsActive { get; set; } = true;
    }
} 