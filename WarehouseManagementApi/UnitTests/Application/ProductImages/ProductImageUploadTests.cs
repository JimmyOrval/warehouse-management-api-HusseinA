using Application.Features.Products.Commands.UploadProductImage;
using Domain.Interfaces;
using Domain.Models;
using FluentValidation.TestHelper;
using Infrastructure.Storage;
using Microsoft.Extensions.Options;
using Moq;
using Tests.Builders;
using Tests.Helpers;

namespace Tests.Application.ProductImages;

public class ProductImageUploadTests
{
    [Theory]
    [InlineData("key1", "test1.jpg", "image/jpg", 123)]
    [InlineData("key2", "test2.png", "image/png", 456)]
    public async Task Uploading_ValidExtension_Succeeds(
        string objectKey, string fileName, string contentType, int fileSize)
    {
        var product = new ProductBuilder().Build();
        
        var repository = RepositoryMockHelper.MockRepository<IProductRepository>();
        repository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var mockStorage = new Mock<IFileStorageService>();
        mockStorage.Setup(s =>
            s.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UploadedFileInfo(objectKey, fileName, contentType, fileSize));

        var mapper = CommonMocksHelper.MockMapper(cfg =>
        {
            cfg.CreateMap<UploadProductImageCommand, ProductImage>();
        });
        
        var handler = new UploadProductImageCommandHandler(
            repository.Object,
            mockStorage.Object,
            CommonMocksHelper.MockEventPublisher(),
            Mock.Of<IOptions<MinIoStorage>>(o => o.Value == new MinIoStorage()),
            mapper,
            CommonMocksHelper.MockLogger<UploadProductImageCommandHandler>());
        
        var result = await handler.Handle(new UploadProductImageCommand
        (
            product.Id,
            new MemoryStream(),
            fileSize,
            fileName, contentType),
        CancellationToken.None);
        
        Assert.NotNull(result);
        repository.Verify(r => r.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
    
    [Theory]
    [InlineData("test1.pdf", "document/pdf", 123)]
    [InlineData("test2.txt", "document/txt", 456)]
    public void Uploading_InvalidExtension_Fails(
        string fileName, string contentType, int fileSize)
    {
        var product = new ProductBuilder().Build();
        
        var repository = RepositoryMockHelper.MockRepository<IProductRepository>();
        repository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var validator = new UploadProductImageCommandValidator();

        var command = new UploadProductImageCommand
        (
            product.Id,
            new MemoryStream(),
            fileSize,
            fileName,
            contentType);

        var result = validator.TestValidate(command);
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e =>
            e.ErrorMessage == 
            "Content type must be one of: image/jpg, image/png");
    }
    
    [Theory]
    [InlineData("test1.png", "image/png", 5000000)]
    [InlineData("test2.jpg", "image/jpg", 10000000)]
    public void FileSize_GreaterThan2Mb_Fails(
        string fileName, string contentType, int fileSize)
    {
        var product = new ProductBuilder().Build();
        
        var repository = RepositoryMockHelper.MockRepository<IProductRepository>();
        repository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var validator = new UploadProductImageCommandValidator();

        var command = new UploadProductImageCommand
        (
            product.Id,
            new MemoryStream(),
            fileSize,
            fileName,
            contentType);

        var result = validator.TestValidate(command);
        Assert.Contains(result.Errors, e =>
            e.ErrorMessage == 
            "Image size cannot exceed 2MB");
    }
    
    [Fact]
    public void EmptyFileSize_Fails()
    {
        var product = new ProductBuilder().Build();
        
        var repository = RepositoryMockHelper.MockRepository<IProductRepository>();
        repository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var validator = new UploadProductImageCommandValidator();

        var command = new UploadProductImageCommand
        (
            product.Id,
            new MemoryStream(),
            0,
            "image.png",
            "image/png");

        var result = validator.TestValidate(command);
        Assert.Contains(result.Errors, e =>
            e.ErrorMessage == 
            "No image was provided");
    }
    
    [Fact]
    public async Task Uploading_CorrectlyGenerates_Path()
    {
        var product = new ProductBuilder().Build();
        
        var repository = RepositoryMockHelper.MockRepository<IProductRepository>();
        repository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var mockStorage = new Mock<IFileStorageService>();
        mockStorage.Setup(s =>
                s.UploadAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UploadedFileInfo($"{Guid.NewGuid().ToString()}.png", "test.png", "image/png", 100));

        var mapper = CommonMocksHelper.MockMapper(cfg =>
        {
            cfg.CreateMap<UploadProductImageCommand, ProductImage>();
        });
        
        var handler = new UploadProductImageCommandHandler(
            repository.Object,
            mockStorage.Object,
            CommonMocksHelper.MockEventPublisher(),
            Mock.Of<IOptions<MinIoStorage>>(o => o.Value == new MinIoStorage()),
            mapper,
            CommonMocksHelper.MockLogger<UploadProductImageCommandHandler>());
        
        ProductImage capturedImage = null!;
        repository.Setup(r => r.AddImage(It.IsAny<ProductImage>()))
            .Callback<ProductImage>(img => capturedImage = img);
        
        var result = await handler.Handle(new UploadProductImageCommand
            (
                product.Id,
                new MemoryStream(),
                100,
                "test.png",
                "image/png"),
            CancellationToken.None);
        
        
        Assert.NotNull(result);
        Assert.EndsWith(".png", capturedImage.ObjectKey);
        
        var guid = capturedImage.ObjectKey.Replace(".png", "");
        var isValid = Guid.TryParse(guid, out _);
        Assert.True(isValid);
    }
}