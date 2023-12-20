using Discord;

namespace DcBot.Common.PermissionHandler
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PermissionControlAttribute : Attribute
    {
        public GuildPermission RequiredPermission { get; }

        public PermissionControlAttribute(GuildPermission requiredPermission)
        {
            RequiredPermission = requiredPermission;
        }
    }
}