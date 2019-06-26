using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Models
{
    public class User
    {
        public readonly ulong ID;
        public List<ulong> SharedGuilds;
        public List<int> Tickets;
        public bool IsBlacklisted;

        public User(SocketUser user)
        {
            ID = user.Id;
            SharedGuilds = user.MutualGuilds.Select(x => x.Id).ToList();
            Tickets = new List<int>();
            IsBlacklisted = false;
        }

        public User ToggleBlacklist()
        {
            IsBlacklisted = !IsBlacklisted;
            return this;
        }

        public User UpdateSharedGuilds(SocketUser user)
        {
            SharedGuilds = user.MutualGuilds.Select(x => x.Id).ToList();
            return this;
        }

        public User AddTicket(int TicketID)
        {
            Tickets.Add(TicketID);
            return this;
        }
    }
}
