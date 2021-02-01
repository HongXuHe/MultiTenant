using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenant.Entities.Identity;

using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenant.Entities.EntityConfiguration
{

    public class R_User_PermissionConfiguration : IEntityTypeConfiguration<R_User_Permission>
    {
        public void Configure(EntityTypeBuilder<R_User_Permission> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Id4User).WithMany().HasForeignKey(x => x.Id4UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Id4Permission).WithMany().HasForeignKey(x => x.Id4PermissionId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
