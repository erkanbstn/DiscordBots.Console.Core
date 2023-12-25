using DcBot.Common.MessageHandler;
using DcBot.Common.PermissionHandler;
using DcBot.Common.PrefixHandler;
using DcBot.Core.Core;
using DcBot.Service.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DcBot.GeoUGuard.Handler
{
    public class MessageReceiveHandler : ModuleBase<SocketCommandContext>
    {
        private readonly IMessageControl _messageControl;
        private readonly IPrefixControl _prefixControl;
        private readonly CommandService _commandService;
        private readonly IDcServerService _dcServerService;
        private readonly IUserService _userService;

        public MessageReceiveHandler(IMessageControl messageControl, IPrefixControl prefixControl, CommandService commandService, IDcServerService dcServerService, IUserService userService)
        {
            _messageControl = messageControl;
            _prefixControl = prefixControl;
            _commandService = commandService;
            _dcServerService = dcServerService;
            _userService = userService;
        }

        [Command("guhelp")]
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

        [Command("gusync")]
        [Summary("Kullanıcı Senkronize")]
        [PermissionControlAttribute(GuildPermission.Administrator)]
        public async Task SyncCommand()
        {
            var dcServer = await _dcServerService.FirstOrDefaultAsync(x => x.DiscordId == Context.Guild.Id.ToString());

            var usersData = await _userService.ToListByFilterAsync(x => x.DcServerId == dcServer.Id);

            foreach (var user in usersData)
            {
                await _userService.DeleteAsync(user);
            }

            await Task.Delay(1500);

            var usersDcData = Context.Guild.Users.ToList();

            foreach (var user in usersDcData)
            {
                var userData = new User();
                userData.DiscordId = user.Id.ToString();
                userData.UserName = !string.IsNullOrEmpty(user.Username) ? user.Username : user.GlobalName;
                userData.DcServerId = dcServer.Id;
                userData.Money = 100;
                await _userService.InsertAsync(userData);
            }

            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "globe with meridians", "Kullanıcılar Senkronize Edildi.!"));
        }
    }
}