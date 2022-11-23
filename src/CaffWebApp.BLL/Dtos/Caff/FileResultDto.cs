namespace CaffWebApp.BLL.Dtos.Caff;

public class FileResultDto
{
    public FileResultDto(byte[] content, string fileNameWithExtension, string mimeType)
    {
        Content = content;
        FileName = fileNameWithExtension;
        MimeType = mimeType;
    }

    public byte[] Content { get; }

    public string FileName { get; }

    public string MimeType { get; }
}