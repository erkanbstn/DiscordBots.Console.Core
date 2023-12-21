using DcBot.Core.Core;
using DcBot.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DcBot.Data.Datas
{
    public class EFAfkDal : EFRepositoryDal<Afk>, IAfkDal
    {
        private readonly AppDbContext _appDbContext;
        public EFAfkDal(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task EnsureAfkExistsAsync(string userId, string serverId, string afkReason)
        {
            var serverInfo = await _appDbContext.DcServers.FirstOrDefaultAsync(x => x.DiscordId == serverId);
            var afkInfo = await _appDbContext.Afks.FirstOrDefaultAsync(x => x.DcServerId == serverInfo.Id && x.DiscordId == userId);

            if (afkInfo == null)
            {
                await _appDbContext.Afks.AddAsync(new Afk
                {
                    AfkStatus = true,
                    AfkTime = DateTime.Now,
                    DcServerId = serverInfo.Id,
                    Reason = afkReason,
                    DiscordId = userId
                });
                await _appDbContext.SaveChangesAsync();
            }
            else
            {
                afkInfo.AfkStatus = true;
                afkInfo.AfkTime = DateTime.Now;
                afkInfo.Reason = afkReason;
                await _appDbContext.SaveChangesAsync();
            }
        }
    }
}