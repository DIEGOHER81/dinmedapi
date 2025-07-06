namespace DimmedAPI.DTOs
{
    public class EmployeeResponseDTO
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = null!;
        public string? Charge { get; set; }
        public string? SystemIdBC { get; set; }
        public bool? ATC { get; set; }
        public bool? MResponsible { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Branch { get; set; }
        public int QuotationsCount { get; set; }
    }

    public class EmployeeWithQuotationsDTO
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = null!;
        public string? Charge { get; set; }
        public string? SystemIdBC { get; set; }
        public bool? ATC { get; set; }
        public bool? MResponsible { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Branch { get; set; }
        public List<QuotationSummaryDTO>? Quotations { get; set; }
    }

    public class EmployeeStatisticsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Charge { get; set; }
        public string? Branch { get; set; }
        public int TotalQuotations { get; set; }
        public decimal TotalQuotationValue { get; set; }
        public decimal AverageQuotationValue { get; set; }
        public DateTime? LastQuotationDate { get; set; }
    }
} 