using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.Name)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(15)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(u => u.ProfileImageUrl)
                .IsRequired(false);

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.Property(u => u.LastSeenAt)
                .IsRequired();

            builder.HasMany(u => u.ChatMembers)
                .WithOne()
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
