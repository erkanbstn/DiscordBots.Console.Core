using Discord.Commands;
using System.Collections.Concurrent;

namespace DcBot.Common.CooldownHandler
{
    public class CoolDownControl
    {
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<ulong, DateTime>> CommandCooldowns =
         new ConcurrentDictionary<string, ConcurrentDictionary<ulong, DateTime>>();

        public static void UpdateTime(string commandName, ulong userId)
        {
            var commandCooldowns = CommandCooldowns.GetOrAdd(commandName, _ => new ConcurrentDictionary<ulong, DateTime>());
            commandCooldowns.AddOrUpdate(userId, DateTime.UtcNow, (_, oldValue) => DateTime.UtcNow);
        }

        public static bool CoolDown(SocketCommandContext context, string commandName, out TimeSpan remainingTime)
        {
            if (context.User.Id == context.Guild.OwnerId)
            {
                remainingTime = TimeSpan.Zero;
                return false;
            }

            remainingTime = TimeSpan.Zero;

            if (CommandCooldowns.TryGetValue(commandName, out var commandCooldowns) &&
                commandCooldowns.TryGetValue(context.User.Id, out var lastCommandTime))
            {
                var cooldownTime = TimeSpan.FromSeconds(7);
                var elapsedTime = DateTime.UtcNow - lastCommandTime;

                if (elapsedTime < cooldownTime)
                {
                    remainingTime = cooldownTime - elapsedTime;
                    return true;
                }
            }

            return false;
        }
    }
}