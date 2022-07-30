
using System.Threading.Tasks;
using BepInEx.Logging;
using Discord;
using SkanksAIO.Utils;

namespace SkanksAIO.Logger.Handler;

class DiscordLogHandler : ILogHandler
{
    public bool Accept(LogLevel level)
    {
        return level <= LogLevel.Fatal;
    }

    public async Task Handle(LogLevel level, string message)
    {
        var builder = DiscordUtils.CreateEmbedBuilder(level.ToString())
            .WithDescription(message);

        switch (level) {
            case LogLevel.Debug:
                builder.WithColor(new Color(0, 0, 255));
                break;
            case LogLevel.Warning:
                builder.WithColor(new Color(255, 255, 0));
                break;
            case LogLevel.Fatal:
            case LogLevel.Error:
                builder.WithColor(new Color(255, 0, 0));
                break;
            case LogLevel.Info:
            case LogLevel.Message:
                builder.WithColor(new Color(0, 255, 0));
                break;
            default:
                builder.WithColor(new Color(255, 255, 255));
                break;
        }

        await App.Instance!.Discord.SendMessageAsync(embed: builder.Build());
    }
}
