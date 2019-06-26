using Azure.Models;
using Discord.WebSocket;
using Newtonsoft.Json;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure
{
    public static class FileSystem
    {
        const string BASE_PATH = "files/";
        public const string GUILDS = BASE_PATH + "guilds/";
        public const string USERS = BASE_PATH + "users/";
        public const string TICKETS = BASE_PATH + "tickets/";

        private static bool BaseDirectoriesExist()
        {
            if (!Directory.Exists(BASE_PATH) ||
                !Directory.Exists(GUILDS) ||
                !Directory.Exists(USERS) ||
                !Directory.Exists(TICKETS))
                return false;

            return true;
        }

        private static void CreateBaseDirectories()
        {
            if (BaseDirectoriesExist()) return;

            Directory.CreateDirectory(GUILDS);
            Directory.CreateDirectory(USERS);
            Directory.CreateDirectory(TICKETS);
        }

        private static async Task WriteToFile(string Path, string Text, bool Append = false)
        {
            CreateBaseDirectories();
            var Writer = new StreamWriter(Path, Append);
            await Writer.WriteLineAsync(Text);
            Writer.Close();
        }

        private static T GetObject<T>(string Path)
        {
            CreateBaseDirectories();
            if (!File.Exists(Path))
                throw new FileNotFoundException();
            string Content = File.ReadAllText(Path);
            T Result = JsonConvert.DeserializeObject<T>(Content);
            return Result;
        }

        private static string GetJSON(object Obj)
        {
            string Result = JsonConvert.SerializeObject(Obj);
            return Result;
        }

        public static User GetUser(SocketUser user)
        {
            string Path = USERS + user.Id + ".json";
            User Result;
            try {
                Result = GetObject<User>(Path);
            } catch(FileNotFoundException e) {
                Result = new User(user);
                SetUser(Result);
            }
            
            return Result;
        }

        public static async Task SetUser(User user)
        {
            string JSON = GetJSON(user);
            string Path = USERS + user.ID + ".json";
            await WriteToFile(Path, JSON);
        }

        public static Guild GetGuild(SocketGuild guild)
        {
            string Path = GUILDS + guild.Id + ".json";
            Guild Result;
            try {
                Result = GetObject<Guild>(Path);
            }
            catch (FileNotFoundException e) {
                Result = new Guild(guild);
                SetGuild(Result);
            }

            return Result;
        }

        public static async Task SetGuild(Guild guild)
        {
            string JSON = GetJSON(guild);
            string Path = GUILDS + guild.ID + ".json";
            await WriteToFile(Path, JSON);
        }

        public static Ticket GetTicket(int TicketID)
        {
            string Path = TICKETS + TicketID + ".json";
            Ticket Result;
            Result = GetObject<Ticket>(Path);
            return Result;
        }

        public static async Task SetTicket(Ticket ticket)
        {
            string JSON = GetJSON(ticket);
            string Path = TICKETS + ticket.ID + ".json";
            await WriteToFile(Path, JSON);
        }
    }
}
