using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using UnityEngine;

namespace SkanksAIO.Discord;

class DiscordBehaviour : MonoBehaviour
{
    public delegate Task MessageReceivedHandler(SocketMessage message);
    public event MessageReceivedHandler? MessageReceived;

    public delegate Task SlashCommandExecutedHandler(SocketSlashCommand command);
    public event SlashCommandExecutedHandler? SlashCommandExecuted;

    public static DiscordBehaviour? Instance { get; private set; }

    private Queue<SocketMessage> messageQueue = new Queue<SocketMessage>();
    private Queue<SocketSlashCommand> slashCommandQueue = new Queue<SocketSlashCommand>();

    public DiscordBehaviour()
    {
        Instance = this;
    }

    public void QueueMessage(SocketMessage message)
    {
        messageQueue.Enqueue(message);
    }

    public void QueueSlashCommand(SocketSlashCommand command)
    {
        slashCommandQueue.Enqueue(command);
    }

    private void Update()
    {
        if (messageQueue.Count > 0)
        {
            var message = messageQueue.Dequeue();
            MessageReceived?.Invoke(message);
        }

        if (slashCommandQueue.Count > 0)
        {
            var command = slashCommandQueue.Dequeue();
            SlashCommandExecuted?.Invoke(command);
        }
    }
}