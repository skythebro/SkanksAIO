
using Bloodstone.API;
using System;
using ProjectM.Network;
using SkanksAIO.Chat.Attributes;
using SkanksAIO.Models;
using SkanksAIO.Utils;
using Unity.Transforms;
using SkanksAIO.Utils.Config;

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
    
    [ChatCommand("listcommands", "lists all server commands")]
    [Admin]
    internal void listCommands()
    {
        var commands = ChatCommandLib.GetCommands;
        foreach (var (key, value) in commands)
        {
            Reply($"Command: {key} Description: {value.Description}");
        }
    }

    [ChatCommand("playercount", "gets playercount")]
    internal void PlayerCountCommand()
    {
        var totalUserCount = UserUtils.GetOnlineUsers().Count;
        Reply($"There are {totalUserCount} players online");
    }
    
    [ChatCommand("listmarkertypes", "lists all the usable marker types")]
    [Admin]
    internal void ListMarkerTypesCommand()
    {
        var allMarkerTypes = Enum.GetValues(typeof(MarkerType));
        foreach (var markerType in allMarkerTypes)
        {
            Reply($"MarkerType: {markerType}");
        }
    }
    
    [ChatCommand("addmarker", "add a custom marker to the map from where you're standing. Usage: addMarker <markername> <type>, for a list of types, use the command listmarkertypes")]
    [Admin]
    internal void AddMarkerCommand(string markername, string typeName)
    {
        if (!Enum.IsDefined(typeof(MarkerType), typeName))
        {
            Reply($"Invalid marker type '{typeName}'. Use the command listmarkertypes to see the list of valid types.");
            return;
        }
        
        MarkerType type = (MarkerType)Enum.Parse(typeof(MarkerType), typeName);
        
        var user = GetUser();
        var player = UserUtils.UserToPlayerEntity(user);
        VWorld.Server.EntityManager.TryGetComponentData<Translation>(player, out var playerPosition);
        var locationData = new JsonConfigHelper.MarkerLocationData(type, playerPosition.Value.x, playerPosition.Value.z);
        var added = JsonConfigHelper.AddMarker(markername, locationData);
        
        
        Reply(added ? "Marker successfully added" : $"Marker with name {markername} couldn't be added, it might already exist");
    }
    
    [ChatCommand("removemarker", "removes marker with the given name. Usage: removemarker <markername>")]
    [Admin]
    internal void RemoveMarkerCommand(string markername)
    {
        var removed = JsonConfigHelper.RemoveMarker(markername);
        Reply(removed ? "Marker successfully removed" : $"Marker with name {markername} not found");
    }
    
    [ChatCommand("listplacedmarkers", "lists all placed markers")]
    [Admin]
    internal void ListPlacedMarkersCommand()
    {
        Reply("Current custom markers are:");
        foreach (var kv in JsonConfigHelper.GetMarkers())
        {
            Reply($"Name: {kv.Key} Type: {kv.Value.Type} Location: {kv.Value.X}, {kv.Value.Y}");
        }
    }

}
