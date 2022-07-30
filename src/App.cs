using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using ProjectM.Network;
using SkanksAIO.Chat;
using SkanksAIO.Discord;
using SkanksAIO.Utils;
using UnityEngine;

namespace SkanksAIO;

public class App : MonoBehaviour
{
    internal static App? Instance { get; private set; }

    internal DiscordCommandLib Discord;

    internal ChatCommandLib Chat;

    public App()
    {
        App.Instance = this;
        Discord = new DiscordCommandLib(Plugin.Instance!.Token!.Value);
        Discord.Ready += OnReady;
        Chat = new ChatCommandLib();
    }

    private void OnReady()
    {
        Chat.Init();
    }

    private Task OnMessageReceived(SocketUserMessage message)
    {
        Messaging.SendGlobalMessage(ServerChatMessageType.Global, $"[Discord] {message.Author.Username}: {message.Content}");
        Plugin.Logger?.LogInfo($"[Discord] {message.Author.Username}: {message.Content}");

        return Task.CompletedTask;
    }

    private void Start()
    {
        if (!String.IsNullOrEmpty(Plugin.Instance!.Token!.Value))
        {
            StartCoroutine(nameof(CoroutineStart));
        }
    }

    internal void Stop()
    {
        if (!String.IsNullOrEmpty(Plugin.Instance!.Token!.Value))
        {
            StartCoroutine(nameof(CoroutineStop));
        }
    }

    public void CoroutineStart()
    {
        Discord.MessageReceived += OnMessageReceived;
        Discord.Start().GetAwaiter().GetResult();
    }

    public void CoroutineStop()
    {
        Discord.MessageReceived -= OnMessageReceived;
        Discord.Stop().GetAwaiter().GetResult();
    }
}
