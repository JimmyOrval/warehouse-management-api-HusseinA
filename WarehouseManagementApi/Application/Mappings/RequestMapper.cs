using Application.Features.Products.Commands.CreateProduct;
using Application.Features.Suppliers.Commands.CreateSupplier;
using Application.Features.WarehouseItems.Commands.CreateWarehouseItem;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings;

public class RequestMapper : Profile
{
    public RequestMapper()
    {
        CreateMap<CreateProductCommand, Product>();
        CreateMap<CreateSupplierCommand, Supplier>();
        CreateMap<CreateWarehouseItemCommand, WarehouseItem>();
    }
}