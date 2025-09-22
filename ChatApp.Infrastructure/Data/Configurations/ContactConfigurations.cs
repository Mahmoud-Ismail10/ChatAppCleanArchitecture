using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.Configurations
{
    public class ContactConfigurations : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.HasOne(c => c.User)
                .WithMany(u => u.Contacts)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.ContactUser)
                .WithMany(u => u.ContactOf)
                .HasForeignKey(c => c.ContactUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
