using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.Contacts.Commands.Models
{
    public record RemoveFromContactsCommand(Guid contactId) : IRequest<ApiResponse<string>>;
}
