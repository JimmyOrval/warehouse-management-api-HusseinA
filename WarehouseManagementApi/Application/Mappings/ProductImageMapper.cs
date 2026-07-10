using Application.ViewModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings;

public class ProductImageMapper : Profile
{
    public  ProductImageMapper()
    {
        CreateMap<ProductImage, ProductImageViewModel>()
            .ForMember(dest => dest.ProductName,
                opt =>
                    opt.MapFrom(src => src.Product!.Name));
    }
}