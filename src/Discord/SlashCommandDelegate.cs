using System.Threading.Tasks;
using Discord.WebSocket;

namespace SkanksAIO.Discord;

public class SlashCommandDelegate
{
    public delegate Task SlashCommandExecutedHandler(SocketSlashCommand command);
}