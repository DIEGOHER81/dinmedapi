namespace DimmedAPI.DTOs
{
    public class QuotationTypeResponseDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int QuotationsCount { get; set; }
    }

    public class QuotationTypeWithQuotationsDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<QuotationSummaryDTO>? Quotations { get; set; }
    }

    public class QuotationTypeStatisticsDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int QuotationsCount { get; set; }
        public int TotalizingQuotationsCount { get; set; }
        public int EquipmentRemainsCount { get; set; }
    }
} 