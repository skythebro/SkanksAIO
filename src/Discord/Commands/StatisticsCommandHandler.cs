using Discord;
using Discord.WebSocket;
using SkanksAIO.Discord.Attributes;
using SkanksAIO.Models;
using SkanksAIO.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SkanksAIO.Discord.Commands;

[DiscordCommandHandler]
internal class StatisticsCommandHandler
{
    [DiscordCommand("status", "Check the current server status")]
    internal async Task StatusCommand(SocketSlashCommand ctx)
    {
        // Prepare the EmbedBuilder response
        var builder = DiscordUtils.CreateEmbedBuilder("Server Statistics")
            .WithColor(0x18, 0xf7, 0xf7);

        // Get the servers uptime
        var uptime = DateTime.Now - Plugin.ServerStartTime;
        var formattedUptime = $"{uptime.Days} days, {uptime.Hours} hours, {uptime.Minutes} minutes, {uptime.Seconds} seconds";

        builder.AddField("Server Uptime", formattedUptime, false);

        // Get the amount of online players
        var onlineUsers = UserUtils.GetOnlineUsers();
        var onlineCount = onlineUsers.Count();

        builder.AddField("Online Players", onlineCount.ToString(), true);

        // Total PVP Kills
        var pvpKills = Player.GetRepository.FindAll()
            .Sum(x => x.Kills);
        
        builder.AddField("Total PVP Kills", pvpKills.ToString(), true);

        // Average ELO Score
        var averageElo = (int)Math.Round(Player.GetRepository.FindAll()
            .Average(x => x.ELO));

        builder.AddField("Average Player Rating", averageElo.ToString(), true); // should this not always evaluate to 1000?

        await ctx.RespondAsync(embed: builder.Build());
    }
}
