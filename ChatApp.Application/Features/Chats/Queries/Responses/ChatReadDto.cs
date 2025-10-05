namespace ChatApp.Application.Features.Chats.Queries.Responses
{
    public record ChatReadDto(string ChatId, string UserId, DateTimeOffset? LastReadMessageAt);
}
