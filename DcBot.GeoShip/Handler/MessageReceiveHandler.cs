using DcBot.Common.MessageHandler;
using DcBot.Common.PermissionHandler;
using DcBot.Common.PrefixHandler;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DcBot.GeoShip.Handler
{
    public class MessageReceiveHandler : ModuleBase<SocketCommandContext>
    {
        private readonly IMessageControl _messageControl;
        private readonly IPrefixControl _prefixControl;
        private readonly CommandService _commandService;

        public MessageReceiveHandler(IMessageControl messageControl, IPrefixControl prefixControl, CommandService commandService)
        {
            _messageControl = messageControl;
            _prefixControl = prefixControl;
            _commandService = commandService;
        }

        [Command("ship")]
        [Summary("Shiple")]
        [PermissionControlAttribute(GuildPermission.SendMessages)]
        public async Task ShipCommand(SocketGuildUser shippedUser)
        {
            Random random = new Random();
            var shipCount = random.Next(0, 100) ;
            string shipResult;
            string hearts = "heartpulse";
            string brokenHearts = "broken heart";

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
                hearts = _messageControl.RepeatEmoji("sparkling heart", 10);
                brokenHearts = _messageControl.RepeatEmoji(brokenHearts, 0);
            }

            await _messageControl.EmbedShipAsync(Context, shippedUser, shipCount, shipResult, hearts, brokenHearts);
        }

        [Command("shiphelp")]
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
    }
}