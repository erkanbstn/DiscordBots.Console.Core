using DcBot.Common.MessageHandler;
using DcBot.Core.Core;
using DcBot.Service.Interfaces;
using Discord;
using Discord.WebSocket;

namespace DcBot.GeoRGuard.Handler
{
    public class BotCommandHandler
    {
        private readonly IMessageControl _messageControl;

        public BotCommandHandler(IMessageControl messageControl)
        {
            _messageControl = messageControl;
        }

        public async Task BotInitialize(SocketGuild socketGuild, DiscordSocketClient discordSocketClient)
        {
            await discordSocketClient.SetStatusAsync(UserStatus.DoNotDisturb);
            await _messageControl.DeleteAfterSendAsync(await _messageControl.MessageToChannel(socketGuild, "bot", "Geo Role Guard !", "shield"));
        }
    }
}