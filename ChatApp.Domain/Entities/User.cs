namespace ChatApp.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset LastSeenAt { get; set; }

        public ICollection<Chat> CreatedChats { get; set; } = new HashSet<Chat>();
        public ICollection<ChatMember> ChatMembers { get; set; } = new HashSet<ChatMember>();
        public ICollection<MessageStatus> MessageStatuses { get; set; } = new HashSet<MessageStatus>();
        public ICollection<Message> Messages { get; set; } = new HashSet<Message>();
        public ICollection<Contact> Contacts { get; set; } = new HashSet<Contact>(); // Contacts added by this user
        public ICollection<Contact> ContactOf { get; set; } = new HashSet<Contact>(); // Users who have added this user as a contact
    }
}
