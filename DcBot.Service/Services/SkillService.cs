using DcBot.Core.Core;
using DcBot.Data.Interfaces;
using DcBot.Service.Interfaces;
using System.Linq.Expressions;

namespace DcBot.Service.Services
{
    public class SkillService : ISkillService
    {
        private readonly ISkillDal _SkillDal;

        public SkillService(ISkillDal SkillRepository)
        {
            _SkillDal = SkillRepository;
        }

        public async Task ChangeStatusAsync(Skill t, bool status)
        {
            if (status)
            {
                t.Status = false;
            }
            else
            {
                t.Status = true;
            }
            await _SkillDal.ChangeStatusAsync(t);
        }

        public async Task DeleteAllQueryAsync(string tableName)
        {
            await _SkillDal.DeleteAllQueryAsync(tableName);
        }

        public async Task DeleteAsync(Skill t)
        {
            await _SkillDal.DeleteAsync(t);
        }

        public async Task<Skill> FirstOrDefaultAsync(Expression<Func<Skill, bool>> filter)
        {
            return await _SkillDal.FirstOrDefaultAsync(filter);
        }

        public async Task<Skill> GetByDiscordIdAsync(ulong id, int? SkillId)
        {
            return await _SkillDal.GetByDiscordIdAsync(x => x.DiscordId == id.ToString() && x.Id == SkillId);
        }

        public async Task<Skill> GetByIdAsync(int? id)
        {
            return await _SkillDal.GetByIdAsync(id);
        }

        public async Task InsertAsync(Skill t)
        {
            await _SkillDal.InsertAsync(t);
        }

        public async Task<List<Skill>> ToListAsync()
        {
            return await _SkillDal.ToListAsync();
        }

        public async Task<List<Skill>> ToListByFilterAsync(Expression<Func<Skill, bool>> filter)
        {
            return await _SkillDal.ToListByFilterAsync(filter);
        }

        public async Task<List<Skill>> ToListByNoTrackAsync()
        {
            return await _SkillDal.ToListByNoTrackAsync();
        }

        public async Task<List<Skill>> ToListFilteringByNoTrackAsync(Expression<Func<Skill, bool>> filter)
        {
            return await _SkillDal.ToListFilteringByNoTrackAsync(filter);
        }

        public async Task UpdateAsync(Skill t)
        {
            await _SkillDal.UpdateAsync(t);
        }
    }
}