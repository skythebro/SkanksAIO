using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using SkanksAIO.Web;

namespace SkanksAIO.Utils.Config
{
    public class Settings
    {
        public static ConfigFile Config { get; private set; }

        #region DiscordConfigAttributes

        internal static ConfigEntry<string>? Token { get; private set; }
        internal static ConfigEntry<ulong>? ChannelId { get; private set; }
        internal static ConfigEntry<ulong>? AdminRoleId { get; private set; }

        internal static ConfigEntry<ulong>? AnnounceChannelId { get; private set; }

        #endregion

        #region WebserverConfigAttributes

        internal static ConfigEntry<bool> EnableWebServer { get; private set; }
        internal static ConfigEntry<bool> EnablePvPKillTracker { get; private set; }
        internal static ConfigEntry<bool> EnableInteractiveMap { get; private set; }
        internal static ConfigEntry<int> InteractiveMapUpdateInterval { get; private set; }
        internal static ConfigEntry<bool> TrackPlayersOnMap { get; private set; }
        internal static ConfigEntry<int>? WebPort { get; private set; }

        internal static WebServer? WsInstance { get; private set; } = null;

        #endregion

        #region DiscordMessageConfigAttributes

        internal static ConfigEntry<string> MessageFooter { get; private set; }
        internal static ConfigEntry<string> MessageFooterIcon { get; private set; }
        internal static ConfigEntry<string> MessageTitle { get; private set; }
        internal static ConfigEntry<bool> LeaderboardAsList { get; private set; }
        internal static ConfigEntry<string> ChatCommandPrefix { get; private set; }
        
        internal static ConfigEntry<string> GlobalChatLabel { get; private set; }

        #endregion

        #region VipConfigAttributes

        internal static ConfigEntry<bool> EnableVIPFunctionality { get; private set; }

        #endregion

        #region ServerAnnouncementsConfigAttributes

        internal static ConfigEntry<float> AnnounceTimer { get; private set; }

        internal static ConfigEntry<bool> AnnounceRandomOrder { get; private set; }

        internal static ConfigEntry<bool> AnnounceEnabled { get; private set; }

        internal static ConfigEntry<bool> ShowUserConnectedInDc { get; private set; }

        internal static ConfigEntry<bool> ShowUserDisConnectedInDc { get; private set; }

        #endregion

        #region AnnouncementVariables

        public static Random Random;

        internal static int LastEntry;

        #endregion

        #region OfflineRaidingConfigA   

        public static ConfigEntry<bool> DisableOfflineRaiding;
        public static ConfigEntry<bool> FactorAllies;
        public static ConfigEntry<int> MaxAllyCacheAge;

        #endregion

