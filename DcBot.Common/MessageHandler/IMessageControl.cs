using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

namespace DcBot.Common.MessageHandler
{
    public interface IMessageControl
    {
        public Task ReactionAsync(SocketCommandContext context, string emoteKey);
        public Task ReactionAsync(IUserMessage userMessage, string emoteKey);
        public Task DeleteAfterSendAsync(RestUserMessage restUserMessage, int deleteTime = 3000);
        public Task DeleteAfterSendAsync(IUserMessage userMessage, int deleteTime = 3000);
        public Task<RestUserMessage> MessageAsync(SocketCommandContext context, string content, bool isReaction = false, string emoteKey = null);
        public Task<RestUserMessage> EmbedAsync(SocketCommandContext context, Color color, string emoteKey, string content, string footer = "Created By Geo.");
        public Task<IUserMessage> MessageAsync(SocketGuild socketGuild, string content, bool isReaction = false, string emoteKey = null);
        public Task<IUserMessage> MessageAsync(SocketGuildUser socketGuildUser, string channelName, string content, bool isReaction = false, string emoteKey = null);
        public string RepeatEmoji(string emoji, int count);
        public Task<IUserMessage> MessageToChannel(SocketGuild socketGuild, string channelName, string startMessage, string emoteKey);
        public Task<RestUserMessage> EmbedShipAsync(SocketCommandContext context, SocketGuildUser shippedUser, int shipCount, string shipResult, string hearts, string brokenHearts);
    }
}