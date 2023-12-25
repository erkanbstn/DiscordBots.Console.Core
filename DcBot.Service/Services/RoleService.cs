using DcBot.Core.Core;
using DcBot.Core.Enums;
using DcBot.Data.Interfaces;
using DcBot.Service.Interfaces;
using System.Linq.Expressions;

namespace DcBot.Service.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleDal _RoleDal;

        public RoleService(IRoleDal RoleRepository)
        {
            _RoleDal = RoleRepository;
        }

        public async Task ChangeRoleTypeAsync(RoleTypes roleTypes, Role role)
        {
            await _RoleDal.ChangeRoleTypeAsync(roleTypes, role);
        }

        public async Task ChangeStatusAsync(Role t, bool status)
        {
            if (status)
            {
                t.Status = false;
            }
            else
            {
                t.Status = true;
            }
            await _RoleDal.ChangeStatusAsync(t);
        }

        public async Task DeleteAsync(Role t)
        {
            await _RoleDal.DeleteAsync(t);
        }

        public async Task<Role> FirstOrDefaultAsync(Expression<Func<Role, bool>> filter)
        {
            return await _RoleDal.FirstOrDefaultAsync(filter);
        }

        public async Task<Role> GetByDiscordIdAsync(ulong id, int? RoleId)
        {
            return await _RoleDal.GetByDiscordIdAsync(x => x.DiscordId == id.ToString() && x.Id == RoleId);
        }

        public async Task<Role> GetByIdAsync(int? id)
        {
            return await _RoleDal.GetByIdAsync(id);
        }

        public async Task InsertAsync(Role t)
        {
            await _RoleDal.InsertAsync(t);
        }

        public async Task<List<Role>> ToListAsync()
        {
            return await _RoleDal.ToListAsync();
        }

        public async Task<List<Role>> ToListByFilterAsync(Expression<Func<Role, bool>> filter)
        {
            return await _RoleDal.ToListByFilterAsync(filter);
        }

        public async Task UpdateAsync(Role t)
        {
            await _RoleDal.UpdateAsync(t);
        }
    }
}