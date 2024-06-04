using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreManagement.Domain.Entities;

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
