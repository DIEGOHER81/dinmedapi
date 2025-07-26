namespace DimmedAPI.DTOs
{
    public class EmailSendResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorDetails { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
} 