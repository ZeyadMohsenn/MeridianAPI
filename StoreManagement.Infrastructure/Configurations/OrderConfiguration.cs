using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.IdentityModel.Tokens;
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
            //builder.HasOne(o => o.User)
            //     .WithMany(u => u.Orders)
            //     .HasForeignKey(o => o.UserId)
            //     .IsRequired();

            builder.HasMany(o => o.OrderProducts)
                   .WithOne(op => op.Order)
                   .HasForeignKey(op => op.OrderId)
                   .IsRequired();
            builder.HasMany(o => o.OrderProducts)
           .WithOne(op => op.Order)
           .HasForeignKey(op => op.OrderId);

        }
    }
}
