using Discord.Commands;
using Discord.WebSocket;

namespace DcBot.Common.RandomHandler
{
    public interface IRandomControl
    {
        public Task RandomMessage(SocketGuild socketGuild, string content);
    }
}