using Discord;
using Discord.Commands;
using Discord.Rest;

namespace DcBot.Common.MessageHandler
{
    public class MessageControl
    {
        public async Task ReactionAsync(SocketCommandContext context, string emoteKey)
        {
            string newKey = $":{emoteKey}:";
            if (Emoji.TryParse(newKey, out var emoji))
            {
                await context.Message.AddReactionAsync(emoji);
            }
        }
        public async Task DeleteAfterSendAsync(RestUserMessage restUserMessage, int deleteTime = 3000)
        {
            await Task.Delay(deleteTime);
            await restUserMessage.DeleteAsync();
        }
        public async Task<RestUserMessage> MessageAsync(SocketCommandContext context, string content, bool isReaction = false, string emoteKey = null)
        {
            if (isReaction)
                await ReactionAsync(context, emoteKey);

            return await context.Channel.SendMessageAsync(content);
        }
        public async Task<RestUserMessage> EmbedAsync(SocketCommandContext context, Color color, string emoteKey, string content, string footer = "Design By Geo")
        {
            await ReactionAsync(context, emoteKey);

            var user = context.User;

            var embed = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl())
                .WithDescription(content)
                .WithColor(color)
                .WithAuthor(user.Username)
                .WithCurrentTimestamp()
                .WithFooter(footer)
                .Build();

            return await context.Channel.SendMessageAsync(embed: embed);
        }
    }
}