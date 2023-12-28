using DcBot.Common.MessageHandler;
using DcBot.Core.Core;
using DcBot.Core.Enums;
using DcBot.Service.Interfaces;
using Discord.WebSocket;

namespace DcBot.Common.RandomHandler
{
    public class RandomControl : IRandomControl
    {
        private readonly IMessageControl _messageControl;
        private readonly Random _random = new Random();
        private readonly IRoleTypeRelationService _roleTypeRelationService;
        private readonly IDcServerService _dcServerService;

        public RandomControl(IMessageControl messageControl, IRoleTypeRelationService roleTypeRelationService, IDcServerService dcServerService)
        {
            _messageControl = messageControl;
            _roleTypeRelationService = roleTypeRelationService;
            _dcServerService = dcServerService;
        }

        public async Task RandomMessage(SocketGuild socketGuild, string content)
        {
            var dcServer = await _dcServerService.FirstOrDefaultAsync(x => x.DiscordId == socketGuild.Id.ToString());
            var roleUsers = await _roleTypeRelationService.GetRoleTypeAsync(RoleTypes.Cat, dcServer.Id);

            var targetUser = socketGuild.Users.FirstOrDefault(user =>
                !user.IsBot &&
                (user.Nickname?.Equals(content, StringComparison.OrdinalIgnoreCase) == true ||
                 user.Username.Equals(content, StringComparison.OrdinalIgnoreCase) ||
                 user.DisplayName.Equals(content, StringComparison.OrdinalIgnoreCase))
            );

            if (targetUser != null && roleUsers != null)
            {
                var hasRole = targetUser.Roles.Any(role => role.Id.ToString() == roleUsers.DiscordId);

                if (hasRole && _random.NextDouble() < 0.2)
                {
                    await _messageControl.MessageAsync(socketGuild, "Miyaww!", true, "cat2");
                }
            }
        }
    }
}