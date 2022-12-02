using CaffWebApp.BLL.Dtos.Comment;
using CaffWebApp.BLL.Dtos.User;

namespace CaffWebApp.BLL.Dtos.Caff;

public class CaffDetailsDto
{
    public int Id { get; set; }
    public string CreatorName { get; set; }
    public string FileName { get; set; }
    public string FileUri { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public UserDto UploadedBy { get; set; } = default!;
    public DateTimeOffset UploadedAt { get; set; }
    public ICollection<CiffDto> CiffImages { get; set; } = default!; 
    public ICollection<CommentDto> Comments { get; set; } = default!;

    public CaffDetailsDto()
    {
    }

    public CaffDetailsDto(DAL.Entites.Caff entity)
    {
        Id = entity.Id;
        FileName = entity.OriginalFileName;
        FileUri = entity.StoredFileName + ".gif";
        CreatorName = entity.CreatorName;
        CreatedAt = entity.CreatedAt;
        UploadedBy = new UserDto(entity.UploadedBy);
        UploadedAt = entity.UploadedAt;
        CiffImages = entity.CiffImages?.Select(ciff => new CiffDto(ciff)).ToList() ?? new();
        Comments = entity.Comments?.Select(comments => new CommentDto(comments)).ToList() ?? new();
    }
}
