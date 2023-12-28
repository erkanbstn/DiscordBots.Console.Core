using DcBot.Common.MessageHandler;
using DcBot.Common.PermissionHandler;
using DcBot.Common.PrefixHandler;
using DcBot.Core.Core;
using DcBot.Core.Enums;
using DcBot.Service.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Data;

namespace DcBot.GeoRGuard.Handler
{
    public class MessageReceiveHandler : ModuleBase<SocketCommandContext>
    {
        private readonly IMessageControl _messageControl;
        private readonly IPrefixControl _prefixControl;
        private readonly CommandService _commandService;
        private readonly IDcServerService _dcServerService;
        private readonly IRoleService _roleService;
        private readonly IRoleTypeRelationService _roleTypeRelationService;

        public MessageReceiveHandler(IMessageControl messageControl, IPrefixControl prefixControl, CommandService commandService, IDcServerService dcServerService, IRoleService roleService, IRoleTypeRelationService roleTypeRelationService)
        {
            _messageControl = messageControl;
            _prefixControl = prefixControl;
            _commandService = commandService;
            _dcServerService = dcServerService;
            _roleService = roleService;
            _roleTypeRelationService = roleTypeRelationService;
        }

        [Command("rhelp")]
        [Summary("Yardım")]
        [PermissionControlAttribute(GuildPermission.Administrator)]
        public async Task HelpCommand()
        {
            await _prefixControl.GetHelpCommands(Context);
        }

