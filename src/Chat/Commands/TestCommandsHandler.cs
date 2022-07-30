using ProjectM.Network;
using SkanksAIO.Chat.Attributes;

namespace SkanksAIO.Chat.Commands;

internal class TestCommandsHandler : AbstractChatCommandsHandler
{
    public TestCommandsHandler(FromCharacter fromCharacter, string message, User user) :
        base(fromCharacter, message, user)
    { }

    [ChatCommand("test", "just a test")]
    [Admin]
    internal void TestCommand(string username, string reason = "")
    {
        this.Reply($"{this.User.CharacterName} thinks {username} is a beeeeech. ({reason})");
    }

    [ChatCommand("rawr", "just a test")]
    internal void RawrCommand(string username, string reason = "")
    {
        this.Reply($"Public command");
    }

}
