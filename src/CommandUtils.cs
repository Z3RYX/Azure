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

        public static bool HasPermission(SocketGuildUser user, GuildPermission permission)
        {
            var userPerms = user.GuildPermissions.ToList();

            if (userPerms.Contains(permission)) return true;

            return false;
        }

        public static bool HasPermissions(SocketGuildUser user, GuildPermission[] permissions, out List<GuildPermission> missingPerms)
        {
            missingPerms = new List<GuildPermission>();

            foreach (var permission in permissions) {
                var userPerms = user.GuildPermissions.ToList();

                if (!userPerms.Contains(permission)) missingPerms.Add(permission);
            }

            if (missingPerms.Count > 0) return false;

            return true;
        }
    }
}
