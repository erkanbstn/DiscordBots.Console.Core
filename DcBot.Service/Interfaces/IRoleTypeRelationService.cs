using DcBot.Core.Core;
using DcBot.Core.Enums;

namespace DcBot.Service.Interfaces
{
    public interface IRoleTypeRelationService : IRepositoryService<RoleTypeRelation>
    {
        Task ChangeRoleTypeAsync(RoleTypes roleTypes, string roleId, int serverId, string transactionType = null);
        Task<RoleTypeRelation> GetRoleTypeAsync(RoleTypes roleTypes, int? serverId);
    }
}