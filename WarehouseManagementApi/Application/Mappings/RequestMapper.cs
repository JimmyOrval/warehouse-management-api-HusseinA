using Application.Features.Products.Commands.CreateProduct;
using Application.Features.Suppliers.Commands.CreateSupplier;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings;

public class RequestMapper : Profile
{
    public RequestMapper()
    {
        CreateMap<CreateProductCommand, Product>();
        CreateMap<CreateSupplierCommand, Supplier>();
    }
}