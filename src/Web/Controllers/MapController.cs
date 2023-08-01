using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using SkanksAIO.Models;
using SkanksAIO.Utils;
using UnityEngine;

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
            RANDOMTEST = "test"
        };
    }

    [Route("/map-data-player", Methods: new[] { "GET" })]
    public dynamic MapDataAction(HttpListenerRequest request)
    {
        try
        {
            var playerPositions = UserUtils.GetAllPlayerPositions();

            //Plugin.Logger?.LogWarning("There are " + playerPositions.Count + " players online! To track");
            if (playerPositions.Count < 1)
            {
                playerPositions.Add(new PlayerLocation()
                {
                    Name = "No_Players_Online",
                    X = 4000,
                    Y = 800,
                });
            }

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