using System.ComponentModel.DataAnnotations;

namespace DimmedAPI.Validaciones
{
    public class PrimeraLetraAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success;

            }

            //Adquiere la primera letra 
            var primeraletra = value.ToString()![0].ToString();

            if (primeraletra != primeraletra.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayúscula");
            }
            return ValidationResult.Success;
        }
    }
}
