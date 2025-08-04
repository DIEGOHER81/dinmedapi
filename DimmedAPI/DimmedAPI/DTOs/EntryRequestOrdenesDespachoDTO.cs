using System;

namespace DimmedAPI.DTOs
{
    public class EntryRequestOrdenesDespachoDTO
    {
        public int? Id { get; set; }
        public DateTime? Date { get; set; }
        public string? Service { get; set; }
        public int? IdOrderType { get; set; }
        public string? DeliveryPriority { get; set; }
        public int? IdCustomer { get; set; }
        public int? InsurerType { get; set; }
        public int? Insurer { get; set; }
        public int? IdMedic { get; set; }
        public int? IdPatient { get; set; }
        public string? Applicant { get; set; }
        public int? IdATC { get; set; }
        public string? LimbSide { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? OrderObs { get; set; }
        public int? SurgeryTime { get; set; }
        public DateTime? SurgeryInit { get; set; }
        public DateTime? SurgeryEnd { get; set; }
        public string? Status { get; set; }
        public int? IdTraceStates { get; set; }
        public int? SurgeryInitTime { get; set; }
        public int? SurgeryEndTime { get; set; }
        public int? BranchId { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? PurchaseOrder { get; set; }
        public bool? AtcConsumed { get; set; }
        public bool? IsSatisfied { get; set; }
        public string? Observations { get; set; }
        public int? AuxLog { get; set; }
        public bool? Notification { get; set; }
        public int? IdCancelReason { get; set; }
        public string? CancelReason { get; set; }
        public int? IdCancelDetail { get; set; }
        public string? CancelDetail { get; set; }
        public bool? IsReplacement { get; set; }
        public bool? AssemblyComponents { get; set; }
        public string? obsMaint { get; set; }
        public string? priceGroup { get; set; }
        public string? traceState { get; set; }
        public string? CustomerName { get; set; }
        public string? Contact { get; set; }
        public string? PatientName { get; set; }
        public string? MedicName { get; set; }
        public string? InsurerName { get; set; }
        public string? InsurerTypeName { get; set; }
        public string? Branch { get; set; }
        public bool? IsRemLot { get; set; }
        public string? ATCName { get; set; }
        public string? equipos { get; set; }
        public string? Componentes { get; set; }
        public string? RemCustomer { get; set; }
        public string? ShortDesc { get; set; }
    }
} 