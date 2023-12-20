using Discord.Commands;

namespace DcBot.GeoBot.General
{
    public class SccTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            if (context is SocketCommandContext socketCommandContext)
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(socketCommandContext));
            }

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Failed to parse SocketCommandContext."));
        }
    }
}
