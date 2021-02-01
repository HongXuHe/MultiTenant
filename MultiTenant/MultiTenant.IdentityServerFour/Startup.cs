using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MultiTenant.Entities;

namespace MultiTenant.IdentityServerFour
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<MultiTenantContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Id4ConnStr"));
            });
            services.AddAuthentication("Cookies")
                .AddCookie("Cookies", option =>
                {
                 option.LoginPath = "/Auth/Login";
                 option.AccessDeniedPath = "/Auth/Login";
                 option.Cookie.Name = "MultiTenantCookie";
                 });
            services.AddAuthorization();
            #region AddIdentityServer4
            services.AddIdentityServer(options =>
            {
                options.UserInteraction.LoginUrl = "/Auth/Login";
                options.UserInteraction.LogoutUrl = "/Auth/Logout";
            })
            .AddDeveloperSigningCredential()
            .AddInMemoryApiResources(Config.GetApiResources())
            .AddInMemoryApiScopes(Config.GetApiScopes())
            .AddInMemoryIdentityResources(Config.GetIdentityResources())
            .AddInMemoryClients(Config.GetClients())
            .AddTestUsers(Config.GetTestUsers());
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
