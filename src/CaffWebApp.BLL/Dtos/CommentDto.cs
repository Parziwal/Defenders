namespace CaffWebApp.BLL.Dtos;

public class CommentDto
{
    public int Id { get; set; }
    public string Text { get; set; } = default!;
    public string CreatedBy { get; set; } = default!;
    public DateTimeOffset CreateAt { get; set; }
}
