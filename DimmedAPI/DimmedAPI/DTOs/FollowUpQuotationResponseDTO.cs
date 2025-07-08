namespace DimmedAPI.DTOs
{
    public class FollowUpQuotationResponseDTO
    {
        public int Id { get; set; }
        public int Fk_IdQuotation { get; set; }
        public int Fk_IdEmployee { get; set; }
        public int idconceptoseguimiento { get; set; }
        public string Observation { get; set; } = string.Empty;
        public DateTime? CreateDateTime { get; set; }

        // Propiedades de navegación
        public QuotationMasterInfo? Quotation { get; set; }
        public FollowUpEmployeeInfo? Employee { get; set; }
    }

    public class QuotationMasterInfo
    {
        public int Id { get; set; }
        public int FK_idBranch { get; set; }
        public char? CustomerOrigin { get; set; }
        public int FK_idCustomerType { get; set; }
        public int IdCustomer { get; set; }
        public DateTime? CreationDateTime { get; set; }
        public DateTime? DueDate { get; set; }
        public int FK_idEmployee { get; set; }
        public int? FK_QuotationTypeId { get; set; }
        public int? PaymentTerm { get; set; }
        public int? FK_CommercialConditionId { get; set; }
        public bool? TotalizingQuotation { get; set; }
        public double? Total { get; set; }
        public bool? EquipmentRemains { get; set; }
        public double? MonthlyConsumption { get; set; }
    }

    public class FollowUpEmployeeInfo
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = null!;
        public string? Charge { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
} 