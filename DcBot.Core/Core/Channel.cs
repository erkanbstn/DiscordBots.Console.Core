namespace DcBot.Core.Core
{
    public class Channel : BaseModal
    {
        public string Name { get; set; }
        public string ChannelType { get; set; }
        public int? DcServerId { get; set; }
        public DcServer? DcServer { get; set; }
    }
}
