using Discord.Commands;
using Discord.WebSocket;
using System.Data;

namespace DcBot.Common.PrefixHandler
{
    public class PrefixControl
    {
        public bool PrefixFixer(SocketCommandContext context, GeoBotCommands geoBotCommands, out string[] strings,out SocketUser socketUser, out SocketRole socketRole)
        {
            var userInfo = context.Message.MentionedUsers.FirstOrDefault();
            var roleInfo = context.Message.MentionedRoles.FirstOrDefault();
            string[] parameters = context.Message.Content.Trim().Split(' ');

            switch (geoBotCommands)
            {
                case GeoBotCommands.DeleteMessage:
                case GeoBotCommands.WeatherInfo:
                case GeoBotCommands.Afk:

                    if (parameters.Length == 2 && parameters.All(p => !string.IsNullOrEmpty(p)))
                    {
                        strings = parameters.Skip(1).ToArray();
                        socketRole = null;
                        socketUser = null;
                        return true;
                    }
                    break;
                case GeoBotCommands.Where:
                case GeoBotCommands.Avatar:
                case GeoBotCommands.Listen:
                    if (parameters.Length == 2 && parameters.All(p => !string.IsNullOrEmpty(p)) && userInfo != null)
                    {
                        strings = parameters.Skip(1).ToArray();
                        socketRole = null;
                        socketUser = userInfo;
                        return true;
                    }
                    break;
                case GeoBotCommands.Help:
                    if (parameters.Length == 1 && parameters.All(p => !string.IsNullOrEmpty(p)))
                    {
                        strings = null;
                        socketRole = null;
                        socketUser = null;
                        return true;
                    }
                    break;
            }
            strings = null;
            socketRole = null;
            socketUser = null;
            return false;
        }
    }
}