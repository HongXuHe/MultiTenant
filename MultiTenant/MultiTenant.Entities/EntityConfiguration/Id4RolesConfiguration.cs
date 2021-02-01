using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenant.Entities.EntityConfiguration
{

    public class Id4RolesConfiguration : IEntityTypeConfiguration<Id4Role>
    {
        public void Configure(EntityTypeBuilder<Id4Role> builder)
        {
            builder.ToTable("Id4Roles");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.RoleName).IsRequired().HasMaxLength(100);      
        }
    }
}
