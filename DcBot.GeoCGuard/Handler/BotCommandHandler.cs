using DcBot.Common.MessageHandler;
using DcBot.Common.PrefixHandler;
using DcBot.Core.Enums;
using DcBot.Service.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace DcBot.GeoCGuard.Handler
{
    public class BotCommandHandler
    {
        private readonly IMessageControl _messageControl;
        private readonly IConfiguration _configuration;
        private readonly IRoleTypeRelationService _roleTypeRelationService;
        private readonly IDcServerService _dcServerService;
        private readonly IPrefixControl _prefixControl;

        public BotCommandHandler(IMessageControl messageControl, IConfiguration configuration, IRoleTypeRelationService roleTypeRelationService, IDcServerService dcServerService, IPrefixControl prefixControl)
        {
            _messageControl = messageControl;
            _configuration = configuration;
            _roleTypeRelationService = roleTypeRelationService;
            _dcServerService = dcServerService;
            _prefixControl = prefixControl;
        }

        public async Task BotInitialize(SocketGuild socketGuild, DiscordSocketClient discordSocketClient)
        {
            await discordSocketClient.SetStatusAsync(UserStatus.DoNotDisturb);
            await _messageControl.DeleteAfterSendAsync(await _messageControl.MessageToChannel(socketGuild, "bot", "Geo Channel Guard !", "shield"));
        }

        public async Task MessageCheck(SocketMessage socketMessage, DiscordSocketClient discordSocketClient)
        {
            var message = socketMessage as SocketUserMessage;
            var context = new SocketCommandContext(discordSocketClient, message);
            var dcServer = await _dcServerService.FirstOrDefaultAsync(x => x.DiscordId == context.Guild.Id.ToString());

            var roleTypeRelationLink = await _roleTypeRelationService.GetRoleTypeAsync(RoleTypes.Link, dcServer.Id);
            var roleTypeRelationBadWord = await _roleTypeRelationService.GetRoleTypeAsync(RoleTypes.BadWord, dcServer.Id);

            var linkWords = _prefixControl.GetAppSettingsArray("LinkWords", "Word");
            var badWords = _prefixControl.GetAppSettingsArray("BadWords", "Word");

            var hasLinkRole = !string.IsNullOrEmpty(roleTypeRelationLink?.DiscordId) &&
                  context.User is SocketGuildUser guildUser &&
                  guildUser.Roles.Any(role => role.Id.ToString() == roleTypeRelationLink.DiscordId);

            if (!hasLinkRole && linkWords.Any(word => socketMessage.Content.ToLower().Contains(word)))
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(context, "zipper mouth", $"{message.Author.Mention} Link Paylaşmak Yasaktır!"));
                await socketMessage.DeleteAsync();
            }

            var hasBadWordRole = !string.IsNullOrEmpty(roleTypeRelationBadWord?.DiscordId) &&
                  context.User is SocketGuildUser badWordGuildUser &&
                  badWordGuildUser.Roles.Any(role => role.Id.ToString() == roleTypeRelationBadWord.DiscordId);

            if (!hasBadWordRole && badWords.Any(word => socketMessage.Content.ToLower().Contains(word)))
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(context, "zipper mouth", $"{message.Author.Mention} Küfürlü İçerik Paylaşmak Yasaktır!"));
                await socketMessage.DeleteAsync();
            }
        }
    }
}