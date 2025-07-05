namespace DimmedAPI.DTOs
{
    public class QuotationDetailResponseDTO
    {
        public int Id { get; set; }
        public int Fk_IdQuotationMasterId { get; set; }
        public char ProductType { get; set; }
        public string CodProduct { get; set; } = null!;
        public string Unit { get; set; } = null!;
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public double? PorcTax { get; set; }
        public double? TaxValue { get; set; }
        public int? ContractTime { get; set; }
        public string? WarrantyPeriod { get; set; }
        public double? Subtotal { get; set; }
        public double? Total { get; set; }
        public QuotationMasterSummaryDTO? QuotationMaster { get; set; }
    }

    public class QuotationMasterSummaryDTO
    {
        public int Id { get; set; }
        public int IdCustomer { get; set; }
        public DateTime? CreationDateTime { get; set; }
        public DateTime? DueDate { get; set; }
        public int FK_idEmployee { get; set; }
        public bool? TotalizingQuotation { get; set; }
        public bool? EquipmentRemains { get; set; }
    }

    public class QuotationDetailStatisticsDTO
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
        public double? Subtotal { get; set; }
        public double? Total { get; set; }
        public int QuotationMasterId { get; set; }
    }

    public class QuotationDetailCalculationDTO
    {
        public int QuotationDetailId { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public double Subtotal { get; set; }
        public double? TaxPercentage { get; set; }
        public double TaxAmount { get; set; }
        public double Total { get; set; }
        public double? CurrentTaxValue { get; set; }
    }
} 