namespace DcBot.Core.Core
{
    public class User : BaseModal
    {
        public string UserName { get; set; }
        public int? Money { get; set; }
        public List<Skill>? Skill { get; set; }
    }
}