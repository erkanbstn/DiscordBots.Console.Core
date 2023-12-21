using DcBot.Core.Core;
using DcBot.Data.Interfaces;

namespace DcBot.Data.Datas
{
    public class EFSkillDal : EFRepositoryDal<Skill>, ISkillDal
    {
        public EFSkillDal(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}