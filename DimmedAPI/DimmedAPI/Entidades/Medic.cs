using System.ComponentModel.DataAnnotations;

namespace DimmedAPI.Entidades
{
    public class Medic
    {
        
        public int Id { get; set; }
        [Display(Name = "Identificacion")]
        [MaxLength(20, ErrorMessage = "El atributo {0} solo puede contener {1} caracteres.")]
        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        public string? Identification { get; set; }
        [Display(Name = "Nombre")]
        [MaxLength(200, ErrorMessage = "El atributo {0} solo puede contener {1} caracteres.")]
        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        public string? Name { get; set; }
        [Display(Name = "Tipo Identificación")]
        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        public int IdType { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        [RegularExpression("^[0-9]{8,11}$", ErrorMessage = "Teléfono invalido, solo numeros.")]
        public string? Phone { get; set; }
        [Display(Name = "Correo Electrónico")]
        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        [EmailAddress(ErrorMessage = "Formato de correo invalido.")]
        public string? Email { get; set; }
        public string? Observations { get; set; }
        public bool IsTemporal { get; set; }
        public string? TradeRepres { get; set; }
        public string? Segment { get; set; }
        public bool IsActive { get; set; }
        public string? Gender { get; set; }
        public string? Speciality { get; set; }
        public string? LastName { get; set; }
        //public virtual ICollection<EntryRequests> EntryRequests { get; set; }
        //public virtual ICollection<SpecialSurgeries> SpecialSurgeries { get; set; }
    }
}
