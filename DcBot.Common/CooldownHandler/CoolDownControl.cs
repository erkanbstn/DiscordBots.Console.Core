using Discord.Commands;
using System.Collections.Concurrent;

namespace DcBot.Common.CooldownHandler
{
    public class CoolDownControl
    {
        private static readonly ConcurrentDictionary<ulong, DateTime> GeoBotTimes = new ConcurrentDictionary<ulong, DateTime>();
        private static readonly ConcurrentDictionary<ulong, DateTime> GeoMoTimes = new ConcurrentDictionary<ulong, DateTime>();
        private static readonly ConcurrentDictionary<ulong, DateTime> GeoCGuardTimes = new ConcurrentDictionary<ulong, DateTime>();
        private static readonly ConcurrentDictionary<ulong, DateTime> GeoRGuardTimes = new ConcurrentDictionary<ulong, DateTime>();
        private static readonly ConcurrentDictionary<ulong, DateTime> GeoUGuardTimes = new ConcurrentDictionary<ulong, DateTime>();
        private static readonly ConcurrentDictionary<ulong, DateTime> GeoShipTimes = new ConcurrentDictionary<ulong, DateTime>();

        public static void GeoBotUpdateTime(ulong userId)
        {
            GeoBotTimes.AddOrUpdate(userId, DateTime.UtcNow, (_, oldValue) => DateTime.UtcNow);
        }
        public static bool GeoBotCoolDown(SocketCommandContext socketCommandContext, out TimeSpan remainingTime)
        {
            if (socketCommandContext.User.Id == socketCommandContext.Guild.OwnerId)
            {
                remainingTime = TimeSpan.Zero;
                return false;
            }
            remainingTime = TimeSpan.Zero;

            if (GeoBotTimes.TryGetValue(socketCommandContext.User.Id, out var lastCommandTime))
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
        public static void GeoMoUpdateTime(ulong userId)
        {
            GeoMoTimes.AddOrUpdate(userId, DateTime.UtcNow, (_, oldValue) => DateTime.UtcNow);
        }
        public static bool GeoMoCoolDown(SocketCommandContext socketCommandContext, out TimeSpan remainingTime)
        {
            if (socketCommandContext.User.Id == socketCommandContext.Guild.OwnerId)
            {
                remainingTime = TimeSpan.Zero;
                return false;
            }
            remainingTime = TimeSpan.Zero;

            if (GeoMoTimes.TryGetValue(socketCommandContext.User.Id, out var lastCommandTime))
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
        public static void GeoUGuardUpdateTime(ulong userId)
        {
            GeoUGuardTimes.AddOrUpdate(userId, DateTime.UtcNow, (_, oldValue) => DateTime.UtcNow);
        }
        public static bool GeoUGuardCoolDown(SocketCommandContext socketCommandContext, out TimeSpan remainingTime)
        {
            if (socketCommandContext.User.Id == socketCommandContext.Guild.OwnerId)
            {
                remainingTime = TimeSpan.Zero;
                return false;
            }
            remainingTime = TimeSpan.Zero;

            if (GeoUGuardTimes.TryGetValue(socketCommandContext.User.Id, out var lastCommandTime))
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
        public static void GeoRGuardUpdateTime(ulong userId)
        {
            GeoRGuardTimes.AddOrUpdate(userId, DateTime.UtcNow, (_, oldValue) => DateTime.UtcNow);
        }
        public static bool GeoRGuardCoolDown(SocketCommandContext socketCommandContext, out TimeSpan remainingTime)
        {
            if (socketCommandContext.User.Id == socketCommandContext.Guild.OwnerId)
            {
                remainingTime = TimeSpan.Zero;
                return false;
            }
            remainingTime = TimeSpan.Zero;

            if (GeoRGuardTimes.TryGetValue(socketCommandContext.User.Id, out var lastCommandTime))
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
        public static void GeoCGuardUpdateTime(ulong userId)
        {
            GeoCGuardTimes.AddOrUpdate(userId, DateTime.UtcNow, (_, oldValue) => DateTime.UtcNow);
        }
        public static bool GeoCGuardCoolDown(SocketCommandContext socketCommandContext, out TimeSpan remainingTime)
        {
            if (socketCommandContext.User.Id == socketCommandContext.Guild.OwnerId)
            {
                remainingTime = TimeSpan.Zero;
                return false;
            }
            remainingTime = TimeSpan.Zero;

            if (GeoCGuardTimes.TryGetValue(socketCommandContext.User.Id, out var lastCommandTime))
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
        public static void GeoShipUpdateTime(ulong userId)
        {
            GeoShipTimes.AddOrUpdate(userId, DateTime.UtcNow, (_, oldValue) => DateTime.UtcNow);
        }
        public static bool GeoShipCoolDown(SocketCommandContext socketCommandContext, out TimeSpan remainingTime)
        {
            if (socketCommandContext.User.Id == socketCommandContext.Guild.OwnerId)
            {
                remainingTime = TimeSpan.Zero;
                return false;
            }
            remainingTime = TimeSpan.Zero;

            if (GeoShipTimes.TryGetValue(socketCommandContext.User.Id, out var lastCommandTime))
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