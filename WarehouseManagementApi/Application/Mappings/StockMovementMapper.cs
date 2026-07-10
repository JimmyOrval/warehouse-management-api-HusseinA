using AutoMapper;
using Domain.Models;
using Application.ViewModels;

namespace Application.Mappings;

public class StockMovementMapper : Profile
{
    public StockMovementMapper()
    {
        CreateMap<StockMovement, StockMovementViewModel>()
            .ForMember(dest => dest.WarehouseItemName,
                opt
                    => opt.MapFrom(src => src.WarehouseItem!.Product!.Name));
    }
}