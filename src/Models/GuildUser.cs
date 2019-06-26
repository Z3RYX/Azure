using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Models
{
    public class GuildUser
    {
        public readonly ulong ID;
        public int WarnCount;

        public GuildUser(SocketGuildUser user)
        {
            ID = user.Id;
            WarnCount = 0;
        }

        public GuildUser IncreaseWarnCount()
        {
            WarnCount++;
            return this;
        }

        public GuildUser DecreaseWarnCount()
        {
            WarnCount--;
            return this;
        }

        public GuildUser ResetWarnCount()
        {
            WarnCount = 0;
            return this;
        }
    }
}
