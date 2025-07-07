namespace DimmedAPI.DTOs
{
    public class QuotationMasterResponseDTO
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

        // Propiedades de navegación
        public BranchInfo? Branch { get; set; }
        public CustomerTypeInfo? CustomerType { get; set; }
        public QuotationEmployeeInfo? Employee { get; set; }
        public QuotationTypeInfo? QuotationType { get; set; }
        public CommercialConditionInfo? CommercialCondition { get; set; }
        public List<QuotationDetailInfo>? Details { get; set; }
    }

    public class BranchInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? SystemId { get; set; }
        public string? LocationCode { get; set; }
    }

    public class CustomerTypeInfo
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class QuotationEmployeeInfo
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = null!;
        public string? Charge { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

    public class QuotationTypeInfo
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class CommercialConditionInfo
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? CommercialText { get; set; }
        public bool IsActive { get; set; }
    }

    public class QuotationDetailInfo
    {
        public int Id { get; set; }
        public char ProductType { get; set; }
        public string CodProduct { get; set; } = null!;
        public string Unit { get; set; } = null!;
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public double? PorcTax { get; set; }
        public double? TaxValue { get; set; }
        public int? ContractTime { get; set; }
        public string? WarrantyPeriod { get; set; }
    }
} 