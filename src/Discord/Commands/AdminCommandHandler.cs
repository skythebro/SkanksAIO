using System.Threading.Tasks;
using Discord.WebSocket;
using ProjectM;
using ProjectM.Network;
using SkanksAIO.Discord.Attributes;
using SkanksAIO.Utils;
using Stunlock.Network;

namespace SkanksAIO.Discord.Commands;

[DiscordCommandHandler]
[Admin]
class AdminCommandHandler
{
    [DiscordCommand("kick", "Kick an in-game player")]
    internal async Task KickCommand(SocketSlashCommand ctx, string PlayerName)
    {
        if (!UserUtils.TryGetUserByCharacterName(PlayerName, out var user)) {
            Plugin.Logger?.LogInfo($"{PlayerName} was not found.");
            await ctx.RespondAsync($"{PlayerName} was not found.");
            return;
        }

        if (!user!.IsConnected)
        {
            await ctx.RespondAsync($"{user.CharacterName} is not online.");
            return;
        }

        var bootstrapSystem = Plugin.World.GetExistingSystem<ServerBootstrapSystem>();

        bootstrapSystem.Kick(user.PlatformId, ConnectionStatusChangeReason.Kicked, false);

        Plugin.Logger?.LogInfo($"{user.CharacterName} has been kicked by {ctx.User.Username}.");
        await ctx.RespondAsync($"{user.CharacterName} has been kicked.");
    }

    [DiscordCommand("ban", "Bans an in-game player")]
    internal async Task BanCommand(SocketSlashCommand ctx, string PlayerName)
    {
        if(!UserUtils.TryGetUserByCharacterName(PlayerName, out var user))
        {
            Plugin.Logger?.LogInfo($"{PlayerName} was not found.");
            await ctx.RespondAsync($"{PlayerName} was not found.");
            return;
        }

        var em = Plugin.World.EntityManager;

        var entityEvent = em.CreateEntity(
             Unity.Entities.ComponentType.ReadOnly<NetworkEventType>(),
             Unity.Entities.ComponentType.ReadOnly<BanEvent>()
        );

        em.SetComponentData(entityEvent, new BanEvent()
        {
            PlatformId = user!.PlatformId,
            Unban = false
        });

        Plugin.Logger?.LogInfo($"{user.CharacterName} has been banned by {ctx.User.Username}.");
        await ctx.RespondAsync($"{user.CharacterName} has been banned.");
    }

    [DiscordCommand("unban", "Unbans an in-game player")]
    internal async Task UnbanCommand(SocketSlashCommand ctx, string PlayerName)
    {
        if (!UserUtils.TryGetUserByCharacterName(PlayerName, out var user))
        {
            Plugin.Logger?.LogInfo($"{PlayerName} was not found.");
            await ctx.RespondAsync($"{PlayerName} was not found.");
            return;
        }

        var em = Plugin.World.EntityManager;

        var entityEvent = em.CreateEntity(
                        Unity.Entities.ComponentType.ReadOnly<NetworkEventType>(),
                        Unity.Entities.ComponentType.ReadOnly<BanEvent>()
                    );

        em.SetComponentData(entityEvent, new BanEvent()
        {
            PlatformId = user!.PlatformId,
            Unban = true
        });

        Plugin.Logger?.LogInfo($"{user.CharacterName} has been unbanned by {ctx.User.Username}.");
        await ctx.RespondAsync($"{user.CharacterName} has been unbanned.");
    }
}