        [Command("rs")]
        [Summary("Rol Ayar")]
        [PermissionControlAttribute(GuildPermission.Administrator)]
        public async Task RSettingsCommand(string roleId, string roleType, string transactionType = null)
        {
            var dcServer = await _dcServerService.FirstOrDefaultAsync(x => x.DiscordId == Context.Guild.Id.ToString());

            RoleTypes? newRoleType = Enum.GetValues(typeof(RoleTypes))
                                          .Cast<RoleTypes>()
                                          .FirstOrDefault(role => role.ToString().Equals(roleType, StringComparison.OrdinalIgnoreCase));

            if (newRoleType.HasValue && newRoleType != RoleTypes.NoType)
            {
                if (string.IsNullOrEmpty(transactionType))
                {
                    await _roleTypeRelationService.ChangeRoleTypeAsync(newRoleType.Value, roleId, (int)dcServer.Id);
                    await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "notebook", $"`{newRoleType}` Rol Ayarı Yapıldı.!"));

                }
                else if (transactionType.ToLower() == "a")
                {
                    await _roleTypeRelationService.ChangeRoleTypeAsync(newRoleType.Value, roleId, (int)dcServer.Id, transactionType);
                    await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "notebook", $"`{newRoleType}` Rol Ayarı Yapıldı.!"));

                }
                else if (transactionType.ToLower() == "d")
                {
                    await _roleTypeRelationService.ChangeRoleTypeAsync(newRoleType.Value, roleId, (int)dcServer.Id, transactionType);
                    await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "notebook", $"`{roleId}` İçin Rol Ayarları Temizlendi.!"));
                }
            }
            else
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "notebook spiral", "Rol Ayarını Bulamadım.!"));
            }
        }

        [Command("rc")]
        [Summary("Rol Ayar Kontrol")]
        [PermissionControlAttribute(GuildPermission.Administrator)]
        public async Task RSettingsCheckCommand()
        {
            var dcServer = await _dcServerService.FirstOrDefaultAsync(x => x.DiscordId == Context.Guild.Id.ToString());
            var roleRelationsData = await _roleTypeRelationService.ToListFilteringByNoTrackAsync(x => x.DcServerId == dcServer.Id);
            string roleNames = string.Empty;

            var unsyncedRoles = Enum.GetValues(typeof(RoleTypes))
                                    .Cast<RoleTypes>()
                                    .Except(new[] { RoleTypes.NoType })
                                    .Select(roleType => $"`{roleType}`")
                                    .ToList();

            roleNames += string.Join(" - ", unsyncedRoles) + "\n";

            foreach (var role in roleRelationsData)
            {
                var roleData = await _roleService.FirstOrDefaultAsync(x => x.DiscordId == role.DiscordId);
                roleNames += $"`{roleData.Name}` - `{role.DiscordId}` - `{(role.RoleType == RoleTypes.NoType ? "Yok" : "Var")}` \n";
            }

            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "red envelope", $"İşte Senkronize Olmamış Roller : \n {roleNames}"), 60000);
        }

        [Command("rc")]
        [Summary("Rol Ayar Kontrol")]
        [PermissionControlAttribute(GuildPermission.Administrator)]
        public async Task RSettingsCheckCommand(string roleId)
        {
            var dcServer = await _dcServerService.FirstOrDefaultAsync(x => x.DiscordId == Context.Guild.Id.ToString());

            var roleRelationsData = await _roleTypeRelationService.ToListByFilterAsync(x => x.DiscordId == roleId);

            if (roleRelationsData.Count <= 0)
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "100", "Bu Role Ait Senkronizasyon Bulunmuyor."), 30000);
                return;
            }

            string roleNames = string.Empty;

            foreach (var role in roleRelationsData)
            {
                var roleData = await _roleService.FirstOrDefaultAsync(x => x.DiscordId == role.DiscordId);
                roleNames += $"`{roleData.Name}` - `{role.DiscordId}` - `{(role.RoleType == RoleTypes.NoType ? "Yok" : role.RoleType.ToString())}` \n";
            }

            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "red envelope", $"Role Ait Senkronizasyonlar : \n {roleNames}"), 30000);
        }

        [Command("rl")]
        [Summary("Rol Liste")]
        [PermissionControlAttribute(GuildPermission.Administrator)]
        public async Task ListRoleCommand()
        {
            var dcServer = await _dcServerService.FirstOrDefaultAsync(x => x.DiscordId == Context.Guild.Id.ToString());

            var rolesData = await _roleService.ToListFilteringByNoTrackAsync(x => x.DcServerId == dcServer.Id);

            string roleListMessage = "İşte Roller:\n";

            foreach (var role in rolesData)
            {
                roleListMessage += $"`{role.Name}` - `{role.DiscordId}`\n";
            }

            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "notebook with decorative cover", roleListMessage), 15000);
        }

        [Command("rlu")]
        [Summary("Rol Kullanıcı Liste")]
        [PermissionControlAttribute(GuildPermission.Administrator)]
        public async Task ListRoleUserCommand(string roleId)
        {
            if (!ulong.TryParse(roleId, out ulong roleIdAsUlong))
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "warning", "Geçersiz Rol ID'si."));
                return;
            }

            var targetRole = Context.Guild.GetRole(roleIdAsUlong);

            if (targetRole == null)
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "warning", "Belirtilen ID'ye Sahip Rol Bulunamadı."));
                return;
            }

            var usersWithRole = targetRole.Members;

            string userListMessage = $"`{targetRole.Name}` rolüne sahip Kullanıcılar:\n";

            foreach (var user in usersWithRole)
            {
                userListMessage += $"`{user.Username}#{user.Discriminator}` - `{user.Id}`\n";
            }

            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "notebook with decorative cover", userListMessage), 15000);
        }

        [Command("rsync")]
        [Summary("Rol Senkronize")]
        [PermissionControlAttribute(GuildPermission.Administrator)]
        public async Task SyncCommand()
        {
            var dcServer = await _dcServerService.FirstOrDefaultAsync(x => x.DiscordId == Context.Guild.Id.ToString());

            var rolesData = await _roleService.ToListByFilterAsync(x => x.DcServerId == dcServer.Id);

            var rolesRelationsData = await _roleTypeRelationService.ToListByFilterAsync(x => x.DcServerId == dcServer.Id);

            foreach (var item in rolesData)
            {
                await _roleService.DeleteAsync(item);
            }

            foreach (var item in rolesRelationsData)
            {
                await _roleTypeRelationService.DeleteAsync(item);
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


            foreach (var role in rolesDcData)
            {
                await _roleTypeRelationService.InsertAsync(new RoleTypeRelation
                {
                    DiscordId = role.Id.ToString(),
                    DcServerId = dcServer.Id,
                });
            }

            await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(Context, "scroll", "Roller Senkronize Edildi.!"));
        }
    }
}