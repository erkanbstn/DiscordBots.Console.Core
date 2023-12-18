using DcBot.Common.MessageHandler;
using DcBot.Common.PrefixHandler;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DcBot.GeoBot.BotHandler
{
    public class MessageReceiveHandler : ModuleBase<SocketCommandContext>
    {
        private readonly PrefixControl _prefixControl;
        private readonly MessageControl _messageControl;

        public MessageReceiveHandler(PrefixControl prefixControl, MessageControl messageControl)
        {
            _prefixControl = prefixControl;
            _messageControl = messageControl;
        }

        [Command("avatar")]
        public async Task AvatarCommand(SocketCommandContext context)
        {
            if (_prefixControl.PrefixFixer(context, GeoBotCommands.Avatar, out _, out SocketUser user, out _))
            {
                var avatarUrl = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();
                await _messageControl.DeleteAfterSendAsync(await _messageControl.MessageAsync(context, avatarUrl, true, "green"), 10000);
            }
        }

        [Command("dm")]
        public async Task DeleteMessageCommand(SocketCommandContext context)
        {
            if (_prefixControl.PrefixFixer(context, GeoBotCommands.DeleteMessage, out string[] parameters, out _, out _))
            {
                int count = Convert.ToInt32(parameters[0]);
                var messagesToDelete = await context.Channel.GetMessagesAsync(count).FlattenAsync();
                var filteredMessages = messagesToDelete.Take(count);
                await (context.Channel as ITextChannel).DeleteMessagesAsync(filteredMessages);
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(context, Color.Blue, "green", $"{filteredMessages.Count()} Adet Mesajı `{context.Channel.Name}` Kanalından Sildim."));
            }
        }
    }
}