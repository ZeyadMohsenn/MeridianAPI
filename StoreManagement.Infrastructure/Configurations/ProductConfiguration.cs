using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreManagement.Bases.Infrastructure;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Infrastructure.Configurations
{
    public class ProductConfiguration : BaseConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);

            builder.Property(s => s.Name)
                   .IsRequired();

            builder.Property(s => s.Description)
                   .HasMaxLength(500);

            builder.Property(s => s.Photo)
                   .HasMaxLength(100);
            builder.Property(s => s.StockQuantity);
            builder.Property(s => s.isActive);
            builder.Property(s => s.Price);
            builder.Property(s => s.Discount);
            builder.Property(s => s.SubCategory_Id)
            .IsRequired();
            builder.HasOne<SubCategory>(s => s.SubCategory)
                   .WithMany(c => c.Products)
                   .HasForeignKey(s => s.SubCategory_Id)
                   .IsRequired();
            builder.HasMany(p => p.OrderProducts)
                .WithOne(op => op.Product)
                .HasForeignKey(op => op.ProductId);
        }
    }

}
