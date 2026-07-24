namespace Application.Common;

public record DownloadedFileResult(
    Stream Content,
    string ContentType,
    string FileName);