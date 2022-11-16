namespace CaffWebApp.BLL.Dtos;

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
}
