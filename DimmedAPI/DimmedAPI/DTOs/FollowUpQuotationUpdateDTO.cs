using System.ComponentModel.DataAnnotations;

namespace DimmedAPI.DTOs
{
    public class FollowUpQuotationUpdateDTO
    {
        [Required]
        public int Fk_IdQuotation { get; set; }

        [Required]
        public int Fk_IdEmployee { get; set; }

        [Required]
        public int idconceptoseguimiento { get; set; }

        [Required]
        public string Observation { get; set; } = string.Empty;
    }
} 