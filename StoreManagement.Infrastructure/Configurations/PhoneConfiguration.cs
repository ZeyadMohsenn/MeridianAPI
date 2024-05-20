using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Domain.Entities.StoreManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.Infrastructure.Configurations
{
    public class PhoneConfiguration : IEntityTypeConfiguration<Phone>
    {
        public void Configure(EntityTypeBuilder<Phone> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Number)
                   .IsRequired();
            builder.HasOne(p => p.Client)
                   .WithMany(c => c.Phones)
                   .HasForeignKey(p => p.ClientId);
        }
    }
}
