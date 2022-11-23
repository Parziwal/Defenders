namespace CaffWebApp.BLL.Dtos.Parser;

public class CaffParsedDto
{
    public string FileName { get; set; } = default!;
    public string CreaterName { get; set; } = default!;
    public int AnimationDuration { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<CiffParsedDto> CiffData { get; set; } = default!;
}
