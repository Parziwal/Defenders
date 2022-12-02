namespace CaffWebApp.DAL.Entites;

public class Ciff
{
    public int Id { get; set; }
    public string Caption { get; set; } = default!;
    public int Width { get; set; }
    public int Height { get; set; }
    public string Tags { get; set; } = default!;
    public int Duration { get; set; }
    public int CaffImageId { get; set; }
    public Caff CaffImage { get; set; } = default!;
}
