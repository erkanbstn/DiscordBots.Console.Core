namespace DcBot.Core.Core
{
    public class Skill : BaseModal
    {
        public string Name { get; set; }
        public int? XpRequired { get; set; }
        public int? DailyGgCount { get; set; }
        public int? Xp { get; set; }
        public int? Level { get; set; }
        public User? User { get; set; }
        public int? UserId { get; set; }
    }
}