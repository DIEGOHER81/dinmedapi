#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DimmedAPI.Entidades
{
    public class ClienteLead
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(IdentificationType))]
        public int TipoIdentificacion { get; set; }

        [Required]
        public string NumeroIdentificacion { get; set; } = null!;

        [Required]
        public string RazonSocial { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string? Telefono { get; set; }

        public string? Ciudad { get; set; }

        public string? Direccion { get; set; }

        public string? RepresentanteComercial { get; set; }

        public string? PriceGroup { get; set; }

        public string? Observaciones { get; set; }

        public string? Horario_LV { get; set; }

        public string? Horario_SD { get; set; }

        // Navegación
        public IdentificationTypes? IdentificationType { get; set; }
    }
}
