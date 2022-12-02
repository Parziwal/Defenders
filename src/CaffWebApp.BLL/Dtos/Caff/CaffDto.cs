namespace CaffWebApp.BLL.Dtos.Caff;

public class CaffDto
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string CreatorName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string UploadedBy { get; set; }
    public DateTimeOffset UploadedAt { get; set; }
    public ICollection<string> Captions { get; set; }
    public ICollection<string> Tags { get; set; }

    public CaffDto(DAL.Entites.Caff entity)
    {
        Id = entity.Id;
        FileName = entity.StoredFileName;
        CreatorName = entity.CreatorName;
        CreatedAt = entity.CreatedAt;
        UploadedBy = entity.UploadedBy.Fullname;
        UploadedAt = entity.UploadedAt;
        Captions = entity.CiffImages.Select(ciff => ciff.Caption).ToHashSet();
        Tags = entity.CiffImages.SelectMany(ciff => ciff.Tags.Split(',')).ToHashSet();
    }
}
