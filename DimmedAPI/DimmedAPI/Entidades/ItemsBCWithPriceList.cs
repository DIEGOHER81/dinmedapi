namespace DimmedAPI.Entidades
{
    public class ItemsBCWithPriceList
    {
        public int Id { get; set; }
        public string No { get; set; }
        public string Description { get; set; }
        public string BaseUnitMeasure { get; set; }
        public decimal? UnitCost { get; set; }
        public bool? PriceIncludesVAT { get; set; }
        public string SalesCode { get; set; }
        public decimal? UnitPrice { get; set; }
        public string UnitMeasureCode { get; set; }
        public string AuxiliaryIndex1 { get; set; }
        public string AuxiliaryIndex2 { get; set; }
        public string AuxiliaryIndex3 { get; set; }
    }
} 