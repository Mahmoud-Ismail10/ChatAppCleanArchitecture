using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.Configurations
{
    public class ChatConfigurations : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.Property(c => c.IsGroup)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.CreatedBy)
                .IsRequired(false);

            builder.Property(c => c.GroupImageUrl)
                .IsRequired(false);

            builder.Property(c => c.Description)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(c => c.DeletedAt)
                .IsRequired(false);

            builder.HasOne(c => c.CreatedByUser)
                .WithMany(u => u.CreatedChats)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.LastMessage)
                .WithMany()
                .HasForeignKey(c => c.LastMessageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.ChatMembers)
                .WithOne()
                .HasForeignKey(cm => cm.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
