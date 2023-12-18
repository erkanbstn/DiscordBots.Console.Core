using DcBot.GeoBot.BotConfig;
using DcBot.GeoBot.BotHandler;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Reflection;
namespace DcBot.GeoBot.BotCommon
{
    public class BotEventHandler
    {
        private readonly InitializeConfig _initializeBot;
        private readonly OnReadyHandler _onReadyHandler;
        private CommandService _commandService;
        private IConfiguration _configuration;
        private IServiceProvider _services;
        public BotEventHandler(InitializeConfig initializeBot, OnReadyHandler onReadyHandler, CommandService commandService, IConfiguration configuration, IServiceProvider services)
        {
            _initializeBot = initializeBot;
            _onReadyHandler = onReadyHandler;
            _commandService = commandService;
            _configuration = configuration;
            _services = services;
        }

        private async Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.Message);
        }

        public async Task InitializeHandlers()
        {
            await _initializeBot.InitializeClient();
            _initializeBot.Client.Log += LogAsync;
            _initializeBot.Client.MessageReceived += MessageReceivedAsync;
            _initializeBot.Client.Ready += BotOnReadyAsync;
            _initializeBot.Client.UserJoined += UserJoinedAsync;
            _initializeBot.Client.UserLeft += UserLeftAsync;
            _commandService.AddTypeReader(typeof(SocketCommandContext), new SocketCommandContextTypeReader());
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task UserJoinedAsync(SocketGuildUser user)
        {
            await _onReadyHandler.UserJoinedAsync(user);
        }
        private async Task UserLeftAsync(SocketGuild socketGuild, SocketUser socketUser)
        {
            await _onReadyHandler.UserLeftAsync(socketGuild, socketUser);
        }

        private async Task BotOnReadyAsync()
        {
            foreach (var guild in _initializeBot.Client.Guilds)
            {
                await _initializeBot.Client.SetStatusAsync(UserStatus.DoNotDisturb);
                await _onReadyHandler.ServerInitializeAsync(guild);
            }
        }

        private async Task MessageReceivedAsync(SocketMessage socketMessage)
        {
            var message = socketMessage as SocketUserMessage;
            var context = new SocketCommandContext(_initializeBot.Client, message);

            if (message.Author.IsBot) return;

            int index = 0;
            while (true)
            {
                string key = $"BotPrefixs:Prefix:{index}";
                string prefix = _configuration[key];
                if (prefix == null)
                {
                    break;
                }
                index++;

                int argPos = 0;
                if (message.HasStringPrefix(prefix, ref argPos, StringComparison.OrdinalIgnoreCase))
                {
                    var result = await _commandService.ExecuteAsync(context, argPos, _services);
                    if (!result.IsSuccess)
                        await socketMessage.Channel.SendMessageAsync(result.ErrorReason);
                    break;
                }
            }
        }
    }
}