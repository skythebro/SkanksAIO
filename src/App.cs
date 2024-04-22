using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BepInEx.Core.Logging.Interpolation;
using Discord.WebSocket;
using ProjectM;
using ProjectM.Network;
using SkanksAIO.Chat;
using SkanksAIO.Discord;
using SkanksAIO.Utils;
using SkanksAIO.Utils.Config;
using UnityEngine;

namespace SkanksAIO;

public class App : MonoBehaviour
{
    internal static App? Instance { get; private set; }

    internal DiscordCommandLib? Discord;

    internal ChatCommandLib? Chat;

    internal float _currenttime;

    public App()
    {
        Instance = this;
        Discord = new DiscordCommandLib(Settings.Token!.Value);
        Discord.Ready += OnReady;
        Chat = new ChatCommandLib();
    }

    private void OnReady()
    {
        Chat.Init();
    }

    public void Restart()
    {
        if (Discord == null || Chat == null || Instance == null)
        {
            Instance = this;
            Discord = new DiscordCommandLib(Settings.Token!.Value);
            Chat = new ChatCommandLib();
            Discord.Ready += OnReady;
            Chat.Init();
        }

        Stop();
        _currenttime = JsonConfigHelper.GetAnnouncementInterval();
        announcementCount = JsonConfigHelper.GetAnnouncements().Count;
        Start();
    }

    private int announcementCount = JsonConfigHelper.GetAnnouncements().Count;

    private void Update()
    {
        if (!Settings.AnnounceEnabled.Value || JsonConfigHelper.GetAnnouncements().Count == 0) return;
        _currenttime += Time.deltaTime;
        if (!(_currenttime >= JsonConfigHelper.GetAnnouncementInterval())) return;

        Plugin.Logger?.LogDebug("Posting announcement: " + JsonConfigHelper.GetAnnouncements()[Settings.LastEntry]);
        Discord?.SendMessageAsync(JsonConfigHelper.GetAnnouncements()[Settings.LastEntry], isAnnouncement: true);

        // Make sure there are at least two announcements before attempting to select a different one
        if (Settings.AnnounceRandomOrder.Value && announcementCount > 1)
        {
            int nextIndex;
            do
            {
                nextIndex = Settings.Random.Next(0, announcementCount);
            } while (nextIndex == Settings.LastEntry);

            Settings.LastEntry = nextIndex;
        }
        else
        {
            Settings.LastEntry++;
            if (Settings.LastEntry == JsonConfigHelper.GetAnnouncements().Count)
            {
                Settings.LastEntry = 0;
            }
        }

        _currenttime = 0f;
    }

    private async Task OnMessageReceived(SocketUserMessage message)
    {
        try
        {
            await ClientOnMessageReceived(message);
        }
        catch (Exception e)
        {
            Plugin.Logger?.LogError($"[Discord] error: {e.Message} + {e.StackTrace}");
        }
        
    }
    
    private async Task ClientOnMessageReceived(SocketMessage socketMessage)
    {
        await Task.Run(() =>
        {
            //Activity is not from a Bot.
            if (!socketMessage.Author.IsBot)
            {
                Plugin.Logger?.LogDebug("channel id message: " + socketMessage.Channel.Id + " settings channel id: " + Settings.ChannelId!.Value);
                if (socketMessage.Channel.Id != Settings.ChannelId!.Value) return Task.CompletedTask;

                Messaging.SendGlobalMessage(ServerChatMessageType.Global,
                    $"[Discord] {socketMessage.Author.Username}: {socketMessage.Content}");
                Plugin.Logger?.LogInfo($"[Discord] {socketMessage.Author.Username}: {socketMessage.Content}");
                
            }
            return Task.CompletedTask;
        });
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(Settings.Token!.Value))
        {
            this._currenttime = 0f;
            StartCoroutine(nameof(CoroutineStart));
        }
    }

    internal void Stop()
    {
        if (!string.IsNullOrEmpty(Settings.Token!.Value))
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