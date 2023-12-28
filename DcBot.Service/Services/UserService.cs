using DcBot.Core.Core;
using DcBot.Data.Interfaces;
using DcBot.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DcBot.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserDal _UserDal;

        public UserService(IUserDal UserRepository)
        {
            _UserDal = UserRepository;
        }

        public async Task ChangeStatusAsync(User t, bool status)
        {
            if (status)
            {
                t.Status = false;
            }
            else
            {
                t.Status = true;
            }
            await _UserDal.ChangeStatusAsync(t);
        }

        public async Task DeleteAllQueryAsync(string tableName)
        {
            await _UserDal.DeleteAllQueryAsync(tableName);
        }

        public async Task DeleteAsync(User t)
        {
            await _UserDal.DeleteAsync(t);
        }

        public async Task<User> FirstOrDefaultAsync(Expression<Func<User, bool>> filter)
        {
            return await _UserDal.FirstOrDefaultAsync(filter);
        }

        public async Task<User> GetByDiscordIdAsync(ulong id, int? dcServerId)
        {
            return await _UserDal.GetByDiscordIdAsync(x => x.DiscordId == id.ToString() && x.Id == dcServerId);
        }

        public async Task<User> GetByIdAsync(int? id)
        {
            return await _UserDal.GetByIdAsync(id);
        }

        public async Task InsertAsync(User t)
        {
            await _UserDal.InsertAsync(t);
        }

        public async Task<List<User>> ToListAsync()
        {
            return await _UserDal.ToListAsync();
        }

        public async Task<List<User>> ToListByFilterAsync(Expression<Func<User, bool>> filter)
        {
            return await _UserDal.ToListByFilterAsync(filter);
        }

        public async Task<List<User>> ToListByNoTrackAsync()
        {
            return await _UserDal.ToListByNoTrackAsync();
        }

        public async Task<List<User>> ToListFilteringByNoTrackAsync(Expression<Func<User, bool>> filter)
        {
            return await _UserDal.ToListFilteringByNoTrackAsync(filter);
        }

        public async Task UpdateAsync(User t)
        {
            await _UserDal.UpdateAsync(t);
        }
    }
}
