using DcBot.Core.Core;
using DcBot.Data.Interfaces;

namespace DcBot.Data.Datas
{
    public class EFChannelDal : EFRepositoryDal<Channel>, IChannelDal
    {
        private readonly AppDbContext _appDbContext;
        public EFChannelDal(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
    }
}