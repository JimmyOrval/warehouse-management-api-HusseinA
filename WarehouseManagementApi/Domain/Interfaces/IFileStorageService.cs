namespace Domain.Interfaces;

public record UploadedFileInfo(string ObjectKey, string FileName, string ContentType, long Size);

public interface IFileStorageService
{
    Task<UploadedFileInfo> UploadAsync(
        Stream content, string originalFileName, string contentType, CancellationToken cancellationToken);
    
    Task<(Stream Content, string contentType)> DownloadAsync(
        string objectKey, CancellationToken cancellationToken);
    
    Task DeleteAsync(string objectKey, CancellationToken cancellationToken);
}