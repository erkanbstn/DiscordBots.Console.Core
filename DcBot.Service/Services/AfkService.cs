using DcBot.Core.Core;
using DcBot.Data.Interfaces;
using DcBot.Service.Interfaces;
using System.Linq.Expressions;

namespace DcBot.Service.Services
{
    public class AfkService : IAfkService
    {
        private readonly IAfkDal _AfkDal;

        public AfkService(IAfkDal AfkRepository)
        {
            _AfkDal = AfkRepository;
        }

        public async Task ChangeStatusAsync(Afk t, bool status)
        {
            if (status)
            {
                t.Status = false;
            }
            else
            {
                t.Status = true;
            }
            await _AfkDal.ChangeStatusAsync(t);
        }

        public async Task DeleteAllQueryAsync(string tableName)
        {
            await _AfkDal.DeleteAllQueryAsync(tableName);
        }

        public async Task DeleteAsync(Afk t)
        {
            await _AfkDal.DeleteAsync(t);
        }

        public async Task EnsureAfkExistsAsync(string userId, string serverId, string afkReason)
        {
            await _AfkDal.EnsureAfkExistsAsync(userId, serverId, afkReason);
        }

        public async Task<Afk> FirstOrDefaultAsync(Expression<Func<Afk, bool>> filter)
        {
            return await _AfkDal.FirstOrDefaultAsync(filter);
        }

        public async Task<Afk> GetByDiscordIdAsync(ulong id, int? AfkId)
        {
            return await _AfkDal.GetByDiscordIdAsync(x => x.DiscordId == id.ToString() && x.Id == AfkId);
        }

        public async Task<Afk> GetByIdAsync(int? id)
        {
            return await _AfkDal.GetByIdAsync(id);
        }

        public async Task InsertAsync(Afk t)
        {
            await _AfkDal.InsertAsync(t);
        }

        public async Task<List<Afk>> ToListAsync()
        {
            return await _AfkDal.ToListAsync();
        }

        public async Task<List<Afk>> ToListByFilterAsync(Expression<Func<Afk, bool>> filter)
        {
            return await _AfkDal.ToListByFilterAsync(filter);
        }

        public async Task<List<Afk>> ToListByNoTrackAsync()
        {
             return await _AfkDal.ToListByNoTrackAsync();
        }

        public async Task<List<Afk>> ToListFilteringByNoTrackAsync(Expression<Func<Afk, bool>> filter)
        {
             return await _AfkDal.ToListFilteringByNoTrackAsync(filter);
        }

        public async Task UpdateAsync(Afk t)
        {
            await _AfkDal.UpdateAsync(t);
        }
    }
}
