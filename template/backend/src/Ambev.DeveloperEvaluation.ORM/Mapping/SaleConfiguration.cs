using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
               .HasColumnType("uuid")
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.SaleNumber)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(s => s.SaleDate)
               .IsRequired();

        builder.Property(s => s.UserId)
               .HasColumnType("uuid")
               .IsRequired();

        builder.Property(s => s.BranchId)
               .HasColumnType("uuid")
               .IsRequired();

        builder.Property(s => s.IsCancelled)
               .IsRequired();

        builder.Property(s => s.TotalAmount)
               .HasColumnType("numeric(18,2)")
               .IsRequired();

        builder.Property(s => s.CreatedAt)
               .IsRequired();

        // UpdatedAt can be null
        builder.Property(s => s.UpdatedAt);
    }
}
