// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


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
                new IdentityResources.Email()

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
                    ClientId = "mvc client",
                    ClientSecrets =  {new Secret("mvc-secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    //where to redirect to after login
                    RedirectUris = {"https://localhost:5002/signin-oidc"},

                    //where to redirect to after logout

                    PostLogoutRedirectUris = {"https://localhost:5002/signout-callback-oidc"},

                    AlwaysIncludeUserClaimsInIdToken = true ,//带着用户claims


                    AllowOfflineAccess = true,//refreshToken

                    AllowedScopes = new List<string>
                    {
                        //"api1",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Phone
                    },
                    
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