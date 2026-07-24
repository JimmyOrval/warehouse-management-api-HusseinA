using Domain.Exceptions;

namespace Application.Validation;

public class FileValidation
{
    private static readonly string[] AllowedImageTypes =
        ["image/jpg", "image/png"];

    private static readonly string[] AllowedDocumentTypes =
        ["application/pdf", "application/txt"];
    
    private const long MaxImageSize = 2 * 1024 * 1024;
    private const long MaxDocumentSize = 5 * 1024 * 1024;

    public static void ValidateImage(long size, string contentType)
    {
        if (size > MaxImageSize)
            throw new BusinessRuleException($"Image size cannot exceed 2MB");
        if(!AllowedImageTypes.Contains(contentType))
            throw new BusinessRuleException($"Content type {contentType} is not allowed");
    }
    
    public static void ValidateDocument(long size, string contentType)
    {
        if (size > MaxDocumentSize)
            throw new BusinessRuleException($"Document size cannot exceed 2MB");
        if(!AllowedDocumentTypes.Contains(contentType))
            throw new BusinessRuleException($"Content type {contentType} is not allowed");
    }
}