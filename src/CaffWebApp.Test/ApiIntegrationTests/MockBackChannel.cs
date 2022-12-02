using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaffWebApp.Test.ApiIntegrationTests;

public class MockBackChannel : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri.AbsoluteUri.Equals("https://localhost:5001/.well-known/openid-configuration"))
        {
            return Task.FromResult(GetOpenIdConfigurationAsResponseMessage("openid-configuration.json"));
        }
        if (request.RequestUri.AbsoluteUri.Equals("https://localhost:5001/.well-known/openid-configuration/jwks"))
        {
            return Task.FromResult(GetOpenIdConfigurationAsResponseMessage("openid-jwks.json"));
        }
        throw new NotImplementedException();
    }

    private HttpResponseMessage GetOpenIdConfigurationAsResponseMessage(string resource)
    {
        string path = $"ApiIntegrationTests/well-known/{resource}";
        using (var s = new StreamReader(path))
        {
            var body = s.ReadToEnd();
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            return new HttpResponseMessage()
            {
                Content = content,
            };
        }
    }
}
