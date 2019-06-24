using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Azure
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private Config BotConfig;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            if (File.Exists("../../config.json")) {
                BotConfig = JsonConvert.DeserializeObject<Config>(File.ReadAllText("../../config.json"));
            }
            else {
                Console.WriteLine("Config File not found!\nClosing process.");
                Thread.Sleep(3000);
                Environment.Exit(0);
            }

            _client.Log += Log;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, BotConfig.Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(),
                services: null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            if (!(messageParam is SocketUserMessage message)) return;

            int argpos = 0;

            if (!(message.HasStringPrefix(BotConfig.Prefix, ref argpos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argpos)) ||
                message.Author.IsBot)
                return;

            var ctx = new SocketCommandContext(_client, message);

            var result = await _commands.ExecuteAsync(
                context: ctx,
                argPos: argpos,
                services: null);

            if (!result.IsSuccess)
                Console.WriteLine(result.ErrorReason);
        }
    }
}
