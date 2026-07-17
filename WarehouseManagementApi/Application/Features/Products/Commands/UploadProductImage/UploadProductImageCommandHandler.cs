using Application.ViewModels;
using AutoMapper;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Products.Commands.UploadProductImage;

public class UploadProductImageCommandHandler(IProductRepository productRepository, IMapper mapper)
    : IRequestHandler<UploadProductImageCommand, ProductImageViewModel>
{
    public async Task<ProductImageViewModel> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        // check if product exists first
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        
        if (product == null)
            throw new NotFoundException($"Product '{request.ProductId}' not found");
        
        /* NOW IS DONE INSIDE VALIDATOR
        // if file is invalid
        if(request.ImageLength == 0)
            throw new ArgumentException("No image was provided");
        
        // set image size limit
        const long maxFileSize = 2 * 1024 * 1024;
        
        // check if image size exceeds the limit
        if (request.ImageLength > maxFileSize)
            throw new ArgumentException("Image size cannot exceed 2MB");
        */
        
        // get the image's extension
        var extension = Path.GetExtension(request.FileName).ToLower();
        
        // check if extension is valid
        if (!extension.Contains("png") && !extension.Contains("jpg"))
        {
            throw new ArgumentException("Image extension invalid. Use only .jpg or .png");
        }
        
        // set upload directory
        var uploadFolderPath = Path.GetFullPath("wwwroot/uploads");

        // if directory doesn't exist, create it
        if (!Directory.Exists(uploadFolderPath))
            Directory.CreateDirectory(uploadFolderPath);
        
        // create a unique file name
        var fileName = $"{Guid.NewGuid()}{extension}";
        // combine full path with new file name
        var filePath = Path.Combine(uploadFolderPath, fileName);

        // had to keep manually mapped since FilePath is code-generated not input 
        var productImage = new ProductImage
        {
            Id = Guid.NewGuid().ToString(),
            ProductId = product.Id,
            FileName = fileName,
            FilePath = filePath
        };
        
        // open a file stream in create mode using our new file path
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        // copy the image to the uploads using the file stream
        await request.Image.CopyToAsync(fileStream, cancellationToken);
        
        return mapper.Map<ProductImageViewModel>(productImage);
    }
}