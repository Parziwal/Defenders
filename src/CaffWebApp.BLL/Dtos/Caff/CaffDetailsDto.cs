using CaffWebApp.BLL.Dtos.Comment;
using CaffWebApp.BLL.Dtos.User;

namespace CaffWebApp.BLL.Dtos.Caff;

public class CaffDetailsDto
{
    public int Id { get; set; }
    public string CreaterName { get; set; } = default!;
    public int AnimationDuration { get; set; }
    public string FileName { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
    public UserDto UploadedBy { get; set; } = default!;
    public DateTimeOffset UploadedAt { get; set; }
    public ICollection<CiffDto> CiffImages { get; set; } = default!;
    public ICollection<CommentDto> Comments { get; set; } = default!;
}
