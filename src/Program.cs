using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private Config BotConfig;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            if (File.Exists("../../config.json")) {
                BotConfig = JsonConvert.DeserializeObject<Config>(File.ReadAllText("../../config.json"));
            }
            else {
                Console.WriteLine("Config File not found!\nClosing process.");
                Thread.Sleep(3000);
                Environment.Exit(0);
            }

            Console.WriteLine(BotConfig.InviteLink);

            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, BotConfig.Token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
