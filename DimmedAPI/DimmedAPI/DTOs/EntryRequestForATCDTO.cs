using DimmedAPI.Entidades;

namespace DimmedAPI.DTOs
{
    public class EntryRequestForATCDTO
    {
        public int Id { get; set; }
        public DateTime? SurgeryInit { get; set; }
        public DateTime? SurgeryEnd { get; set; }
        public string? Status { get; set; }
        public int IdCustomer { get; set; }
        public int? IdATC { get; set; }
        public int? BranchId { get; set; }
        
        // Relación con Customer
        public Customer? Customer { get; set; }
    }
} 