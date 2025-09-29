using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.Configurations
{
    public class ChatMemberConfigurations : IEntityTypeConfiguration<ChatMember>
    {
        public void Configure(EntityTypeBuilder<ChatMember> builder)
        {
            builder.Property(cm => cm.Role)
                .HasConversion(
                    cm => cm.ToString(),
                    cm => (Role)Enum.Parse(typeof(Role), cm!))
                .IsRequired();

            builder.Property(cm => cm.JoinedAt)
                .IsRequired();

            builder.Property(cm => cm.LeftAt)
                .IsRequired(false);

            builder.Property(cm => cm.Status)
                .HasConversion(
                    cm => cm.ToString(),
                    cm => (MemberStatus)Enum.Parse(typeof(MemberStatus), cm!))
                .IsRequired();

            builder.Property(cm => cm.IsPinned)
                .IsRequired();

            builder.HasOne(cm => cm.Chat)
                .WithMany(c => c.ChatMembers)
                .HasForeignKey(cm => cm.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cm => cm.User)
                .WithMany(c => c.ChatMembers)
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
