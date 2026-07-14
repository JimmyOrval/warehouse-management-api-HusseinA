namespace Presentation.Reflection;

public class ValidationMetadata
{
    public required string TypeName { get; set; }
    
    public required List<PropertyValidationMetadata> Properties { get; set; }
}