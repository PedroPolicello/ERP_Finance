using ERP_Finance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP_Finance.Data.Configurations;

public class TabConfiguration : IEntityTypeConfiguration<Tab>
{
    public void Configure(EntityTypeBuilder<Tab> builder)
    {
        builder.HasKey(tab => tab.Id);

        builder.Property(tab => tab.TabNumber).IsRequired();

        builder.HasIndex(t => t.TabNumber).IsUnique();

        builder.Property(tab => tab.Status).IsRequired();

        builder.Property(tab => tab.ServiceType).IsRequired();

        builder.Property(tab => tab.TableNumber).IsRequired(false);

        builder.Property(tab => tab.CreatedAt).IsRequired();

        builder.Property(tab => tab.ClosedAt).IsRequired(false);

        builder.Property(tab => tab.Note).IsRequired(false);

        builder.HasMany(tab => tab.Orders)
               .WithOne()
               .HasForeignKey(order => order.TabId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
