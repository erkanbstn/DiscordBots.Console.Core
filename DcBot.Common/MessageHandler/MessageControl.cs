using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

namespace DcBot.Common.MessageHandler
{
    public class MessageControl : IMessageControl
    {
        public Emoji GetReactionAsync(string emoteKey)
        {
            string newKey = $":{emoteKey.Replace(" ", "_")}:";
            if (Emoji.TryParse(newKey, out var emoji))
            {
                return emoji;
            }
            return null;
        }
        public async Task ReactionAsync(SocketCommandContext context, string emoteKey)
        {
            string newKey = $":{emoteKey.Replace(" ", "_")}:";
            if (Emoji.TryParse(newKey, out var emoji))
            {
                await context.Message.AddReactionAsync(emoji);
            }
        }
        public async Task ReactionAsync(IUserMessage userMessage, string emoteKey)
        {
            string newKey = $":{emoteKey.Replace(" ", "_")}:";
            if (Emoji.TryParse(newKey, out var emoji))
            {
                await userMessage.AddReactionAsync(emoji);
            }
        }
        public async Task DeleteAfterSendAsync(RestUserMessage restUserMessage, int deleteTime = 3000)
        {
            Task.Delay(deleteTime).ContinueWith(_ => restUserMessage.DeleteAsync());
        }
        public async Task DeleteAfterSendAsync(IUserMessage userMessage, int deleteTime = 3000)
        {
            Task.Delay(deleteTime).ContinueWith(_ => userMessage.DeleteAsync());
        }
        public async Task<RestUserMessage> MessageAsync(SocketCommandContext context, string content, bool isReaction = false, string emoteKey = null)
        {
            if (isReaction)
                await ReactionAsync(context, emoteKey);

            return await context.Channel.SendMessageAsync(content);
        }
        public async Task<RestUserMessage> EmbedAsync(SocketCommandContext context, Color color, string emoteKey, string content, string footer = "Created By Geo.")
        {
            await ReactionAsync(context, emoteKey);

            var user = context.User;

            var embedAuthor = new EmbedAuthorBuilder()
                .WithIconUrl(user.GetAvatarUrl())
                .WithName(user.Username);

            var embedFooter = new EmbedFooterBuilder()
                .WithIconUrl(context.Guild.IconUrl)
                .WithText(footer);


            var embedMessage = new EmbedBuilder()
                .WithDescription(content)
                .WithColor(color)
                .WithAuthor(embedAuthor)
                .WithFooter(embedFooter)
                .WithCurrentTimestamp()
                .Build();



            return await context.Channel.SendMessageAsync(embed: embedMessage);
        }
        public async Task<IUserMessage> MessageAsync(SocketGuild socketGuild, string content, bool isReaction = false, string emoteKey = null)
        {
            var channel = (socketGuild.Channels.FirstOrDefault(x => x.Name.Contains("bot")) as ITextChannel);
            var message = await channel.SendMessageAsync(content);

            if (isReaction)
                await ReactionAsync(message, emoteKey);

            return message;
        }
        public async Task<IUserMessage> MessageAsync(SocketGuildUser socketGuildUser, string channelName, string content, bool isReaction = false, string emoteKey = null)
        {
            var channel = socketGuildUser.Guild.Channels.FirstOrDefault(channel => channel.Name.Equals(channelName));
            var message = await (channel as ITextChannel).SendMessageAsync(content);

            if (isReaction)
                await ReactionAsync(message, emoteKey);

            return message;
        }
        public string RepeatEmoji(string emoteKey, int count)
        {
            var emoji = GetReactionAsync(emoteKey);
            return string.Join(" ", Enumerable.Repeat(emoji, count));
        }
        public async Task<RestUserMessage> EmbedShipAsync(SocketCommandContext context, SocketGuildUser shippedUser, int shipCount, string shipResult, string hearts, string brokenHearts)
        {
            var shipperUser = context.User as SocketGuildUser;

            var embedMessage = new EmbedBuilder()
                .WithColor(Color.Magenta)
                .WithTitle("Aşk Yüzdesi")
                .WithDescription($"{shipperUser.Mention} ve {shippedUser.Mention} Aşk Yüzdeniz: **{shipCount}%**\n\n`{shipResult}`\n\n{hearts} {brokenHearts}")
                .WithFooter(new EmbedFooterBuilder
                {
                    IconUrl = shipperUser.GetAvatarUrl(),
                    Text = shipperUser.Username
                })
                .WithThumbnailUrl(shippedUser.GetAvatarUrl())
                .Build();

            var message = await context.Channel.SendMessageAsync(embed: embedMessage);

            if (shipCount >= 50)
            {
                await message.AddReactionAsync(GetReactionAsync("heart on fire"));
            }
            else
            {
                await message.AddReactionAsync(GetReactionAsync("mending heart"));
            }

            return message;
        }
        public async Task<IUserMessage> MessageToChannel(SocketGuild socketGuild, string channelName, string startMessage, string emoteKey)
        {
            var channel = (socketGuild.Channels.FirstOrDefault(x => x.Name.Contains(channelName)) as ITextChannel);
            return await MessageAsync(socketGuild, startMessage, true, emoteKey);
        }
    }
}