using TaxPayApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaxPayApi.Infrastructure.Persistence.Configurations;

public class EntryConfiguration : IEntityTypeConfiguration<Entry>
{
    public void Configure(EntityTypeBuilder<Entry> builder)
    {
        builder.HasCheckConstraint("CK_Entry_Amount", "[Amount] >= 0");
        
        builder.Property(e => e.Amount)
            .HasColumnType("decimal(18, 2)");

    }
}