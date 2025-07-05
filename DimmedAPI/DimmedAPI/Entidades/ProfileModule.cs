using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Security;

namespace DimmedAPI.Entidades
{
    public class ProfileModule
    {
        public int Id { get; set; }
        public int IdPermission { get; set; }
        //public virtual Permission Permission { get; set; }
        public int IdProfile { get; set; }
        [NotMapped]
        public virtual Profile Profile { get; set; }
        public int IdModule { get; set; }
        //public virtual Modules IdModuleNavigation { get; set; }
    }
}
