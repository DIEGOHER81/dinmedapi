namespace DimmedAPI.DTOs
{
    public class LogoUploadResponseDTO
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? LogoUrl { get; set; }
        public string? FileName { get; set; }
        public long FileSize { get; set; }
        public string? ContentType { get; set; }
    }
} 