using FluentValidation;

namespace Application.Features.Products.Commands.UploadProductImage;

public class UploadProductImageCommandValidator
    : AbstractValidator<UploadProductImageCommand>
{
    public UploadProductImageCommandValidator()
    {
        RuleFor(p => p.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required")
            .Length(36)
            .WithMessage("Product ID format invalid");

        RuleFor(p => p.Image)
            .NotEmpty()
            .WithMessage("Image is required");
        
        RuleFor(p => p.ImageLength)
            .GreaterThan(0)
            .WithMessage("No image was provided")
            .LessThan(2 * 1024 * 1024)
            .WithMessage("Image size cannot exceed 2MB");

        RuleFor(p => p.FileName)
            .NotEmpty()
            .WithMessage("File name is required");
    }
}