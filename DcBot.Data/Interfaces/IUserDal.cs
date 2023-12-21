using DcBot.Core.Core;

namespace DcBot.Data.Interfaces
{
    public interface IUserDal : IRepositoryDal<User>
    {
        public Task<User> EnsureUserExistsAsync(string userId, string userName);
    }
}