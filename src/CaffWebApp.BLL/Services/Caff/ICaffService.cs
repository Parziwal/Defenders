using CaffWebApp.BLL.Dtos.Caff;

namespace CaffWebApp.BLL.Services.Caff;

public interface ICaffService
{
    Task<List<CaffDto>> ListCaffImages(CaffFilterDto filter);
    Task<CaffDetailsDto> GetCaffDetails(int caffId);
    Task<FileResultDto> DownloadCaff(int caffId);
    Task<CaffDetailsDto> UploadCaffFile(AddCaffDto caffDto);
    Task DeleteCaff(int caddId);
}
