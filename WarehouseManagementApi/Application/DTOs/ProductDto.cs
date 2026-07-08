using System.ComponentModel.DataAnnotations;
using Domain.Models;

namespace Application.DTOs;

public record ProductDto
(
    string Id,
    string Name,
    string Sku,
    string Description,
    decimal Price,
    string SupplierId,
    DateTime ExpiryDate,
    bool IsArchived,
    DateTime CreatedAt,
    DateTime LastUpdatedAt
);