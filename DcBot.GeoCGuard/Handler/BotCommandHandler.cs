using DcBot.Common.MessageHandler;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace DcBot.GeoCGuard.Handler
{
    public class BotCommandHandler
    {
        private readonly IMessageControl _messageControl;
        private readonly IConfiguration _configuration;
        public BotCommandHandler(IMessageControl messageControl, IConfiguration configuration)
        {
            _messageControl = messageControl;
            _configuration = configuration;
        }

        public async Task BotInitialize(SocketGuild socketGuild, DiscordSocketClient discordSocketClient)
        {
            await discordSocketClient.SetStatusAsync(UserStatus.DoNotDisturb);
            await _messageControl.DeleteAfterSendAsync(await _messageControl.MessageToChannel(socketGuild, "bot", "Geo Channel Guard !", "shield"));
            
        }

        public async Task MessageCheck(SocketMessage socketMessage, DiscordSocketClient discordSocketClient)
        {
            var message = socketMessage as SocketUserMessage;
            var context = new SocketCommandContext(discordSocketClient, message);

            List<string> badWords = new List<string>();
            List<string> linkWords = new List<string>();
            int index = 0;

            while (true)
            {
                string badWordsKey = $"BadWords:Word:{index}";
                string linkWordsKey = $"LinkWords:Word:{index}";

                string badWord = _configuration[badWordsKey];
                string linkWord = _configuration[linkWordsKey];

                if (badWord == null && linkWord == null)
                    break;

                if (badWord != null)
                    badWords.Add(badWord);

                if (linkWord != null)
                    linkWords.Add(linkWord);

                index++;
            }

            if (linkWords.Any(word => socketMessage.Content.ToLower().Contains(word)))
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(context, "zipper mouth", $"{message.Author.Mention} Link Paylaşmak Yasaktır!"));
                await socketMessage.DeleteAsync();
            }

            if (badWords.Any(word => socketMessage.Content.ToLower().Contains(word)))
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(context, "zipper mouth", $"{message.Author.Mention} Küfürlü İçerik Paylaşmak Yasaktır!"));
                await socketMessage.DeleteAsync();
            }
        }
    }
}