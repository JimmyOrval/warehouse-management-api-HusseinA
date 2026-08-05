using Application.ViewModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings;

public class ShipmentMapper : Profile
{
    public ShipmentMapper()
    {
        CreateMap<Shipment, ShipmentViewModel>()
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null));

        CreateMap<ShipmentItem, ShipmentItemViewModel>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));
    }
}
