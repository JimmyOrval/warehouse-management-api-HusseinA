namespace Application.DTOs;

public record ProductImageDto(
    string Id,
    string ProductId,
    string FileName,
    string FilePath
);