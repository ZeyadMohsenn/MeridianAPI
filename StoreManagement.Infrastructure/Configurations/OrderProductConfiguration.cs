using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreManagement.Bases.Infrastructure;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Infrastructure.Configurations
{
    public class OrderProductConfiguration : BaseConfiguration<OrderProduct>
    {
        public override void Configure(EntityTypeBuilder<OrderProduct> builder)
        {
            base.Configure(builder);
        }
    }
}
