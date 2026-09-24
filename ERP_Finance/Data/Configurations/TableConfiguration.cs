using ERP_Finance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP_Finance.Data.Configurations;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.HasKey(table => table.Id);

        builder.Property(table => table.TableNumber).IsRequired();

        builder.Property(table => table.Status).IsRequired();

    }
}
