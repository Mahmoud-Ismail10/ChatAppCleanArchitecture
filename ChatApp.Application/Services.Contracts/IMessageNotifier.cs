using ChatApp.Application.Features.Messages.Commands.Responses;

namespace ChatApp.Application.Services.Contracts
{
    public interface IMessageNotifier
    {
        Task NotifyMessageToContactAsync(SendMessageDto message);
    }
}
