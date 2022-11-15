namespace CaffWebApp.Api.Options;

public class CaffWebApiOptions
{
    required public string BaseUrl { get; set; }
    required public string Audience { get; set; }
    required public string Authority { get; set; }
    required public string ApiScope { get; set; }
    required public string ApiScopeDisplayName { get; set; }
    required public string SwaggerClientId { get; set; }
}
