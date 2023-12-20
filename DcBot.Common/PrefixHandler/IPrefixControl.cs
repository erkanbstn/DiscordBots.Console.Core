using Discord.WebSocket;

namespace DcBot.Common.PrefixHandler
{
    public interface IPrefixControl
    {
        public List<string> GeoBotPrefixes();
        public Task GeoBotPrefixer(DiscordSocketClient discordSocketClient, SocketMessage socketMessage);
    }
}