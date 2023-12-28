using DcBot.Common.PrefixHandler;
using DcBot.Common.RandomHandler;
using DcBot.Common.SccTypeHandler;
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
        private readonly BotCommandHandler _botCommandHandler;
        private CommandService _commandService;
        private IServiceProvider _services;
        private readonly IPrefixControl _prefixControl;
        private readonly IRandomControl _randomControl;

        public BotEvents(InitializeBot initializeBot, BotCommandHandler botCommandHandler, CommandService commandService, IServiceProvider services, IPrefixControl prefixControl, IRandomControl randomControl)
        {
            _initializeBot = initializeBot;
            _botCommandHandler = botCommandHandler;
            _commandService = commandService;
            _services = services;
            _prefixControl = prefixControl;
            _randomControl = randomControl;
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
                await _botCommandHandler.ServerInitializeAsync(guild, _initializeBot.Client);
            }
        }

        private async Task MessageReceivedAsync(SocketMessage socketMessage)
        {
            if (socketMessage is SocketUserMessage userMessage && !userMessage.Author.IsBot)
            {
                await _botCommandHandler.AfkTaggedCommand(socketMessage, _initializeBot.Client);
                await _botCommandHandler.ExitAfkCommand(socketMessage, _initializeBot.Client);
                await _prefixControl.GeoCommandPrefixer(_initializeBot.Client, socketMessage, "GeoBot");
                await _randomControl.RandomMessage((socketMessage.Channel as SocketGuildChannel)?.Guild, socketMessage.Content);
            }
        }
    }
}