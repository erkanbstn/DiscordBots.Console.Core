using DcBot.Common.MessageHandler;
using DcBot.Common.PermissionHandler;
using DcBot.Common.PrefixHandler;
using DcBot.Core.Core;
using DcBot.Service.Interfaces;
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
        private readonly IDcServerService _dcServerService;
        private readonly IRoleService _roleService;

        public MessageReceiveHandler(IMessageControl messageControl, IPrefixControl prefixControl, CommandService commandService, IDcServerService dcServerService, IRoleService roleService)
        {
            _messageControl = messageControl;
            _prefixControl = prefixControl;
            _commandService = commandService;
            _dcServerService = dcServerService;
            _roleService = roleService;
        }

        [Command("grhelp")]
        [Summary("Yardım")]
        [PermissionControlAttribute(GuildPermission.Administrator)]
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

            await _messageControl.EmbedAsync(Context, "white check mark", helpMessage);
        }

        [Command("grsync")]
        [Summary("Rol Senkronize")]
        [PermissionControlAttribute(GuildPermission.Administrator)]
        public async Task SyncCommand()
        {
            var dcServer = await _dcServerService.FirstOrDefaultAsync(x => x.DiscordId == Context.Guild.Id.ToString());

            var rolesData = _roleService.ToListByFilterAsync(x => x.DcServerId == dcServer.Id);

            foreach (var item in await rolesData)
            {
                await _roleService.DeleteAsync(item);
            }

            await Task.Delay(1500);

            var rolesDcData = Context.Guild.Roles.ToList();

            foreach (var role in rolesDcData)
            {
                await _roleService.InsertAsync(new Role
                {
                    DiscordId = role.Id.ToString(),
                    Name = role.Name,
                    DcServerId = dcServer.Id,
                });
            }

            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "scroll", "Roller Senkronize Edildi.!"));
        }
    }
}