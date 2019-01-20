using System.Net.Http;

namespace IdentityModel.AspNetCore
{
    public static class HttpRequestMessageExtensions
    {
        public static void SetToken(this HttpRequestMessage request, string token)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}
