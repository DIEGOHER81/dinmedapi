using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace DimmedAPI.Entidades
{
    public class Users
    {
        public Users()
        {
            //EntryRequestHistory = new HashSet<EntryRequestHistory>();
            //EventLog = new HashSet<EventLog>();
            //ExceptionLog = new HashSet<ExceptionLog>();
            UserBranch = new HashSet<UserBranch>();
        }
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public bool? IsActive { get; set; }
        public int? ProfileId { get; set; }
        public string? charge { get; set; }

        [ForeignKey("ProfileId")]
        public virtual Profile? Profile { get; set; }
        //public virtual ICollection<EntryRequestHistory> EntryRequestHistory { get; set; }
        //public virtual ICollection<EventLog> EventLog { get; set; }
        //public virtual ICollection<ExceptionLog> ExceptionLog { get; set; }

        // Relación con UserBranch
        public virtual ICollection<UserBranch> UserBranch { get; set; }

        [NotMapped]
        //public ICollection<UserBranch> UserBranch { get; set; }
        //[NotMapped]
        public string? currentBranch { get; set; }
        [NotMapped]
        public int currentBranchId { get; set; }
    }
}
