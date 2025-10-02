using ChatApp.Application.Bases;
using ChatApp.Application.Features.Users.Queries.Responses;
using MediatR;

namespace ChatApp.Application.Features.Users.Queries.Models
{
    public record GetCurrentUserQuery() : IRequest<ApiResponse<GetCurrentUserResponse>>;
}
