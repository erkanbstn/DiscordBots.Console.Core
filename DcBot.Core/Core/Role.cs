﻿using DcBot.Core.Enums;

namespace DcBot.Core.Core
{
    public class Role : BaseModal
    {
        public string Name { get; set; }
        public int? DcServerId { get; set; }
        public DcServer? DcServer { get; set; }
    }
}