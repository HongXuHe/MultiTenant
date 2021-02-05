using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MultiTenant.Entities;
using MultiTenant.IdentityServerFour.ApplicationModule;
using MultiTenant.IdentityServerFour.InitData;
using MultiTenant.IdentityServerFour.Profiles;
using Serilog;

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

            services.AddIdentity<Id4User, Id4Role>(setup =>
            {
                setup.Password.RequireDigit = false;
                setup.Password.RequiredLength = 4;
                setup.Password.RequireLowercase = false;
                setup.Password.RequireNonAlphanumeric = false;
                setup.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<MultiTenantContext>()
            .AddDefaultTokenProviders();

            #region AddIdentityServer4
             string connectionString = Configuration.GetConnectionString("Id4ConnStr");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddIdentityServer(options =>
            {
                options.UserInteraction.LoginUrl = "/Auth/Login";
                options.UserInteraction.LogoutUrl = "/Auth/Logout";
            })
            .AddDeveloperSigningCredential()
            //.AddInMemoryApiResources(Config.GetApiResources())
            //.AddInMemoryApiScopes(Config.GetApiScopes())
            //.AddInMemoryIdentityResources(Config.GetIdentityResources())
            //.AddInMemoryClients(Config.GetClients())
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder =>
                {
                    builder.UseSqlServer(connectionString,
                     sql => sql.MigrationsAssembly(migrationsAssembly));
                };
            }).AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));                
            })
            .AddAspNetIdentity<Id4User>()
            .AddProfileService<Id4Profile>();
            services.ConfigureApplicationCookie(c =>
            {
                c.LoginPath = "/Auth/Login";
                c.LogoutPath = "/Auth/Logout";
                c.AccessDeniedPath = "/Auth/AccessDenied";
            });

            #endregion
            services.AddAuthorization(c =>
            {
                c.AddPolicy("AdminRole", p =>
                {
                    p.RequireRole("Admin");
                });
            });
        }
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new DIModule());
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
            InitUserData.InitializeUserData(app, Log.Logger);
        }
    }
}
