using DcBot.Common.MessageHandler;
using DcBot.Common.PermissionHandler;
using DcBot.Common.PrefixHandler;
using DcBot.Common.QuestionHandler;
using DcBot.Core.Core;
using DcBot.Service.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace DcBot.GeoBot.Handler
{
    public class MessageReceiveHandler : ModuleBase<SocketCommandContext>
    {
        private readonly IMessageControl _messageControl;
        private readonly IQuestionControl _questionControl;
        private readonly IAfkService _afkService;
        private readonly IConfiguration _configuration;
        private readonly IPrefixControl _prefixControl;
        private readonly IDcServerService _dcServerService;
        private readonly CommandService _commandService;
        public MessageReceiveHandler(IMessageControl messageControl, IQuestionControl questionControl, IConfiguration configuration, CommandService commandService, IPrefixControl prefixControl, IAfkService afkService, IDcServerService dcServerService)
        {
            _messageControl = messageControl;
            _questionControl = questionControl;
            _configuration = configuration;
            _commandService = commandService;
            _prefixControl = prefixControl;
            _afkService = afkService;
            _dcServerService = dcServerService;
        }

        [Command("avatar")]
        [Summary("Avatar Göster")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task AvatarCommand(SocketGuildUser? socketGuildUser = null)
        {
            socketGuildUser ??= (SocketGuildUser)Context.User;
            var avatarUrl = socketGuildUser.GetAvatarUrl() ?? socketGuildUser.GetDefaultAvatarUrl();
            await _messageControl.DeleteAfterSendAsync(await _messageControl.MessageAsync(Context, avatarUrl, true, "heart"), 10000);
        }

        [Command("dm")]
        [Summary("Mesaj Sil")]
        [PermissionControl(GuildPermission.ManageMessages)]
        public async Task DeleteMessageCommand([Remainder] int count)
        {
            if (count >= 100)
            {
                count = 100;
            }

            var messagesToDelete = await Context.Channel.GetMessagesAsync(count).FlattenAsync();
            var filteredMessages = messagesToDelete.Take(count);
            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "white check mark", $"{filteredMessages.Count()} Adet Mesajı `{Context.Channel.Name}` Kanalından Siliyorum."), 2000);
            await (Context.Channel as ITextChannel).DeleteMessagesAsync(filteredMessages);
        }

        [Command("winfo")]
        [Summary("Hava Durumu Bilgisi")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task WeatherCommand([Remainder] string cityName)
        {
            var apiKey = _configuration["OpenWeatherCredential:OwcToken"];
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetStringAsync($"http://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={apiKey}&units=metric");
                    var weatherData = JObject.Parse(response);
                    var temperature = weatherData["main"]["temp"];
                    var weatherDescription = weatherData["weather"][0]["description"];
                    await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "white check mark", $"`{cityName.ToUpper()}`  `{temperature}°C`"), 10000);
                }
                catch (Exception)
                {
                    await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "white check mark", $"`Böyle Bir Şehir Bulamadım ¿`"), 5000);
                }
            }
        }

        [Command("ulisten")]
        [Summary("Mesaj İlet")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task UserListenCommand(SocketGuildUser socketGuildUser, [Remainder] string message)
        {
            await _messageControl.MessageAsync(Context, $"{message} {socketGuildUser.Mention}", true, "white check mark");
        }

        [Command("listen")]
        [Summary("Mesaj İlet")]
        [PermissionControl(GuildPermission.Administrator)]
        public async Task ListenCommand(SocketGuildUser socketGuildUser, string channelName, [Remainder] string message)
        {
            if (channelName != null)
            {
                await _messageControl.MessageToChannel(socketGuildUser, channelName, $"{message} {socketGuildUser.Mention}", true, "white check mark");
            }
            else
            {
                await _messageControl.MessageAsync(Context, $"{message} {socketGuildUser.Mention}", true, "white check mark");
            }
        }

        [Command("ghelp")]
        [Summary("Yardım")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task HelpCommand()
        {
            await _prefixControl.GetHelpCommands(Context);
        }

        [Command("go")]
        [Summary("Kanala Git")]
        [PermissionControl(GuildPermission.Connect)]
        public async Task GoCommand(SocketGuildUser receiverUser)
        {
            var questionerUser = Context.User as SocketGuildUser;

            if (receiverUser.VoiceChannel == null)
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "question", $"{receiverUser.Mention} Şu Anda Ses Kanalında `Değil!`"), 3000);
                return;
            }

            if (questionerUser.VoiceChannel == null)
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "exclamation", $"{questionerUser.Mention}, Önce Bir Ses Kanalına `Katılmalısınız!`"), 3000);
                return;
            }

            if (await _questionControl.Questioner(Context, receiverUser, await _messageControl.EmbedAsync(Context, "white check mark", $"{receiverUser.Mention}, {questionerUser.Mention} Kanalına Gelmek İstiyor Kabul Ediyor musun?"), $"{questionerUser.Mention} `{receiverUser.VoiceChannel.Name}` Kanalına Taşındı!", $"Taşıma İşlemi `İptal Edildi.`"))
            {
                await questionerUser.ModifyAsync(properties => properties.Channel = receiverUser.VoiceChannel);
            }

        }

        [Command("pull")]
        [Summary("Kanala Çek")]
        [PermissionControl(GuildPermission.MoveMembers)]
        public async Task PullCommand(SocketGuildUser receiverUser)
        {
            var questionerUser = Context.User as SocketGuildUser;

            if (receiverUser.VoiceChannel == null)
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "question", $"{receiverUser.Mention} Şu Anda Ses Kanalında `Değil!`"), 3000);
                return;
            }

            if (questionerUser.VoiceChannel == null)
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "exclamation", $"{questionerUser.Mention}, Önce Bir Ses Kanalına `Katılmalısınız!`"), 3000);
                return;
            }

            if (await _questionControl.Questioner(Context, receiverUser, await _messageControl.EmbedAsync(Context, "white check mark", $"{receiverUser.Mention}, {questionerUser.Mention} Kanalına Seni Çekmek İstiyor Kabul Ediyor musun?"), $"{questionerUser.Mention}, {receiverUser.Mention} Kullanıcısını Senin Bulunduğun `{questionerUser.VoiceChannel.Name}` Kanalına Taşıdım.", $"Taşıma İşlemi `İptal Edildi.`"))
            {
                await receiverUser.ModifyAsync(properties => properties.Channel = questionerUser.VoiceChannel);
            }
        }

        [Command("afk")]
        [Summary("Afk Moduna Geç")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task AfkCommand([Remainder] string? afkReason = null)
        {
            var user = Context.User as SocketGuildUser;

            await _afkService.EnsureAfkExistsAsync(user.Id.ToString(), user.Guild.Id.ToString(), afkReason);

            await user.ModifyAsync(x => x.Nickname = $"[AFK] {user.Nickname}");

            if (string.IsNullOrEmpty(afkReason))
            {
                await _messageControl.EmbedAsync(Context, "ice cube", $"{user.Mention}, Başarıyla AFK Moduna Geçtiniz.");
            }
            else
            {
                await _messageControl.EmbedAsync(Context, "ice cube", $"{user.Mention}, Başarıyla AFK Moduna Geçtiniz. Sebep: `{afkReason}`");
            }
        }

        [Command("w")]
        [Summary("Nerede")]
        [PermissionControl(GuildPermission.SendMessages)]
        public async Task WhereCommand(SocketGuildUser socketGuildUser)
        {
            var voiceChannel = (socketGuildUser as IVoiceState)?.VoiceChannel;

            if (voiceChannel != null)
            {
                await _messageControl.EmbedAsync(Context, "microphone2", $"{socketGuildUser.Mention}, `{voiceChannel.Name}` Odasında Bulunuyor.");
            }
            else
            {
                await _messageControl.EmbedAsync(Context, "microphone2", $"{socketGuildUser.Mention}, Herhangi Bir Odada `Bulunmuyor`.");
            }
        }

        [Command("c")]
        [Summary("Ses Kanallarında Aktif Kullanıcı Sayısı")]
        [PermissionControl(GuildPermission.Administrator)]
        public async Task CountCommand()
        {
            var voiceChannels = await (Context.Guild as IGuild)?.GetVoiceChannelsAsync();

            if (voiceChannels == null || !voiceChannels.Any())
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "busts in silhouette", "Sesli Kanal `Bulunamadı`."), 10000);
                return;
            }

            int totalUserCount = 0;

            foreach (var voiceChannel in voiceChannels)
            {
                var channelUsers = await voiceChannel.GetUsersAsync().FlattenAsync();
                var connectedUserCount = channelUsers.Count(x => x.VoiceChannel != null);
                totalUserCount += connectedUserCount;
            }

            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "busts in silhouette", $"Ses Kanallarında Aktif Kullanıcı Sayısı: `{totalUserCount}`"), 10000);
        }

        [Command("gsync")]
        [Summary("Geo Senkronize")]
        [PermissionControlAttribute(GuildPermission.Administrator)]
        public async Task SyncCommand()
        {
            var server = await _dcServerService.FirstOrDefaultAsync(x => x.DiscordId == Context.Guild.Id.ToString());

            if (server == null)
            {
                await _dcServerService.InsertAsync(new DcServer
                {
                    DiscordId = Context.Guild.Id.ToString(),
                    Name = Context.Guild.Name,
                    OwnerId = Context.Guild.OwnerId.ToString(),
                    OwnerName = Context.Guild.Owner.GlobalName
                });
            }
            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "diamond shape with a dot inside", "Geo Bot Senkronize Edildi.!"));
        }
    }
}