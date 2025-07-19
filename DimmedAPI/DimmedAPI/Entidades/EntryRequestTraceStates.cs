using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    public class EntryRequestTraceStates
    {
        public int Id { get; set; }
        public int EntryRequestId { get; set; }
        public int TraceabilityStatesId { get; set; }
        public DateTime? LoadDate { get; set; }
        public int UserId { get; set; }
        public string Sourse { get; set; }

        [ForeignKey("EntryRequestId")]
        public virtual EntryRequests EntryRequest { get; set; }
        [ForeignKey("TraceabilityStatesId")]
        public virtual TraceabilityStates TraceabilityStates { get; set; }
        [NotMapped]
        public string Name { get; set; }
        [NotMapped]
        public string UserName { get; set; }
    }
}
