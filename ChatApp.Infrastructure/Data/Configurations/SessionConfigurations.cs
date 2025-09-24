using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Infrastructure.Data.Configurations
{
    public class SessionConfigurations : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.Property(s => s.KeyHash)
                .IsRequired();

            builder.Property(s => s.Revoked)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.HasOne(s => s.User)
                .WithOne()
                .HasForeignKey<Session>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
