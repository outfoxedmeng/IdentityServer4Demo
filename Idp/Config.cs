// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Idp
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(), //必须
                new IdentityResources.Profile(),

                new IdentityResources.Address(),
                new IdentityResources.Phone(),
                new IdentityResources.Email(),


                new IdentityResource("roles","角色",new List<string>{ JwtClaimTypes.Role}),

                new IdentityResource("location","地点",new List<string>{ "location" })

                //new IdentityResource(
                //    name: "openid",
                //    userClaims: new[] { "sub" },
                //    displayName: "Your user identifier"
                //),
                //new IdentityResource(
                //    name: "profile",
                //    userClaims: new[] { "name", "email", "website" },
                //    displayName: "Your profile data"
                //    )
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("api1"),
                new ApiScope("My API"),
                
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "console client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("api-secret".Sha256()) },

                    AllowedScopes = {
                        "api1"
                    }
                },

                //interactive Asp.net core MVC client
                new Client
                {
                    ClientId = "mvcclient",
                    ClientSecrets =  {new Secret("mvc-secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,

                    //where to redirect to after login
                    RedirectUris = {"https://localhost:5002/signin-oidc"},

                    //where to redirect to after logout

                    PostLogoutRedirectUris = {"https://localhost:5002/signout-callback-oidc"},

                    AlwaysIncludeUserClaimsInIdToken = true ,//带着用户claims

                    AccessTokenLifetime = 30,//AccessToken有效期
                    AllowOfflineAccess = true,//refreshToken
                    
                    //RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    //AbsoluteRefreshTokenLifetime = 3600 * 24,
                    //SlidingRefreshTokenLifetime = 10,
                    //RefreshTokenExpiration = TokenExpiration.Sliding,

                    AllowedScopes = new List<string>
                    {
                        "api1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Phone
                    },

                },

                //angular client, implicit flow 
                new Client
                {
                  ClientId = "angular-client",
                  ClientName = "angular spa client ",//随意写
                  ClientUri = "http://localhost:4200",//必须
                  
                  AllowedGrantTypes = GrantTypes.Implicit,

                  AllowAccessTokensViaBrowser = true,//允许浏览器传输accesstoken
                  RequireConsent = true,//用户点击同意
                  AccessTokenLifetime = 60*5,//这种情况通常较短

                  RedirectUris = {
                        "http://localhost:4200/signin-oidc",//登录成功后跳转回来的uri
                        "http://localhost:4200/redirect-silentrenew"//刷新accesstoken ，通常用户不可见
                    },

                  PostLogoutRedirectUris =
                    {
                        "http://localhost:4200"
                    },

                  //跨域设置
                  AllowedCorsOrigins =
                    {
                        "http://localhost:4200" // 注意，这里如果写成"http://localhost:4200/"，便无法认证授权
                    },

                  AllowedScopes =
                    {
                          "api1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Phone
                    }
                },
                new Client
                {
                    ClientId = "hybrid client",
                    ClientName = "Asp net core Mvc client",
                    ClientSecrets = {new Secret("hybrid secret".Sha256())},

                    AllowedGrantTypes = GrantTypes.Hybrid,
                    //这里注意
                    RequirePkce =false,//https://stackoverflow.com/questions/60028530/i-am-getting-code-challenge-required-when-using-identityserver4

                    RedirectUris = {"https://localhost:5003/signin-oidc"},
                    PostLogoutRedirectUris= {"https://localhost:5003/signout-callback-oidc"},

                    AllowOfflineAccess = true,
                    AllowedScopes =
                        {
                              "api1",
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile,
                            IdentityServerConstants.StandardScopes.Address,
                            IdentityServerConstants.StandardScopes.Email,
                            IdentityServerConstants.StandardScopes.Phone,
                            "roles",
                            "location"
                        },
                    RequireConsent = true,

                    AlwaysIncludeUserClaimsInIdToken = true,




                }


                //// interactive client using code flow + pkce
                //new Client
                //{
                //    ClientId = "interactive",
                //    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },
                    
                //    AllowedGrantTypes = GrantTypes.Code,

                //    RedirectUris = { "https://localhost:44300/signin-oidc" },
                //    FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                //    PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                //    AllowOfflineAccess = true,
                //    AllowedScopes = { "openid", "profile", "scope2" }
                //},
            };
    }
}