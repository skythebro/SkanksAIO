using System;
using System.Collections.Generic;
using Discord;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using SkanksAIO.Models;
using SkanksAIO.Utils;
using Unity.Collections;
using Unity.Entities;

namespace SkanksAIO.Patches;

[HarmonyPatch]
public static class KillDeath_Patches
{
    [HarmonyPatch(typeof(StatChangeSystem), nameof(StatChangeSystem.ApplyHealthChangeToEntity))]
    [HarmonyPostfix]
    public static void OnApplyHealthChangeToEntity_Patch(StatChangeSystem __instance, Entity statChangeEntity, StatChangeEvent statChange, EntityCommandBufferSafe commandBuffer, double currentTime)
    {
        if (__instance._DamageTakenEventQuery == null) return;

        var em = __instance.EntityManager;
        if (!em.HasComponent<EntityOwner>(statChange.Source)) return;

        var attacker = em.GetComponentData<EntityOwner>(statChange.Source).Owner;

        if (!em.HasComponent<PlayerCharacter>(attacker)) return;

        var attackingPlayerCharacter = em.GetComponentData<PlayerCharacter>(attacker);
        var attackingUser = em.GetComponentData<User>(attackingPlayerCharacter.UserEntity._Entity);

        var damageTakenEvents = __instance._DamageTakenEventQuery.ToComponentDataArray<DamageTakenEvent>(Allocator.Temp);

        foreach (var damageTakenEvent in damageTakenEvents)
        {
            var target = damageTakenEvent.Entity;

            if (!em.HasComponent<PlayerCharacter>(target))
            {
                continue; // the target is not a player
            }

            var targetPlayerCharacter = em.GetComponentData<PlayerCharacter>(target);
            var targetUser = em.GetComponentData<User>(targetPlayerCharacter.UserEntity._Entity);
            var targetHealth = em.GetComponentData<Health>(target);

            if (!targetUser.IsConnected) continue; // don't be a cunt!
            if (targetHealth.Value > 0) continue;   // target not dead
            if (attackingUser.CharacterName.ToString() == targetUser.CharacterName.ToString()) return; // don't record suicides

            var attackingPlayer = Player.GetRepository.FindOne(x => x.PlatformId == attackingUser.PlatformId);
            var targetPlayer = Player.GetRepository.FindOne(x => x.PlatformId == targetUser.PlatformId);

            attackingPlayer.Kills++;
            targetPlayer.Deaths++;

            // calculate new ELO for both players
            CalculateElo(attackingPlayer, targetPlayer, out var newAttackerElo, out var newTargetElo);

            attackingPlayer.ELO = newAttackerElo;
            targetPlayer.ELO = newTargetElo;

            Plugin.Logger?.LogInfo($"{attackingPlayer.CharacterName} ({attackingPlayer.ELO}) killed {targetPlayer.CharacterName} ({targetPlayer.ELO})");

            var embedMessage = BuildEmbedMessage(attacker, target);
          
            App.Instance!.Discord.SendMessageAsync($"**{attackingPlayer.CharacterName}** killed **{targetPlayer.CharacterName}**").GetAwaiter().GetResult();

            if (!Player.GetRepository.Update(attackingPlayer))
                Plugin.Logger?.LogError($"Failed to update {attackingPlayer.CharacterName}");

            if (!Player.GetRepository.Update(targetPlayer))
                Plugin.Logger?.LogError($"Failed to update {targetPlayer.CharacterName}");
        }
    }

    private static void CalculateElo(Player attacker, Player target, out int newAttackerElo, out int newTargetElo)
    {
        float score = 1f / (1f + (float)Math.Pow(10, (Math.Abs(target.ELO - attacker.ELO) / 400f)));

        newAttackerElo = (int)Math.Round(attacker.ELO + 20 * (1 - score));
        newTargetElo = (int)Math.Round(target.ELO - 20 * (1 - score));

        if (target.ELO < attacker.ELO)
        {
            newAttackerElo = (int)Math.Round(attacker.ELO - 20 * (0 - score));
            newTargetElo = (int)Math.Round(target.ELO + 20 * (0 - score));
        }
    }

    private static EmbedBuilder BuildEmbedMessage(Entity Winner, Entity Loser)
    {
        var builder = DiscordUtils.CreateEmbedBuilder("PvP Duel Results")
            .WithColor(new Color(0, 255, 0));

        var winnerUnitLevel = Plugin.World!.EntityManager.GetComponentData<UnitLevel>(Winner);
        var loserUnitLevel = Plugin.World!.EntityManager.GetComponentData<UnitLevel>(Loser);

        var winnerUser = Plugin.World!.EntityManager.GetComponentData<User>(Winner);
        var loserUser = Plugin.World!.EntityManager.GetComponentData<User>(Loser);

        var winnerHealthComponent = Plugin.World!.EntityManager.GetComponentData<Health>(Winner);
        var winnerHealthPercentage = winnerHealthComponent.Value / winnerHealthComponent.MaxHealth;

        var winnerName = winnerUser.CharacterName;
        var loserName = loserUser.CharacterName;

        List<string> messages = new List<string>();

        if (winnerHealthPercentage < 10f)
        { // Winner has less than 10% health left.
            messages.Add($"An extremely close victory was achived by **{winnerName}** over **{loserName}**!");

        }
        else if (winnerHealthPercentage > 75f)
        { // Winner has more than 75% health left.
            messages.Add($"With an overwhelming victory, **{winnerName}** has defeated **{loserName}**!");
            messages.Add($"**{winnerName}** slaughtered **{loserName}**!");

        }
        else if (winnerUnitLevel.Level <= loserUnitLevel.Level - 10)
        { // Winner gearscore is at least 10 lower than Loser.
            messages.Add($"**Against all odds, **{winnerName}** has defeated **{loserName}**!");

        }
        else if (winnerUnitLevel.Level >= loserUnitLevel.Level + 10)
        { // Winner gearscore is at least 10 levels higher than Loser.
            messages.Add($"**{loserName}** never stood a chance against **{winnerName}**!");

        }
        else
        { // Generic victory messages.
            messages.Add($"**{winnerName}** is victorious over **{loserName}**!");
            messages.Add($"**{winnerName}** defeated **{loserName}**, **{loserName}** needs to pick up the slack!");
            messages.Add($"**{loserName}** is counting sheep at the blade of **{winnerName}**!");

        }

        // Random messages from messages
        var randomMessage = messages[UnityEngine.Random.Range(0, messages.Count)];
        builder.WithDescription(randomMessage);

        // needs character name and gearscore and maybe remaining health
        builder.AddField("Winner", $"{winnerName} ({winnerUnitLevel.Level})", true);
        builder.AddField("Loser", $"{loserName} ({loserUnitLevel.Level})", true);

        return builder;
    }

}