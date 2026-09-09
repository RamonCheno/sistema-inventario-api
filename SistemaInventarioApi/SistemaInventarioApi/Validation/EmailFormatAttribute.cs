using System.ComponentModel.DataAnnotations;

namespace SistemaInventarioApi.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class EmailFormatAttribute : ValidationAttribute
{
    private readonly EmailAddressAttribute _emailAddress = new();

    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        if (value is not string text)
            return false;

        return string.IsNullOrWhiteSpace(text)
            || _emailAddress.IsValid(text);
    }
}
