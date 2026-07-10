using Application.ViewModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings;

public class WarehouseItemMapper : Profile
{
    public WarehouseItemMapper()
    {
        CreateMap<WarehouseItem, WarehouseItemViewModel>()
            .ForMember(
                dest => dest.ProductName,
                opt =>
                    opt.MapFrom(src => src.Product!.Name));
    }
}