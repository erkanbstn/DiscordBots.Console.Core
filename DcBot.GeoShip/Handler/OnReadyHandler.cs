﻿using DcBot.Common.MessageHandler;
using DcBot.Service.Interfaces;
using Discord;
using Discord.WebSocket;

namespace DcBot.GeoShip.Handler
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
        public async Task BotInitialize(SocketGuild socketGuild, DiscordSocketClient discordSocketClient)
        {
            await discordSocketClient.SetStatusAsync(UserStatus.DoNotDisturb);
            await _messageControl.MessageToChannel(socketGuild, "bot", "Geo Ship !", "love letter");
        }
    }
}