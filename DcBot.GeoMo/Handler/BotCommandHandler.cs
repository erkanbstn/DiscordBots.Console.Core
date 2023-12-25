using DcBot.Common.MessageHandler;
using DcBot.Service.Interfaces;
using Discord;
using Discord.WebSocket;

namespace DcBot.GeoMo.Handler
{
    public class BotCommandHandler
    {
        private readonly IDcServerService _dcServerService;
        private readonly IMessageControl _messageControl;
        public BotCommandHandler(IDcServerService dcServerService, IMessageControl messageControl)
        {
            _dcServerService = dcServerService;
            _messageControl = messageControl;
        }
        public async Task BotInitialize(SocketGuild socketGuild, DiscordSocketClient discordSocketClient)
        {
            await discordSocketClient.SetStatusAsync(UserStatus.DoNotDisturb);
            await _messageControl.DeleteAfterSendAsync(await _messageControl.MessageToChannel(socketGuild, "bot", "Geo Money !", "dollar"));
        }
    }
}