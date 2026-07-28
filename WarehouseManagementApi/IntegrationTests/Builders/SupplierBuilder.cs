using Domain.Models;

namespace IntegrationTests.Builders;

public class SupplierBuilder
{
    private readonly string _id = Guid.NewGuid().ToString();
    private string _name = "Supplier1";
    private const string Country = "Lebanon";
    private const string ContactEmail = "supplier1@email.com";
    private const string Phone = "+9611234567";
    
    // to deactivate supplier on-creation (same as product status)
    private bool _deactivated;

    public SupplierBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public SupplierBuilder Inactive()
    {
        _deactivated = true;
        return this;
    }

    public Supplier Build()
    {
        var supplier = new Supplier
        {
            Id = _id,
            Name = _name,
            Country = Country,
            ContactEmail = ContactEmail,
            Phone = Phone
        };

        // if we need it deactivated on-creation,
        // this automates it
        if (_deactivated)
        {
            supplier.Deactivate();
        }

        return supplier;
    }
}
