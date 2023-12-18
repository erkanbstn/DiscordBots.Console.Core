using DcBot.Service.Interfaces;
using Discord.WebSocket;
using DcBot.Core.Concrete;
using Discord;

namespace DcBot.GeoBot.BotHandler
{
    public class OnReadyHandler
    {
        private readonly IDcServerService _dcServerService;

        public OnReadyHandler(IDcServerService dcServerService)
        {
            _dcServerService = dcServerService;
        }

        public async Task ServerInitializeAsync(SocketGuild socketGuild)
        {
            var server = _dcServerService.FirstOrDefaultAsync(x => x.DiscordId == socketGuild.Id.ToString());

            if (server == null)
            {
                await _dcServerService.InsertAsync(new DcServer
                {
                    DiscordId = socketGuild.Id.ToString(),
                    Name = socketGuild.Name,
                    OwnerId = socketGuild.OwnerId.ToString(),
                    OwnerName = socketGuild.Owner.GlobalName
                });
            }

            var channel = (socketGuild.Channels.FirstOrDefault(x => x.Name.Contains("bot")) as ITextChannel);
            await channel.SendMessageAsync("Merhaba");
        }
        public async Task UserJoinedAsync(SocketGuildUser user)
        {
            var role = user.Guild.GetRole(1185709334789886033);
            await user.AddRoleAsync(role);

            var welcomeChannel = user.Guild.GetTextChannel(1185670107528187937);
            await welcomeChannel.SendMessageAsync($"Hoşgeldin {user.Mention} Sana {role.Mention} Rolünü Atadım.!");
        }
        public async Task UserLeftAsync(SocketGuild socketGuild, SocketUser socketUser)
        {
            var welcomeChannel = socketGuild.GetTextChannel(1185670107528187937);
            await welcomeChannel.SendMessageAsync($"Görüşürüz {socketUser.Mention}.!");
        }
    }
}