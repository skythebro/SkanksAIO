using System.Threading.Tasks;
using Bloodstone.API;
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
    internal async Task KickCommand(SocketSlashCommand ctx, string playerName)
    {
        if (!UserUtils.TryGetUserByCharacterName(playerName, out var user))
        {
            Plugin.Logger?.LogInfo($"{playerName} was not found.");
            await ctx.RespondAsync($"{playerName} was not found.");
            return;
        }

        if (!user.IsConnected)
        {
            await ctx.RespondAsync($"{user.CharacterName} is not online.");
            return;
        }

        var bootstrapSystem = VWorld.Server.GetExistingSystemManaged<ServerBootstrapSystem>();

        bootstrapSystem.Kick(user.PlatformId, ConnectionStatusChangeReason.Kicked, false);

        Plugin.Logger?.LogInfo($"{user.CharacterName} has been kicked by {ctx.User.Username}.");
        await ctx.RespondAsync($"{user.CharacterName} has been kicked.");
    }

    [DiscordCommand("ban", "Bans an in-game player")]
    internal async Task BanCommand(SocketSlashCommand ctx, string playerName)
    {
        if (!UserUtils.TryGetUserByCharacterName(playerName, out var user))
        {
            Plugin.Logger?.LogInfo($"{playerName} was not found.");
            await ctx.RespondAsync($"{playerName} was not found.");
            return;
        }

        var em = VWorld.Server.EntityManager;

        var entityEvent = em.CreateEntity(
            Unity.Entities.ComponentType.ReadOnly<NetworkEventType>(),
            Unity.Entities.ComponentType.ReadOnly<BanEvent>()
        );

        entityEvent.WithComponentData((ref BanEvent banEvent) =>
        {
            banEvent.PlatformId = user!.PlatformId;
            banEvent.Unban = false;
        });
        Plugin.Logger?.LogInfo($"{user.CharacterName} has been banned by {ctx.User.Username}.");
        await ctx.RespondAsync($"{user.CharacterName} has been banned.");
    }

    [DiscordCommand("unban", "Unbans an in-game player")]
    internal async Task UnbanCommand(SocketSlashCommand ctx, string playerName)
    {
        if (!UserUtils.TryGetUserByCharacterName(playerName, out var user))
        {
            Plugin.Logger?.LogInfo($"{playerName} was not found.");
            await ctx.RespondAsync($"{playerName} was not found.");
            return;
        }

        var em = VWorld.Server.EntityManager;

        var entityEvent = em.CreateEntity(
            Unity.Entities.ComponentType.ReadOnly<NetworkEventType>(),
            Unity.Entities.ComponentType.ReadOnly<BanEvent>()
        );

        entityEvent.WithComponentData((ref BanEvent banEvent) =>
        {
            banEvent.PlatformId = user.PlatformId;
            banEvent.Unban = true;
        });

        Plugin.Logger?.LogInfo($"{user.CharacterName} has been unbanned by {ctx.User.Username}.");
        await ctx.RespondAsync($"{user.CharacterName} has been unbanned.");
    }
}