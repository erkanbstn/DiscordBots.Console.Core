using DcBot.Common.MessageHandler;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

namespace DcBot.Common.QuestionHandler
{
    public class QuestionControl : IQuestionControl
    {
        private readonly IMessageControl _messageControl;

        public QuestionControl(IMessageControl messageControl)
        {
            _messageControl = messageControl;
        }

        public async Task<bool> Questioner(SocketCommandContext socketCommandContext, SocketGuildUser receiverUser, RestUserMessage questionMessage, string successMessage, string cancelMessage)
        {
            await questionMessage.AddReactionAsync(new Emoji("👍"));
            await questionMessage.AddReactionAsync(new Emoji("👎"));

            var reactionResult = await WaitForUserReaction(receiverUser, questionMessage);
            await _messageControl.DeleteAfterSendAsync(questionMessage, 15000);
            if (reactionResult == "👍")
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(socketCommandContext, "loud sound", successMessage), 3000);
                return true;
            }
            else
            {
                await _messageControl.DeleteAfterSendAsync(await _messageControl.EmbedAsync(socketCommandContext, "mute", cancelMessage), 3000);
                return false;
            }
        }
        public async Task<string> WaitForUserReaction(SocketGuildUser user, IUserMessage message)
        {
            var timeout = TimeSpan.FromSeconds(15);
            var startTime = DateTime.Now;

            while (DateTime.Now - startTime < timeout)
            {
                var reactions = await message.GetReactionUsersAsync(new Emoji("👍"), 10).FlattenAsync();

                if (reactions.Any(u => u.Id == user.Id))
                {
                    return "👍";
                }

                reactions = await message.GetReactionUsersAsync(new Emoji("👎"), 10).FlattenAsync();

                if (reactions.Any(u => u.Id == user.Id))
                {
                    return "👎";
                }

                await Task.Delay(1000);
            }

            return null;
        }
    }
}