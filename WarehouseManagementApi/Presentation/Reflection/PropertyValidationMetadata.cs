namespace Presentation.Reflection;

public class PropertyValidationMetadata
{
    public required string PropertyName { get; set; }
    
    public required List<string> ValidationAttributes { get; set; }
}