using ChatApp.Application.Bases;
using ChatApp.Application.Features.MessageStatuses.Queries.Responses;
using MediatR;

namespace ChatApp.Application.Features.MessageStatuses.Queries.Models
{
    public record GetMessageStatusesQuery(Guid MessageId) : IRequest<ApiResponse<List<MessageStatusDto>>>;
}
