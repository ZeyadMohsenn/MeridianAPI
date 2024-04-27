using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StoreManagement.Bases.Infrastructure;

public abstract class BaseConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity<Guid>
{
  public virtual void Configure(EntityTypeBuilder<T> builder)
  {
    builder.HasKey(a => a.Id);
    builder.ToTable(builder.Metadata.ClrType.Name + "s");
    builder.HasIndex(a => a.Is_Deleted);

  }

}
