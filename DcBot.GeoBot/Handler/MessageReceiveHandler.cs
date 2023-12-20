using Azure.Core;
using DcBot.Common.MessageHandler;
using DcBot.Common.PermissionHandler;
using DcBot.Common.PrefixHandler;
using DcBot.Common.QuestionHandler;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;

namespace DcBot.GeoBot.Handler
{
    public class MessageReceiveHandler : ModuleBase<SocketCommandContext>
    {
        private readonly IMessageControl _messageControl;
        private readonly IQuestionControl _questionControl;
        private readonly IConfiguration _configuration;
        private readonly IPrefixControl _prefixControl;
        private readonly CommandService _commandService;
        public MessageReceiveHandler(IMessageControl messageControl, IQuestionControl questionControl, IConfiguration configuration, CommandService commandService, IPrefixControl prefixControl)
        {
            _messageControl = messageControl;
            _questionControl = questionControl;
            _configuration = configuration;
            _commandService = commandService;
            _prefixControl = prefixControl;
        }

        [Command("avatar")]
        [Summary("Avatar Göster")]
        [PermissionControlAttribute(GuildPermission.SendMessages)]
        public async Task AvatarCommand(SocketGuildUser socketGuildUser = null)
        {
            socketGuildUser ??= (SocketGuildUser)Context.User;
            var avatarUrl = socketGuildUser.GetAvatarUrl() ?? socketGuildUser.GetDefaultAvatarUrl();
            await _messageControl.DeleteAfterSendAsync(await _messageControl.MessageAsync(Context, avatarUrl, true, "heart"), 10000);
        }

