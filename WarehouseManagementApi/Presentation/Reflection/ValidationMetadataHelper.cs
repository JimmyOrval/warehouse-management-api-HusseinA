using System.ComponentModel.DataAnnotations;

namespace Presentation.Reflection;

public class ValidationMetadataHelper
{
    public static ValidationMetadata GetValidationMetadata<T>()
    {
        // T can be something like CreateProductCommand
        var type = typeof(T);
        
        var properties = type
            // gets all public fields
            .GetProperties()
            // gets metadata for each field
            .Select(property => new PropertyValidationMetadata
            {
                PropertyName = property.Name,
                
                // used to get properties like [Required] or [StringLength()]
                ValidationAttributes = property
                    .GetCustomAttributes(typeof(ValidationAttribute), false)
                    .Cast<ValidationAttribute>()
                    .Select(attribute => attribute.GetType().Name.Replace("Attribute", string.Empty))
                    .ToList()
            })
            .ToList();

        return new ValidationMetadata
        {
            TypeName = type.Name,
            Properties = properties
        };
    }
}