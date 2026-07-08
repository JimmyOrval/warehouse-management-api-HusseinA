using Application.DTOs;
using Domain.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.Products.Commands.UploadProductImage;

public class UploadProductImageCommandHandler(IProductRepository productRepository)
    : IRequestHandler<UploadProductImageCommand, ProductImageDto>
{
    public async Task<ProductImageDto> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        // check if product exists first
        var product = productRepository.GetById(request.ProductId);
        
        if (product == null)
            throw new KeyNotFoundException("Product not found");
        
        // if file is invalid
        if(request.ImageLength == 0)
            throw new ArgumentException("No image was provided");
        
        // set image size limit
        const long maxFileSize = 2 * 1024 * 1024;
        
        // check if image size exceeds the limit
        if (request.ImageLength > maxFileSize)
            throw new ArgumentException("Image size cannot exceed 2MB");
        
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
        
        return new ProductImageDto(
            productImage.Id, productImage.ProductId, productImage.FileName, productImage.FilePath);
    }
}