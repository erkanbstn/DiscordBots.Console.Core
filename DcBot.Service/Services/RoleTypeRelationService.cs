using DcBot.Core.Core;
using DcBot.Core.Enums;
using DcBot.Data.Interfaces;
using DcBot.Service.Interfaces;
using System.Linq.Expressions;

namespace DcBot.Service.Services
{
    public class RoleTypeRelationService : IRoleTypeRelationService
    {
        private readonly IRoleTypeRelationDal _RoleTypeRelationDal;

        public RoleTypeRelationService(IRoleTypeRelationDal RoleTypeRelationRepository)
        {
            _RoleTypeRelationDal = RoleTypeRelationRepository;
        }

        public async Task ChangeRoleTypeAsync(RoleTypes roleTypes, string roleId, int serverId, string transactionType = null)
        {
            await _RoleTypeRelationDal.ChangeRoleTypeAsync(roleTypes, roleId, serverId, transactionType);
        }

        public async Task ChangeStatusAsync(RoleTypeRelation t, bool status)
        {
            if (status)
            {
                t.Status = false;
            }
            else
            {
                t.Status = true;
            }
            await _RoleTypeRelationDal.ChangeStatusAsync(t);
        }

        public async Task DeleteAllQueryAsync(string tableName)
        {
            await _RoleTypeRelationDal.DeleteAllQueryAsync(tableName);
        }

        public async Task DeleteAsync(RoleTypeRelation t)
        {
            await _RoleTypeRelationDal.DeleteAsync(t);
        }

        public async Task<RoleTypeRelation> FirstOrDefaultAsync(Expression<Func<RoleTypeRelation, bool>> filter)
        {
            return await _RoleTypeRelationDal.FirstOrDefaultAsync(filter);
        }

        public async Task<RoleTypeRelation> GetByDiscordIdAsync(ulong id, int? RoleTypeRelationId)
        {
            return await _RoleTypeRelationDal.GetByDiscordIdAsync(x => x.DiscordId == id.ToString() && x.Id == RoleTypeRelationId);
        }

        public async Task<RoleTypeRelation> GetByIdAsync(int? id)
        {
            return await _RoleTypeRelationDal.GetByIdAsync(id);
        }

        public async Task<RoleTypeRelation> GetRoleTypeAsync(RoleTypes roleTypes, int? serverId)
        {
            return await _RoleTypeRelationDal.GetRoleTypeAsync(roleTypes, serverId);
        }

        public async Task InsertAsync(RoleTypeRelation t)
        {
            await _RoleTypeRelationDal.InsertAsync(t);
        }

        public async Task<List<RoleTypeRelation>> ToListAsync()
        {
            return await _RoleTypeRelationDal.ToListAsync();
        }

        public async Task<List<RoleTypeRelation>> ToListByFilterAsync(Expression<Func<RoleTypeRelation, bool>> filter)
        {
            return await _RoleTypeRelationDal.ToListByFilterAsync(filter);
        }

        public async Task<List<RoleTypeRelation>> ToListByNoTrackAsync()
        {
            return await _RoleTypeRelationDal.ToListByNoTrackAsync();
        }

        public async Task<List<RoleTypeRelation>> ToListFilteringByNoTrackAsync(Expression<Func<RoleTypeRelation, bool>> filter)
        {
            return await _RoleTypeRelationDal.ToListFilteringByNoTrackAsync(filter);
        }

        public async Task UpdateAsync(RoleTypeRelation t)
        {
            await _RoleTypeRelationDal.UpdateAsync(t);
        }
    }
}