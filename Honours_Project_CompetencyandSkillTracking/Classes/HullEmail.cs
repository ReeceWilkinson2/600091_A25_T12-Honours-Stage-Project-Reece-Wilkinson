using System.ComponentModel.DataAnnotations;

public class HullEmailOnlyAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var email = value as string;

        if (string.IsNullOrWhiteSpace(email) || !email.EndsWith("@hull.ac.uk", StringComparison.OrdinalIgnoreCase))
        {
            return new ValidationResult("Email must end with @hull.ac.uk");
        }

        return ValidationResult.Success;
    }
}