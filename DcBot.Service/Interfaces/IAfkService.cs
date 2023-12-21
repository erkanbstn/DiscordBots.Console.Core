using DcBot.Core.Core;

namespace DcBot.Service.Interfaces
{
    public interface IAfkService : IRepositoryService<Afk>
    {
        public Task EnsureAfkExistsAsync(string userId, string serverId, string afkReason);
    }
}