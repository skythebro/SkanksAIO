using ProjectM.Network;
using SkanksAIO.Utils;
using Unity.Entities;

abstract class AbstractChatCommandsHandler
{
    /// <summary>
    /// The user entity that sent the message.
    /// </summary>
    protected Entity UserEntity { get; private set; }
    /// <summary>
    /// The character entity of the user that sent the message.
    /// </summary>
    protected Entity CharacterEntity { get; private set; }
    /// <summary>
    /// The message sent by the user.
    /// </summary>
    protected string Message { get; private set; }
    /// <summary>
    /// The user component of the user that sent the message.
    /// </summary>
    protected User User { get; private set; }

    internal AbstractChatCommandsHandler(FromCharacter fromCharacter, string message, User user)
    {
        UserEntity = fromCharacter.User;
        CharacterEntity = fromCharacter.Character;
        Message = message;
        User = user;
    }

    /// <summary>
    /// Reply to the user by sending a system message to just that user.
    /// </summary>
    /// <param name="message"></param>
    internal void Reply(string message, ServerChatMessageType type = ServerChatMessageType.System)
    {
        Messaging.SendMessage(User, type, message);
    }
    /// <summary>
    /// Send a system message to every client that is currently connected to the server.
    /// </summary>
    /// <param name="message"></param>
    internal void SendToAll(string message, ServerChatMessageType type = ServerChatMessageType.System)
    {
        Messaging.SendGlobalMessage(type, $"[ALL] {message}");
    }
}