using DcBot.Core.Core;
using DcBot.Core.Enums;
using DcBot.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DcBot.Data.Datas
{
    public class EFRoleTypeRelationDal : EFRepositoryDal<RoleTypeRelation>, IRoleTypeRelationDal
    {
        private readonly AppDbContext _appDbContext;

        public EFRoleTypeRelationDal(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task ChangeRoleTypeAsync(RoleTypes roleTypes, string roleId, int serverId, string transactionType = null)
        {
            var roleTypeRelation = _appDbContext.RoleTypeRelations.FirstOrDefault(x => x.DiscordId == roleId && x.DcServerId == serverId);

            if (transactionType?.ToLower() == "a" || roleTypeRelation == null)
            {
                RoleTypeRelation roleType = new RoleTypeRelation
                {
                    RoleType = roleTypes,
                    DiscordId = roleId,
                    DcServerId = serverId
                };
                await _appDbContext.RoleTypeRelations.AddAsync(roleType);
            }
            else if (transactionType?.ToLower() == "d")
            {
                var roleTypeRelations = await _appDbContext.RoleTypeRelations.Where(x => x.DiscordId == roleId && x.DcServerId == serverId).ToListAsync();
                _appDbContext.RoleTypeRelations.RemoveRange(roleTypeRelations);

                RoleTypeRelation roleType = new RoleTypeRelation
                {
                    RoleType = RoleTypes.NoType,
                    DiscordId = roleId,
                    DcServerId = serverId
                };
                await _appDbContext.RoleTypeRelations.AddAsync(roleType);
            }
            else
            {
                roleTypeRelation.RoleType = roleTypes;
            }
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<RoleTypeRelation> GetRoleTypeAsync(RoleTypes roleTypes, int? serverId)
        {
            var roleTypeRelation = await _appDbContext.RoleTypeRelations.FirstOrDefaultAsync(x => x.DcServerId == serverId && x.RoleType == roleTypes);
            return roleTypeRelation;
        }
    }
}