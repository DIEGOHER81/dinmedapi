namespace DimmedAPI.DTOs
{
    public class EmailSendRequestDTO
    {
        public required string ToEmail { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public bool IsHtml { get; set; } = false;
    }
} 