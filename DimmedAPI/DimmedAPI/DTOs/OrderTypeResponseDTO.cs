namespace DimmedAPI.DTOs
{
    public class OrderTypeResponseDTO
    {
        public long Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
} 