using Application.Common;
using MediatR;

namespace Application.Features.Products.Queries.DownloadProductImage;

public record DownloadProductImageQuery(string ImageId)
    : IRequest<DownloadedFileResult>;