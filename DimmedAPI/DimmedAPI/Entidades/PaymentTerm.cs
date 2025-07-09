using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    [Table("PaymentTerm")]
    public class PaymentTerm
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string DueDateCalculation { get; set; }
    }
} 