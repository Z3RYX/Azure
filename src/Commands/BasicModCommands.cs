using Azure.Models;
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

        [Command("help")]
        public async Task Help()
        {
            await ReplyAsync($"I am {config.AppName}\n\nI use the prefix `//`.\nYou can view my commands over at my GitHub page: {config.SourceLink}/wiki/Commands");
        }

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
            }
            catch (OperationCanceledException e) {
                return;
            }
            catch (Exception e) {
                await ReplyAsync("Something has gone horribly wrong and the dev has been notified.");
                await Context.Client.GetUser(config.AuthorId).SendMessageAsync($"ERROR on message from {Context.User.ToString()}:\n{Context.Message.Content}\nThrew error: {e.Message}");
            }
        }

        [Command("ban")]
        public async Task BanAsync(SocketGuildUser User, [Remainder] string Reason = null)
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
                await ReplyAsync($"{User.ToString()} has been banned from the server.\n**Reason**: {Reason ?? "None"}");
                Embed modlog = await CommandUtils.CreateModlog(Context.Guild, Context.User, User as SocketUser, CommandUtils.ModlogType.Ban, Reason);
                await Context.Guild.GetTextChannel(FileSystem.GetGuild(Context.Guild).ModlogChannel.Value).SendMessageAsync("", embed: modlog);
            }
            catch (OperationCanceledException e) {
                return;
            }
            catch (Exception e) {
                await ReplyAsync("Something has gone horribly wrong and the dev has been notified.");
                await Context.Client.GetUser(config.AuthorId).SendMessageAsync($"ERROR on message from {Context.User.ToString()}:\n{Context.Message.Content}\nThrew error: {e.Message}");
            }
        }

        [Command("mute")]
        public async Task MuteAsync(SocketGuildUser User, [Remainder] string Reason = null)
        {
            if (!CommandUtils.HasPermission(Context.Guild.GetUser(Context.User.Id), GuildPermission.ManageRoles)) {
                await ReplyAsync("You are missing the permissions to mute other users.");
                return;
            }

            if (!CommandUtils.HasPermission(Context.Guild.CurrentUser, GuildPermission.ManageRoles)) {
                await ReplyAsync("I am missing the permissions to mute other users.");
                return;
            }

            var Guild = FileSystem.GetGuild(Context.Guild);
            if (Guild.MuteRole.Equals(null)) {
                await ReplyAsync("No role for the mute command assigned.\nUse `//setmuterole <rolename>`to assign a mute role.");
                return;
            }

            if (User.Roles.Where(x => x.Id == Guild.MuteRole.Value).Count() != 0) {
                await ReplyAsync("User is already muted");
                return;
            }

            try {
                await User.AddRoleAsync(Context.Guild.GetRole(Guild.MuteRole.Value));
                await ReplyAsync($"{User.ToString()} has been muted.\nTo unmute them, use `//unmute`\n\n**Reason**: {Reason ?? "None"}");
                Embed modlog = await CommandUtils.CreateModlog(Context.Guild, Context.User, User as SocketUser, CommandUtils.ModlogType.Muted, Reason);
                await Context.Guild.GetTextChannel(FileSystem.GetGuild(Context.Guild).ModlogChannel.Value).SendMessageAsync("", embed: modlog);
            }
            catch (OperationCanceledException e) {
                return;
            }
            catch (Exception e) {
                await ReplyAsync("Something has gone horribly wrong and the dev has been notified.");
                await Context.Client.GetUser(config.AuthorId).SendMessageAsync($"ERROR on message from {Context.User.ToString()}:\n{Context.Message.Content}\nThrew error: {e.Message}");
            }
        }

        [Command("unmute")]
        public async Task UnMuteAsync(SocketGuildUser User, [Remainder] string Reason = null)
        {
            if (!CommandUtils.HasPermission(Context.Guild.GetUser(Context.User.Id), GuildPermission.ManageRoles)) {
                await ReplyAsync("You are missing the permissions to unmute other users.");
                return;
            }

            if (!CommandUtils.HasPermission(Context.Guild.CurrentUser, GuildPermission.ManageRoles)) {
                await ReplyAsync("I am missing the permissions to unmute other users.");
                return;
            }

            var Guild = FileSystem.GetGuild(Context.Guild);
            if (Guild.MuteRole.Equals(null)) {
                await ReplyAsync("User can't be unmuted because no mute role has been set.");
                return;
            }

            if (User.Roles.Where(x => x.Id == Guild.MuteRole.Value).Count() == 0) {
                await ReplyAsync("User isn't muted");
                return;
            }

            try {
                await User.AddRoleAsync(Context.Guild.GetRole(Guild.MuteRole.Value));
                await ReplyAsync($"{User.ToString()} has been muted.\nTo unmute them, use `//unmute`\n\n**Reason**: {Reason ?? "None"}");
                Embed modlog = await CommandUtils.CreateModlog(Context.Guild, Context.User, User as SocketUser, CommandUtils.ModlogType.Muted, Reason);
                await Context.Guild.GetTextChannel(FileSystem.GetGuild(Context.Guild).ModlogChannel.Value).SendMessageAsync("", embed: modlog);
            }
            catch (OperationCanceledException e) {
                return;
            }
            catch (Exception e) {
                await ReplyAsync("Something has gone horribly wrong and the dev has been notified.");
                await Context.Client.GetUser(config.AuthorId).SendMessageAsync($"ERROR on message from {Context.User.ToString()}:\n{Context.Message.Content}\nThrew error: {e.Message}");
            }
        }

        [Command("warn")]
        public async Task WarnAsync(SocketGuildUser User, [Remainder] string Reason = null)
        {
            if (!CommandUtils.HasPermission(Context.Guild.GetUser(Context.User.Id), GuildPermission.BanMembers)) {
                await ReplyAsync("You are missing the permissions to warn other users.");
                return;
            }

            if (!CommandUtils.HasPermissions(Context.Guild.CurrentUser, new GuildPermission[] { GuildPermission.BanMembers, GuildPermission.KickMembers })) {
                await ReplyAsync("I am missing the permissions to warn other users.");
                return;
            }

            var Guild = FileSystem.GetGuild(Context.Guild);
            var user = Guild.GuildUsers.Where(x => x.ID == User.Id).First();

            if (user.WarnCount + 1 >= Guild.WarnsForAction && Guild.WarnsForAction != 0) {
                if (Guild.BanForWarns) {
                    try {
                        await User.BanAsync(reason: Reason);
                        await ReplyAsync($"{User.ToString()} has been banned from the server due to reaching the warn limit.\n**Reason**: {Reason ?? "None"}");
                        Embed modlog = await CommandUtils.CreateModlog(Context.Guild, Context.User, User as SocketUser, CommandUtils.ModlogType.Warn, Reason);
                        await Context.Guild.GetTextChannel(FileSystem.GetGuild(Context.Guild).ModlogChannel.Value).SendMessageAsync("", embed: modlog);
                        Embed modlog2 = await CommandUtils.CreateModlog(Context.Guild, Context.User, User as SocketUser, CommandUtils.ModlogType.Ban, $"{User.ToString()} reached the warn limit and got banned");
                        await Context.Guild.GetTextChannel(FileSystem.GetGuild(Context.Guild).ModlogChannel.Value).SendMessageAsync("", embed: modlog);
                    }
                    catch (OperationCanceledException e) {
                        return;
                    }
                    catch (Exception e) {
                        await ReplyAsync("Something has gone horribly wrong and the dev has been notified.");
                        await Context.Client.GetUser(config.AuthorId).SendMessageAsync($"ERROR on message from {Context.User.ToString()}:\n{Context.Message.Content}\nThrew error: {e.Message}");
                    }
                } else {
                    try {
                        await User.KickAsync(Reason);
                        await ReplyAsync($"{User.ToString()} has been kicked from the server due to reaching the warn limit.\n**Reason**: {Reason ?? "None"}");
                        Embed modlog = await CommandUtils.CreateModlog(Context.Guild, Context.User, User as SocketUser, CommandUtils.ModlogType.Warn, Reason);
                        await Context.Guild.GetTextChannel(FileSystem.GetGuild(Context.Guild).ModlogChannel.Value).SendMessageAsync("", embed: modlog);
                        Embed modlog2 = await CommandUtils.CreateModlog(Context.Guild, Context.User, User as SocketUser, CommandUtils.ModlogType.Kick, $"{User.ToString()} reached the warn limit and got kicked");
                        await Context.Guild.GetTextChannel(FileSystem.GetGuild(Context.Guild).ModlogChannel.Value).SendMessageAsync("", embed: modlog);
                    }
                    catch (OperationCanceledException e) {
                        return;
                    }
                    catch (Exception e) {
                        await ReplyAsync("Something has gone horribly wrong and the dev has been notified.");
                        await Context.Client.GetUser(config.AuthorId).SendMessageAsync($"ERROR on message from {Context.User.ToString()}:\n{Context.Message.Content}\nThrew error: {e.Message}");
                    }
                }

                return;
            }

            try {
                await ReplyAsync($"{User.ToString()} has been warned.\nTo remove a warn from them, use `//pardon <user>`\n\n**Reason**: {Reason ?? "None"}");
                Embed modlog = await CommandUtils.CreateModlog(Context.Guild, Context.User, User as SocketUser, CommandUtils.ModlogType.Muted, Reason);
                await Context.Guild.GetTextChannel(FileSystem.GetGuild(Context.Guild).ModlogChannel.Value).SendMessageAsync("", embed: modlog);
            }
            catch (OperationCanceledException e) {
                return;
            }
            catch (Exception e) {
                await ReplyAsync("Something has gone horribly wrong and the dev has been notified.");
                await Context.Client.GetUser(config.AuthorId).SendMessageAsync($"ERROR on message from {Context.User.ToString()}:\n{Context.Message.Content}\nThrew error: {e.Message}");
            }
        }

        [Command("pardon")]
        public async Task PardonAsync(SocketGuildUser User, [Remainder] string Reason = null)
        {
            if (!CommandUtils.HasPermission(Context.Guild.GetUser(Context.User.Id), GuildPermission.BanMembers)) {
                await ReplyAsync("You are missing the permissions to warn other users.");
                return;
            }

            var Guild = FileSystem.GetGuild(Context.Guild);
            var user = Guild.GuildUsers.Where(x => x.ID == User.Id).First();

            if (user.WarnCount == 0) {
                await ReplyAsync("User doesn't have any warns.");

                return;
            }

            try {
                await ReplyAsync($"{User.ToString()} has been pardoned and a warn has been removed.\n**Reason**: {Reason ?? "None"}");
                Embed modlog = await CommandUtils.CreateModlog(Context.Guild, Context.User, User as SocketUser, CommandUtils.ModlogType.Muted, Reason);
                await Context.Guild.GetTextChannel(FileSystem.GetGuild(Context.Guild).ModlogChannel.Value).SendMessageAsync("", embed: modlog);
            }
            catch (OperationCanceledException e) {
                return;
            }
            catch (Exception e) {
                await ReplyAsync("Something has gone horribly wrong and the dev has been notified.");
                await Context.Client.GetUser(config.AuthorId).SendMessageAsync($"ERROR on message from {Context.User.ToString()}:\n{Context.Message.Content}\nThrew error: {e.Message}");
            }
        }

        [Command("setmuterole")]
        public async Task SetMuteRole(SocketRole role)
        {
            if (!CommandUtils.HasPermission(Context.Guild.GetUser(Context.User.Id), GuildPermission.ManageRoles)) {
                await ReplyAsync("You are missing the permissions to set the mute role.");
                return;
            }

            if (!CommandUtils.HasPermission(Context.Guild.CurrentUser, GuildPermission.ManageRoles)) {
                await ReplyAsync("I am missing the permissions to set the mute role.");
                return;
            }

            var Guild = FileSystem.GetGuild(Context.Guild);
            Guild.SetMuteRole(role);
            await FileSystem.SetGuild(Guild);

            await ReplyAsync("Mute role has been set");
        }

        [Command("resetmuterole")]
        public async Task ResetMuteRole()
        {
            if (!CommandUtils.HasPermission(Context.Guild.GetUser(Context.User.Id), GuildPermission.ManageRoles)) {
                await ReplyAsync("You are missing the permissions to reset the mute role.");
                return;
            }

            if (!CommandUtils.HasPermission(Context.Guild.CurrentUser, GuildPermission.ManageRoles)) {
                await ReplyAsync("I am missing the permissions to reset the mute role.");
                return;
            }

            var Guild = FileSystem.GetGuild(Context.Guild);
            Guild.ResetMuteRole();
            await FileSystem.SetGuild(Guild);

            await ReplyAsync("Mute role has been reset");
        }

        [Command("setmodlogchannel")]
        public async Task SetModlogChannel(SocketTextChannel channel)
        {
            if (!CommandUtils.HasPermission(Context.Guild.GetUser(Context.User.Id), GuildPermission.ManageChannels)) {
                await ReplyAsync("You are missing the permissions to set the modlog channel.");
                return;
            }

            if (!CommandUtils.HasPermission(Context.Guild.CurrentUser, GuildPermission.ManageChannels)) {
                await ReplyAsync("I am missing the permissions to set the modlog channel.");
                return;
            }

            var Guild = FileSystem.GetGuild(Context.Guild);
            Guild.SetModlogChannel(channel);
            await FileSystem.SetGuild(Guild);

            await ReplyAsync("Modlog channel has been set");
        }

        [Command("resetmodlogchannel")]
        public async Task ResetModlogChannel()
        {
            if (!CommandUtils.HasPermission(Context.Guild.GetUser(Context.User.Id), GuildPermission.ManageRoles)) {
                await ReplyAsync("You are missing the permissions to reset the modlog channel.");
                return;
            }

            if (!CommandUtils.HasPermission(Context.Guild.CurrentUser, GuildPermission.ManageRoles)) {
                await ReplyAsync("I am missing the permissions to reset the modlog channel.");
                return;
            }

            var Guild = FileSystem.GetGuild(Context.Guild);
            Guild.ResetMuteRole();
            await FileSystem.SetGuild(Guild);

            await ReplyAsync("Modlog channel has been reset");
        }

        [Command("disablemodlog")]
        public async Task DisableModlog()
        {
            if (!CommandUtils.HasPermission(Context.Guild.GetUser(Context.User.Id), GuildPermission.ManageRoles)) {
                await ReplyAsync("You are missing the permissions to disable the modlog channel.");
                return;
            }

            if (!CommandUtils.HasPermission(Context.Guild.CurrentUser, GuildPermission.ManageRoles)) {
                await ReplyAsync("I am missing the permissions to disable the modlog channel.");
                return;
            }

            var Guild = FileSystem.GetGuild(Context.Guild);
            Guild.SetModlogAllowed(false);
            await FileSystem.SetGuild(Guild);

            await ReplyAsync("Modlog channel has been disabled");
        }

        [Command("setticketchannel")]
        public async Task SetTicketChannel(SocketTextChannel channel)
        {
            if (!CommandUtils.HasPermission(Context.Guild.GetUser(Context.User.Id), GuildPermission.ManageChannels)) {
                await ReplyAsync("You are missing the permissions to set the ticket channel.");
                return;
            }

            if (!CommandUtils.HasPermission(Context.Guild.CurrentUser, GuildPermission.ManageChannels)) {
                await ReplyAsync("I am missing the permissions to set the ticket channel.");
                return;
            }

            var Guild = FileSystem.GetGuild(Context.Guild);
            Guild.SetModlogChannel(channel);
            await FileSystem.SetGuild(Guild);

            await ReplyAsync("Ticket channel has been set");
        }

        [Command("resetticketchannel")]
        public async Task ResetTicketChannel()
        {
            if (!CommandUtils.HasPermission(Context.Guild.GetUser(Context.User.Id), GuildPermission.ManageRoles)) {
                await ReplyAsync("You are missing the permissions to reset the ticket channel.");
                return;
            }

            if (!CommandUtils.HasPermission(Context.Guild.CurrentUser, GuildPermission.ManageRoles)) {
                await ReplyAsync("I am missing the permissions to reset the ticket channel.");
                return;
            }

            var Guild = FileSystem.GetGuild(Context.Guild);
            Guild.ResetMuteRole();
            await FileSystem.SetGuild(Guild);

            await ReplyAsync("Ticket channel has been reset");
        }

        [Command("disabletickets")]
        public async Task DisableTickets()
        {
            if (!CommandUtils.HasPermission(Context.Guild.GetUser(Context.User.Id), GuildPermission.ManageRoles)) {
                await ReplyAsync("You are missing the permissions to disable the tickets.");
                return;
            }

            if (!CommandUtils.HasPermission(Context.Guild.CurrentUser, GuildPermission.ManageRoles)) {
                await ReplyAsync("I am missing the permissions to disable the tickets.");
                return;
            }

            var Guild = FileSystem.GetGuild(Context.Guild);
            Guild.SetModlogAllowed(false);
            await FileSystem.SetGuild(Guild);

            await ReplyAsync("Tickets have been disabled");
        }

        [Command("openticket")]
        public async Task OpenTicket([Remainder] string TicketText)
        {
            var Guild = FileSystem.GetGuild(Context.Guild);
            var User = FileSystem.GetUser(Context.User);
            if (!Guild.AllowTickets) {
                await ReplyAsync("Tickets aren't enabled on this server.");
                return;
            }

            var GuildUser = Context.Guild.GetUser(Context.User.Id);
            var ticket = new Ticket(GuildUser, TicketText);
            Guild.AddTicket(ticket.ID);
            User.AddTicket(ticket.ID);

            await FileSystem.SetGuild(Guild);
            await FileSystem.SetUser(User);

            Embed em = new EmbedBuilder()
                .WithAuthor($"Ticket #{ticket.ID} | Status: Open")
                .WithColor(Color.Blue)
                .WithTitle($"{Context.User.ToString()} opened a new ticket")
                .WithDescription(TicketText)
                .WithCurrentTimestamp()
                .Build();

            var msg = await Context.Guild.GetTextChannel(Guild.TicketChannel.Value).SendMessageAsync("", embed: em);
            ticket.SetMessageID(msg.Id);
            await FileSystem.SetTicket(ticket);
        }

        [Command("closeticket")]
        public async Task CloseTicket(int TicketID, [Remainder] string Reason = null)
        {
            var Ticket = FileSystem.GetTicket(TicketID);
            if (!(CommandUtils.HasPermission(Context.Guild.GetUser(Context.User.Id), GuildPermission.ManageMessages) &&
                Context.Guild.Id == Ticket.GuildID) ||
                Context.User.Id != Ticket.UserID) {

                await ReplyAsync("You do not have the permission to close this ticket");
                return;
            }
            var user = Context.Guild.GetUser(Context.User.Id);
            Ticket.CloseTicket(user, Reason);

            Embed em = new EmbedBuilder()
                .WithAuthor($"Ticket #{Ticket.ID} | Status: Closed")
                .WithColor(Color.Blue)
                .WithTitle($"{Context.User.ToString()} opened a new ticket")
                .WithDescription(Ticket.Content + $"\n\n{Context.User.ToString()} closed this ticket.\n**Reason:** {Reason ?? "None"}")
                .WithCurrentTimestamp()
                .Build();

            var msg = await Context.Guild.GetTextChannel(FileSystem.GetGuild(Context.Guild).TicketChannel.Value).GetMessageAsync(Ticket.MessageID.Value);
            await (msg as SocketUserMessage).ModifyAsync(x => x.Embed = em);

            await ReplyAsync("Closed ticket");
            await Context.User.SendMessageAsync($"Your ticket #{Ticket.ID} in {Context.Guild.ToString()} has been closed", embed: em);
        }

        [Command("mytickets")]
        public async Task MyTickets()
        {
            var user = FileSystem.GetUser(Context.User);
            var tickets = user.Tickets;
            string ticketList = "";
            foreach (var ticketID in tickets) {
                var Ticket = FileSystem.GetTicket(ticketID);
                ticketList += $"#{Ticket.ID} in {Context.Client.GetGuild(Ticket.GuildID).ToString()} [Status: {(Ticket.IsOpen ? "Open" : "Closed")}]\n";
            }

            await ReplyAsync(ticketList);
        }

        [Command("getticket")]
        public async Task GetTicket(int TicketID)
        {
            var Ticket = FileSystem.GetTicket(TicketID);
            if (Context.User.Id != Ticket.UserID) {
                await ReplyAsync("You did not open this ticket, therefore are not allowed to view its content. If you are a moderator of the server where the ticket was opened, please search for the ticket ID in the ticket channel.");
                return;
            }

            Embed em;

            if (Ticket.IsOpen) {
                em = new EmbedBuilder()
                .WithAuthor($"Ticket #{Ticket.ID} | Status: Open")
                .WithColor(Color.Red)
                .WithTitle($"{Context.User.ToString()} opened a new ticket")
                .WithDescription(Ticket.Content)
                .WithTimestamp(Ticket.CreationDate)
                .Build();
            } else {
                em = new EmbedBuilder()
                .WithAuthor($"Ticket #{Ticket.ID} | Status: Closed")
                .WithColor(Color.Green)
                .WithTitle($"{Context.User.Username}'s ticket")
                .WithDescription(Ticket.Content + $"\n\n{Context.Client.GetUser(Ticket.ClosedBy.Value).ToString()} closed this ticket.\n**Reason:** {Ticket.ClosingReason ?? "None"}")
                .WithTimestamp(Ticket.CloseDate.Value)
                .Build();
            }
        }
    }
}
