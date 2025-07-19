using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    public class EntryRequestHistory
    {
        public int Id { get; set; }
        public int? IdEntryRequest { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime? DateLoad { get; set; }
        public int? UserId { get; set; }
        public string Information { get; set; }

        [ForeignKey("IdEntryRequest")]
        public virtual EntryRequests IdEntryRequestNavigation { get; set; }
        [ForeignKey("UserId")]
        public virtual Users User { get; set; }
        [NotMapped]
        public string Name { get; set; }
    }
}