        private static void Initialize(ConfigFile config)
        {
            Config = config;

            //Server settings
            EnablePvPKillTracker = config.Bind("Server", "EnablePvPKillTracker", true,
                "Enables the PvP Kill Tracker. Warning: if disabled ELO wont update when killing other players (because it doesnt track kills anymore (only applicable to PvP)).");

            #region DiscordConfig

            // Discord Settings
            Token = config.Bind("Discord", "Token", "", "Bot Token from https://discord.com/developers/applications");
            ChannelId = config.Bind("Discord", "ChannelId", (ulong)0, "Channel ID of the channel to post messages to");
            AnnounceChannelId = config.Bind("Discord", "AnnounceChannelId", (ulong)0,
                $"Channel ID of the channel to post Announcements to (leave default if you want it to be in the same channel as the {nameof(ChannelId)}) setting");
            AdminRoleId = config.Bind("Discord", "AdminRoleId", (ulong)0,
                "ID of an Administrative role in your discord server.");

            #endregion

            #region DiscordMessageConfig

            MessageTitle = config.Bind("Discord", "Title", "", "Title for embedded message");
            MessageFooter = config.Bind("Discord", "Footer", "", "Footer for embedded message");
            MessageFooterIcon = config.Bind("Discord", "FooterIcon", "", "Footer icon for embedded message");

            // Leaderboard List View Setting
            LeaderboardAsList = config.Bind("Discord", "ShowLeaderboardAsList", false,
                "If true, the leaderboard will be shown as a table instead of a grid.");
            
            GlobalChatLabel = config.Bind("Chat", "GlobalChatLabel", "GLOBAL", "Label for global chat messages that get sent to discord.");

            #endregion
            
            #region WebserverConfig

            // Webserver Settings
            EnableWebServer = config.Bind("Web", "Enable", false, "Enable the webserver");
            WebPort = config.Bind("Web", "Port", 8080, "Port the webserver will run on");
            EnableInteractiveMap = config.Bind("Web", "EnableInteractiveMap", false, "Enables the interactive map");
            TrackPlayersOnMap = config.Bind("Web", "TrackPlayersOnMap", false,
                "Enables tracking of players on the interactive map");
            InteractiveMapUpdateInterval = config.Bind("Web", "InteractiveMapUpdateInterval", 10,
                "Interval in seconds for the interactive map to update. Don't set this too low or you might get rate limited.");

            #endregion

            // Chat Settings
            ChatCommandPrefix = config.Bind("Chat", "CommandPrefix", "!", "Prefix for all chat commands");


            #region VipConfig

            EnableVIPFunctionality = config.Bind("VIP", "EnableVIP", false,
                "Enables the VIP functionality. the txt file will generate in the bepInEx config folder SkanksAIO/SkanksAIO.VIPlist.txt folder after restart. This txt file will be read at startup or when reloading. To add a user to VIP you need to add their steamid64 to the file. (1 per line)");
            

            #endregion


            #region WebserverExtras

            if (EnableWebServer.Value)
            {
                WebServer ws;
                if (WsInstance != null)
                {
                    ws = WsInstance;
                    ws.Stop();
                }
                else
                {
                    ws = new WebServer(WebPort.Value);
                }

                WsInstance = ws;
                ws.Start();
            }

            #endregion


            #region ServerAnnouncementsConfig

            ShowUserConnectedInDc = config.Bind("Announcements", "ShowUserConnectedInDC", true,
                "Show in discord chat when users connect");
            ShowUserDisConnectedInDc = config.Bind("Announcements", "ShowUserDisConnectedInDC", true,
                "Show in discord chat when users disconnect");
            AnnounceTimer =
                config.Bind("Announcements", "AnnounceTimer", 180f, "Time between messages in seconds");
            AnnounceEnabled =
                config.Bind("Announcements", "AnnounceEnabled", false, "Enable auto messages system");
            AnnounceRandomOrder = config.Bind("Announcements", "AnnounceRandomOrder", false,
                "Random order for announcement messages");

            JsonConfigHelper.SetAnnouncementInterval(AnnounceTimer.Value);
            
            LastEntry = 0;
            Random = new Random();
            if (App.Instance != null)
            {
                App.Instance.Restart();
            }
            
            #endregion

            #region OfflineRaidingConfig

            DisableOfflineRaiding = Config.Bind("Config", "DisableOfflineRaiding", false, "Enable/disable OfflineRaiding protection.");
            FactorAllies = Config.Bind("Config", "FactorAllies", true, "Include the besieged player allies online status before blocking siege.");
            MaxAllyCacheAge = Config.Bind("Config", "MaxAllyCacheAge", 300, "Max age of the besieged player allies cache in seconds.\n" +
                                                                               "If the cache age is older than specified, the cache will be renewed.\n" +
                                                                               "Don't set this too short as allies gathering process can slightly impact your server performance.\n" +
                                                                               "This cache is only for allies gathering, their online/offline status is updated instantly.");

            #endregion
            
        }

        internal static void Reload(ConfigFile config)
        {
            Plugin.Logger?.LogDebug("config reloaded");
            JsonConfigHelper.CreateJsonConfigs();
            JsonConfigHelper.LoadAllConfigs();
            Initialize(config);
        }
    }
}