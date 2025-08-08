namespace DimmedAPI.DTOs
{
    public class EntryRequestComponentDetailDTO
    {
        public int EntryRequestId { get; set; }
        public string RequestNumber { get; set; }
        public int ComponentId { get; set; }
        public string ComponentCode { get; set; }
        public string ComponentDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityConsumed { get; set; }
        public int IdEntryReq { get; set; }
    }
}
