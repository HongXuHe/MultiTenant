using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MultiTenant.IdentityServerFour.ApplicationModule
{
    public class DIModule:Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //var RepoAss = Assembly.Load("MultiTenant.Repo");
            //builder.RegisterAssemblyTypes(RepoAss).Where(x => !x.IsAbstract)
            //    .AsImplementedInterfaces();
            //var uowAss = Assembly.Load("MultiTenant.UOW");
            //builder.RegisterAssemblyTypes(uowAss).Where(x => !x.IsAbstract)
            // .AsImplementedInterfaces();
        }
    }
}
