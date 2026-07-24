using Application.ViewModels;
using AutoMapper;
using Domain.Models;

namespace Application.Mappers;

public class NotificationMapper : Profile
{
    public NotificationMapper()
    {
        CreateMap<Notification, NotificationViewModel>();
    }
}