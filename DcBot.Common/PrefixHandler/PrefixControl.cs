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
        public async Task GeoCommandPrefixer(DiscordSocketClient discordSocketClient, SocketMessage socketMessage, string commandName)
        {
            if (IsOnBot(socketMessage, discordSocketClient, out SocketCommandContext socketCommandContext, out SocketUserMessage socketUserMessage, out List<string> prefixes))
            {
                foreach (var prefix in prefixes)
                {
                    int argPos = 0;
                    if (socketUserMessage.HasStringPrefix(prefix, ref argPos, StringComparison.OrdinalIgnoreCase))
                    {
                        if (CoolDownControl.CoolDown(socketCommandContext, commandName, out var remainingTime))
                        {
                            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(socketCommandContext, Color.Magenta, "hourglass", $"Komutu Tekrar Göndermeden Önce {remainingTime.TotalSeconds:F2} Saniye Beklemelisin."));
                            return;
                        }

                        CoolDownControl.UpdateTime(commandName, socketCommandContext.User.Id);

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
        public bool IsOnBot(SocketMessage socketMessage, DiscordSocketClient discordSocketClient, out SocketCommandContext socketCommandContext, out SocketUserMessage socketUserMessage, out List<string> prefixes)
        {
            socketUserMessage = socketMessage as SocketUserMessage;
            socketCommandContext = new SocketCommandContext(discordSocketClient, socketUserMessage);
            prefixes = GeoBotPrefixes();

            if (!(socketMessage is SocketUserMessage) || socketUserMessage.Author.IsBot)
                return false;

            foreach (var prefix in prefixes)
            {
                var commandName = socketMessage.Content.Split(' ')[0].Replace(prefix, "");
                foreach (var module in _commandService.Modules)
                {
                    foreach (var command in module.Commands)
                    {
                        if (command.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}