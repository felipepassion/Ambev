using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
               .HasColumnType("uuid")
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(p => p.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(p => p.Description)
               .HasMaxLength(500);

        builder.Property(p => p.UnitPrice)
               .HasColumnType("numeric(18,2)")
               .IsRequired();

        builder.Property(p => p.IsActive)
               .IsRequired();

        builder.Property(p => p.CreatedAt)
               .IsRequired();

        builder.Property(p => p.UpdatedAt);
    }
}
