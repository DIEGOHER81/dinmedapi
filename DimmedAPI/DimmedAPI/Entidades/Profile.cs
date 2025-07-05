using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    public class Profile
    {

        public Profile()
        {
            Users = new HashSet<Users>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Users> Users { get; set; }
        [NotMapped]
        public virtual ICollection<ProfileModule> ProfileModules { get; set; }
    }
}
