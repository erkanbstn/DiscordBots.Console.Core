using DcBot.Common.MessageHandler;
using DcBot.Core.Enums;
using DcBot.Service.Interfaces;
using Discord;
using Discord.WebSocket;

namespace DcBot.GeoUGuard.Handler
{
    public class BotCommandHandler
    {
        private readonly IMessageControl _messageControl;
        private readonly IRoleTypeRelationService _roleTypeRelationService;

        public BotCommandHandler(IMessageControl messageControl, IRoleTypeRelationService roleTypeRelationService)
        {
            _messageControl = messageControl;
            _roleTypeRelationService = roleTypeRelationService;
        }

        public async Task BotInitialize(SocketGuild socketGuild, DiscordSocketClient discordSocketClient)
        {
            await discordSocketClient.SetStatusAsync(UserStatus.DoNotDisturb);
            await _messageControl.DeleteAfterSendAsync(await _messageControl.MessageToChannel(socketGuild, "bot", "Geo User Guard !", "shield"));
        }
        public async Task AntiRaid(SocketGuildUser user)
        {
            var waitingForJoinRoleData = await _roleTypeRelationService.FirstOrDefaultAsync(x => x.RoleType == RoleTypes.Wait);
            if (waitingForJoinRoleData != null)
            {
                var finalRoleData = await _roleTypeRelationService.FirstOrDefaultAsync(x => x.RoleType == RoleTypes.New);
                if (finalRoleData != null)
                {
                    var finalRoleId = Convert.ToUInt64(finalRoleData.DiscordId);
                    var waitingForJoinRoleId = Convert.ToUInt64(waitingForJoinRoleData.DiscordId);

                    await user.AddRoleAsync(waitingForJoinRoleId);

                    await Task.Delay(TimeSpan.FromSeconds(15))
                              .ContinueWith(async _ =>
                              {
                                  await user.AddRoleAsync(finalRoleId);
                                  await user.RemoveRoleAsync(waitingForJoinRoleId);
                              });
                }
            }
        }
    }
}