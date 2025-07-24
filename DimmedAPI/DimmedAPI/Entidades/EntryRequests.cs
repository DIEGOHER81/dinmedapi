using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    public class EntryRequests
    {
        public EntryRequests()
        {
            EntryRequestAssembly = new HashSet<EntryRequestAssembly>();
            EntryRequestComponents = new HashSet<EntryRequestComponents>();
            EntryRequestDetails = new HashSet<EntryRequestDetails>();
            EntryRequestHistory = new HashSet<EntryRequestHistory>();
            EntryRequestTraceStates = new HashSet<EntryRequestTraceStates>();
        }
        public int Id { get; set; }
        [Display(Name = "Fecha y hora de solicitud")]
        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        public DateTime Date { get; set; }
        [Display(Name = "Servicio")]
        [MaxLength(100, ErrorMessage = "El atributo {0} solo puede contener {1} caracteres.")]
        public string? Service { get; set; }
        [Display(Name = "Tipo de Pedido")]
        public int IdOrderType { get; set; }
        [Display(Name = "Entrega")]
        [MaxLength(100, ErrorMessage = "El atributo {0} solo puede contener {1} caracteres.")]
        public string? DeliveryPriority { get; set; }
        [Display(Name = "Cliente")]
        public int IdCustomer { get; set; }
        [Display(Name = "Tipo Aseguradora")]
        public int InsurerType { get; set; }
        [Display(Name = "Aseguradora")]
        public int? Insurer { get; set; }
        [Display(Name = "Medico")]
        public int? IdMedic { get; set; }
        [Display(Name = "Paciente")]
        public int? IdPatient { get; set; }
        [Display(Name = "Solicitante")]
        [MaxLength(100, ErrorMessage = "El atributo {0} solo puede contener {1} caracteres.")]
        public string? Applicant { get; set; }
        [Display(Name = "ATC")]
        public int? IdATC { get; set; }
        [Display(Name = "Extremidad Paciente")]
        [MaxLength(50, ErrorMessage = "El atributo {0} solo puede contener {1} caracteres.")]
        public string? LimbSide { get; set; }
        [Display(Name = "Fecha Despacho")]
        public DateTime DeliveryDate { get; set; }
        [MaxLength(200, ErrorMessage = "El atributo {0} solo puede contener {1} caracteres.")]
        public string? OrderObs { get; set; }
        [Display(Name = "Duración Cirugía")]
        public int SurgeryTime { get; set; }
        [Display(Name = "Fecha de Inicio")]
        public DateTime? SurgeryInit { get; set; }
        [Display(Name = "Fecha de Final")]
        public DateTime? SurgeryEnd { get; set; }
        public string? Status { get; set; }
        public int? IdTraceStates { get; set; }
        public int? BranchId { get; set; }

        [Display(Name = "Hora de Inicio")]
        public TimeSpan? SurgeryInitTime { get; set; }
        [Display(Name = "Hora Final")]
        public TimeSpan? SurgeryEndTime { get; set; }
        [Display(Name = "Dirección de Entrega")]
        [MaxLength(1000, ErrorMessage = "El atributo {0} solo puede contener {1} caracteres.")]
        public string? DeliveryAddress { get; set; }
        [MaxLength(1000, ErrorMessage = "El atributo {0} solo puede contener {1} caracteres.")]
        public string? PurchaseOrder { get; set; }
        public bool? AtcConsumed { get; set; }
        public bool? IsSatisfied { get; set; }
        [MaxLength(1000, ErrorMessage = "El atributo {0} solo puede contener {1} caracteres.")]
        public string? Observations { get; set; }
        [MaxLength(1000, ErrorMessage = "El atributo {0} solo puede contener {1} caracteres.")]
        public string? obsMaint { get; set; }
        public int? AuxLog { get; set; }
        public int? IdCancelReason { get; set; }
        public int? IdCancelDetail { get; set; }
        [MaxLength(200, ErrorMessage = "El atributo {0} solo puede contener {1} caracteres.")]
        public string? CancelReason { get; set; }
        [MaxLength(200, ErrorMessage = "El atributo {0} solo puede contener {1} caracteres.")]
        public string? CancelDetail { get; set; }
        public bool? Notification { get; set; }
        public bool? IsReplacement { get; set; }
        public bool? AssemblyComponents { get; set; }
        [ForeignKey("Insurer")]
        public virtual Insurer? InsurerNavigation { get; set; }
        [ForeignKey("InsurerType")]
        public virtual InsurerType? InsurerTypeNavigation { get; set; }
        [ForeignKey("BranchId")]
        public virtual Branch? Branch { get; set; }
        [ForeignKey("IdCustomer")]
        public virtual Customer? IdCustomerNavigation { get; set; }
        [ForeignKey("IdMedic")]
        public virtual Medic? IdMedicNavigation { get; set; }
        [ForeignKey("IdPatient")]
        public virtual Patient? IdPatientNavigation { get; set; }
        [ForeignKey("IdTraceStates")]
        public virtual TraceabilityStates? IdTraceStatesNavigation { get; set; }
        [ForeignKey("IdATC")]
        public virtual Employee? IdAtcNavigation { get; set; }
        public virtual ICollection<EntryRequestAssembly>? EntryRequestAssembly { get; set; }
        public virtual ICollection<EntryRequestDetails>? EntryRequestDetails { get; set; }
        public virtual ICollection<EntryRequestComponents>? EntryRequestComponents { get; set; }
        public virtual ICollection<EntryRequestTraceStates>? EntryRequestTraceStates { get; set; }
        public virtual ICollection<EntryRequestHistory>? EntryRequestHistory { get; set; }
        [NotMapped]
        public virtual ICollection<ItemsBC>? Items { get; set; }
        [NotMapped]
        public virtual ICollection<EntryRequestFiles>? EntryRequestFiles { get; set; }
        [NotMapped]
        public Insurer? InsurerData { get; set; }
        [NotMapped]
        public InsurerType? InsurerTypeData { get; set; }
        [NotMapped]
        [Display(Name = "Hora Final")]
        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        public TimeSpan DeliveryTime { get; set; }
        [NotMapped]
        public virtual Employee? Atc { get; set; }
        [NotMapped]
        public string? Name { get; set; }

        [NotMapped]
        public string? CustomerName { get; set; }
        [NotMapped]
        public string? PatientName { get; set; }

        [NotMapped]
        public string? MedicName { get; set; }
        [NotMapped]
        public string? NoRequest { get; set; }
        [NotMapped]
        public string? sEquipments { get; set; }
        [NotMapped]
        public string? sAssembly { get; set; }
        [MaxLength(50, ErrorMessage = "El atributo {0} solo puede contener {1} caracteres.")]
        public string? priceGroup { get; set; }
    }
}
