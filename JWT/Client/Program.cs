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
            var disco = await DiscoveryClient.GetAsync("https://demo.identityserver.io");
            if (disco.IsError) throw new Exception(disco.Error);

            var tokenClient = new TokenClient(
                disco.TokenEndpoint,
                "client",
                "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync();

            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            Console.WriteLine(await apiClient.GetStringAsync("http://localhost:3308/test"));
        }
    }
}