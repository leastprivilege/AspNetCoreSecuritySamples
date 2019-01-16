// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var protocolClient = new HttpClient();

            var disco = await protocolClient.GetDiscoveryDocumentAsync("https://demo.identityserver.io");
            if (disco.IsError) throw new Exception(disco.Error);

            var tokenResponse = await protocolClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret"
            });

            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            Console.WriteLine(await apiClient.GetStringAsync("http://localhost:3308/test"));
        }
    }
}