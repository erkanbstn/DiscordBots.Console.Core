using Discord.Commands;
using System.Collections.Concurrent;

namespace DcBot.Common.CooldownHandler
{
    public class CoolDownControl
    {
        private static readonly ConcurrentDictionary<ulong, DateTime> LastCommandTimes = new ConcurrentDictionary<ulong, DateTime>();

        public static void UpdateLastCommandTime(ulong userId)
        {
            LastCommandTimes.AddOrUpdate(userId, DateTime.UtcNow, (_, oldValue) => DateTime.UtcNow);
        }
        public static bool IsOnCooldown(SocketCommandContext socketCommandContext, out TimeSpan remainingTime)
        {
            if (socketCommandContext.User.Id == socketCommandContext.Guild.OwnerId)
            {
                remainingTime = TimeSpan.Zero;
                return false;
            }
            remainingTime = TimeSpan.Zero;

            if (LastCommandTimes.TryGetValue(socketCommandContext.User.Id, out var lastCommandTime))
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