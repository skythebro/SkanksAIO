using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Bloodstone;
using Bloodstone.API;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using LiteDB;
using ProjectM.Network;
using SkanksAIO.Discord;
using SkanksAIO.Logger.Handler;
using SkanksAIO.Utils;
using SkanksAIO.Utils.Config;
using SkanksAIO.Web;

namespace SkanksAIO;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.Bloodstone")]
[BepInDependency("gg.deca.Bloodstone")]
public class Plugin : BasePlugin, IRunOnInitialized
{
    internal static Plugin? Instance { get; private set; }

    public static readonly string ConfigPath = Path.Combine(Paths.ConfigPath, "SkanksAIO");

    internal static DateTime ServerStartTime { get; private set; }

    internal static SkanksAIO.Logger.Logger Logger = null!;

    internal static LiteDatabase? Database;

    internal static ManualLogSource LogInstance { get; private set; }


    private App? App { get; set; }

    private Harmony? harmony;


    public override void Load()
    {
        if (!VWorld.IsServer)
        {
            Log.LogWarning("This plugin is a server-only plugin! Stopping plugin.");
            return;
        }

        LogInstance = Log;
        SetBindingRedirect();

        Instance = this;
    }

    public static void SetBindingRedirect()
    {
        AppDomain currentDomain = AppDomain.CurrentDomain;
        currentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
    }

    private static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
    {
        if (args.Name.ToLower().Contains("Newtonsoft".ToLower()))
        {
            LogInstance.LogDebug("Redirecting assembly load for Newtonsoft.Json.dll");
            var assemblyFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                                   "\\Newtonsoft.Json.dll";
            LogInstance.LogDebug($"Assemblyfilename: {assemblyFileName}");
            return Assembly.LoadFrom(assemblyFileName);
        }
        else
            return null;
    }

    public void OnGameInitialized()
    {
        if (!VWorld.IsServer) return;
        try
        {
            Logger = new SkanksAIO.Logger.Logger();

            Logger.RegisterLogHandler(new ConsoleLogHandler());
            Logger.RegisterLogHandler(new FileLogHandler());
            Logger.RegisterLogHandler(new DiscordLogHandler());

            Settings.Reload(Config);


            ClassInjector.RegisterTypeInIl2Cpp<App>();
            ClassInjector.RegisterTypeInIl2Cpp<WebBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<DiscordBehaviour>();

            harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
            App = Instance!.AddComponent<App>();
        }
        catch (Exception e)
        {
            Log.LogError("An error occured:" + e);
        }

        ServerStartTime = DateTime.Now;
        
        try
        {
            //var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string dllName = Assembly.GetExecutingAssembly().GetName().Name!;
            var pluginDirectory = Path.GetFullPath(Paths.PluginPath);
            var configDirectory = Path.Combine(pluginDirectory, dllName, "config");

           
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }
            
            var dbPathFull = Path.Combine(configDirectory, $"{dllName}.db");
            Log.LogDebug($"dbPathFull {dbPathFull}");
            
            Database = new LiteDatabase(dbPathFull);
        }
        catch (Exception e)
        {
            Log.LogError("An error occured trying to access/create database file. ErrorMessage:" + e.Message);
        }

        Log.LogInfo($"{MyPluginInfo.PLUGIN_GUID} is Loaded.");
    }


    public override bool Unload()
    {
        if (!VWorld.IsServer) return true;

        if (Settings.EnableWebServer.Value)
        {
            Settings.WsInstance?.Stop();
        }

        App?.Stop();

        harmony?.UnpatchSelf();

        return true;
    }
}