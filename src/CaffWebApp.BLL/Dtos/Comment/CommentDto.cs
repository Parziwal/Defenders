namespace CaffWebApp.BLL.Dtos.Comment;

public class CommentDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset CreateAt { get; set; }

    public CommentDto(DAL.Entites.Comment entity)
    {
        Id = entity.Id;
        Text = entity.Text;
        CreatedBy = entity.CreatedBy.Fullname;
        CreateAt = entity.CreateAt;
    }
}
