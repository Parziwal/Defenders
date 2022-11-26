using CaffWebApp.BLL.Dtos.Caff;
using CaffWebApp.BLL.Dtos.Parser;

namespace CaffWebApp.BLL.Services.Parser;

public interface IParserService
{
    Task<CaffParsedDto> ParseCaffFile(AddCaffDto caffDto);
    Task<byte[]> GetCaffFileContent(string fileName);
}
