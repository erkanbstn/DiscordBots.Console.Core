using DcBot.Core.Core;

namespace DcBot.Data.Interfaces
{
    public interface IAfkDal : IRepositoryDal<Afk>
    {
        public Task EnsureAfkExistsAsync(string userId, string serverId, string afkReason);
    }
}