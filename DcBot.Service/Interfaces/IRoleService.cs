using DcBot.Core.Core;
using DcBot.Core.Enums;

namespace DcBot.Service.Interfaces
{
    public interface IRoleService : IRepositoryService<Role>
    {
        Task ChangeRoleTypeAsync(RoleTypes roleTypes, Role role);
    }
}