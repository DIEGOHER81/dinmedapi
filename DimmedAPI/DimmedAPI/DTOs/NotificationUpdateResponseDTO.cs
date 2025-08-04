namespace DimmedAPI.DTOs
{
    public class NotificationUpdateResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? UpdatedId { get; set; }
        public string? ErrorDetails { get; set; }
    }
} 