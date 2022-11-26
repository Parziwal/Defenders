namespace CaffWebApp.BLL.Options;

public class CaffWebApiOptions
{
    public string BaseUrl { get; set; } = default!;
    public string SwaggerClientId { get; set; } = default!;
    public Dictionary<string, string> SwaggerClientScopes { get; set; } = new();
}
