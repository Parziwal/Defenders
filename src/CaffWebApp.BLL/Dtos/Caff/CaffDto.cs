namespace CaffWebApp.BLL.Dtos.Caff;

public class CaffDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = default!;
    public string CreatorName { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
    public string UploadedBy { get; set; } = default!;
    public DateTimeOffset UploadedAt { get; set; }
    public ICollection<string> Captions { get; set; } = default!;
    public ICollection<string> Tags { get; set; } = default!;

    public CaffDto(DAL.Entites.Caff entity)
    {
        FileName = entity.StoredFileName;
        CreatorName = entity.CreatorName;
        CreatedAt = entity.CreatedAt;
        UploadedBy = entity.UploadedBy.Fullname;
        UploadedAt = entity.UploadedAt;
        Captions = entity.CiffImages.Select(ciff => ciff.Caption).ToList();
        Tags = entity.CiffImages.SelectMany(ciff => ciff.Tags.Split(',')).ToHashSet();
    }
}
