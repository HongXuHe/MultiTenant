using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenant.Entities.EntityConfiguration
{
    class Id4PermissionsConfiguration : IEntityTypeConfiguration<Id4Permission>
    {
        public void Configure(EntityTypeBuilder<Id4Permission> builder)
        {
            builder.ToTable("Id4Permissions");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.PermissionName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.PermissionValue).IsRequired().HasMaxLength(100);
        }
    }
}
