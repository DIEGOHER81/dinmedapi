namespace DimmedAPI.DTOs
{
    public class EntryRequestCancelUpdateResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int EntryRequestId { get; set; }
        public string Status { get; set; }
        public int? IdCancelReason { get; set; }
        public string? CancelReason { get; set; }
        public int? IdCancelDetail { get; set; }
        public string? CancelDetail { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
