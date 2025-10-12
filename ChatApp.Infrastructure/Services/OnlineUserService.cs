using ChatApp.Application.Services.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace ChatApp.Infrastructure.Services
{
    public class OnlineUserService : IOnlineUserService
    {
        private readonly IMemoryCache _cache;
        private const string CacheKey = "OnlineUsers";

        public OnlineUserService(IMemoryCache cache)
        {
            _cache = cache;
        }

        private Dictionary<Guid, List<string>>? GetOnlineUsers()
        {
            if (!_cache.TryGetValue(CacheKey, out Dictionary<Guid, List<string>>? onlineUsers))
            {
                onlineUsers = new Dictionary<Guid, List<string>>();
                _cache.Set(CacheKey, onlineUsers);
            }
            return onlineUsers;
        }

        public void UserConnected(Guid userId, string connectionId)
        {
            var onlineUsers = GetOnlineUsers();
            if (!onlineUsers!.ContainsKey(userId))
                onlineUsers[userId] = new List<string>();

            onlineUsers[userId].Add(connectionId);
        }

        public void UserDisconnected(Guid userId, string connectionId)
        {
            var onlineUsers = GetOnlineUsers();
            if (onlineUsers!.ContainsKey(userId))
            {
                onlineUsers[userId].Remove(connectionId);
                if (!onlineUsers[userId].Any())
                    onlineUsers.Remove(userId);
            }
        }

        public bool IsUserOnline(Guid userId)
        {
            var onlineUsers = GetOnlineUsers();
            return onlineUsers!.ContainsKey(userId);
        }

        public List<string> GetOnlineUsersAsync(List<Guid> ActiveUserIds)
        {
            var onlineUsers = GetOnlineUsers();
            return onlineUsers!.Where(u => ActiveUserIds.Contains(u.Key))
                .SelectMany(u => u.Value).ToList();
        }
    }
}
