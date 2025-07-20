namespace DimmedAPI.DTOs
{
    public class EntryrequestServiceResponseDTO
    {
        public long Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
} 