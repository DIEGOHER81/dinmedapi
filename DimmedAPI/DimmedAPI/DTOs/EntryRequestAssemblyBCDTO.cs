using System;

namespace DimmedAPI.DTOs
{
    public class EntryRequestAssemblyBCDTO
    {
        public string no { get; set; }
        public string description { get; set; }
        public string description2 { get; set; }
        public string documentNo { get; set; }
        public decimal reservedQuantity { get; set; }
        public decimal? unitPriceSales { get; set; }
        public int? line_No_ { get; set; }
        public int? position { get; set; }
        public string taxCode { get; set; }
        public string rsClasifRegistro { get; set; }
        public string rsFechaVencimiento { get; set; }
        public bool? llRot { get; set; }
        public decimal quantity { get; set; }
        public string locationCode { get; set; }
        public decimal remainingQuantity { get; set; }
        public string locationCodeile { get; set; }
        public decimal? reservedQuantityile { get; set; }
        public string llClasif { get; set; }
        public string llStatus { get; set; }
        public string lotNo { get; set; }
        public decimal quantityToConsume { get; set; }
        public decimal quantityile { get; set; }
        public string invima { get; set; }
        public decimal? unitCost { get; set; }
        public string expirationDate { get; set; }
    }

    public class EntryRequestAssemblyBCResponse
    {
        public List<EntryRequestAssemblyBCDTO> value { get; set; }
    }
} 