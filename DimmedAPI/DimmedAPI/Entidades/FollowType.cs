using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DimmedAPI.Entidades
{
    [Table("FollowTypes")]
    public class FollowType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public bool? IsActive { get; set; }
    }
}
