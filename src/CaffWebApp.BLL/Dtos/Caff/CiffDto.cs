namespace CaffWebApp.BLL.Dtos.Caff;

public class CiffDto
{
    public string Caption { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public List<string> Tags { get; set; }

    public CiffDto() { }

    public CiffDto(DAL.Entites.Ciff entity)
    {
        Caption = entity.Caption;
        Width = entity.Width;
        Height = entity.Height;
        Tags = entity.Tags.Split(',').ToList();
    }
}
