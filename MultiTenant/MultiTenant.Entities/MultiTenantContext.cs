using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiTenant.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MultiTenant.Entities
{
    public class MultiTenantContext:IdentityDbContext<Id4User,Id4Role,Guid>
    {
        public MultiTenantContext(DbContextOptions<MultiTenantContext> options):base(options)
        {

        }

        public DbSet<Id4Role_Id4Permission> Id4Role_Id4Permissions { get; set; }
        public DbSet<Id4User_Id4Permission> Id4User_Id4Permissions { get; set; }
        public DbSet<Id4Permission> Id4Permissions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //var assembly = Assembly.GetExecutingAssembly();
            //modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            modelBuilder.Entity<Id4Role_Id4Permission>(setup =>
            {
                setup.HasKey(x => new { x.Id4PermissionId, x.Id4RoleId });
                setup.HasOne(x => x.Id4Role).WithMany(x=>x.Id4Permissions).HasForeignKey(y=>y.Id4RoleId).OnDelete(DeleteBehavior.NoAction);
                setup.HasOne(x => x.Id4Permission).WithMany(x=>x.Id4Roles).HasForeignKey(x=>x.Id4PermissionId).OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<Id4User_Id4Permission>(setup =>
            {
                setup.HasKey(x => new { x.Id4PermissionId, x.Id4UserId });
                setup.HasOne(x => x.Id4User).WithMany(x=>x.Id4Permissions).HasForeignKey(z=>z.Id4UserId).OnDelete(DeleteBehavior.NoAction);
                setup.HasOne(x => x.Id4Permission).WithMany(x=>x.Id4Users).HasForeignKey(z => z.Id4PermissionId).OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<Id4Permission>(setup =>
            {
                setup.Property(x => x.PermissionName).HasMaxLength(200).IsRequired();
                setup.Property(x => x.PermissionValue).HasMaxLength(200).IsRequired();
            });
        }
    }
}
