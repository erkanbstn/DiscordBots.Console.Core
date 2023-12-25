using DcBot.Common.MessageHandler;
using DcBot.Core.Core;
using DcBot.Service.Interfaces;
using DcBot.Service.Services;
using Discord;
using Discord.WebSocket;

namespace DcBot.GeoUGuard.Handler
{
    public class BotCommandHandler
    {
        private readonly IDcServerService _dcServerService;
        private readonly IMessageControl _messageControl;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        public BotCommandHandler(IDcServerService dcServerService, IMessageControl messageControl, IUserService userService, IRoleService roleService)
        {
            _dcServerService = dcServerService;
            _messageControl = messageControl;
            _userService = userService;
            _roleService = roleService;
        }
        public async Task BotInitialize(SocketGuild socketGuild, DiscordSocketClient discordSocketClient)
        {
            await discordSocketClient.SetStatusAsync(UserStatus.DoNotDisturb);
            await _messageControl.DeleteAfterSendAsync(await _messageControl.MessageToChannel(socketGuild, "bot", "Geo User Guard !", "shield"));
        }
        public async Task AntiRaid(SocketGuildUser user)
        {
            var recentJoins = await user.Guild.GetUsersAsync().Where(u => (DateTime.Now - (u as IGuildUser).JoinedAt?.DateTime) < TimeSpan.FromMinutes(1)).ToListAsync();

            var waitingForJoinRoleData = await _roleService.FirstOrDefaultAsync(x => x.RoleType == Core.Enums.RoleTypes.WaitingForJoinUserRole);
            if (waitingForJoinRoleData != null)
            {
                var waitingForJoinRole = user.Guild.Roles.FirstOrDefault(x => x.Id.ToString() == waitingForJoinRoleData.DiscordId);
                var finalRoleData = await _roleService.FirstOrDefaultAsync(x => x.RoleType == Core.Enums.RoleTypes.NewUserRole);
                if (finalRoleData != null)
                {
                    var finalRole = user.Guild.Roles.FirstOrDefault(x => x.Id.ToString() == finalRoleData.DiscordId);

                    if (recentJoins.Count >= 5)
                    {
                        await user.AddRoleAsync(waitingForJoinRole);

                        await Task.Delay(TimeSpan.FromSeconds(10))
                                  .ContinueWith(async _ =>
                                  {
                                      await user.AddRoleAsync(finalRole);
                                  });
                    }
                }
            }
            else
            {
                //await _messageControl.MessageToChannel(socketGuild, "bot", "Geo User Guard !", "shield");
            }
        }
    }
}