using DimmedAPI.Entidades;

namespace DimmedAPI.DTOs
{
    public class EntryRequestResponseDTO
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

        // Propiedades de navegación
        public virtual Insurer? InsurerNavigation { get; set; }
        public virtual InsurerType? InsurerTypeNavigation { get; set; }
        public virtual Branch? Branch { get; set; }
        public virtual Customer? IdCustomerNavigation { get; set; }
        public virtual Medic? IdMedicNavigation { get; set; }
        public virtual Patient? IdPatientNavigation { get; set; }
        public virtual TraceabilityStates? IdTraceStatesNavigation { get; set; }
        public virtual Employee? IdAtcNavigation { get; set; }

        // Propiedades calculadas
        public string? CustomerName { get; set; }
        public string? PatientName { get; set; }
        public string? MedicName { get; set; }
        public string? NoRequest { get; set; }
        public string? sEquipments { get; set; }
        public string? sAssembly { get; set; }
    }
} 