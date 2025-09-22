namespace ChatApp.Domain.Entities
{
    public class Contact
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ContactUserId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public User? User { get; set; }
        public User? ContactUser { get; set; }
    }
}
