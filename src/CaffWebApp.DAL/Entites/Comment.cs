namespace CaffWebApp.DAL.Entites;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; } = default!;
    public Caff CaffImage { get; set; } = default!;
    public ApplicationUser CreatedBy { get; set; } = default!;
    public DateTimeOffset CreateAt { get; set; }
}
