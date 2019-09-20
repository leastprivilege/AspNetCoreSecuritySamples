using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientCredentialsFlow
{
    class Program
    {
        static DiscoveryCache Cache = new DiscoveryCache("https://demo.identityserver.io");

        static async Task Main(string[] args)
        {
            var disco = await Cache.GetAsync();
            if (disco.IsError) throw new Exception(disco.Error);

            var tokenClient = new HttpClient();
            var tokenResponse = await tokenClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret"
            });

            if (tokenResponse.IsError) throw new Exception(tokenResponse.Error);

            // call API
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            Console.WriteLine(await apiClient.GetStringAsync("https://demo.identityserver.io/api/test"));
        }
    }
}
