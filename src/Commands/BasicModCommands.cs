using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Commands
{
    public class BasicModCommands : ModuleBase<SocketCommandContext>
    {
        Config config = Config.GetConfig();

        [Command("kick")]
        public async Task KickAsync(SocketGuildUser User, [Remainder] string Reason = null)
        {
            if (!CommandUtils.HasPermission(Context.Guild.GetUser(Context.User.Id), GuildPermission.KickMembers)) {
                await ReplyAsync("You are missing the permissions to kick other users.");
                return;
            }

            if (!CommandUtils.HasPermission(Context.Guild.CurrentUser, GuildPermission.KickMembers)) {
                await ReplyAsync("I am missing the permissions to kick other users.");
                return;
            }

            try {
                await User.KickAsync(Reason);
                await ReplyAsync($"{User.ToString()} has been kicked from the server.\n**Reason**: {Reason ?? "None"}");
                Embed modlog = await CommandUtils.CreateModlog(Context.Guild, Context.User, User as SocketUser, CommandUtils.ModlogType.Kick, Reason);
                await Context.Guild.GetTextChannel(FileSystem.GetGuild(Context.Guild).ModlogChannel.Value).SendMessageAsync("", embed: modlog);
            } catch(Exception e) {
                await ReplyAsync("Something has gone horribly wrong and the dev has been notified.");
                await Context.Client.GetUser(config.AuthorId).SendMessageAsync($"ERROR on message from {Context.User.ToString()}:\n{Context.Message.Content}\nThrew error: {e.Message}");
            }
        }

        [Command("ban")]
        public async Task BanAsync(SocketGuildUser User, string Reason = "")
        {
            if (!CommandUtils.HasPermission(Context.Guild.GetUser(Context.User.Id), GuildPermission.BanMembers)) {
                await ReplyAsync("You are missing the permissions to ban other users.");
                return;
            }

            if (!CommandUtils.HasPermission(Context.Guild.CurrentUser, GuildPermission.BanMembers)) {
                await ReplyAsync("I am missing the permissions to ban other users.");
                return;
            }

            try {
                await User.BanAsync(reason: Reason);
                await ReplyAsync($"{User.ToString()} has been banned from the server.\n**Reason**: {Reason}");
                Embed modlog = await CommandUtils.CreateModlog(Context.Guild, Context.User, User as SocketUser, CommandUtils.ModlogType.Ban, Reason);
                await Context.Guild.GetTextChannel(FileSystem.GetGuild(Context.Guild).ModlogChannel.Value).SendMessageAsync("", embed: modlog);
            }
            catch (Exception e) {
                await ReplyAsync("Something has gone horribly wrong and the dev has been notified.");
                await Context.Client.GetUser(config.AuthorId).SendMessageAsync($"ERROR on message from {Context.User.ToString()}:\n{Context.Message.Content}\nThrew error: {e.Message}");
            }
        }
    }
}
