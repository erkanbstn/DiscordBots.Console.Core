using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace DcBot.GeoMo.General
{
    public class InitializeBot
    {
        public DiscordSocketClient Client { get; private set; }
        private readonly IConfiguration _configuration;
        public InitializeBot(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task InitializeClient()
        {
            var botToken = _configuration["BotCredentials:GeoMoToken"];

            if (string.IsNullOrEmpty(botToken))
            {
                Console.WriteLine("Bot Token Bulunamadı.?");
                return;
            }
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All | GatewayIntents.MessageContent
            });
            await Client.LoginAsync(TokenType.Bot, botToken);
            await Client.StartAsync();
        }
    }
}
