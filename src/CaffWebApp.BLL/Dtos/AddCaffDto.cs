using Microsoft.AspNetCore.Http;

namespace CaffWebApp.BLL.Dtos;

public class AddCaffDto
{
    public IFormFile CaffFile { get; set; } = default!;
}
