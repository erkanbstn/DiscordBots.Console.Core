using DcBot.Core.Concrete;
using DcBot.Data.Interfaces;
using DcBot.Service.Interfaces;
using System.Data;
using System.Linq.Expressions;

namespace DcBot.Service.Services
{
    public class DcServerService : IDcServerService
    {
        private readonly IDcServerDal _DcServerDal;

        public DcServerService(IDcServerDal DcServerRepository)
        {
            _DcServerDal = DcServerRepository;
        }

        public async Task ChangeStatusAsync(DcServer t, bool status)
        {
            if (status)
            {
                t.Status = false;
            }
            else
            {
                t.Status = true;
            }
            await _DcServerDal.ChangeStatusAsync(t);
        }

        public async Task DeleteAsync(DcServer t)
        {
            await _DcServerDal.DeleteAsync(t);
        }

        public async Task<DcServer> FirstOrDefaultAsync(Expression<Func<DcServer, bool>> filter)
        {
            return await _DcServerDal.FirstOrDefaultAsync(filter);
        }

        public async Task<DcServer> GetByDiscordIdAsync(ulong id, int? dcServerId)
        {
            return await _DcServerDal.GetByDiscordIdAsync(x => x.DiscordId == id.ToString() && x.Id == dcServerId);
        }

        public async Task<DcServer> GetByIdAsync(int? id)
        {
            return await _DcServerDal.GetByIdAsync(id);
        }

        public async Task InsertAsync(DcServer t)
        {
            await _DcServerDal.InsertAsync(t);
        }

        public async Task<List<DcServer>> ToListAsync()
        {
            return await _DcServerDal.ToListAsync();
        }

        public async Task<List<DcServer>> ToListByFilterAsync(Expression<Func<DcServer, bool>> filter)
        {
            return await _DcServerDal.ToListByFilterAsync(filter);
        }

        public async Task UpdateAsync(DcServer t)
        {
            await _DcServerDal.UpdateAsync(t);
        }
    }
}