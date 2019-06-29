using Azure.Models;
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

        public enum ModlogType
        {
            Kick,
            Ban,
            Muted,
            Unmuted,
            Warn,
            Pardon
        }

        public static async Task<Embed> CreateModlog(SocketGuild guild, SocketUser caller, SocketUser user, ModlogType type, string Reason = null)
        {
            var guildObj = FileSystem.GetGuild(guild);
            var guilduser = guildObj.GuildUsers.Where(x => x.ID == user.Id).First();
            if (!guildObj.AllowModlog) throw new OperationCanceledException("Modlog isn't enabled on this server\nUse //setmodlogchannel to enable modlog");
            var embed = new EmbedBuilder();
            Embed Result = null;
            switch (type) {
                case ModlogType.Ban:
                    Result = embed
                        .WithColor(Color.Red)
                        .WithCurrentTimestamp()
                        .WithTitle($"[#{guildObj.ModlogCount + 1}] {caller.ToString()} banned {user.ToString()} from the server")
                        .WithDescription($"**Reason:** {Reason ?? "No reason given"}")
                        .Build();
                    break;
                case ModlogType.Kick:
                    Result = embed
                        .WithColor(Color.Orange)
                        .WithCurrentTimestamp()
                        .WithTitle($"[#{guildObj.ModlogCount + 1}] {caller.ToString()} kicked {user.ToString()} from the server")
                        .WithDescription($"**Reason:** {Reason ?? "No reason given"}")
                        .Build();
                    break;
                case ModlogType.Muted:
                    Result = embed
                        .WithColor(Color.Purple)
                        .WithCurrentTimestamp()
                        .WithTitle($"[#{guildObj.ModlogCount + 1}] {caller.ToString()} muted {user.ToString()}")
                        .WithDescription($"**Reason:** {Reason ?? "No reason given"}")
                        .Build();
                    break;
                case ModlogType.Warn:
                    guilduser = guilduser.IncreaseWarnCount();
                    guildObj.SetGuildUser(guilduser);
                    Result = embed
                        .WithColor(Color.Gold)
                        .WithCurrentTimestamp()
                        .WithTitle($"[#{guildObj.ModlogCount + 1}] {caller.ToString()} warned {user.ToString()}")
                        .WithDescription($"{user.Username} has a total of {guilduser.WarnCount} now\n**Reason:** {Reason ?? "No reason given"}")
                        .Build();
                    break;
                case ModlogType.Unmuted:
                    Result = embed
                        .WithColor(Color.Purple)
                        .WithCurrentTimestamp()
                        .WithTitle($"[#{guildObj.ModlogCount + 1}] {caller.ToString()} unmuted {user.ToString()}")
                        .WithDescription($"**Reason:** {Reason ?? "No reason given"}")
                        .Build();
                    break;
                case ModlogType.Pardon:
                    guilduser = guilduser.DecreaseWarnCount();
                    guildObj.SetGuildUser(guilduser);
                    Result = embed
                        .WithColor(Color.Green)
                        .WithCurrentTimestamp()
                        .WithTitle($"[#{guildObj.ModlogCount + 1}] {caller.ToString()} pardoned {user.ToString()} and a warn has been removed")
                        .WithDescription($"{user.Username} has a total of {guilduser.WarnCount} now\n**Reason:** {Reason ?? "No reason given"}")
                        .Build();
                    break;
            }

            guildObj = guildObj.IncreaseModlogCount();
            await FileSystem.SetGuild(guildObj);
            return Result;
        }
    }
}
