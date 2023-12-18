namespace DcBot.Core.Concrete
{
    public class BaseModal
    {
        public int? Id { get; set; }
        public string? DiscordId { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public DateTime? DeletedAt { get; set; }
        public bool Status { get; set; } = true;
    }
}