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
        public int FK_idBranch { get; set; }

        public char? CustomerOrigin { get; set; }

        [Required]
        public int FK_idCustomerType { get; set; }

        [Required]
        public int IdCustomer { get; set; }

        public DateTime? CreationDateTime { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public int FK_idEmployee { get; set; }

        public int? FK_QuotationTypeId { get; set; }

        public int? PaymentTerm { get; set; }

        public int? FK_CommercialConditionId { get; set; }

        public bool? TotalizingQuotation { get; set; }

        public bool? EquipmentRemains { get; set; }

        public double? MonthlyConsumption { get; set; }

        // Propiedades de navegación (opcional si estás usando EF Core)

        public Branch? Branch { get; set; }

        public CustomerType? CustomerType { get; set; }

        public Employee? Employee { get; set; }

        public QuotationType? QuotationType { get; set; }

        public CommercialCondition? CommercialCondition { get; set; }
    }
}
