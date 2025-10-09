namespace ChatApp.Application.Features.Messages.Commands.Responses
{
    public record ReceiveMessageDto(string Id, Guid ChatId);
}
