using System.ComponentModel.DataAnnotations;

namespace DimmedAPI.DTOs
{
    public class QuotationMasterCreateDTO
    {
        [Required]
        public int FK_idBranch { get; set; }

        public char? CustomerOrigin { get; set; }

        [Required]
        public int FK_idCustomerType { get; set; }

        [Required]
        public int IdCustomer { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public int FK_idEmployee { get; set; }

        public int? FK_QuotationTypeId { get; set; }

        public int? PaymentTerm { get; set; }

        public int? FK_CommercialConditionId { get; set; }

        public bool? TotalizingQuotation { get; set; }

        public bool? EquipmentRemains { get; set; }

        public double? MonthlyConsumption { get; set; }
    }
} 