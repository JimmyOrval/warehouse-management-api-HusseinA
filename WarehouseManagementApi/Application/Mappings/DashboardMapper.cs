using Application.Common;
using AutoMapper;
using Application.ViewModels;

namespace Application.Mappings;

public class DashboardMapper : Profile
{
    public DashboardMapper()
    {
        CreateMap<DashboardViewModel, Result<DashboardViewModel>>();
    }
}