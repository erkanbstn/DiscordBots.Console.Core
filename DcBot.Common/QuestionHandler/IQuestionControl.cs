using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

namespace DcBot.Common.QuestionHandler
{
    public interface IQuestionControl
    {
        public Task<bool> Questioner(SocketCommandContext socketCommandContext, SocketGuildUser receiverUser, RestUserMessage questionMessage, string successMessage, string cancelMessage);
        public Task<string> WaitForUserReaction(SocketGuildUser user, IUserMessage message);
    }
}