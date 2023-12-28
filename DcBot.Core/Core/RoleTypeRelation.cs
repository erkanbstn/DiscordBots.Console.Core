using DcBot.Core.Enums;

namespace DcBot.Core.Core
{
    public class RoleTypeRelation : BaseModal
    {
        public RoleTypes RoleType { get; set; }
        public int? DcServerId { get; set; }
        public DcServer? DcServer { get; set; }
    }
}