using ERP_Finance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP_Finance.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(orderItem => orderItem.Id);

        builder.Property(orderItem => orderItem.OrderId).IsRequired();

        builder.Property(orderItem => orderItem.ProductId).IsRequired();

        builder.Property(orderItem => orderItem.Quantity).HasPrecision(18, 3).IsRequired();

        builder.Property(orderItem => orderItem.UnitPrice).HasPrecision(18, 2).IsRequired();

        builder.Property(orderItem => orderItem.Subtotal).HasPrecision(18, 2).IsRequired();

        builder.Property(orderItem => orderItem.Note).IsRequired(false);

        builder.HasOne(orderItem => orderItem.Product)
               .WithMany(product => product.OrderItems)
               .HasForeignKey(orderItem => orderItem.ProductId)
               .IsRequired().OnDelete(DeleteBehavior.Restrict);
    }
}
