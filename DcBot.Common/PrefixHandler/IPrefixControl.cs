using DcBot.Common.CooldownHandler;
using Discord.Commands;
using Discord.WebSocket;

namespace DcBot.Common.PrefixHandler
{
    public interface IPrefixControl
    {
        public List<string> GeoBotPrefixes();
        public Task GeoCommandPrefixer(DiscordSocketClient discordSocketClient, SocketMessage socketMessage, string commandName);
    }
}