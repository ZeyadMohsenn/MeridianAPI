using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Infrastructure.Configurations
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {

            builder.HasKey(p => p.Id);
            builder.Property(p => p.StoredFileName)
                   .IsRequired();
            builder.HasOne(p => p.Product)
                   .WithMany(c => c.Images)
                   .HasForeignKey(p => p.ProductId);
        }
    }
}
