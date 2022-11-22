using CaffWebApp.BLL.Services.Caff;
using CaffWebApp.BLL.Services.Comment;
using CaffWebApp.BLL.Services.Parser;
using CaffWebApp.BLL.Services.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CaffWebApp.BLL;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCaffBll(this IServiceCollection services)
    {
        services.AddScoped<ICaffService, CaffService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IParserService, ParserService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
