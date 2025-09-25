using ChatApp.Application.Bases;
using ChatApp.Application.Features.ChatsMember.Queries.Responses;
using MediatR;

namespace ChatApp.Application.Features.ChatsMember.Queries.Models
{
    public record GetAllChatsMemberQuery : IRequest<ApiResponse<List<GetAllChatsMemberResponse>>>;
}
