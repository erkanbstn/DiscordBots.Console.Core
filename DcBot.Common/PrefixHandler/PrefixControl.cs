using DcBot.Common.CooldownHandler;
using DcBot.Common.MessageHandler;
using DcBot.Common.PermissionHandler;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace DcBot.Common.PrefixHandler
{
    public class PrefixControl : IPrefixControl
    {
        private readonly IConfiguration _configuration;
        private CommandService _commandService;
        private IServiceProvider _services;
        private readonly IMessageControl _messageControl;
        private readonly IPermissionControl _permissionControl;

        public PrefixControl(IConfiguration configuration, CommandService commandService, IServiceProvider services, IMessageControl messageControl, IPermissionControl permissionControl)
        {
            _configuration = configuration;
            _commandService = commandService;
            _services = services;
            _messageControl = messageControl;
            _permissionControl = permissionControl;
        }

        public List<string> GeoBotPrefixes()
        {
            List<string> prefixes = new List<string>();
            int index = 0;

            while (true)
            {
                string key = $"BotPrefixs:Prefix:{index}";
                string prefix = _configuration[key];

                if (prefix == null)
                {
                    break;
                }

                prefixes.Add(prefix);
                index++;
            }

            return prefixes;
        }
        public async Task GeoBotPrefixer(DiscordSocketClient discordSocketClient, SocketMessage socketMessage)
        {
            var socketUserMessage = socketMessage as SocketUserMessage;

            if (!(socketMessage is SocketUserMessage) || socketUserMessage.Author.IsBot)
                return;

            var socketCommandContext = new SocketCommandContext(discordSocketClient, socketUserMessage);

            var prefixes = GeoBotPrefixes();
            foreach (var prefix in prefixes)
            {
                int argPos = 0;
                if (socketUserMessage.HasStringPrefix(prefix, ref argPos, StringComparison.OrdinalIgnoreCase))
                {
                    if (CoolDownControl.IsOnCooldown(socketCommandContext, out var remainingTime))
                    {
                        await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(socketCommandContext, Color.Magenta, "hourglass", $"Komutu Tekrar Göndermeden Önce {remainingTime.TotalSeconds:F2} Saniye Beklemelisin."));
                        return;
                    }

                    CoolDownControl.UpdateLastCommandTime(socketCommandContext.User.Id);

                    if (await _permissionControl.CheckCommandPermissionAsync(socketCommandContext, socketUserMessage, prefix))
                    {
                        var result = await _commandService.ExecuteAsync(socketCommandContext, argPos, _services);
                        if (!result.IsSuccess)
                            await socketMessage.Channel.SendMessageAsync(result.ErrorReason);
                        break;
                    }
                }
            }
        }
    }
}