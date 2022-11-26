using CaffWebApp.BLL.Exceptions;
using Hellang.Middleware.ProblemDetails;

namespace CaffWebApp.Api.ProblemDetails;

public static class ProblemDetailsExtensions
{
    public static IServiceCollection AddCaffWebAppProblemDetails(this IServiceCollection services) =>
        services.AddProblemDetails(options =>
        {
            options.Map<EntityNotFoundException>((context, exception) =>
            {
                var problemDetails = StatusCodeProblemDetails.Create(StatusCodes.Status404NotFound);
                problemDetails.Title = exception.Message;
                return problemDetails;
            });
        });
}
