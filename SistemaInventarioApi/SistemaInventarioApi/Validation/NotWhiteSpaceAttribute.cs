using System.ComponentModel.DataAnnotations;

namespace SistemaInventarioApi.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class NotWhiteSpaceAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        if (value is string text && !string.IsNullOrWhiteSpace(text))
            return ValidationResult.Success;

        return new ValidationResult(
            ErrorMessage
            ?? $"El campo {validationContext.DisplayName} es obligatorio.");
    }
}
