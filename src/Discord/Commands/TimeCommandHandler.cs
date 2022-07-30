using Discord;
using Discord.WebSocket;
using ProjectM;
using ProjectM.Gameplay.Systems;
using SkanksAIO.Discord.Attributes;
using SkanksAIO.Utils;
using System.Threading.Tasks;

namespace SkanksAIO.Discord.Commands;

[DiscordCommandHandler]
internal class TimeCommandHandler
{
    [DiscordCommand("time", "Show the current time on the server")]
    internal async Task Time(SocketSlashCommand ctx)
    {
        var builder = DiscordUtils.CreateEmbedBuilder("Time")
            .WithColor(0x18, 0xf7, 0xf7);

        var em = WorldUtility.FindWorld("Server").EntityManager;
        var gameplaySystem = WorldUtility.FindWorld("Server").GetExistingSystem<HandleGameplayEventsSystem>();
        var access = gameplaySystem._DayNightCycle.GetSingletonEntity();
        var dnc = em.GetComponentData<DayNightCycle>(access);
        var now = dnc.GameDateTimeNow;

        var year = string.Format("{0:0000}", now.Year);
        var month = string.Format("{0:00}", now.Month);
        var day = string.Format("{0:00}", now.Day);
        var hour = string.Format("{0:00}", now.Hour);
        var minute = string.Format("{0:00}", now.Minute);

        var dayStartHour = dnc.DayTimeStartInSeconds / (dnc.DayDurationInSeconds / 24);
        var dayEndHour = dayStartHour + (dnc.DayTimeDurationInSeconds / (dnc.DayDurationInSeconds / 24));

        bool isBMD = dnc.IsBloodMoonDay();
        string isDay = dnc.TimeOfDay == TimeOfDay.Day ? "Day" : "Night";

        builder.AddField($"The current time on {Plugin.Instance!.MessageTitle.Value} is: ", $"**{hour}**:**{minute}** - **[{isDay}]**");

        await ctx.RespondAsync(embed: builder.Build());
    }
}

