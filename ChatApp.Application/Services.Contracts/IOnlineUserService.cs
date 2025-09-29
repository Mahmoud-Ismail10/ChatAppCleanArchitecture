namespace ChatApp.Application.Services.Contracts
{
    public interface IOnlineUserService
    {
        List<string> GetUserConnections(Guid userId);
        bool IsUserOnline(Guid userId);
        void UserConnected(Guid userId, string connectionId);
        void UserDisconnected(Guid userId, string connectionId);
    }
}
