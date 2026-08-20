using ERP_Finance.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP_Finance.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.SKU).HasMaxLength(50).IsRequired();

        builder.HasIndex(product => product.SKU).IsUnique();

        builder.Property(product => product.Name).HasMaxLength(50).IsRequired();

        builder.Property(product => product.Description).HasMaxLength(300).IsRequired();

        builder.Property(product => product.Price).HasPrecision(18, 2).IsRequired();

        builder.Property(product => product.Category).IsRequired();

        builder.Property(product => product.CreatedAt).IsRequired();

        builder.Property(product => product.LastUpdateAt).IsRequired();

        builder.OwnsOne(product => product.Details,
            details =>
            {
                details.Property(detail => detail.BrandName)
                    .HasColumnName("BrandName")
                    .HasMaxLength(100)
                    .IsRequired();

                details.Property(detail => detail.WeightOrVolume)
                    .HasColumnName("WeightOrVolume")
                    .HasPrecision(18, 3)
                    .IsRequired();

                details.Property(detail => detail.MeasureType)
                    .HasColumnName("MeasureType")
                    .IsRequired();
            });
    }
}