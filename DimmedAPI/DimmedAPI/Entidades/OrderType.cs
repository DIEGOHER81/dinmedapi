using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DimmedAPI.Entidades
{
    [Table("OrderType")]
    public class OrderType
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("description")]
        [StringLength(100)]
        public required string Description { get; set; }

        [Column("isactive")]
        public bool? IsActive { get; set; }
    }
}
