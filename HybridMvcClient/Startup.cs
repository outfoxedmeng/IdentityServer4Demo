using HybridMvcClient.Auth;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace HybridMvcClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.AccessDeniedPath = "/Authorization/AccessDenied";
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "https://localhost:5001";//where the trusted token service is located

                    //options.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(30);
                    //options.TokenValidationParameters.RequireExpirationTime = true;


                    //Identify this client via ClientId and ClientSecret
                    options.ClientId = "hybrid client";//
                    options.ClientSecret = "hybrid secret";

                    options.ResponseType = "code id_token";

                    options.SaveTokens = true;

                    // options.Scope.Clear();

                    //options.Scope.Add("api1");
                    options.Scope.Add(OidcConstants.StandardScopes.OfflineAccess);
                    options.Scope.Add(OidcConstants.StandardScopes.Address);
                    options.Scope.Add(OidcConstants.StandardScopes.Email);
                    options.Scope.Add(OidcConstants.StandardScopes.Phone);
                    options.Scope.Add("roles");
                    options.Scope.Add("location");

                    //从过滤集合中移除nbf（这样nbf就不会被过滤掉）
                    options.ClaimActions.Remove("nbf");
                    options.ClaimActions.Remove("amr");
                    options.ClaimActions.Remove("exp");

                    //添加过滤
                    options.ClaimActions.DeleteClaim("sid");
                    options.ClaimActions.DeleteClaim("sub");
                    options.ClaimActions.DeleteClaim("idp");

                    //让Claim中的Role成为Mvc能够识别的Role
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.Name,
                        RoleClaimType = JwtClaimTypes.Role   //这样，User.IsInRole("...")也有效了
                    };

                });



            //ABAC Attribute-based Access Control （PBAC Policy...)
            //推荐策略

            //方案一
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("SmithInSomewhere",builder=> {
            //        builder.RequireAuthenticatedUser();
            //        builder.RequireClaim(JwtClaimTypes.FamilyName, "Smith");
            //        builder.RequireClaim("location", "somewhere");//非标准Claim，需要在Authority添加一个IdentityResources

            //    });
            //});

            //方案二，使用Requiement
            services.AddAuthorization(options =>
            {
                options.AddPolicy("SmithInSomewhere", builder =>
                {
                    //这里可以通过构造函数传入参数
                    builder.AddRequirements(new SmithInSomwhereRequirement());
                });
            });
            services.AddSingleton<IAuthorizationHandler, SmithInSomewhereHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
