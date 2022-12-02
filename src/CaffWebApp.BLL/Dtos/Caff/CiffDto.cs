namespace CaffWebApp.BLL.Dtos.Caff;

public class CiffDto
{
    public string Caption { get; set; } = default!;
    public int Width { get; set; }
    public int Height { get; set; }
    public int Duration { get; set; }
    public List<string> Tags { get; set; } = default!;

    public CiffDto() { }

    public CiffDto(DAL.Entites.Ciff entity)
    {
        Caption = entity.Caption;
        Duration = entity.Duration;
        Width = entity.Width;
        Height = entity.Height;
        Tags = entity.Tags.Split(',').ToList();
    }
}
