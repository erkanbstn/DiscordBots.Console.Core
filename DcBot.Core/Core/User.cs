﻿namespace DcBot.Core.Core
{
    public class User : BaseModal
    {
        public string? UserName { get; set; }
        public int? Money { get; set; }
        public int? DcServerId { get; set; }
        public DcServer? DcServer { get; set; }
    }
}