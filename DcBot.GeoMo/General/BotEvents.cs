using DcBot.Common.PrefixHandler;
using DcBot.Common.SccTypeHandler;
using DcBot.GeoMo.Handler;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace DcBot.GeoMo.General
{
    public class BotEvents
    {
        private readonly InitializeBot _initializeBot;
        private readonly OnReadyHandler _onReadyHandler;
        private CommandService _commandService;
        private IServiceProvider _services;
        private readonly IPrefixControl _prefixControl;

        public BotEvents(InitializeBot initializeBot, OnReadyHandler onReadyHandler, CommandService commandService, IServiceProvider services, IPrefixControl prefixControl)
        {
            _initializeBot = initializeBot;
            _onReadyHandler = onReadyHandler;
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
                await _onReadyHandler.BotInitialize(guild, _initializeBot.Client);
            }
        }

        private async Task MessageReceivedAsync(SocketMessage socketMessage)
        {
            await _prefixControl.GeoMoPrefixer(_initializeBot.Client, socketMessage);
        }
    }
}