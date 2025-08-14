using System.ComponentModel.DataAnnotations;

namespace DimmedAPI.DTOs
{
    public class EntryRequestCancelUpdateDTO
    {
        [Required(ErrorMessage = "El status es requerido")]
        [MaxLength(50, ErrorMessage = "El status no puede exceder 50 caracteres")]
        public string Status { get; set; }

        public int? IdCancelReason { get; set; }

        [MaxLength(200, ErrorMessage = "La razón de cancelación no puede exceder 200 caracteres")]
        public string? CancelReason { get; set; }

        public int? IdCancelDetail { get; set; }

        [MaxLength(200, ErrorMessage = "El detalle de cancelación no puede exceder 200 caracteres")]
        public string? CancelDetail { get; set; }
    }
}
