using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreManagement.Bases.Infrastructure;
using StoreManagement.Domain.Entities;

namespace StoreManagement.Infrastructure.Configurations
{
    public class SubCategoryConfiguration : BaseConfiguration<SubCategory>
    {
        public override void Configure(EntityTypeBuilder<SubCategory> builder)
        {
            base.Configure(builder);

            builder.Property(s => s.Name)
                   .IsRequired();

            builder.Property(s => s.Description)
                   .HasMaxLength(500);

            builder.HasOne<Category>(s => s.Category)
                   .WithMany(c => c.SubCategories)
                   .HasForeignKey(s => s.Category_Id)
                   .IsRequired();
        }
    }
}
