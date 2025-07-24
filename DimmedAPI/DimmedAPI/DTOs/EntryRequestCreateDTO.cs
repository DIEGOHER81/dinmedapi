using System.ComponentModel.DataAnnotations;

namespace DimmedAPI.DTOs
{
    public class EntryRequestCreateDTO
    {
        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime Date { get; set; }

        [MaxLength(100, ErrorMessage = "El servicio no puede exceder 100 caracteres")]
        public string? Service { get; set; }

        public int IdOrderType { get; set; }

        [MaxLength(100, ErrorMessage = "La prioridad de entrega no puede exceder 100 caracteres")]
        public string? DeliveryPriority { get; set; }

        public int IdCustomer { get; set; }

        public int InsurerType { get; set; }

        public int Insurer { get; set; }

        public int? IdMedic { get; set; }

        public int? IdPatient { get; set; }

        [MaxLength(100, ErrorMessage = "El solicitante no puede exceder 100 caracteres")]
        public string? Applicant { get; set; }

        public int? IdATC { get; set; }

        [MaxLength(50, ErrorMessage = "La extremidad del paciente no puede exceder 50 caracteres")]
        public string? LimbSide { get; set; }

        public DateTime DeliveryDate { get; set; }

        [MaxLength(200, ErrorMessage = "Las observaciones del pedido no pueden exceder 200 caracteres")]
        public string? OrderObs { get; set; }

        public int SurgeryTime { get; set; }

        public DateTime? SurgeryInit { get; set; }

        public DateTime? SurgeryEnd { get; set; }

        public string? Status { get; set; }

        public int? IdTraceStates { get; set; }

        public int? BranchId { get; set; }

        public TimeSpan? SurgeryInitTime { get; set; }

        public TimeSpan? SurgeryEndTime { get; set; }

        [MaxLength(1000, ErrorMessage = "La dirección de entrega no puede exceder 1000 caracteres")]
        public string? DeliveryAddress { get; set; }

        [MaxLength(1000, ErrorMessage = "La orden de compra no puede exceder 1000 caracteres")]
        public string? PurchaseOrder { get; set; }

        public bool? AtcConsumed { get; set; }

        public bool? IsSatisfied { get; set; }

        [MaxLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
        public string? Observations { get; set; }

        [MaxLength(1000, ErrorMessage = "Las observaciones de mantenimiento no pueden exceder 1000 caracteres")]
        public string? obsMaint { get; set; }

        public int? AuxLog { get; set; }

        public int? IdCancelReason { get; set; }

        public int? IdCancelDetail { get; set; }

        [MaxLength(200, ErrorMessage = "La razón de cancelación no puede exceder 200 caracteres")]
        public string? CancelReason { get; set; }

        [MaxLength(200, ErrorMessage = "El detalle de cancelación no puede exceder 200 caracteres")]
        public string? CancelDetail { get; set; }

        public bool? Notification { get; set; }

        public bool? IsReplacement { get; set; }

        public bool? AssemblyComponents { get; set; }

        [MaxLength(50, ErrorMessage = "El grupo de precios no puede exceder 50 caracteres")]
        public string? priceGroup { get; set; }
    }
} 