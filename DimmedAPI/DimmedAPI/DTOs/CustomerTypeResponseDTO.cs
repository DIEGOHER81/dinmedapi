namespace DimmedAPI.DTOs
{
    public class CustomerTypeResponseDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int QuotationsCount { get; set; }
    }

    public class CustomerTypeWithQuotationsDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<QuotationSummaryDTO>? Quotations { get; set; }
    }

    public class QuotationSummaryDTO
    {
        public int Id { get; set; }
        public int IdCustomer { get; set; }
        public DateTime? CreationDateTime { get; set; }
        public DateTime? DueDate { get; set; }
        public int FK_idEmployee { get; set; }
        public bool? TotalizingQuotation { get; set; }
        public bool? EquipmentRemains { get; set; }
    }
} 