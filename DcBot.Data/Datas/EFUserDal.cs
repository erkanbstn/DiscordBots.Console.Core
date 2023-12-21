using DcBot.Core.Core;
using DcBot.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DcBot.Data.Datas
{
    public class EFUserDal : EFRepositoryDal<User>, IUserDal
    {
        private readonly AppDbContext _appDbContext;
        public EFUserDal(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<User> EnsureUserExistsAsync(string userId, string userName)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.DiscordId == userId);
            if (user == null)
            {
                user = new User { Money = 0, DiscordId = userId.ToString(), UserName = userName };
                _appDbContext.Users.Add(user);
                await _appDbContext.SaveChangesAsync();

                var addedUser = await _appDbContext.Users.FirstOrDefaultAsync(x => x.DiscordId == userId);
                var skill = new Skill { DailyGgCount = 1, Level = 1, Name = "Earn", Xp = 0, XpRequired = 1400, UserId = addedUser.Id };
                await _appDbContext.Skills.AddAsync(skill);
                await _appDbContext.SaveChangesAsync();
            }
            return user;
        }
    }
}