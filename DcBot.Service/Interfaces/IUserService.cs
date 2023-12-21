using DcBot.Core.Core;

namespace DcBot.Service.Interfaces
{
    public interface IUserService : IRepositoryService<User>
    {
        public Task<User> EnsureUserExistsAsync(string userId, string userName);
    }
}