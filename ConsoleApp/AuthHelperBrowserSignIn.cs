using IdentityModel.OidcClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using Zebpay.Model.Response;

namespace ZebClient.Consumer.ConsoleApp
{
    public static class AuthHelperBrowserSignIn
    {
        static string _authority;
        static string _api;
        static OidcClient _oidcClient;
        static HttpClient _apiClient;

        public static AuthResponse SignIn(IConfiguration configuration)
        {
            _api = configuration["ZebpaySettings:BaseApiUrl"];
            _authority = configuration["ZebpaySettings:BaseAuthUrl"];
            _apiClient = new HttpClient { BaseAddress = new Uri(_api) };
            // create a redirect URI using an available port on the loopback address.
            // requires the OP to allow random ports on 127.0.0.1 - otherwise set a static port
            var browser = new CustomBrowser(4999);
            string redirectUri = string.Format($"http://127.0.0.1:{browser.Port}");

            var options = new OidcClientOptions
            {
                Authority = _authority,
                //ClientId = "ac236908-e732-4efd-9b9a-12d1b3048572",
                //ClientSecret = "7744b300-cbf0-4ac9-90b6-9990d105bfe4",
                ClientId = Program.clientId,//"cbb35552-1d7a-4b0f-a45c-8fcee6342f93",
                ClientSecret = Program.clientSecret,//"8876cc4a-12aa-4c17-888f-fc99b8e53d62",
                RedirectUri = redirectUri,
                Scope = Program.zebpayApiScopes,//"openid profile wallet:transactions:read trade:read trade:create",
                FilterClaims = false,
                Browser = browser,
                PostLogoutRedirectUri = redirectUri,
                Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect
            };

            _oidcClient = new OidcClient(options);
            var loginRequest = new LoginRequest { BrowserDisplayMode = IdentityModel.OidcClient.Browser.DisplayMode.Hidden, BrowserTimeout = 10 };
            var result = _oidcClient.LoginAsync(loginRequest).Result;

            ShowResult(result);
            return NextSteps(result);
        }

        private static void ShowResult(LoginResult result)
        {
            if (result.IsError)
            {
                Console.WriteLine("\n\nError:\n{0}", result.Error);
                return;
            }

            Console.WriteLine("\n\nClaims:");
            foreach (var claim in result.User.Claims)
            {
                Console.WriteLine("{0}: {1}", claim.Type, claim.Value);
            }

            Console.WriteLine($"\nidentity token: {result.IdentityToken}");
            Console.WriteLine($"access token:   {result.AccessToken}");
            Console.WriteLine($"refresh token:  {result?.RefreshToken ?? "none"}");
        }

        private static AuthResponse NextSteps(LoginResult result)
        {
            var currentAccessToken = result.AccessToken;
            var currentRefreshToken = result.RefreshToken;

            return new AuthResponse { access_token = result.AccessToken, refresh_token = result.RefreshToken, expires_in = result.AccessTokenExpiration.ToLongTimeString() };

            //var menu = "  x...exit  c...call api   ";
            //if (currentRefreshToken != null) menu += "r...refresh token   ";

            //while (true)
            //{
            //    Console.WriteLine("\n\n");

            //    Console.Write(menu);
            //    var key = Console.ReadKey();

            //    if (key.Key == ConsoleKey.X) return;
            //    //if (key.Key == ConsoleKey.C) await CallApi(currentAccessToken);
            //    //if (key.Key == ConsoleKey.L) await LogOff();
            //    if (key.Key == ConsoleKey.R)
            //    {
            //        var refreshResult = await _oidcClient.RefreshTokenAsync(currentRefreshToken);
            //        if (result.IsError)
            //        {
            //            Console.WriteLine($"Error: {refreshResult.Error}");
            //        }
            //        else
            //        {
            //            currentRefreshToken = refreshResult.RefreshToken;
            //            currentAccessToken = refreshResult.AccessToken;

            //            Console.WriteLine("\n\n");
            //            Console.WriteLine($"access token:   {result.AccessToken}");
            //            Console.WriteLine($"refresh token:  {result?.RefreshToken ?? "none"}");
            //        }
            //    }
            //}
        }
    }
}
