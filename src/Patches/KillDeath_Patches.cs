using System;
using System.Collections.Generic;
using Bloodstone.API;
using Discord;
using HarmonyLib;
using ProjectM;
using ProjectM.Gameplay.Systems;
using ProjectM.Network;
using SkanksAIO.Models;
using SkanksAIO.Utils;
using SkanksAIO.Utils.Config;
using Unity.Collections;
using Unity.Entities;

namespace SkanksAIO.Patches;

[HarmonyPatch]
public static class KillDeath_Patches
{
    [HarmonyPatch(typeof(DeathEventListenerSystem), nameof(DeathEventListenerSystem.OnUpdate))]
    [HarmonyPostfix]
    public static void OnUpdate(DeathEventListenerSystem __instance)
    {
       
        if (!Settings.EnablePvPKillTracker.Value || __instance._DeathEventQuery.IsEmpty) return;
        
        var em = __instance.EntityManager;
        
        NativeArray<DeathEvent> deathEvents =
            __instance._DeathEventQuery.ToComponentDataArray<DeathEvent>(Allocator.Temp);
        foreach (DeathEvent deathEvent in deathEvents)
        {
            if (!em.TryGetComponentData<PlayerCharacter>(deathEvent.Died, out var targetPlayerCharacter)) continue;
            if (!em.TryGetComponentData<PlayerCharacter>(deathEvent.Killer, out var attackingPlayerCharacter)) continue;
            if (!em.HasComponent<Equipment>(deathEvent.Died)) continue;
            if (!em.HasComponent<Equipment>(deathEvent.Killer)) continue;
            if (!em.HasComponent<Blood>(deathEvent.Died)) continue;
            if (!em.HasComponent<Blood>(deathEvent.Killer)) continue;
            em.TryGetComponentData<User>(attackingPlayerCharacter.UserEntity, out var attackingUser);
            em.TryGetComponentData<User>(targetPlayerCharacter.UserEntity, out var targetUser);
            var target = deathEvent.Died;
            var killer = deathEvent.Killer;
            
            if (!targetUser.IsConnected) continue; // don't be a cunt!
            if (attackingUser.CharacterName.ToString() == targetUser.CharacterName.ToString()) return; // don't record suicides

            var attackingPlayer = Player.GetPlayerRepository.FindOne(x => x.PlatformId == attackingUser.PlatformId);
            var targetPlayer = Player.GetPlayerRepository.FindOne(x => x.PlatformId == targetUser.PlatformId);

            attackingPlayer.Kills++;
            targetPlayer.Deaths++;

            // calculate new ELO for both players
            CalculateElo(attackingPlayer, targetPlayer, out var newAttackerElo, out var newTargetElo);

            attackingPlayer.ELO = newAttackerElo;
            targetPlayer.ELO = newTargetElo;

            Plugin.Logger?.LogInfo($"{attackingPlayer.CharacterName} ({attackingPlayer.ELO}) killed {targetPlayer.CharacterName} ({targetPlayer.ELO})");

            var embedMessage = BuildEmbedMessage(killer, target);
            
            App.Instance!.Discord!.SendMessageAsync($"**{attackingPlayer.CharacterName}** killed **{targetPlayer.CharacterName}**").GetAwaiter().GetResult();
            
            App.Instance!.Discord!.SendMessageAsync("" , false , embedMessage.Build()).GetAwaiter().GetResult();
            if (!Player.GetPlayerRepository.Update(attackingPlayer))
                Plugin.Logger?.LogError($"Failed to update {attackingPlayer.CharacterName}");

            if (!Player.GetPlayerRepository.Update(targetPlayer))
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

        var em = VWorld.Server.EntityManager;
        
        em.TryGetComponentData<Equipment>(Winner, out var winnerEquipment);
        em.TryGetComponentData<Equipment>(Loser, out var loserEquipment);
        
        // Need to look into this method: *Equipment.GetFullLevel()
        var winnerEquipmentLevel =  winnerEquipment.ArmorLevel+ winnerEquipment.WeaponLevel+ winnerEquipment.SpellLevel;
        var loserEquipmentLevel = loserEquipment.ArmorLevel + loserEquipment.WeaponLevel + loserEquipment.SpellLevel;
        
        em.TryGetComponentData<User>(Winner, out var winnerUser);
        em.TryGetComponentData<User>(Loser, out var loserUser);
        
        em.TryGetComponentData<Health>(Winner, out var winnerHealthComponent);
        
        var winnerHealthPercentage = winnerHealthComponent.Value / winnerHealthComponent.MaxHealth;
        var winnerName = winnerUser.CharacterName;
        var loserName = loserUser.CharacterName;

        var messages = new List<string>();

        switch (winnerHealthPercentage)
        {
            case < 10f: // Winner has less than 10% health left.
                messages.Add($"An extremely close victory was achieved by **{winnerName}** over **{loserName}**!");
                break;
            case > 75f: // Winner has more than 75% health left.
                messages.Add($"With an overwhelming victory, **{winnerName}** has defeated **{loserName}**!");
                messages.Add($"**{winnerName}** slaughtered **{loserName}**!");
                break;
            default:
            {
                if (winnerEquipmentLevel <= loserEquipmentLevel - 10f)
                { // Winner gearscore is at least 10 lower than Loser.
                    messages.Add($"**Against all odds, **{winnerName}** has defeated **{loserName}**!");

                }
                else if (winnerEquipmentLevel >= loserEquipmentLevel + 10f)
                { // Winner gearscore is at least 10 levels higher than Loser.
                    messages.Add($"**{loserName}** never stood a chance against **{winnerName}**!");

                }
                else
                { // Generic victory messages.
                    messages.Add($"**{winnerName}** is victorious over **{loserName}**!");
                    messages.Add($"**{winnerName}** defeated **{loserName}**, **{loserName}** needs to pick up the slack!");
                    messages.Add($"**{loserName}** is counting sheep at the blade of **{winnerName}**!");

                }

                break;
            }
        }

        // Random messages from messages
        var randomMessage = messages[UnityEngine.Random.Range(0, messages.Count)];
        builder.WithDescription(randomMessage);

        // needs character name and gearscore and maybe remaining health
        builder.AddField("Winner", $"{winnerName} ({winnerEquipmentLevel:F1})", true);
        builder.AddField("Loser", $"{loserName} ({loserEquipmentLevel:F1})", true);

        return builder;
    }

}