namespace DcBot.Core.Core
{
    public class DcServer : BaseModal
    {
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public List<Afk> Afks { get; set; }
    }
}