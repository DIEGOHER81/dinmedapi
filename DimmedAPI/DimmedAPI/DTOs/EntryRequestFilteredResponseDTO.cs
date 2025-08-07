using DimmedAPI.Entidades;

namespace DimmedAPI.DTOs
{
    public class EntryRequestFilteredResponseDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Service { get; set; }
        public int IdOrderType { get; set; }
        public string? DeliveryPriority { get; set; }
        public int IdCustomer { get; set; }
        public int InsurerType { get; set; }
        public int? Insurer { get; set; }
        public int? IdMedic { get; set; }
        public int? IdPatient { get; set; }
        public string? Applicant { get; set; }
        public int? IdATC { get; set; }
        public string? LimbSide { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? OrderObs { get; set; }
        public int SurgeryTime { get; set; }
        public DateTime? SurgeryInit { get; set; }
        public DateTime? SurgeryEnd { get; set; }
        public string? Status { get; set; }
        public int? IdTraceStates { get; set; }
        public int? BranchId { get; set; }
        public TimeSpan? SurgeryInitTime { get; set; }
        public TimeSpan? SurgeryEndTime { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? PurchaseOrder { get; set; }
        public bool? AtcConsumed { get; set; }
        public bool? IsSatisfied { get; set; }
        public string? Observations { get; set; }
        public string? obsMaint { get; set; }
        public int? AuxLog { get; set; }
        public int? IdCancelReason { get; set; }
        public int? IdCancelDetail { get; set; }
        public string? CancelReason { get; set; }
        public string? CancelDetail { get; set; }
        public bool? Notification { get; set; }
        public bool? IsReplacement { get; set; }
        public bool? AssemblyComponents { get; set; }
        public string? priceGroup { get; set; }
        
        // Relación con Customer
        public Customer? Customer { get; set; }
        
        // Relación con Patient
        public Patient? Patient { get; set; }
        
        // Relación con CustomerAddress (colección de direcciones del cliente)
        public ICollection<CustomerAddress>? CustomerAddresses { get; set; }
    }
} 