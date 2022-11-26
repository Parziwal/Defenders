using CaffWebApp.BLL.Options;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace CaffWebApp.Api.StaticFile;

public static class StaticFilesExtensions
{
    public static IApplicationBuilder UseCaffStaticFiles(this IApplicationBuilder app)
    {
        var imagePath = app.ApplicationServices.GetService<IOptions<CaffImagePathOptions>>()!.Value;
        Directory.CreateDirectory(imagePath.Raw);
        Directory.CreateDirectory(imagePath.Parsed);

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.GetFullPath("images")),
            RequestPath = "/images/raw",
        });

        return app;
    }
}
