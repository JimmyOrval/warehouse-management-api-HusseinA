using Application.Contracts;
using Application.Features.Products.Commands.CreateProduct;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings;

public class RequestMapper : Profile
{
    public RequestMapper()
    {
        CreateMap<CreateProductCommand, Product>();
        CreateMap<CreateSupplierRequest, Supplier>();
    }
}