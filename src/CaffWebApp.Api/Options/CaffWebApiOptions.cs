namespace CaffWebApp.Api.Options;

public class CaffWebApiOptions
{
    public string BaseUrl { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string Authority { get; set; } = default!;
    public string ApiScope { get; set; } = default!;
    public string ApiScopeDisplayName { get; set; } = default!;
    public string SwaggerClientId { get; set; } = default!;
}
