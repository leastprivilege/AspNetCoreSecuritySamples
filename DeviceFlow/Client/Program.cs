// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace Client
{
    class Program
    {
        static readonly IDiscoveryCache _cache = new DiscoveryCache("https://demo.identityserver.io");

        public static async Task Main()
        {
            Console.Title = "Console Device Flow";

            var authorizeResponse = await RequestAuthorizationAsync();

            var tokenResponse = await RequestTokenAsync(authorizeResponse);
            Console.WriteLine(tokenResponse.AccessToken);

            Console.ReadLine();
            await CallServiceAsync(tokenResponse.AccessToken);
        }

        static async Task<DeviceAuthorizationResponse> RequestAuthorizationAsync()
        {
            var disco = await _cache.GetAsync();
            if (disco.IsError) throw new Exception(disco.Error);

            var client = new HttpClient();
            var response = await client.RequestDeviceAuthorizationAsync(new DeviceAuthorizationRequest
            {
                Address = disco.DeviceAuthorizationEndpoint,
                ClientId = "device"
            });

            if (response.IsError) throw new Exception(response.Error);

            Console.WriteLine($"user code   : {response.UserCode}");
            Console.WriteLine($"device code : {response.DeviceCode}");
            Console.WriteLine($"URL         : {response.VerificationUri}");
            Console.WriteLine($"Complete URL: {response.VerificationUriComplete}");

            Console.WriteLine($"\nPress enter to launch browser ({response.VerificationUri})");
            Console.ReadLine();

            Process.Start(new ProcessStartInfo(response.VerificationUri) { UseShellExecute = true });
            return response;
        }

        private static async Task<TokenResponse> RequestTokenAsync(DeviceAuthorizationResponse authorizeResponse)
        {
            var disco = await _cache.GetAsync();
            if (disco.IsError) throw new Exception(disco.Error);

            var client = new HttpClient();

            while (true)
            {
                var response = await client.RequestDeviceTokenAsync(new DeviceTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "device",
                    DeviceCode = authorizeResponse.DeviceCode
                });

                if (response.IsError)
                {
                    if (response.Error == OidcConstants.TokenErrors.AuthorizationPending || response.Error == OidcConstants.TokenErrors.SlowDown)
                    {
                        Console.WriteLine($"{response.Error}...waiting.");
                        await Task.Delay(authorizeResponse.Interval * 1000);
                    }
                    else
                    {
                        throw new Exception(response.Error);
                    }
                }
                else
                {
                    return response;
                }
            }
        }

        static async Task CallServiceAsync(string token)
        {
            var client = new HttpClient();
            
            client.SetBearerToken(token);
            var response = await client.GetStringAsync("https://demo.identityserver.io/api/test");

            Console.WriteLine(JArray.Parse(response));
        }
    }
}