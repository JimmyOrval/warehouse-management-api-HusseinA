using Application.ViewModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings;

public class ProductMapper : Profile
{
    public ProductMapper()
    {
        CreateMap<Product, ProductViewModel>()
            .ForMember(
                dest => dest.SupplierName,
                opt =>
                    opt.MapFrom(src => src.Supplier != null
                    ? src.Supplier.Name
                    : null));
    }
}