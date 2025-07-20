using DimmedAPI.Entidades;

namespace DimmedAPI.DTOs
{
    public class EntryRequestForATCResponseDTO
    {
        public int Id { get; set; }
        public string? SurgeryInit { get; set; }
        public string? SurgeryEnd { get; set; }
        public string? Status { get; set; }
        public int IdCustomer { get; set; }
        public int? IdATC { get; set; }
        public int? BranchId { get; set; }
        
        // Relación con Customer
        public Customer? Customer { get; set; }
    }
} 