using Microsoft.AspNetCore.Http;

namespace CaffWebApp.BLL.Dtos.Caff;

public class AddCaffDto
{
    public IFormFile CaffFile { get; set; } = default!;
}
