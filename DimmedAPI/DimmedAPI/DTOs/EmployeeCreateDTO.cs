using System.ComponentModel.DataAnnotations;

namespace DimmedAPI.DTOs
{
    public class EmployeeCreateDTO
    {
        [StringLength(50, ErrorMessage = "El código no puede tener más de 50 caracteres")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede tener más de 200 caracteres")]
        public string Name { get; set; } = null!;

        [StringLength(100, ErrorMessage = "El cargo no puede tener más de 100 caracteres")]
        public string? Charge { get; set; }

        [StringLength(50, ErrorMessage = "El SystemIdBC no puede tener más de 50 caracteres")]
        public string? SystemIdBC { get; set; }

        public bool? ATC { get; set; }

        public bool? MResponsible { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede tener más de 20 caracteres")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        public string? Phone { get; set; }

        [StringLength(100, ErrorMessage = "El email no puede tener más de 100 caracteres")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "La sucursal no puede tener más de 100 caracteres")]
        public string? Branch { get; set; }
    }
} 