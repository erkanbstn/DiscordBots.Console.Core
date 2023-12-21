﻿using DcBot.Common.MessageHandler;
using DcBot.Core.Core;
using DcBot.Service.Interfaces;
using Discord;
using Discord.WebSocket;

namespace DcBot.GeoBot.Handler
{
    public class OnReadyHandler
    {
        private readonly IDcServerService _dcServerService;
        private readonly IMessageControl _messageControl;
        public OnReadyHandler(IDcServerService dcServerService, IMessageControl messageControl)
        {
            _dcServerService = dcServerService;
            _messageControl = messageControl;
        }
        public async Task ServerInitializeAsync(SocketGuild socketGuild, DiscordSocketClient discordSocketClient)
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

            await discordSocketClient.SetStatusAsync(UserStatus.DoNotDisturb);
            await _messageControl.MessageToChannel(socketGuild, "bot", "Geo Bot !", "robot");
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