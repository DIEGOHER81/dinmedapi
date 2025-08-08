namespace DimmedAPI.DTOs
{
    public class UpdateResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorDetails { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int? UpdatedId { get; set; }
        public int? Id { get; set; }
        public decimal? PreviousValue { get; set; }
        public decimal? NewValue { get; set; }
    }
} 