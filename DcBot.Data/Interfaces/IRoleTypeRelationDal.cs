using DcBot.Core.Core;
using DcBot.Core.Enums;

namespace DcBot.Data.Interfaces
{
    public interface IRoleTypeRelationDal : IRepositoryDal<RoleTypeRelation>
    {
        Task ChangeRoleTypeAsync(RoleTypes roleTypes, string roleId, int serverId, string transactionType = null);
        Task<RoleTypeRelation> GetRoleTypeAsync(RoleTypes roleTypes, int? serverId);
    }
}