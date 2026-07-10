using Application.ViewModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings;

public class SupplierMapper : Profile
{
    public SupplierMapper()
    {
        CreateMap<Supplier, SupplierViewModel>();
    }
}