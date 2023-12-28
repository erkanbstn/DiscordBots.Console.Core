namespace DcBot.Core.Core
{
    public class DcServer : BaseModal
    {
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public List<Afk> Afks { get; set; }
        public List<Role> Roles { get; set; }
        public List<User> Users { get; set; }
        public List<Channel> Channels { get; set; }
        public List<Skill> Skills { get; set; }
        public List<RoleTypeRelation> RoleTypeRelations { get; set; }
    }
}