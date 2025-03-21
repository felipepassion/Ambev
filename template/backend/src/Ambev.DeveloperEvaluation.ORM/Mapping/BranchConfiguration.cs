using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id)
               .HasColumnType("uuid")
               .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(b => b.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(b => b.IsActive)
               .IsRequired();

        builder.Property(b => b.CreatedAt)
               .IsRequired();

        builder.Property(b => b.UpdatedAt);
    }
}
