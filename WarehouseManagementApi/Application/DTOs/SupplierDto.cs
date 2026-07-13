namespace Application.DTOs;

public record SupplierDto(
    string Id,
    string Name,
    string Country,
    string ContactEmail,
    string Phone,
    bool IsActive
);