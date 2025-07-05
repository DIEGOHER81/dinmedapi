using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    [Table("UserBranch")]
    public class UserBranch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int IdUser { get; set; }

        [Required]
        [ForeignKey("Branch")]
        public int IdBranch { get; set; }

        // Relaciones de navegación
        public Users? User { get; set; }
        public Branch? Branch { get; set; }
    }
} 