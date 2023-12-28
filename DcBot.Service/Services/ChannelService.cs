using DcBot.Core.Core;
using DcBot.Data.Interfaces;
using DcBot.Service.Interfaces;
using System.Linq.Expressions;

namespace DcBot.Service.Services
{
    public class ChannelService : IChannelService
    {
        private readonly IChannelDal _ChannelDal;

        public ChannelService(IChannelDal ChannelRepository)
        {
            _ChannelDal = ChannelRepository;
        }

        public async Task ChangeStatusAsync(Channel t, bool status)
        {
            if (status)
            {
                t.Status = false;
            }
            else
            {
                t.Status = true;
            }
            await _ChannelDal.ChangeStatusAsync(t);
        }

        public async Task DeleteAllQueryAsync(string tableName)
        {
             await _ChannelDal.DeleteAllQueryAsync(tableName);
        }

        public async Task DeleteAsync(Channel t)
        {
            await _ChannelDal.DeleteAsync(t);
        }

        public async Task<Channel> FirstOrDefaultAsync(Expression<Func<Channel, bool>> filter)
        {
            return await _ChannelDal.FirstOrDefaultAsync(filter);
        }

        public async Task<Channel> GetByDiscordIdAsync(ulong id, int? ChannelId)
        {
            return await _ChannelDal.GetByDiscordIdAsync(x => x.DiscordId == id.ToString() && x.Id == ChannelId);
        }

        public async Task<Channel> GetByIdAsync(int? id)
        {
            return await _ChannelDal.GetByIdAsync(id);
        }

        public async Task InsertAsync(Channel t)
        {
            await _ChannelDal.InsertAsync(t);
        }

        public async Task<List<Channel>> ToListAsync()
        {
            return await _ChannelDal.ToListAsync();
        }

        public async Task<List<Channel>> ToListByFilterAsync(Expression<Func<Channel, bool>> filter)
        {
            return await _ChannelDal.ToListByFilterAsync(filter);
        }

        public async Task<List<Channel>> ToListByNoTrackAsync()
        {
            return await _ChannelDal.ToListByNoTrackAsync();
        }

        public async Task<List<Channel>> ToListFilteringByNoTrackAsync(Expression<Func<Channel, bool>> filter)
        {
            return await _ChannelDal.ToListFilteringByNoTrackAsync(filter);
        }

        public async Task UpdateAsync(Channel t)
        {
            await _ChannelDal.UpdateAsync(t);
        }
    }
}
