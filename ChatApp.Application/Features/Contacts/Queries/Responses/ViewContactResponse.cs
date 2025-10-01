namespace ChatApp.Application.Features.Contacts.Queries.Responses
{
    public record ViewContactResponse(
        Guid? ChatId,
        Guid ChatMemberId,
        Guid ContactUserId,
        string ContactUserName,
        string ContactUserPhoneNumber,
        string? ContactUserProfileImageUrl,
        DateTimeOffset LastSeenAt
    );
}
