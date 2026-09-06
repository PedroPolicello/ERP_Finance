using ERP_Finance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP_Finance.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(order => order.Id);

        builder.Property(order => order.OrderNumber).IsRequired();

        builder.Property(order => order.TabId).IsRequired();

        builder.Property(order => order.Status).IsRequired();

        builder.Property(order => order.CreatedAt).IsRequired();

        builder.Property(order => order.Note).IsRequired(false);

        builder.HasMany(order => order.OrderItems)
               .WithOne()
               .HasForeignKey(orderItem => orderItem.OrderId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
