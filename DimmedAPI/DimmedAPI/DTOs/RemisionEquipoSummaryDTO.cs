using System;

namespace DimmedAPI.DTOs
{
    public class RemisionEquipoSummaryDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int IdOrderType { get; set; }
        public string OrderTypeName { get; set; } = string.Empty;
        public int IdCustomer { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
    }
} 