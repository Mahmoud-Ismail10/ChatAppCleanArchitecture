using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public Guid SenderId { get; set; }
        public MessageType Type { get; set; }
        public string? Content { get; set; }
        public string? FilePath { get; set; }
        public int? Duration { get; set; }
        public DateTimeOffset SentAt { get; set; }
        public bool IsEdited { get; set; }
        public bool IsDeleted { get; set; }

        public Chat? Chat { get; set; }
        public User? Sender { get; set; }
        public ICollection<MessageStatus> MessageStatuses { get; set; } = new HashSet<MessageStatus>();
    }
}
