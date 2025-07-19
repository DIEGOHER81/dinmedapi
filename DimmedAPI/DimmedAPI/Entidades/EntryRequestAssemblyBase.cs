using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    public class EntryRequestAssemblyBase
    {
        public int Id { get; set; }
        public int IdEntryReqDetail { get; set; }
        [ForeignKey("IdEntryReqDetail")]
        public virtual EntryRequestDetails IdEntryReqDetailNavigation { get; set; }
    }
} 