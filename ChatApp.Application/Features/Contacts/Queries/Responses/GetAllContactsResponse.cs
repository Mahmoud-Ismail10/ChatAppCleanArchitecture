namespace ChatApp.Application.Features.Contacts.Queries.Responses
{
    public record GetAllContactsResponse(
        Guid ContactId,
        Guid ContactUserId,
        string ContactUserName,
        string ContactUserPhoneNumber,
        string? ContactUserProfileImageUrl
    );
}
