namespace DcBot.Core.Core
{
    public class Afk : BaseModal
    {
        public string? Reason { get; set; }
        public bool? AfkStatus { get; set; }
        public DateTime? AfkTime { get; set; }
        public int? DcServerId { get; set; }
        public DcServer? DcServer { get; set; }
    }
}