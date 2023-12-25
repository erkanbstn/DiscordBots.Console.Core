using DcBot.Core.Core;
using DcBot.Core.Enums;
using DcBot.Data.Interfaces;

namespace DcBot.Data.Datas
{
    public class EFRoleDal : EFRepositoryDal<Role>, IRoleDal
    {
        private readonly AppDbContext _appDbContext;
        public EFRoleDal(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task ChangeRoleTypeAsync(RoleTypes roleTypes, Role role)
        {
            role.RoleType = roleTypes;
            await _appDbContext.SaveChangesAsync();
        }
    }
}