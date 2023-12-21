using DcBot.Common.MessageHandler;
using DcBot.Common.PermissionHandler;
using DcBot.Common.PrefixHandler;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DcBot.GeoRGuard.Handler
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

        [Command("grhelp")]
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