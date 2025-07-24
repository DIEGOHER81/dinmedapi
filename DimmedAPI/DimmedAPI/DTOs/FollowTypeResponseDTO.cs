namespace DimmedAPI.DTOs
{
    public class FollowTypeResponseDTO
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
} 