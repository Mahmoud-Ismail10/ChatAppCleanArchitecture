using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ChatApp.Infrastructure.Data
{
    public class ChatAppBbContext : DbContext
    {
        public ChatAppBbContext(DbContextOptions<ChatAppBbContext> options) : base(options)
        {
        }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMember> ChatMembers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageStatus> MessageStatuses { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /// Execute all Configurations that implement from IEntityTypeConfiguration<>
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
