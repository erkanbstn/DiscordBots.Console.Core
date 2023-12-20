using DcBot.Common.PrefixHandler;
using DcBot.GeoBot.Handler;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace DcBot.GeoBot.General
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
            _initializeBot.Client.UserJoined += UserJoinedAsync;
            _initializeBot.Client.UserLeft += UserLeftAsync;
            _commandService.AddTypeReader(typeof(SocketCommandContext), new SccTypeReader());
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
            await _prefixControl.GeoBotPrefixer(_initializeBot.Client, socketMessage);
        }
    }
}