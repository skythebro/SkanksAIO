using Discord;
using Discord.WebSocket;
using ProjectM;
using SkanksAIO.Discord.Attributes;
using SkanksAIO.Extensions;
using SkanksAIO.Models;
using SkanksAIO.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bloodstone.API;
using SkanksAIO.Utils.Config;

namespace SkanksAIO.Discord.Commands;

[DiscordCommandHandler]
internal class LeaderboardCommandHandler
{
    [DiscordCommand("leaderboard", "Check the leaderboard")]
    internal async Task Leaderboard(SocketSlashCommand ctx, string PlayerName = "")
    {
        var bootstrapSystem = VWorld.Server.GetExistingSystem<ServerBootstrapSystem>();

        var players = Player.GetPlayerRepository.FindAll()
            .OrderByDescending(x => x.Kills)
            .ToList();

        var builder = DiscordUtils.CreateEmbedBuilder("Leaderboard")
            .WithDescription($"**{players.Count}** Players")
            .WithColor(0x18, 0xf7, 0xf7);

        var selectedPlayer = players.FirstOrDefault(x => x.CharacterName!.ToLower() == PlayerName.ToLower());

        var startIndex = 0;
        var endIndex = 8;
        var selectedPlayerRank = 0;
        var bold = "";

        if (selectedPlayer != null)
        {
            selectedPlayerRank = players.IndexOf(selectedPlayer);
            var selectedPlayerIndex = Math.Clamp(selectedPlayerRank, 4, Math.Max(4, players.Count - 5));
            bold = "**";
            startIndex = selectedPlayerIndex - 4;
            endIndex = selectedPlayerIndex + 4;

            builder.WithDescription($"{bold}{selectedPlayer.CharacterName}{bold} is ranked {bold}{(selectedPlayerRank + 1)}{bold}{(selectedPlayerRank + 1).Ordinal()} /w KD: {selectedPlayer.KD.ToString("0.00")} & ELO: {selectedPlayer.ELO}");
        }

        bold = "";
        if (!Settings.LeaderboardAsList.Value)
        {
            for (var i = startIndex; i <= endIndex; i++)
            {
                if (i < 0 || i >= players.Count) continue;
                var player = players[i];

                bold = (selectedPlayer != null && (selectedPlayerRank) == i) ? "**" : "";

                builder.AddField(
                    $"**{(i + 1)}**{(i + 1).Ordinal()} {bold}{player.CharacterName}{bold}",
                    $"{bold}Kills: {player.Kills}\nKD: {player.KD:0.00}\nELO: {bold}{player.ELO}",
                    true
                );
            }

            await ctx.RespondAsync(embed: builder.Build());
            return;
        }

        var names = new List<string>();
        var kills = new List<string>();
        var elos = new List<string>();

        for (var i = startIndex; i <= endIndex; i++)
        {
            if (i < 0 || i >= players.Count) continue;
            var player = players[i];

            bold = (selectedPlayer != null && (selectedPlayerRank) == i) ? "**" : "";

            names.Add($"**{(i + 1)}**{(i + 1).Ordinal()} {bold}{player.CharacterName}{bold}");
            kills.Add($"{bold}{player.Kills} ({player.KD.ToString("0.00")}){bold}");
            elos.Add($"{bold}{player.ELO}{bold}");
        }

        if (players.Count > 0)
        {
            builder.AddField("Name", string.Join("\n", names), true);
            builder.AddField("Kills (KDA)", string.Join("\n", kills), true);
            builder.AddField("Elo", string.Join("\n", elos), true);
        }

        await ctx.RespondAsync(embed: builder.Build());
    }
}
