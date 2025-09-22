namespace ChatApp.Domain.Entities
{
    public class MessageStatus
    {
        public Guid Id { get; set; }
        public Guid MessageId { get; set; }
        public Guid UserId { get; set; }
        public Enums.MessageStatus Status { get; set; }
        public DateTimeOffset? DeliveredAt { get; set; }
        public DateTimeOffset? ReadAt { get; set; }
        public DateTimeOffset? PlayedAt { get; set; }

        public User? User { get; set; }
        public Message? Message { get; set; }
    }
}
