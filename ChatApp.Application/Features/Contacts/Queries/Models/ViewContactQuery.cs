using ChatApp.Application.Bases;
using ChatApp.Application.Features.Contacts.Queries.Responses;
using MediatR;

namespace ChatApp.Application.Features.Contacts.Queries.Models
{
    public record ViewContactQuery(Guid ChatOtherMemberId) : IRequest<ApiResponse<ViewContactResponse>>;
}
