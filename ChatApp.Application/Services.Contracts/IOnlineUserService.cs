namespace ChatApp.Application.Services.Contracts
{
    public interface IOnlineUserService
    {
        List<string> GetOnlineUsersAsync(List<Guid> ActiveUserIds);
        bool IsUserOnline(Guid userId);
        void UserConnected(Guid userId, string connectionId);
        void UserDisconnected(Guid userId, string connectionId);
    }
}
