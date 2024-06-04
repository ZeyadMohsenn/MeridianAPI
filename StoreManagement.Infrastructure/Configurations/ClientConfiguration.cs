using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreManagement.Bases.Infrastructure;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Infrastructure.Configurations
{
    public class ClientConfiguration : BaseConfiguration<Client>
    {
        public override void Configure(EntityTypeBuilder<Client> builder)
        {
            base.Configure(builder);

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name)
                 .IsRequired();
            builder.Property(x => x.Email)
                             .IsRequired();

            builder.Property(x => x.Address)
                .IsRequired();

            builder.HasMany(c => c.Orders)
                    .WithOne(o => o.Client)
                    ;

            builder.HasMany(c => c.Phones)
                   .WithOne(p => p.Client)
                   .HasForeignKey(p => p.ClientId)
                   .IsRequired();

        }
    }
}
