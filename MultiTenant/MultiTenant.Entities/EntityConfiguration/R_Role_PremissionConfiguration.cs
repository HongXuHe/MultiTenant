using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenant.Entities.Identity;

using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenant.Entities.EntityConfiguration
{

    class R_Role_PremissionConfiguration : IEntityTypeConfiguration<R_Role_Permission>
    {
        public void Configure(EntityTypeBuilder<R_Role_Permission> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Id4Role).WithMany().HasForeignKey(x => x.Id4RoleId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Id4Permission).WithMany().HasForeignKey(x => x.Id4PermissionId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
