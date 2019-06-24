using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure
{
    public class Config
    {
        [JsonProperty("APP_NAME")]
        public readonly string AppName;
        [JsonProperty("APP_ID")]
        public readonly ulong AppId;
        [JsonProperty("AUTHOR")]
        public readonly string Author;
        [JsonProperty("AUTHOR_ID")]
        public readonly ulong AuthorId;
        [JsonProperty("INVITE_BASE")]
        private readonly string InviteBase;
        [JsonProperty("PERMISSIONS")]
        private readonly int Permissions;
        [JsonProperty("SOURCE_CODE")]
        public readonly string SourceLink;
        [JsonProperty("TOKEN")]
        public readonly string Token;
        [JsonProperty("DEFAULT_PREFIX")]
        public readonly string Prefix;

        public string InviteLink { get { return InviteBase + AppId + "&permissions=" + Permissions + "&scope=bot"; } }
    }
}
