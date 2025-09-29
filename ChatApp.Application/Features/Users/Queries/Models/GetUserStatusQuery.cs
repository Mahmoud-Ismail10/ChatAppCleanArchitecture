using ChatApp.Application.Bases;
using ChatApp.Application.Features.Users.Queries.Responses;
using MediatR;

namespace ChatApp.Application.Features.Users.Queries.Models
{
    public record GetUserStatusQuery(Guid UserId) : IRequest<ApiResponse<GetUserStatusResponse>>;
}
