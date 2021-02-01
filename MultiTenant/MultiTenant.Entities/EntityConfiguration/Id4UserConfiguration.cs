using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTenant.Entities.EntityConfiguration
{

    public class Id4UserConfiguration : IEntityTypeConfiguration<Id4User>
    {
        public void Configure(EntityTypeBuilder<Id4User> builder)
        {
            builder.ToTable("Id4Users");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserEmail).IsRequired().HasColumnType("Email").HasMaxLength(100);
            builder.Property(x => x.UserName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.UserPasswordHash).IsRequired();
        }
    }
}
