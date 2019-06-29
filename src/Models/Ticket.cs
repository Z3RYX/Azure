using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Models
{
    public class Ticket
    {
        public readonly int ID;
        public readonly DateTime CreationDate;
        public DateTime? CloseDate;
        public ulong? MessageID;
        public bool IsOpen;
        public readonly ulong GuildID;
        public readonly ulong UserID;
        public ulong? ClosedBy;
        public string Content;
        public string ClosingReason;

        public Ticket(SocketGuildUser user, string Content)
        {
            int TicketCount = Directory.GetFiles(FileSystem.TICKETS).Count();
            ID = TicketCount + 1;
            CreationDate = DateTime.UtcNow;
            MessageID = null;
            CloseDate = null;
            IsOpen = true;
            GuildID = user.Guild.Id;
            UserID = user.Id;
            ClosedBy = null;
            this.Content = Content;
            ClosingReason = null;
        }

        public Ticket EditTicket(string content, string closeReason = null)
        {
            Content = content;
            ClosingReason = closeReason;
            return this;
        }

        public Ticket CloseTicket(SocketGuildUser user, string Reason = null)
        {
            IsOpen = false;
            CloseDate = DateTime.UtcNow;
            ClosedBy = user.Id;
            ClosingReason = Reason;
            return this;
        }

        public Ticket SetMessageID(ulong msgID)
        {
            MessageID = msgID;
            return this;
        }
    }
}
