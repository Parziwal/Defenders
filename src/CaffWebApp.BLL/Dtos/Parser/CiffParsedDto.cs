namespace CaffWebApp.BLL.Dtos.Parser;

public class CiffParsedDto
{
    public string Caption { get; set; } = default!;
    public int Width { get; set; }
    public int Height { get; set; }
    public string Tags { get; set; } = default!;
    public int Duration { get; set; }
}
