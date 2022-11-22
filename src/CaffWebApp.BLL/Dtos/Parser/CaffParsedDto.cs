namespace CaffWebApp.BLL.Dtos.Parser;

public class CaffParsedDto
{
    public string CreaterName { get; set; } = default!;
    public int AnimationDuration { get; set; }
    public List<CiffParsedDto> CiffData { get; set; } = default!;
}
