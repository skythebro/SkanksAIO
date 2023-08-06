using Discord;
using SkanksAIO.Utils.Config;

namespace SkanksAIO.Utils;

internal static class DiscordUtils
{

    internal static EmbedBuilder CreateEmbedBuilder(string title)
    {
        var builder = new EmbedBuilder();

        builder.WithTitle($"**{Settings.MessageTitle.Value} - {title}**");

        if (!string.IsNullOrEmpty(Settings.MessageFooter.Value) || !string.IsNullOrEmpty(Settings.MessageFooterIcon.Value))
        {
            builder.WithFooter((footer) =>
            {
                if (!string.IsNullOrEmpty(Settings.MessageFooter.Value))
                    footer.WithText(Settings.MessageFooter.Value);

                if (!string.IsNullOrEmpty(Settings.MessageFooterIcon.Value))
                    footer.WithIconUrl(Settings.MessageFooterIcon.Value);
            });
        }

        return builder;
    }

}