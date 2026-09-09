using System.ComponentModel.DataAnnotations;

namespace SistemaInventarioApi.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class PositiveIdAttribute : RangeAttribute
{
    public PositiveIdAttribute()
        : base(1, int.MaxValue)
    {
        ErrorMessage = "El parámetro 'id' debe ser un entero positivo.";
    }
}
