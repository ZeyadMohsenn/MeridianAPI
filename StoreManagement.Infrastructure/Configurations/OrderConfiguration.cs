using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreManagement.Bases.Infrastructure;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Infrastructure.Configurations
{
    public class OrderConfiguration : BaseConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);
            builder.Property(s => s.DateTime);
            builder.Property(s => s.Status);
            builder.Property(s => s.TotalPrice);

            builder.HasMany(o => o.OrderProducts)
                   .WithOne(op => op.Order)
                   .HasForeignKey(op => op.OrderId)
                   .IsRequired();

            builder.HasOne(o => o.Client)
                   .WithMany(c => c.Orders)
                   .HasForeignKey(o => o.Client_Id)
                   .IsRequired();

        }
    }
}
