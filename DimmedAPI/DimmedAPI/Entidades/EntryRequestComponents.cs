using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    public class EntryRequestComponents
    {
        public int Id { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string Warehouse { get; set; }
        public decimal? Quantity { get; set; }
        public int IdEntryReq { get; set; }
        public string SystemId { get; set; }
        public decimal? QuantityConsumed { get; set; }
        public string Branch { get; set; }
        public string Lot { get; set; }
        public decimal? UnitPrice { get; set; }
        public string status { get; set; }
        public string AssemblyNo { get; set; }
        public virtual EntryRequests IdEntryReqNavigation { get; set; }
        // [NotMapped]
        public string TaxCode { get; set; }
        public string shortDesc { get; set; }
        [NotMapped]
        public string locationCodeStock { get; set; }
        [NotMapped]
        public string Name { get; set; }
        [NotMapped]
        public int? UserIdTraceState { get; set; }
        public string Invima { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string TraceState { get; set; }
        public DateTime? RSFechaVencimiento { get; set; }
        public string RSClasifRegistro { get; set; }
    }
}
