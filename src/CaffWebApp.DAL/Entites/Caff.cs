using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaffWebApp.DAL.Entites;

public class Caff
{
    public int Id { get; set; }
    public string CreatorName { get; set; } = default!;
    public int AnimationDuration { get; set; }
    public string OriginalFileName { get; set; } = default!;
    public string StoredFileName { get; set; } = default!;
    public DateTimeOffset CreatedAt { get; set; }
    public ApplicationUser UploadedBy { get; set; } = default!;
    public DateTimeOffset UploadedAt { get; set; }
    public ICollection<Ciff> CiffImages { get; set; } = default!;
    public ICollection<Comment> Comments { get; set; } = default!;
}
