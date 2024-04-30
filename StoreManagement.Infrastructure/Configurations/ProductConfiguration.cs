using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.Infrastructure.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                   .IsRequired();

            builder.Property(s => s.Description)
                   .HasMaxLength(500);

            builder.Property(s => s.Photo)
                   .HasMaxLength(100);
            builder.Property(s => s.StockQuantity);
            builder.Property(s => s.Price);

            builder.HasOne<SubCategory>(s => s.SubCategory)
                   .WithMany(c => c.Products)
                   .HasForeignKey(s => s.SubCategory_Id)
                   .IsRequired();
        }
    }

}
