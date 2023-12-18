using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DcBot.GeoBot.BotCommon
{
    public class InitializeConfig
    {
        public DiscordSocketClient Client { get; private set; }
        private readonly IConfiguration _configuration;
        public InitializeConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task InitializeClient()
        {
            var botToken = _configuration["BotCredentials:DefaultBotToken"];

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
