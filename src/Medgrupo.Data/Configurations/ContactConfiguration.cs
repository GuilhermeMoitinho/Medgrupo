using Medgrupo.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medgrupo.Data.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();
        builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
        builder.Property(c => c.BirthDate).IsRequired().HasColumnType("date");
        builder.Property(c => c.Gender).IsRequired().HasConversion<int>();
        builder.Property(c => c.Active).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt);
        builder.Ignore(c => c.Age);

        builder.HasIndex(c => c.Active);
    }
}
