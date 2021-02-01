using Microsoft.EntityFrameworkCore;
using MultiTenant.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MultiTenant.Entities
{
    public class MultiTenantContext:DbContext
    {
        public MultiTenantContext(DbContextOptions<MultiTenantContext> options):base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var assembly = Assembly.GetExecutingAssembly();
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }

        public DbSet<Id4User> Id4Users { get; set; }
        public DbSet<Id4Role> Id4Roles { get; set; }
        public DbSet<Id4Permission> Id4Permissions { get; set; }
        public DbSet<R_User_Role> R_User_Roles { get; set; }
        public DbSet<R_Role_Permission> R_Role_Permissions { get; set; }
        public DbSet<R_User_Permission> R_User_Permissions { get; set; }
    }
}
