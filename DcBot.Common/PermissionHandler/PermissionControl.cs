using DcBot.Common.MessageHandler;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DcBot.Common.PermissionHandler
{
    public class PermissionControl : IPermissionControl
    {
        private readonly CommandService _commandService;
        private readonly IMessageControl _messageControl;
        public PermissionControl(CommandService commandService, IMessageControl messageControl)
        {
            _commandService = commandService;
            _messageControl = messageControl;
        }

        public async Task<bool> CheckCommandPermissionAsync(SocketCommandContext context, SocketUserMessage message, string prefix)
        {
            var command = _commandService.Commands.FirstOrDefault(c => c.Aliases.Any(a => message.Content.StartsWith($"{prefix}{a}", StringComparison.OrdinalIgnoreCase)));

            if (command != null)
            {
                var requirePermissionAttribute = command.Attributes.OfType<PermissionControlAttribute>().FirstOrDefault();

                if (requirePermissionAttribute != null)
                {
                    var user = context.User as SocketGuildUser;

                    if (!user.GuildPermissions.Has(requirePermissionAttribute.RequiredPermission))
                    {
                        //if (requirePermissionAttribute.RequiredPermission==GuildPermission.Administrator)
                        //{

                        //}
                        await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(context, "no entry", "Bu Komutu Kullanma Yetkiniz Bulunmamaktadır."));
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}