using Discord;

namespace SkanksAIO.Utils;

internal static class DiscordUtils
{

    internal static EmbedBuilder CreateEmbedBuilder(string title)
    {
        var builder = new EmbedBuilder();

        builder.WithTitle($"**{Plugin.Instance!.MessageTitle.Value} - {title}**");

        if (!string.IsNullOrEmpty(Plugin.Instance!.MessageFooter.Value) || !string.IsNullOrEmpty(Plugin.Instance!.MessageFooterIcon.Value))
        {
            builder.WithFooter((footer) =>
            {
                if (!string.IsNullOrEmpty(Plugin.Instance!.MessageFooter.Value))
                    footer.WithText(Plugin.Instance!.MessageFooter.Value);

                if (!string.IsNullOrEmpty(Plugin.Instance!.MessageFooterIcon.Value))
                    footer.WithIconUrl(Plugin.Instance!.MessageFooterIcon.Value);
            });
        }

        return builder;
    }

}