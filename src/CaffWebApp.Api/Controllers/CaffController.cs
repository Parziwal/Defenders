using CaffWebApp.BLL.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CaffWebApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaffController : ControllerBase
    {
        [HttpGet]
        public Task<List<CaffDto>> ListCaffImages([FromQuery] CaffFilterDto filter)
        {
            return default!;
        }

        [HttpGet("{caffId}")]
        public Task<CaffDetailsDto> GetCaffDetails(int caffId)
        {
            return default!;
        }

        [HttpGet("{caffId}/download")]
        public Task<FileResult> DownloadCaff(int caffId)
        {
            return default!;
        }

        [HttpPost("upload")]
        public Task UploadCaffFile([FromForm] AddCaffDto caffDto)
        {
            return default!;
        }

        [HttpDelete("{caffId}")]
        public Task DeleteCaff(int caffId)
        {
            return default!;
        }
    }
}
