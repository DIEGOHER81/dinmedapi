namespace DimmedAPI.DTOs
{
    public class CommercialConditionResponseDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? CommercialText { get; set; }
        public bool IsActive { get; set; }
        public int QuotationsCount { get; set; }
    }

    public class CommercialConditionWithQuotationsDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? CommercialText { get; set; }
        public bool IsActive { get; set; }
        public List<QuotationSummaryDTO>? Quotations { get; set; }
    }

    public class CommercialConditionStatisticsDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? CommercialText { get; set; }
        public bool IsActive { get; set; }
        public int QuotationsCount { get; set; }
        public int TotalizingQuotationsCount { get; set; }
        public int EquipmentRemainsCount { get; set; }
        public double? AveragePaymentTerm { get; set; }
    }
} 