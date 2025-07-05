using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    [Table("Branches")]
    public class Branch
    {
        public Branch()
        {
            UserBranch = new HashSet<UserBranch>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(10)]
        [Column(TypeName = "nchar(10)")]
        public string? SystemId { get; set; }

        [StringLength(3)]
        public string? LocationCode { get; set; }

        [StringLength(3)]
        public string? LocationCodeStock { get; set; }

        // Relación con UserBranch
        public virtual ICollection<UserBranch> UserBranch { get; set; }
    }
}
