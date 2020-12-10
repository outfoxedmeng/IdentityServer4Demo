using HybridMvcClient.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IdpMvc.Controllers
{
    [Authorize(Roles = "管理员,普通用户")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {

            var r = User.IsInRole("管理员");
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");

            if (!disco.IsError)
            {
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "console client",
                    ClientSecret = "api-secret",
                    Scope = "api1"
                });
                if (!tokenResponse.IsError)
                {
                    var apiClient = new HttpClient();

                    apiClient.SetBearerToken(tokenResponse.AccessToken);//在请求头中，带上AccessToken

                    var response = await apiClient.GetAsync("https://localhost:6001/api/identity");//HttpGet

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        return View((object)content);
                    }

                }
            }
            return View();
        }

        //[Authorize(Roles ="管理员")]
        [Authorize(Policy = "SmithInSomewhere")]
        public async Task<IActionResult> Privacy()
        {
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var idToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            //此处无法获取code，可以浏览器设置preserve log后，从signin-odic请求的FormData中查看code
            //var code = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.Code);

            var properties = (await HttpContext.AuthenticateAsync()).Properties.Items;


            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Logout()
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }

    }
}
