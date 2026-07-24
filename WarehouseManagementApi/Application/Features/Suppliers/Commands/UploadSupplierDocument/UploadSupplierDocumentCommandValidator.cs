using FluentValidation;

namespace Application.Features.Suppliers.Commands.UploadSupplierDocument;

public class UploadSupplierDocumentCommandValidator : AbstractValidator<UploadSupplierDocumentCommand>
{
    private static readonly string[] AllowedContentTypes = ["application/pdf", "application/txt"];
    private const long MaxSize = 10 * 1024 * 1024;

    public UploadSupplierDocumentCommandValidator()
    {
        RuleFor(d => d.SupplierId)
            .NotEmpty()
            .WithMessage("Supplier ID is required")
            .Length(36)
            .WithMessage("Supplier ID format invalid");

        RuleFor(d => d.File)
            .NotEmpty()
            .WithMessage("File is required");

        RuleFor(d => d.FileLength)
            .GreaterThan(0)
            .WithMessage("No file was provided")
            .LessThanOrEqualTo(MaxSize)
            .WithMessage("File size cannot exceed 10MB");

        RuleFor(d => d.FileName)
            .NotEmpty()
            .WithMessage("File name is required");

        RuleFor(d => d.ContentType)
            .NotEmpty()
            .WithMessage("Content type is required")
            .Must(ct => AllowedContentTypes.Contains(ct))
            .WithMessage($"Content type must be one of: {string.Join(", ", AllowedContentTypes)}");
    }
}