using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.Contacts.Commands.Models
{
    public record AddToContactsByPhoneNumberCommand(string PhoneNumber) : IRequest<ApiResponse<string>>;
}
