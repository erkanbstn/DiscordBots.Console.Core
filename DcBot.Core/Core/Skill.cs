namespace DcBot.Core.Core
{
    public class Skill : BaseModal
    {
        public string Name { get; set; }
        public int? XpRequired { get; set; }
        public int? GmCount { get; set; }
        public int? DailyGmCount { get; set; }
        public int? Xp { get; set; }
        public int? CashAverageMin { get; set; }
        public int? CashAverageMax { get; set; }
        public int? Level { get; set; }
        public DcServer? DcServer { get; set; }
        public int? DcServerId { get; set; }
    }
}