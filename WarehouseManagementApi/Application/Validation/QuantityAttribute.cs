using System.ComponentModel.DataAnnotations;

namespace Application.Validation;

public class QuantityAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is int quantity && quantity > 0;
    }
}