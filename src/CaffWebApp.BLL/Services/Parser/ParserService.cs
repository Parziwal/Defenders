using CaffWebApp.BLL.Dtos.Caff;
using CaffWebApp.BLL.Dtos.Parser;
using CaffWebApp.BLL.Options;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;

namespace CaffWebApp.BLL.Services.Parser;

public class ParserService : IParserService
{
    private readonly CaffImagePathOptions _imagePath;

    private const string _parserPath = "native/CaffWebApp.Parser.dll";

    [DllImport(_parserPath, CallingConvention = CallingConvention.Cdecl)]
    private static extern int AddNumber(int a, int b);

    public ParserService(IOptions<CaffImagePathOptions> imagePath)
    {
        this._imagePath = imagePath.Value;
    }

    public async Task<byte[]> GetCaffFileContent(string fileName)
    {
        return await File.ReadAllBytesAsync(Path.Combine(_imagePath.Raw, fileName));
    }

    public async Task<CaffParsedDto> ParseCaffFile(AddCaffDto caffDto)
    {
        var a = AddNumber(10, 11);

        return new CaffParsedDto()
        {
            FileName = "Test",
            CreaterName = "Test",
            CreatedAt = DateTimeOffset.Now,
            AnimationDuration = a,
        };
    }
}
