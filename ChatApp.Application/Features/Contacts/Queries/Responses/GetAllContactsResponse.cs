namespace ChatApp.Application.Features.Contacts.Queries.Responses
{
    public record GetAllContactsResponse(
        Guid ContactId,
        string ContactName,
        string ContactPhoneNumber,
        string? ContactImageUrl
    );
}
