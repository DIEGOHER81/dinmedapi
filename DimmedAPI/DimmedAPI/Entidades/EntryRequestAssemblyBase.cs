using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    public class EntryRequestAssemblyBase
    {
        public int Id { get; set; }
        public int? EntryRequestDetailId { get; set; }
        [ForeignKey("EntryRequestDetailId")]
        public virtual EntryRequestDetails EntryRequestDetail { get; set; }
    }
} 