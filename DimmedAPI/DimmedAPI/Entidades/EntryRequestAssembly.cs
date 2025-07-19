using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    public class EntryRequestAssembly : EntryRequestAssemblyBase
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string ShortDesc { get; set; }
        public string Invima { get; set; }
        public string Lot { get; set; }
        public decimal Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string AssemblyNo { get; set; }
        public int EntryRequestId { get; set; }
        public int? EntryRequestDetailId { get; set; }
        public decimal? QuantityConsumed { get; set; }
        public DateTime? ExpirationDate { get; set; }
        [ForeignKey("EntryRequestId")]
        public virtual EntryRequests EntryRequest { get; set; }
        [NotMapped]
        public string Name { get; set; }
        public decimal? ReservedQuantity { get; set; }
        public string Location_Code_ile { get; set; }
        public string Classification { get; set; }
        public string Status { get; set; }
        public int LineNo { get; set; }
        public int? Position { get; set; }
        public decimal? Quantity_ile { get; set; }
        public string TaxCode { get; set; }
        public bool LowTurnover { get; set; }
        public bool? IsComponent { get; set; }
        public DateTime? RSFechaVencimiento { get; set; }
        public string RSClasifRegistro { get; set; }
        [NotMapped]
        public string LocationCode { get; set; }
    }
} 