namespace ChatApp.Domain.Entities
{
    public class Chat
    {
        public Guid Id { get; set; }
        public bool IsGroup { get; set; }
        public string? Name { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; } // AdminId
        public string? GroupImageUrl { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public Guid? LastMessageId { get; set; }

        public User? CreatedByUser { get; set; } // Admin
        public Message? LastMessage { get; set; }
        public ICollection<ChatMember> ChatMembers { get; set; } = new HashSet<ChatMember>();
        public ICollection<Message> Messages { get; set; } = new HashSet<Message>();
    }
}
