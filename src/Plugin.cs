using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using Discord;
using Discord.WebSocket;
using HarmonyLib;
using LiteDB;
using SkanksAIO.Discord;
using SkanksAIO.Logger.Handler;
using SkanksAIO.Utils;
using SkanksAIO.Web;
using UnhollowerRuntimeLib;
using Unity.Entities;
using UnityEngine;

namespace SkanksAIO;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static Plugin? Instance { get; private set; }

    internal static DateTime ServerStartTime {get; private set;}

    internal static World World
    {
        get
        {
            if (_serverWorld == null)
                _serverWorld = GetWorld("Server") ?? throw new System.Exception("Unable to find Server World! Aborting.");

            return _serverWorld;
        }
    }

    internal static bool IsServer => Application.productName == "VRisingServer";

    internal static SkanksAIO.Logger.Logger Logger;
    internal static LiteDatabase Database;

    internal ConfigEntry<string>? Token;
    internal ConfigEntry<ulong>? ChannelId;
    internal ConfigEntry<ulong>? AdminRoleId;
    internal ConfigEntry<int>? WebPort;
    internal ConfigEntry<string> MessageFooter;
    internal ConfigEntry<string> MessageFooterIcon;
    internal ConfigEntry<string> MessageTitle;
    internal ConfigEntry<bool> LeaderboardAsList;

    internal ConfigEntry<string> ChatCommandPrefix;

    private static World? _serverWorld;
    private Harmony? _harmony;

    private WebServer ws;

    private App? app;

    static Plugin()
    {
        Logger = new SkanksAIO.Logger.Logger();

        Logger.RegisterLogHandler(new ConsoleLogHandler());
        Logger.RegisterLogHandler(new FileLogHandler());
        Logger.RegisterLogHandler(new DiscordLogHandler());

        ServerStartTime = DateTime.Now;
        var basepath = Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        var dbPathFull = Path.Combine(basepath!, "config", "SkanksAIO.db");
        Database = new LiteDatabase(dbPathFull);
    }

    public Plugin()
    {
        Instance = this;

        // Discord Settings
        Token = Config.Bind("Discord", "Token", "", "Bot Token from https://discord.com/developers/applications");
        ChannelId = Config.Bind("Discord", "ChannelId", (ulong)0, "Channel ID of the channel to post messages to");
        AdminRoleId = Config.Bind("Discord", "AdminRoleId", (ulong)0, "ID of an Administrative role in your discord server.");

        MessageTitle = Config.Bind("Discord", "Title", "", "Title for embedded message");
        MessageFooter = Config.Bind("Discord", "Footer", "", "Footer for embedded message");
        MessageFooterIcon = Config.Bind("Discord", "FooterIcon", "", "Footer icon for embedded message");

        // Leaderboard List View Setting
        LeaderboardAsList = Config.Bind("Discord", "ShowLeaderboardAsList", false, "If true, the leaderboard will be shown as a table instead of a grid.");

        // Webserver Settings
        WebPort = Config.Bind("Web", "Port", 8080, "Port the webserver will run on");

        // Chat Settings
        ChatCommandPrefix = Config.Bind("Chat", "CommandPrefix", ".", "Prefix for all chat commands");

        ws = new WebServer(WebPort.Value);
    }

    public static void Init()
    {
        if (!IsServer) return;

        _serverWorld = GetWorld("Server");
        if (_serverWorld == null)
        {
            Plugin.Logger?.LogError("Failed to initialize Plugin. World not found.");
            return;
        }

        Plugin.Instance!.app = Plugin.Instance.AddComponent<App>();
        Plugin.Logger?.LogMessage("Plugin Ready.");
    }

    public override void Load()
    {
        if (!IsServer) return;

        ws.Start();

        ClassInjector.RegisterTypeInIl2Cpp<App>();
        ClassInjector.RegisterTypeInIl2Cpp<WebBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<DiscordBehaviour>();

        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginInfo.PLUGIN_GUID);
    }

    public override bool Unload()
    {
        if (!IsServer) return true;

        ws.Stop();
        app?.Stop();

        _harmony?.UnpatchSelf();

        return true;
    }

    private static World? GetWorld(string name)
    {
        foreach (var world in World.s_AllWorlds)
        {
            if (world.Name == name)
            {
                _serverWorld = world;
                return world;
            }
        }

        return null;
    }
}
