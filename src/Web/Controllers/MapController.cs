using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using SkanksAIO.Models;
using SkanksAIO.Utils;
using SkanksAIO.Utils.Config;

namespace SkanksAIO.Web.Controllers;

[Controller]
public class MapController
{
    [Route("/map", Methods: new[] { "GET" })]
    [Template("Map.html.twig")]
    public dynamic MapAction(HttpListenerRequest request)
    {
        return new
        {
            NONE = ""
        };
    }
    
    [Route("/map-data-CustomMarkers", Methods: new[] { "GET" })]
    public dynamic CustomMarkersDataAction(HttpListenerRequest request)
    {
        try
        {
            var markers = JsonConfigHelper.GetMarkers();
            
            return CustomMarkersToJson(markers);
        }
        catch (Exception e)
        {
            return "";
        }
    }

    [Route("/map-data-Territories", Methods: new[] { "GET" })]
    public dynamic TerritoryDataAction(HttpListenerRequest request)
    {
        try
        {
            var territoryPositions = UserUtils.GetPlayerTerritory();
            
            return TerritoryPositionToJson(territoryPositions);
        }
        catch (Exception e)
        {
            return "";
        }
    }

    [Route("/map-data-player", Methods: new[] { "GET" })]
    public dynamic PlayerDataAction(HttpListenerRequest request)
    {
        try
        {
            if (!Settings.TrackPlayersOnMap.Value)
            {
                var json = new StringBuilder();
                
                json.Append('{');
                json.Append("\"Name\":\"EMPTYSKANKSERVER\",");
                json.Append("\"X\":0,");
                json.Append("\"Y\":0");
                json.Append('}');

                return json.ToString();
            }

            var playerPositions = UserUtils.GetAllPlayerPositions();

            // Return the JSON data as Content with application/json content type
            return PlayerPositionToJson(playerPositions);
        }
        catch (Exception e)
        {
            //Plugin.Logger?.LogWarning("Something went wrong while trying to fetch the interactive map position data!" + e.Message);
            //Plugin.Logger?.LogWarning("Stack Trace: " + e.StackTrace);
            return "";
        }
    }

    [Route("/map-update", Methods: new[] { "GET" })]
    public dynamic UpdateIntervalAction(HttpListenerRequest request)
    {
        var response = Settings.InteractiveMapUpdateInterval.Value.ToString(); // Convert the int to a string
        return response;
    }

    [Route("map-data-regions", Methods: new[] { "GET" })]
    public dynamic RegionDataAction(HttpListenerRequest request)
    {
        try
        {
            return "";
        }
        catch (Exception e)
        {
            return "";
        }
    }
    
    
    private string TerritoryPositionToJson(Dictionary<string, UserUtils.TerritoryData> territoryPositions)
    {
        var json = new StringBuilder();

        json.Append('[');

        foreach (var entry in territoryPositions)
        {
            json.Append('{');

            json.Append($"\"Territory\":\"{entry.Key}\",");
            json.Append($"\"PlayerCount\":{entry.Value.PlayerCount},");
            json.Append($"\"X\":{entry.Value.X.ToString(CultureInfo.InvariantCulture).Replace(',', '.')},");
            json.Append($"\"Y\":{entry.Value.Y.ToString(CultureInfo.InvariantCulture).Replace(',', '.')},");
            json.Append($"\"BX\":{entry.Value.BX.ToString(CultureInfo.InvariantCulture).Replace(',', '.')},");
            json.Append($"\"BY\":{entry.Value.BY.ToString(CultureInfo.InvariantCulture).Replace(',', '.')},");
            json.Append($"\"TX\":{entry.Value.TX.ToString(CultureInfo.InvariantCulture).Replace(',', '.')},");
            json.Append($"\"TY\":{entry.Value.TY.ToString(CultureInfo.InvariantCulture).Replace(',', '.')}");

            json.Append('}');
            json.Append(',');
        }

        // Remove the last comma if there is any entry
        if (territoryPositions.Count > 0)
        {
            json.Length--; // Remove the last character (the comma)
        }

        json.Append(']');

        return json.ToString();
    }
    
    private string CustomMarkersToJson(Dictionary<string,JsonConfigHelper.MarkerLocationData> markerPositions)
    {
        var json = new StringBuilder();

        json.Append('[');

        foreach (var entry in markerPositions)
        {
            json.Append('{');

            json.Append($"\"MarkerName\":\"{entry.Key}\",");
            json.Append($"\"MarkerType\":\"{entry.Value.Type.ToString()}\",");
            json.Append($"\"X\":{entry.Value.X.ToString(CultureInfo.InvariantCulture).Replace(',', '.')},");
            json.Append($"\"Y\":{entry.Value.Y.ToString(CultureInfo.InvariantCulture).Replace(',', '.')}");

            json.Append('}');
            json.Append(',');
        }

        // Remove the last comma if there is any entry
        if (markerPositions.Count > 0)
        {
            json.Length--; // Remove the last character (the comma)
        }

        json.Append(']');

        return json.ToString();
    }
    
    private string PlayerPositionToJson(List<PlayerLocation> playerPositions)
    {
        var json = new StringBuilder();

        json.Append('[');

        for (int i = 0; i < playerPositions.Count; i++)
        {
            var playerPosition = playerPositions[i];
            json.Append('{');


            json.Append($"\"Name\":\"{playerPosition.Name}\",");
            json.Append($"\"X\":{playerPosition.X.ToString(CultureInfo.InvariantCulture).Replace(',', '.')},");
            json.Append($"\"Y\":{playerPosition.Z.ToString(CultureInfo.InvariantCulture).Replace(',', '.')}");
            //Plugin.Logger?.LogWarning($"Name: {playerPosition.Name} X: {playerPosition.X.ToString(CultureInfo.InvariantCulture).Replace(',', '.')} Y: {playerPosition.Z.ToString(CultureInfo.InvariantCulture).Replace(',', '.')}");
            json.Append('}');

            if (i < playerPositions.Count - 1)
            {
                json.Append(',');
            }
        }

        json.Append(']');

        return json.ToString();
    }
}