        [Command("dm")]
        [Summary("Mesaj Sil")]
        [PermissionControlAttribute(GuildPermission.ManageMessages)]
        public async Task DeleteMessageCommand([Remainder] int count)
        {
            if (count >= 100)
            {
                count = 100;
            }

            var messagesToDelete = await Context.Channel.GetMessagesAsync(count).FlattenAsync();
            var filteredMessages = messagesToDelete.Take(count);
            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, Color.Red, "white check mark", $"{filteredMessages.Count()} Adet Mesajı `{Context.Channel.Name}` Kanalından Siliyorum."), 2000);
            await (Context.Channel as ITextChannel).DeleteMessagesAsync(filteredMessages);
        }

        [Command("winfo")]
        [Summary("Hava Durumu Bilgisi")]
        [PermissionControlAttribute(GuildPermission.SendMessages)]
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
                    await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, Color.Orange, "white check mark", $"`{cityName.ToUpper()}`  `{temperature}°C`"), 10000);
                }
                catch (Exception)
                {
                    await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, Color.Orange, "white check mark", $"`Böyle Bir Şehir Bulamadım ¿`"), 5000);
                }

            }
        }

        [Command("ulisten")]
        [Summary("Mesaj İlet")]
        [PermissionControlAttribute(GuildPermission.SendMessages)]
        public async Task UserListenCommand(SocketGuildUser socketGuildUser, [Remainder] string message)
        {
            await _messageControl.MessageAsync(Context, $"{message} {socketGuildUser.Mention}", true, "white check mark");
        }

        [Command("listen")]
        [Summary("Mesaj İlet")]
        [PermissionControlAttribute(GuildPermission.Administrator)]
        public async Task ListenCommand(SocketGuildUser socketGuildUser, string channelName, [Remainder] string message)
        {
            if (channelName != null)
            {
                await _messageControl.MessageAsync(socketGuildUser, channelName, $"{message} {socketGuildUser.Mention}", true, "white check mark");
            }
            else
            {
                await _messageControl.MessageAsync(Context, $"{message} {socketGuildUser.Mention}", true, "white check mark");
            }
        }

        [Command("help")]
        [Summary("Yardım")]
        [PermissionControlAttribute(GuildPermission.SendMessages)]
        public async Task HelpCommand()
        {
            var prefixes = _prefixControl.GeoBotPrefixes();

            string prefixList = string.Join(" | ", prefixes);

            string helpMessage = $"**Prefixler:**\n| {prefixList} |\n\n**Komutlar:**\n";

            var commandGroups = _commandService.Modules
                .Select(module => new
                {
                    ModuleName = module.Name,
                    Commands = module.Commands
                    .Where(command => !command.Attributes.OfType<PermissionControlAttribute>().Any() || command.Attributes.OfType<PermissionControlAttribute>().Any(attr => (Context.User as SocketGuildUser).GuildPermissions.Has(attr.RequiredPermission)))
                    .Select(command => $"`{string.Join("`, `", command.Aliases)}` - {command.Summary ?? "Açıklama Yok"}")
                });

            foreach (var group in commandGroups)
            {
                helpMessage += $"**`{group.ModuleName}`**\n{string.Join("\n", group.Commands)}\n\n";
            }

            await _messageControl.EmbedAsync(Context, Color.Purple, "white check mark", helpMessage);
        }

        [Command("go")]
        [Summary("Kanala Git")]
        [PermissionControlAttribute(GuildPermission.Connect)]
        public async Task GoCommand(SocketGuildUser receiverUser)
        {
            var questionerUser = Context.User as SocketGuildUser;

            if (receiverUser.VoiceChannel == null)
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, Color.DarkRed, "question", $"{receiverUser.Mention} Şu Anda Ses Kanalında `Değil!`"), 3000);
                return;
            }

            if (questionerUser.VoiceChannel == null)
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, Color.Purple, "exclamation", $"{questionerUser.Mention}, Önce Bir Ses Kanalına `Katılmalısınız!`"), 3000);
                return;
            }

            if (await _questionControl.Questioner(Context, receiverUser, await _messageControl.EmbedAsync(Context, Color.LightOrange, "white check mark", $"{receiverUser.Mention}, {questionerUser.Mention} Kanalına Gelmek İstiyor Kabul Ediyor musun?"), $"{questionerUser.Mention} `{receiverUser.VoiceChannel.Name}` Kanalına Taşındı!", $"Taşıma İşlemi `İptal Edildi.`"))
            {
                await questionerUser.ModifyAsync(properties => properties.Channel = receiverUser.VoiceChannel);
            }

        }

        [Command("pull")]
        [Summary("Kanala Çek")]
        [PermissionControlAttribute(GuildPermission.MoveMembers)]
        public async Task PullCommand(SocketGuildUser receiverUser)
        {
            var questionerUser = Context.User as SocketGuildUser;

            if (receiverUser.VoiceChannel == null)
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, Color.DarkRed, "question", $"{receiverUser.Mention} Şu Anda Ses Kanalında `Değil!`"), 3000);
                return;
            }

            if (questionerUser.VoiceChannel == null)
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, Color.Purple, "exclamation", $"{questionerUser.Mention}, Önce Bir Ses Kanalına `Katılmalısınız!`"), 3000);
                return;
            }

            if (await _questionControl.Questioner(Context, receiverUser, await _messageControl.EmbedAsync(Context, Color.LightOrange, "white check mark", $"{receiverUser.Mention}, {questionerUser.Mention} Kanalına Seni Çekmek İstiyor Kabul Ediyor musun?"), $"{questionerUser.Mention}, {receiverUser.Mention} Kullanıcısını Senin Bulunduğun `{questionerUser.VoiceChannel.Name}` Kanalına Taşıdım.", $"Taşıma İşlemi `İptal Edildi.`"))
            {
                await receiverUser.ModifyAsync(properties => properties.Channel = questionerUser.VoiceChannel);
            }
        }

        [Command("ship")]
        [Summary("Shiple")]
        [PermissionControlAttribute(GuildPermission.SendMessages)]
        public async Task ShipCommand(SocketGuildUser shippedUser)
        {
            var shipperUser = Context.User as SocketGuildUser;
            Random random = new Random();
            var shipCount = random.Next(0, 100);

            string shipResult;
            string hearts = "💗";
            string brokenHearts = "💔";

            if (shipCount >= 0 && shipCount <= 10)
            {
                shipResult = "Belki de Denememelisiniz..";
                hearts = _messageControl.RepeatEmoji(hearts, 1);
                brokenHearts = _messageControl.RepeatEmoji(brokenHearts, 9);
            }
            else if (shipCount > 10 && shipCount <= 20)
            {
                shipResult = "Aranızdaki Aşk Biraz Zayıf Gibi Görünüyor..";
                hearts = _messageControl.RepeatEmoji(hearts, 2);
                brokenHearts = _messageControl.RepeatEmoji(brokenHearts, 8);
            }
            else if (shipCount > 20 && shipCount <= 30)
            {
                shipResult = "Pek Parlak Değilsiniz..";
                hearts = _messageControl.RepeatEmoji(hearts, 3);
                brokenHearts = _messageControl.RepeatEmoji(brokenHearts, 7);
            }
            else if (shipCount > 30 && shipCount <= 40)
            {
                shipResult = "Denemekten Zarar Gelir mi.?";
                hearts = _messageControl.RepeatEmoji(hearts, 4);
                brokenHearts = _messageControl.RepeatEmoji(brokenHearts, 6);
            }
            else if (shipCount > 40 && shipCount <= 50)
            {
                shipResult = "Belki Bir Şansınız Var..";
                hearts = _messageControl.RepeatEmoji(hearts, 5);
                brokenHearts = _messageControl.RepeatEmoji(brokenHearts, 5);
            }
            else if (shipCount > 50 && shipCount <= 60)
            {
                shipResult = "Birbirinize Olan Sevginiz Büyük.!";
                hearts = _messageControl.RepeatEmoji(hearts, 6);
                brokenHearts = _messageControl.RepeatEmoji(brokenHearts, 4);
            }
            else if (shipCount > 60 && shipCount <= 70)
            {
                shipResult = "Birbirinize Olan Sevginiz Devasa.!";
                hearts = _messageControl.RepeatEmoji(hearts, 7);
                brokenHearts = _messageControl.RepeatEmoji(brokenHearts, 3);
            }
            else if (shipCount > 70 && shipCount <= 80)
            {
                shipResult = "Aranızdaki Aşk Muazzam, Birbirinizi Çok Seviyorsunuz.!";
                hearts = _messageControl.RepeatEmoji(hearts, 8);
                brokenHearts = _messageControl.RepeatEmoji(brokenHearts, 2);
            }
            else
            {
                shipResult = "Hanginiz Nikahı Basıyor??";
                hearts = _messageControl.RepeatEmoji("💖", 10);
            }

            var embed = new EmbedBuilder()
                .WithColor(Color.Magenta)
                .WithTitle("Aşk Yüzdesi")
                .WithDescription($"{shipperUser.Mention} ve {shippedUser.Mention} Aşk Yüzdeniz: **{shipCount}%**\n\n`{shipResult}`\n\n{hearts} {brokenHearts}")
                .WithFooter(new EmbedFooterBuilder
                {
                    IconUrl = shipperUser.GetAvatarUrl(),
                    Text = shipperUser.Username
                })
                .WithThumbnailUrl(shippedUser.GetAvatarUrl())
                .Build();

            await Context.Channel.SendMessageAsync(embed: embed);
        }
    }
}