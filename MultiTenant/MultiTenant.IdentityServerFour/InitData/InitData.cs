using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MultiTenant.Entities;
using MultiTenant.Entities.Identity;
using MultiTenant.Shared.Hasher;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenant.IdentityServerFour.InitData
{
    public class InitUserData
    {
        public static void InitializeUserData(IApplicationBuilder builder,ILogger logger)
        {
            //init users
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<MultiTenantContext>();
                    context.Database.EnsureCreated();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Id4Role>>();
                    var id4Role = new Id4Role()
                    {
                        Name = "Admin"
                    };
                    var id4User = new Id4User()
                    {
                        Email = "matt.he@nationalweighing.com.au",
                        UserName = "matthe",
                    };
                    if (!roleManager.Roles.Any())
                    {
                        var resRole = roleManager.CreateAsync(id4Role).GetAwaiter().GetResult();
                        if (resRole.Succeeded)
                        {
                            logger.Information<Id4Role>("Init role", id4Role);
                        }
                        else
                        {
                            logger.Error("Init Role Failed");
                        }
                    }
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Id4User>>();
                    if (!userManager.Users.Any())
                    {
                      
                       var res =  userManager.CreateAsync(id4User, "1234").GetAwaiter().GetResult();
                        if (res.Succeeded)
                        {
                            logger.Information<Id4User>("Init user", id4User);
                            var resAddUserToRole = userManager.AddToRoleAsync(id4User, "Admin").GetAwaiter().GetResult();
                            if (resAddUserToRole.Succeeded)
                            {
                                logger.Information("add user matthe to role Admin");
                            }
                            else
                            {
                                logger.Error("add user to role failed");
                            }
                        }
                        else
                        {
                            logger.Error("Init User Failed");
                        }
                    }

                    if (!context.Id4Permissions.Any())
                    {
                        var id4Permission = new Id4Permission()
                        {
                            PermissionName = "PName",
                            PermissionValue = "PValue"
                        };
                        id4Permission.Id4Roles.Add(new Id4Role_Id4Permission
                        {
                             
                            Id4Role = id4Role
                        });
                        context.Id4Permissions.Add(id4Permission);
                        logger.Information("add permission PValue to role Admin");
                        context.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, "Init data error");
                }

            }

            //init IdentityServer 4 related 
            using (var serviceScope = builder.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
                if (!context.ApiScopes.Any())
                {
                    foreach (var scope in Config.GetApiScopes())
                    {
                        context.ApiScopes.Add(scope.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
