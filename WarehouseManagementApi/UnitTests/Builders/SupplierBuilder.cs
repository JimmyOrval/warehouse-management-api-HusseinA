using Domain.Models;

namespace Tests.Builders;

public class SupplierBuilder
{
    private readonly string _id = Guid.NewGuid().ToString();
    private const string Name = "Supplier1";
    private const string Country = "Lebanon";
    private const string ContactEmail = "supplier1@email.com";
    private const string Phone = "+9611234567";

    public Supplier Build()
    {
        return new Supplier
        {
            Id = _id,
            Name = Name,
            Country = Country,
            ContactEmail = ContactEmail,
            Phone = Phone
        };
    }
}