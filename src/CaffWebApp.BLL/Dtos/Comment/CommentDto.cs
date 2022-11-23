namespace CaffWebApp.BLL.Dtos.Comment;

public class CommentDto
{
    public int Id { get; set; }
    public string Text { get; set; } = default!;
    public string CreatedBy { get; set; } = default!;
    public DateTimeOffset CreateAt { get; set; }

    public CommentDto(DAL.Entites.Comment entity)
    {
        Id = entity.Id;
        Text = entity.Text;
        CreatedBy = entity.CreatedBy.UserName;
        CreateAt = entity.CreateAt;
    }
}
