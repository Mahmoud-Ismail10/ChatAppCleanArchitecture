using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.Configurations
{
    public class MessageConfigurations : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.Property(m => m.Type)
                .HasConversion(
                    m => m.ToString(),
                    m => (Domain.Enums.MessageType)Enum.Parse(typeof(Domain.Enums.MessageType), m!))
                .IsRequired();

            builder.Property(m => m.Content)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(m => m.FilePath)
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Property(m => m.Duration)
                .IsRequired(false);

            builder.Property(m => m.SentAt)
                .IsRequired();

            builder.Property(m => m.IsEdited)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(m => m.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            builder.HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
