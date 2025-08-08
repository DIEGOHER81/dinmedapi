namespace DimmedAPI.DTOs
{
    public class ValidDispatchResponseDTO
    {
        public bool IsSuccess { get; set; }
        public object? Result { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorDetails { get; set; }
        public DateTime ValidatedAt { get; set; } = DateTime.UtcNow;
    }
}

