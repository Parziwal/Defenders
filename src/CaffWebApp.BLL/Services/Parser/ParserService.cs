using CaffWebApp.BLL.Dtos.Caff;
using CaffWebApp.BLL.Dtos.Parser;
using CaffWebApp.BLL.Options;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;

namespace CaffWebApp.BLL.Services.Parser;

public class ParserService : IParserService
{
    private readonly CaffImagePathOptions _imagePath;

    [DllImport("native/CaffWebApp.Parser.dll", CallingConvention = CallingConvention.Cdecl)]
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
        var parsedCaff = new CaffParsedDto()
        {
            StoredFileName = $"{Guid.NewGuid()}.caff",
            OriginalFileName = caffDto.CaffFile.FileName,
            CreaterName = "Test",
            CreatedAt = DateTimeOffset.Now,
            AnimationDuration = 10,
        };
        
        string filePath = Path.Combine(_imagePath.Raw, parsedCaff.StoredFileName);
        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
        {
            await caffDto.CaffFile.CopyToAsync(fileStream);
        }
        
        var a = AddNumber(10, 11);

        return parsedCaff;
    }
}
