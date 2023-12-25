using DcBot.Core.Core;
using DcBot.Core.Enums;

namespace DcBot.Data.Interfaces
{
    public interface IRoleDal : IRepositoryDal<Role>
    {
        Task ChangeRoleTypeAsync(RoleTypes roleTypes, Role role);
    }
}