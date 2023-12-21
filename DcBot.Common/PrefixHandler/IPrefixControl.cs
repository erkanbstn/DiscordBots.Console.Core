using DcBot.Common.CooldownHandler;
using Discord.Commands;
using Discord.WebSocket;

namespace DcBot.Common.PrefixHandler
{
    public interface IPrefixControl
    {
        public List<string> GeoBotPrefixes();
        public Task GeoBotPrefixer(DiscordSocketClient discordSocketClient, SocketMessage socketMessage);
        public Task GeoShipPrefixer(DiscordSocketClient discordSocketClient, SocketMessage socketMessage);
        public  Task GeoUGuardPrefixer(DiscordSocketClient discordSocketClient, SocketMessage socketMessage);
        public  Task GeoCGuardPrefixer(DiscordSocketClient discordSocketClient, SocketMessage socketMessage);
        public  Task GeoRGuardPrefixer(DiscordSocketClient discordSocketClient, SocketMessage socketMessage);
        public  Task GeoMoPrefixer(DiscordSocketClient discordSocketClient, SocketMessage socketMessage);
    }
}