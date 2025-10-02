using ChatApp.Application.Bases;
using MediatR;

namespace ChatApp.Application.Features.Users.Queries.Models
{
    public record GetCurrentUserIdQuery() : IRequest<ApiResponse<Guid>>;
}
