﻿// INNOVT TECNOLOGIA 2014-2021
// Author: Michel Magalhães
// Project: Innovt.Data.EFCore
// Solution: Innovt.Platform
// Date: 2021-04-08
// Contact: michel@innovt.com.br or michelmob@gmail.com

using Innovt.Domain.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Innovt.Data.EFCore.Maps
{
    public class SecurityGroupMap : IEntityTypeConfiguration<SecurityGroup>
    {
        public void Configure(EntityTypeBuilder<SecurityGroup> builder)
        {
            builder.ToTable(nameof(SecurityGroup));

            builder.HasKey(u => u.Id);
            builder.Property(b => b.Name).HasMaxLength(50).IsRequired();
            builder.Property(b => b.Description).HasMaxLength(100).IsRequired(false);
            builder.HasMany(b => b.Users);
            builder.HasMany(b => b.Policies);
        }
    }
}