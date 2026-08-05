using System.Collections.Concurrent;
using Domain.Interfaces;

namespace IntegrationTests.Helpers.Dependencies;

// this replaces the need to upload/download from a real storage
public class InMemoryFileStorageService : IFileStorageService
{
    private readonly ConcurrentDictionary<string,
        (byte[] Data, string ContentType)> _store = new();

    // takes a fake stream
    public async Task<UploadedFileInfo> UploadAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        // create the proper object and store its bytes
        var key = Guid.NewGuid().ToString();
        using var stream = new MemoryStream();
        await content.CopyToAsync(stream, cancellationToken);
        var bytes = stream.ToArray();
        // keeps it in fake in-memory storage so we can use it for downloads/deletes
        _store[key] = (bytes, contentType);
        
        // return info of the file just created
        return new UploadedFileInfo
            (key, fileName, contentType, bytes.Length);
    }

    // 
    public Task<(Stream Content, string contentType)> DownloadAsync(
        string objectKey,
        CancellationToken cancellationToken)
    {
        // use the key to get the file's data from in-memory storage
        var (data, contentType) = _store[objectKey];
        // return the file's stream and content type
        return Task.FromResult<(Stream, string)>(
            (new MemoryStream(data), contentType));
    }
        
    public Task DeleteAsync(string objectKey, CancellationToken cancellationToken)
    {
        // simply removes file from in-memry storage
        _store.TryRemove(objectKey, out _);
        return Task.CompletedTask;
    }
}