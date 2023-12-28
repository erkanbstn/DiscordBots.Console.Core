using DcBot.Common.MessageHandler;
using DcBot.Common.PermissionHandler;
using DcBot.Common.PrefixHandler;
using DcBot.Core.Core;
using DcBot.Service.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DcBot.GeoCGuard.Handler
{
    public class MessageReceiveHandler : ModuleBase<SocketCommandContext>
    {
        private readonly IMessageControl _messageControl;
        private readonly IPrefixControl _prefixControl;
        private readonly CommandService _commandService;
        private readonly IChannelService _channelService;
        private readonly IDcServerService _dcServerService;

        public MessageReceiveHandler(IMessageControl messageControl, IPrefixControl prefixControl, CommandService commandService, IChannelService channelService, IDcServerService dcServerService)
        {
            _messageControl = messageControl;
            _prefixControl = prefixControl;
            _commandService = commandService;
            _channelService = channelService;
            _dcServerService = dcServerService;
        }

        [Command("chelp")]
        [Summary("Yardım")]
        [PermissionControlAttribute(GuildPermission.SendMessages)]
        public async Task HelpCommand()
        {
            await _prefixControl.GetHelpCommands(Context);
        }

        [Command("csync")]
        [Summary("Kanal Senkronize")]
        [PermissionControlAttribute(GuildPermission.Administrator)]
        public async Task SyncCommand()
        {
            var dcServer = await _dcServerService.FirstOrDefaultAsync(x => x.DiscordId == Context.Guild.Id.ToString());

            var channelsData = _channelService.ToListByFilterAsync(x => x.DcServerId == dcServer.Id);

            foreach (var item in await channelsData)
            {
                await _channelService.DeleteAsync(item);
            }

            await Task.Delay(1500);

            var channelsDcData = Context.Guild.Channels.ToList();

            foreach (var channel in channelsDcData)
            {
                await _channelService.InsertAsync(new Channel
                {
                    DiscordId = channel.Id.ToString(),
                    Name = channel.Name,
                    ChannelType = (channel is ITextChannel) ? "Text" : "Voice",
                    DcServerId = dcServer.Id
                });
            }

            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "thought balloon", "Kanallar Senkronize Edildi.!"));
        }
    }
}