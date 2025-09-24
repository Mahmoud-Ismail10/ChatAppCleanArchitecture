namespace ChatApp.Domain.Entities
{
    public class Session
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? KeyHash { get; set; }
        public bool Revoked { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public User? User { get; set; }
    }
}
