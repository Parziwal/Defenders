using IdentityModel.Client;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CaffWebApp.Test.ApiIntegrationTests;

public static class TestExtensions
{
    public static string ToQueryParams(this Object o)
    {
        var query = o.GetType().GetProperties()
           .Where(s => s.GetValue(o, null) != null)
           .Select(s => s.Name + "=" + HttpUtility.UrlEncode(s.GetValue(o, null).ToString()));

        return String.Join("&", query.ToArray());
    }


    public static async Task<string> GetAccessToken(this TestServer authServer, string userName, string password)
    {
        var authClient = authServer.CreateClient();
        var discovery = await authClient.GetDiscoveryDocumentAsync();
        var response = await authClient.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = discovery.TokenEndpoint,
            ClientId = "caff.test.client",
            Scope = "openid profile email caffwebapp.api role",
            UserName = userName,
            Password = password
        });

        if (response.HttpStatusCode != HttpStatusCode.OK) return null;

        return response.AccessToken;
    }
}
