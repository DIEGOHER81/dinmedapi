#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    public class QuotationMaster
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("FK_idBranch")]
        public int FK_idBranch { get; set; }

        [Column("customer_origin")]
        public char? CustomerOrigin { get; set; }

        [Required]
        [Column("FK_idcustomertype")]
        public int FK_idCustomerType { get; set; }

        [Required]
        [Column("idCustomer")]
        public int IdCustomer { get; set; }

        [Column("Creationdatetime")]
        public DateTime? CreationDateTime { get; set; }

        [Column("dueDate")]
        public DateTime? DueDate { get; set; }

        [Required]
        [Column("FK_idEmployee")]
        public int FK_idEmployee { get; set; }

        [Column("FK_quotationtypeId")]
        public int? FK_QuotationTypeId { get; set; }

        [Column("paymentterm")]
        public int? PaymentTerm { get; set; }

        [Column("FK_CommercialConditionId")]
        public int? FK_CommercialConditionId { get; set; }

        [Column("totalizingQuotaton")]
        public bool? TotalizingQuotation { get; set; }

        [Column("equipmentremains")]
        public bool? EquipmentRemains { get; set; }

        [Column("monthlyconsumption")]
        public double? MonthlyConsumption { get; set; }

        [Column("Total")]
        public double? Total { get; set; }

        // Propiedades de navegación
        [ForeignKey("FK_idBranch")]
        public Branch? Branch { get; set; }

        [ForeignKey("FK_idCustomerType")]
        public CustomerType? CustomerType { get; set; }

        [ForeignKey("FK_idEmployee")]
        public Employee? Employee { get; set; }

        [ForeignKey("FK_QuotationTypeId")]
        public QuotationType? QuotationType { get; set; }

        [ForeignKey("FK_CommercialConditionId")]
        public CommercialCondition? CommercialCondition { get; set; }
    }
}
