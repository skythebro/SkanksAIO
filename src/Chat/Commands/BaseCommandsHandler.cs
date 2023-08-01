using System.Linq;
using ProjectM.Network;
using SkanksAIO.Chat.Attributes;
using SkanksAIO.Models;

namespace SkanksAIO.Chat.Commands;

internal class BaseCommandsHandler : AbstractChatCommandsHandler
{
    public BaseCommandsHandler(FromCharacter fromCharacter, string message, User user) :
        base(fromCharacter, message, user)
    { }

    [ChatCommand("reloadskanks", "reloads the server config")]
    [Admin]
    internal void reloadCommand()
    {
        Settings.Reload(Settings.Config);
        Reply("Config reloaded");
    }

    [ChatCommand("playercount", "gets playercount")]
    internal void PlayerCountCommand()
    {
        var totalUserCount = Utils.UserUtils.GetOnlineUsers().Count;
        Reply($"There are {totalUserCount} players online");
    }

}
