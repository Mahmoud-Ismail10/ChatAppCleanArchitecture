using ChatApp.Domain.Enums;

namespace ChatApp.Application.Features.Messages.Commands.Responses
{
    public record ChatMemberUpdatedDto(
        string ChatId,
        string LastMessage,
        DateTimeOffset LastMessageAt,
        MessageType MessageType);
}
