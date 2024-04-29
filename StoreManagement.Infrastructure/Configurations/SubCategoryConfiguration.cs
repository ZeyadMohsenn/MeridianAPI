using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.Infrastructure.Configurations
{
    public class SubCategoryConfiguration : IEntityTypeConfiguration<SubCategory>
    {
        public void Configure(EntityTypeBuilder<SubCategory> builder)
        {

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                   .IsRequired(); 

            builder.Property(s => s.Description)
                   .HasMaxLength(500); 

            builder.Property(s => s.Photo)
                   .HasMaxLength(100); 

            builder.HasOne<Category>(s => s.Category)
                   .WithMany(c => c.SubCategories) 
                   .HasForeignKey(s => s.Category_Id) 
                   .IsRequired();
        }
    }
}
