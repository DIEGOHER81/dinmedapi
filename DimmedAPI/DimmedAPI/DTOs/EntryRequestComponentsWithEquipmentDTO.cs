using System;

namespace DimmedAPI.DTOs
{
    /// <summary>
    /// DTO para EntryRequestComponents que incluye información del equipo
    /// </summary>
    public class EntryRequestComponentsWithEquipmentDTO
    {
        public int Id { get; set; }
        public string? ItemNo { get; set; }
        public string? ItemName { get; set; }
        public string? Warehouse { get; set; }
        public decimal? Quantity { get; set; }
        public int IdEntryReq { get; set; }
        public string? SystemId { get; set; }
        public decimal? QuantityConsumed { get; set; }
        public string? Branch { get; set; }
        public string? Lot { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? Status { get; set; }
        public string? AssemblyNo { get; set; }
        public string? TaxCode { get; set; }
        public string? ShortDesc { get; set; }
        public string? LocationCodeStock { get; set; }
        public string? Name { get; set; }
        public int? UserIdTraceState { get; set; }
        public string? Invima { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? TraceState { get; set; }
        public DateTime? RSFechaVencimiento { get; set; }
        public string? RSClasifRegistro { get; set; }
        
        /// <summary>
        /// Nombre del equipo asociado al AssemblyNo
        /// </summary>
        public string? EquipmentName { get; set; }
        
        /// <summary>
        /// Descripción del equipo asociado al AssemblyNo
        /// </summary>
        public string? EquipmentDescription { get; set; }
        
        /// <summary>
        /// Estado del equipo asociado al AssemblyNo
        /// </summary>
        public string? EquipmentStatus { get; set; }
    }
}
