using System.ComponentModel.DataAnnotations;

namespace Application.Validation;

public class GuidStringAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is string id
               && id.Length == 36
               && Guid.TryParse(id, out _);
    }
}