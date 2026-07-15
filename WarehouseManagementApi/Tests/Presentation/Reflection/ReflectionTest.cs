using Application.Features.Products.Commands.CreateProduct;
using Presentation.Reflection;
using FluentAssertions;

namespace Tests.Presentation.Reflection;

public class ReflectionTest
{
    [Fact]
    public void GetValidationMetadata_ShouldReturnValidationAttributes()
    {
        var metadata = ValidationMetadataHelper.GetValidationMetadata<CreateProductCommand>();

        Assert.NotNull(metadata);

        var nameProperty = metadata.Properties
            .FirstOrDefault(p => p.PropertyName == "Name");

        Assert.NotNull(nameProperty);
        
        Assert.Contains("Required", nameProperty.ValidationAttributes); 
    }
}