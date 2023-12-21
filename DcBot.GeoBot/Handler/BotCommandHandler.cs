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
        public async Task ServerInitializeAsync(SocketGuild socketGuild, DiscordSocketClient discordSocketClient)
        {
            var server = await _dcServerService.FirstOrDefaultAsync(x => x.DiscordId == socketGuild.Id.ToString());

            if (server == null)
            {
                await _dcServerService.InsertAsync(new DcServer
                {
                    DiscordId = socketGuild.Id.ToString(),
                    Name = socketGuild.Name,
                    OwnerId = socketGuild.OwnerId.ToString(),
                    OwnerName = socketGuild.Owner.GlobalName
                });
            }

            await discordSocketClient.SetStatusAsync(UserStatus.DoNotDisturb);
            await _messageControl.MessageToChannel(socketGuild, "bot", "Geo Bot !", "robot");
        }
        public async Task UserJoinedAsync(SocketGuildUser user)
        {
            var role = user.Guild.GetRole(1185709334789886033);
            await user.AddRoleAsync(role);

            var welcomeChannel = user.Guild.GetTextChannel(1185670107528187937);
            await welcomeChannel.SendMessageAsync($"Hoşgeldin {user.Mention} Sana {role.Mention} Rolünü Atadım.!");
        }
        public async Task UserLeftAsync(SocketGuild socketGuild, SocketUser socketUser)
        {
            var welcomeChannel = socketGuild.GetTextChannel(1185670107528187937);
            await welcomeChannel.SendMessageAsync($"Görüşürüz {socketUser.Mention}.!");
        }
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

                await _messageControl.EmbedAsync(socketCommandContext, Color.DarkGreen, "", $"{user.Mention}, Başarıyla AFK Modundan Çıktınız, `{afkDuration}` Kadar AFK Kaldınız.");
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
                    await _messageControl.EmbedAsync(socketCommandContext, Color.DarkTeal, "", $"{taggedUser.Mention}, `{afkDuration}` Kadar, Şu Sebep ile AFK: `{afk.Reason ?? "Sebep Belirtilmemiş."}`");
                }
            }
        }
        private string AfkDateNow(Afk? userId)
        {
            if (userId?.AfkTime != null)
            {
                DateTime createdAt = DateTime.Parse(userId.AfkTime.ToString());
                TimeSpan difference = DateTime.Now - createdAt;
                var totalMinute = difference.TotalMinutes;
                string formattedValue = totalMinute.ToString("N2");
                var timeMessage = totalMinute > 60
                    ? $"{formattedValue} ({(int)totalMinute / 60} Saat {(int)totalMinute % 60} Dakika)"
                    : $"{formattedValue} Saniye";

                return timeMessage;
            }
            else
            {
                return "AFK süresi bilinmiyor.";
            }
        }
    }
}