using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MultiTenant.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //this is used for api authentication
            // services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            // .AddIdentityServerAuthentication(options=>{
            //     options.Authority="https://localhost:5000";
            //     options.ApiName="api1";
            // });

            // mvc is as a client and as an api resource
            services.AddAuthentication(c=>{
                c.DefaultAuthenticateScheme="Cookies";
                c.DefaultChallengeScheme="oidc";
                c.DefaultSignInScheme = "Cookies";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc",setup=>{
                setup.Authority="https://localhost:5000";
                setup.ResponseType="code";
                setup.ClientId= "676d75ca-36ff-4ba7-ba73-b24366402eb7";
                setup.ClientSecret= "secret";
                setup.SaveTokens=true;
            });

            //services.AddAuthorization(configure =>
            //{
            //    configure.AddPolicy("P1", p =>
            //    {
            //       // p.RequireClaim("PName", "PValue");
            //    });
            //});

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
             endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
