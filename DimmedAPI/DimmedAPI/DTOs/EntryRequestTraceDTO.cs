using System;

namespace DimmedAPI.DTOs
{
    public class EntryRequestTraceDTO
    {
        public int? Id { get; set; }
        public DateTime? LoadDate { get; set; }
        public string? TraceState { get; set; }
        public string? CustomerName { get; set; }
        public string? Branch { get; set; }
        public DateTime? SurgeryInit { get; set; }
        public string? Status { get; set; }
        public string? UserName { get; set; }
        public string? EntryRequestTraceState { get; set; }
        public string? EName { get; set; }
        public string? ECode { get; set; }
        public string? Equipos { get; set; }
        public string? Componentes { get; set; }
    }

    public class EntryRequestTraceFilterDTO
    {
        public int? RQID { get; set; }
        public int? BranchId { get; set; }
        public DateTime? DateIni { get; set; }
        public DateTime? DateEnd { get; set; }
    }
} 