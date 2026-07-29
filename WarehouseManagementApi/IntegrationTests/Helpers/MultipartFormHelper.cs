using System.Net.Http.Headers;

namespace IntegrationTests.Helpers;

// when uploading a file, it gets sent in multiple parts
// usually, ef core merges these parts with IFormFile
// but since this is fake data, http client won't
// do the merging for us, so this class helps with that
public static class MultipartFormHelper
{
    // these are the standard byte sequences to represent each file type
    private static readonly byte[] JpegHeader = [0xFF, 0xD8, 0xFF, 0xE0];
    private static readonly byte[] PngHeader = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] PdfHeader = [0x25, 0x50, 0x44, 0x46, 0x2D];

    // this is what ef core usually does to each part it receives
    public static MultipartFormDataContent CreateFileContent(
        byte[] bytes,
        string fileName,
        string contentType,
        string fieldName = "image")
    {
        var form = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        form.Add(fileContent, fieldName, fileName);
        return form;
    }

    // these 2 methods create jpg and png image files
    public static MultipartFormDataContent JpgImage(
        string fileName = "test.jpg",
        int size = 2048,
        string fieldName = "image")
    {
        return CreateFileContent(BuildBytes(JpegHeader, size),
            fileName, "image/jpg", fieldName);
    }

    public static MultipartFormDataContent PngImage(
        string fileName = "test.png",
        int size = 2048,
        string fieldName = "image")
    {
        return CreateFileContent(BuildBytes(PngHeader, size),
            fileName, "image/png", fieldName);
    }

    // creates txt file
    public static MultipartFormDataContent TextDocument(
        string fileName = "test.txt",
        string body = "test txt file",
        string fieldName = "image")
    {
        // no sequence found for txt files
        return CreateFileContent(System.Text.Encoding.UTF8.GetBytes(body),
            fileName, "text/plain", fieldName);
    }

    // 3 methods used to test invalid cases
    public static MultipartFormDataContent InvalidExtensionFile(
        string fileName = "test.txt",
        string fieldName = "image")
    {
        return TextDocument(fileName, "not an image", fieldName);
    }

    public static MultipartFormDataContent OversizedImage(
        string fileName = "test.jpg",
        string fieldName = "image")
    {
        return CreateFileContent(
            BuildBytes(JpegHeader, 3 * 1024 * 1024),
            fileName,
            "image/jpg",
            fieldName);
    }

    public static MultipartFormDataContent WrongContentType(
        string fileName = "test.jpg",
        string fieldName = "image")
    {
        return CreateFileContent(BuildBytes(JpegHeader, 1024),
            fileName, "application/pdf", fieldName);
    }
    
    private static byte[] BuildBytes(byte[] header, int totalSize)
    {
        // if we only got the header with no extra data
        // set the size so we can at least fit the header
        if (totalSize < header.Length)
        {
            totalSize = header.Length;
        }

        // assign first index for the header
        var bytes = new byte[totalSize];
        header.CopyTo(bytes, 0);
        
        // fill the rest of the indexes so they're not null
        for (var i = header.Length; i < totalSize; i++)
        {
            bytes[i] = (byte)(i % 251);
        }

        return bytes;
    }
}
