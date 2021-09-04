using System;
using System.Collections.Generic;
using System.Text;

namespace Cirilla.Steam
{
    public class SteamAccount
    {
        public long SteamId64 { get; set; }
        public string AccountName { get; set; } = "";
        public string PersonaName { get; set; } = "";
        public bool RememberPassword { get; set; }
        public bool MostRecent { get; set; }
        public long Timestamp { get; set; }

        public long SteamId3 => SteamId64 - 76561197960265728;
    }
}
