using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DimmedAPI.Entidades
{
    public class CancelDetails
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int? IdRelation { get; set; }

        [ForeignKey("IdRelation")]
        [JsonIgnore]
        public virtual CancellationReasons CancellationReason { get; set; }

        [NotMapped]
        public string Name { get; set; }

        [NotMapped]
        public string CancellationReasonDescription => CancellationReason?.Description;
    }
}
