#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;

public class DateValidation : ValidationAttribute
{
    protected override ValidationResult? IsValid(object date, ValidationContext validationContext)
    {
        if((DateTime)date < DateTime.Now)
        {
            return new ValidationResult("Must be a date in the future!");
        }

        return ValidationResult.Success;
    }
}