using DcBot.Common.PrefixHandler;
using DcBot.Common.SccTypeHandler;
using DcBot.GeoUGuard.Handler;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace DcBot.GeoUGuard.General
{
    public class BotEvents
    {
        private readonly InitializeBot _initializeBot;
        private readonly BotCommandHandler _botCommandHandler;
        private CommandService _commandService;
        private IServiceProvider _services;
        private readonly IPrefixControl _prefixControl;

        public BotEvents(InitializeBot initializeBot, BotCommandHandler onReadyHandler, CommandService commandService, IServiceProvider services, IPrefixControl prefixControl)
        {
            _initializeBot = initializeBot;
            _botCommandHandler = onReadyHandler;
            _commandService = commandService;
            _services = services;
            _prefixControl = prefixControl;
        }

        private async Task LogAsync(LogMessage log)
        {
            Console.WriteLine($"-{log.Message}");
        }

        public async Task InitializeHandlers()
        {
            await _initializeBot.InitializeClient();
            _initializeBot.Client.Log += LogAsync;
            _initializeBot.Client.MessageReceived += MessageReceivedAsync;
            _initializeBot.Client.Ready += BotOnReadyAsync;

            _commandService.AddTypeReader(typeof(SocketCommandContext), new SccTypeReader());
            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }
        private async Task BotOnReadyAsync()
        {
            foreach (var guild in _initializeBot.Client.Guilds)
            {
                await _botCommandHandler.BotInitialize(guild, _initializeBot.Client);
            }
        }

        private async Task MessageReceivedAsync(SocketMessage socketMessage)
        {
            await _prefixControl.GeoCommandPrefixer(_initializeBot.Client, socketMessage, "GeoUGuard");
        }
    }
}