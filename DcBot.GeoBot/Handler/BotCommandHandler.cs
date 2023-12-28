using DcBot.Common.MessageHandler;
using DcBot.Core.Core;
using DcBot.Service.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DcBot.GeoBot.Handler
{
    public class BotCommandHandler
    {
        private readonly IDcServerService _dcServerService;
        private readonly IMessageControl _messageControl;
        private readonly IAfkService _afkService;
        public BotCommandHandler(IDcServerService dcServerService, IMessageControl messageControl, IAfkService afkService)
        {
            _dcServerService = dcServerService;
            _messageControl = messageControl;
            _afkService = afkService;
        }

        // Initialize
        public async Task ServerInitializeAsync(SocketGuild socketGuild, DiscordSocketClient discordSocketClient)
        {
            await discordSocketClient.SetStatusAsync(UserStatus.DoNotDisturb);
            await _messageControl.DeleteAfterSendAsync(await _messageControl.MessageToChannel(socketGuild, "bot", "Geo Bot !", "robot"));
        }

        // AFK
        public async Task ExitAfkCommand(SocketMessage socketMessage, DiscordSocketClient discordSocketClient)
        {
            var user = (socketMessage.Author as SocketGuildUser);
            var userMessage = (socketMessage as SocketUserMessage);
            var socketCommandContext = new SocketCommandContext(discordSocketClient, userMessage);

            var afk = await _afkService.FirstOrDefaultAsync(x => x.DiscordId == user.Id.ToString() && x.AfkStatus == true);

            if (afk != null)
            {
                afk.Reason = "Afk Değil";
                afk.AfkStatus = false;

                var afkDuration = AfkDateNow(afk);

                await _afkService.UpdateAsync(afk);

                await user.ModifyAsync(x => x.Nickname = user.Username);

                await _messageControl.EmbedAsync(socketCommandContext, "watermelon", $"{user.Mention}, Başarıyla AFK Modundan Çıktınız, `{afkDuration}` Kadar AFK Kaldınız.");
            }
        }
        public async Task AfkTaggedCommand(SocketMessage socketMessage, DiscordSocketClient discordSocketClient)
        {
            var taggedUser = socketMessage.MentionedUsers.FirstOrDefault();

            if (taggedUser != null)
            {
                var userMessage = (socketMessage as SocketUserMessage);
                var socketCommandContext = new SocketCommandContext(discordSocketClient, userMessage);

                var afk = await _afkService.FirstOrDefaultAsync(x => x.DiscordId == taggedUser.Id.ToString() && x.AfkStatus == true);

                if (afk != null)
                {
                    var afkDuration = AfkDateNow(afk);
                    await _messageControl.EmbedAsync(socketCommandContext, "watermelon", $"{taggedUser.Mention}, `{afkDuration}` Kadar, Şu Sebep ile AFK: `{afk.Reason ?? "Sebep Belirtilmemiş."}`");
                }
            }
        }
        private string AfkDateNow(Afk? userId)
        {
            if (userId?.AfkTime != null)
            {
                DateTime createdAt = DateTime.Parse(userId.AfkTime.ToString());
                TimeSpan difference = DateTime.Now - createdAt;

                if (difference.TotalMinutes >= 60)
                {
                    int hours = (int)difference.TotalHours;
                    int minutes = difference.Minutes;

                    return $"{hours} Saat {minutes} Dakika";
                }
                else if (difference.TotalSeconds >= 60)
                {
                    int minutes = (int)difference.TotalMinutes;
                    double seconds = difference.TotalSeconds - (minutes * 60);

                    return $"{minutes} Dakika {seconds:N2} Saniye";
                }
                else
                {
                    return $"{difference.TotalSeconds:N2} Saniye";
                }
            }
            else
            {
                return "AFK süresi bilinmiyor.";
            }
        }
    }
}