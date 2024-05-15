using Discord;
using Discord.WebSocket;
using ProjectM;
using ProjectM.Gameplay.Systems;
using SkanksAIO.Discord.Attributes;
using SkanksAIO.Utils;
using System.Threading.Tasks;
using Bloodstone.API;
using ProjectM.Scripting;
using SkanksAIO.Utils.Config;

namespace SkanksAIO.Discord.Commands;

[DiscordCommandHandler]
internal class TimeCommandHandler
{
    [DiscordCommand("time", "Show the current time on the server")]
    internal async Task Time(SocketSlashCommand ctx)
    {
        var builder = DiscordUtils.CreateEmbedBuilder("Time")
            .WithColor(0x18, 0xf7, 0xf7);

        var em = VWorld.Server.EntityManager;
        var dayNightCycle = VWorld.Server.GetExistingSystemManaged<ServerScriptMapper>()._ServerGameManager
            .DayNightCycle;
        var now = dayNightCycle.GameDateTimeNow;

        var year = $"{now.Year:0000}";
        var month = $"{now.Month:00}";
        var day = $"{now.Day:00}";
        var hour = $"{now.Hour:00}";
        var minute = $"{now.Minute:00}";

        var dayStartHour = dayNightCycle.DayTimeStartInSeconds / (dayNightCycle.DayDurationInSeconds / 24);
        var dayEndHour = dayStartHour + (dayNightCycle.DayTimeDurationInSeconds / (dayNightCycle.DayDurationInSeconds / 24));

        string isBmd = dayNightCycle.IsBloodMoonDay() ? "BloodMoon" : "No BloodMoon";
        string isDay = dayNightCycle.TimeOfDay == TimeOfDay.Day ? "Day" : "Night";

        builder.AddField($"The current time on {Settings.MessageTitle.Value} is: ", $"**{hour}**:**{minute}** - **[{isDay}]** - **[{isBmd}]**");

        await ctx.RespondAsync(embed: builder.Build());
    }
}

