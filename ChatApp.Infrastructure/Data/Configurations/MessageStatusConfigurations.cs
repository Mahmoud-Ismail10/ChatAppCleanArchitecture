using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.Configurations
{
    public class MessageStatusConfigurations : IEntityTypeConfiguration<MessageStatus>
    {
        public void Configure(EntityTypeBuilder<MessageStatus> builder)
        {
            builder.Property(ms => ms.Status)
                .HasConversion(
                    ms => ms.ToString(),
                    ms => (Domain.Enums.MessageStatus)Enum.Parse(typeof(Domain.Enums.MessageStatus), ms!))
                .IsRequired();

            builder.Property(ms => ms.DeliveredAt)
                .IsRequired(false);

            builder.Property(ms => ms.ReadAt)
                .IsRequired(false);

            builder.Property(ms => ms.PlayedAt)
                .IsRequired(false);

            builder.HasOne(ms => ms.User)
                .WithMany(u => u.MessageStatuses)
                .HasForeignKey(ms => ms.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ms => ms.Message)
                .WithMany(m => m.MessageStatuses)
                .HasForeignKey(ms => ms.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
