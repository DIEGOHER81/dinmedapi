namespace DimmedAPI.DTOs
{
    public class EntryRequestAssemblyDetailDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Lot { get; set; }
        public string AssemblyNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityConsumed { get; set; }
    }
}
