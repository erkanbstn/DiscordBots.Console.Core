using Discord.Commands;
using Discord.WebSocket;

namespace DcBot.Common.PermissionHandler
{
    public interface IPermissionControl
    {
        public Task<bool> CheckCommandPermissionAsync(SocketCommandContext context, SocketUserMessage message, string prefix);
    }
}