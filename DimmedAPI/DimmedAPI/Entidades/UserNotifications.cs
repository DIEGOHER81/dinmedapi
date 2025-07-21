using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    public class UserNotifications
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public bool NotificationCons { get; set; }

        public virtual Branch Branch { get; set; }

        [ForeignKey("UserId")]
        public virtual Employee Employee { get; set; }
        [NotMapped]
        public string Name { get; set; }
    }
}
