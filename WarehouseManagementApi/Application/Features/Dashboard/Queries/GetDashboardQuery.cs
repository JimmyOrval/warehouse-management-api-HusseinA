using Application.Common;
using Application.ViewModels;
using MediatR;

namespace Application.Features.Dashboard.Queries;

public record GetDashboardQuery() : IRequest<Result<DashboardViewModel>>;