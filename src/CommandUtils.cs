using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure
{
    public static class CommandUtils
    {
        public static bool HasPermissions(SocketGuildUser user, GuildPermission[] permissions)
        {
            foreach (var permission in permissions) {
                var userPerms = user.GuildPermissions.ToList();

                if (!userPerms.Contains(permission)) return false;
            }

            return true;
        }
    }
}
