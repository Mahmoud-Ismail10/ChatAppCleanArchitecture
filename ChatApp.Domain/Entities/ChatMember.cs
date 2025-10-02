using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities
{
    public class ChatMember
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }
        public Role Role { get; set; }
        public DateTimeOffset JoinedAt { get; set; }
        public DateTimeOffset? LeftAt { get; set; }
        public MemberStatus Status { get; set; }
        public bool IsPinned { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public DateTimeOffset? LastReadMessageAt { get; set; }

        public Chat? Chat { get; set; }
        public User? User { get; set; }
    }
}
