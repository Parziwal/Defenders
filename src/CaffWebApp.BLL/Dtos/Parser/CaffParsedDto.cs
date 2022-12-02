namespace CaffWebApp.BLL.Dtos.Parser;

public class CaffParsedDto
{
    public string StoredFileName { get; set; } = default!;
    public string OriginalFileName { get; set; } = default!;
    public string CreatorName { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
    public List<CiffParsedDto> CiffData { get; set; } = new();
}
