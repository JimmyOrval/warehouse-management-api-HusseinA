using System.IO.IsolatedStorage;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Infrastructure.Storage;

public class MinIoStorageService(
    IMinioClient client,
    IOptions<MinIoStorage> options)
    : IFileStorageService
{
    private readonly string _bucketName = options.Value.BucketName;
    
    public async Task<UploadedFileInfo> UploadAsync(Stream content, string originalFileName, string contentType, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(originalFileName);
        var objectKey = $"{Guid.NewGuid():N}{extension}";

        try
        {
            var putArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectKey)
                .WithContentType(contentType)
                .WithStreamData(content)
                .WithObjectSize(content.Length);

            await client.PutObjectAsync(putArgs, cancellationToken);
        }
        catch (MinioException e)
        {
            throw new StorageException("Failed to upload file to storage");
        }
        
        return new UploadedFileInfo(objectKey, originalFileName, contentType, content.Length);
    }

    public async Task<(Stream Content, string contentType)> DownloadAsync(string objectKey, CancellationToken cancellationToken)
    {
        var memoryStream = new MemoryStream();
        string contentType;

        try
        {
            var statArgs = new StatObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectKey);
            var stat = await client.StatObjectAsync(statArgs, cancellationToken);
            contentType = stat.ContentType;

            var getArgs = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectKey)
                .WithCallbackStream((stream, ct) => stream.CopyToAsync(memoryStream, ct));

            await client.GetObjectAsync(getArgs, cancellationToken);
        }
        catch (ObjectNotFoundException)
        {
            throw new NotFoundException($"File {objectKey} not found");
        }
        catch (MinioException e)
        {
            throw new StorageException("Failed to download file");
        }

        memoryStream.Position = 0;
        return(memoryStream, contentType);
    }

    public async Task DeleteAsync(string objectKey, CancellationToken cancellationToken)
    {
        try
        {
            var args = new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectKey);
            await client.RemoveObjectAsync(args, cancellationToken);
        }
        catch(MinioException e)
        {
            throw new StorageException("Failed to delete file");
        }
    }
}