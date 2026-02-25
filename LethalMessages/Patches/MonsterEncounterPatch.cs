using com.github.luckofthelefty.LethalMessages.Messages;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using Unity.Netcode;

namespace com.github.luckofthelefty.LethalMessages.Patches;

internal static class MonsterEncounterPatch
{
    // Cooldown tracking: prevent spamming the same encounter message
    // Key = NetworkObjectId, Value = last message time (Unity time)
    private static readonly Dictionary<ulong, float> _cooldowns = new Dictionary<ulong, float>();
    private const float CooldownSeconds = 15f;

    private static bool IsOnCooldown(ulong networkObjectId)
    {
        if (_cooldowns.TryGetValue(networkObjectId, out float lastTime))
        {
            if (UnityEngine.Time.time - lastTime < CooldownSeconds)
                return true;
        }
        return false;
    }

    private static void SetCooldown(ulong networkObjectId)
    {
        _cooldowns[networkObjectId] = UnityEngine.Time.time;
    }

    internal static void ResetCooldowns()
    {
        _cooldowns.Clear();
    }

    private static bool CanSendEncounter(EnemyAI enemy)
    {
        if (!ConfigManager.MonsterEncounterMessages.Value) return false;
        if (enemy == null) return false;
        if (!NetworkUtils.IsClientRpcExecution(enemy)) return false;

        var netObj = enemy.GetComponent<NetworkObject>();
        if (netObj == null) return false;

        ulong id = netObj.NetworkObjectId;
        if (!DiscoveryTracker.IsDiscovered(id)) return false;
        if (IsOnCooldown(id)) return false;

        return true;
    }

    private static void SendEncounter(EnemyAI enemy, string playerName)
    {
        string enemyName = enemy.enemyType?.enemyName ?? enemy.GetType().Name;
        string message = MonsterMessages.GetEncounterMessage(enemyName, playerName);
        if (message == null) return;

        var netObj = enemy.GetComponent<NetworkObject>();
        if (netObj != null) SetCooldown(netObj.NetworkObjectId);

        MessageSender.Send(message, MessageTier.Event);
    }

    private static string GetPlayerName(int playerObjectIndex)
    {
        if (StartOfRound.Instance?.allPlayerScripts == null
            || playerObjectIndex < 0
            || playerObjectIndex >= StartOfRound.Instance.allPlayerScripts.Length)
            return "Unknown";

        return StartOfRound.Instance.allPlayerScripts[playerObjectIndex]?.playerUsername ?? "Unknown";
    }

    private static string GetPlayerName(PlayerControllerB playerScript)
    {
        return playerScript?.playerUsername ?? "Unknown";
    }

    /// <summary>
    /// Tries to find the player an enemy is targeting/interacting with.
    /// Checks targetPlayer, inSpecialAnimationWithPlayer, and movingTowardsTargetPlayer.
    /// Falls back to nearest alive player if nothing else works.
    /// </summary>
    private static string GetTargetPlayerName(EnemyAI enemy)
    {
        // Direct target
        if (enemy.targetPlayer != null)
            return enemy.targetPlayer.playerUsername ?? "Unknown";

        // In special animation with a player (grab/kill)
        if (enemy.inSpecialAnimationWithPlayer != null)
            return enemy.inSpecialAnimationWithPlayer.playerUsername ?? "Unknown";

        // Fall back to closest alive player
        if (StartOfRound.Instance?.allPlayerScripts != null)
        {
            float closestDist = float.MaxValue;
            GameNetcodeStuff.PlayerControllerB closest = null;

            foreach (var player in StartOfRound.Instance.allPlayerScripts)
            {
                if (player == null || player.isPlayerDead || !player.isPlayerControlled) continue;

                float dist = UnityEngine.Vector3.Distance(enemy.transform.position, player.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = player;
                }
            }

            if (closest != null)
                return closest.playerUsername ?? "Unknown";
        }

        return "someone";
    }

    // --- Bracken grab (before kill) ---
    [HarmonyPatch(typeof(FlowermanAI), nameof(FlowermanAI.KillPlayerAnimationClientRpc))]
    [HarmonyPostfix]
    private static void BrackenGrab(FlowermanAI __instance, int playerObjectId)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerObjectId));
    }

    // --- Jester cranking ---
    [HarmonyPatch(typeof(JesterAI), nameof(JesterAI.SetJesterInitialValues))]
    [HarmonyPostfix]
    private static void JesterCranking(JesterAI __instance)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetTargetPlayerName(__instance));
    }

    // --- Coilhead moving ---
    [HarmonyPatch(typeof(SpringManAI), nameof(SpringManAI.SetAnimationGoClientRpc))]
    [HarmonyPostfix]
    private static void CoilheadMoving(SpringManAI __instance)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetTargetPlayerName(__instance));
    }

    // --- Masked Mimic created ---
    [HarmonyPatch(typeof(MaskedPlayerEnemy), nameof(MaskedPlayerEnemy.CreateMimicClientRpc))]
    [HarmonyPostfix]
    private static void MaskedMimic(MaskedPlayerEnemy __instance)
    {
        if (!CanSendEncounter(__instance)) return;

        string playerName = __instance.mimickingPlayer != null
            ? GetPlayerName(__instance.mimickingPlayer)
            : GetTargetPlayerName(__instance);

        SendEncounter(__instance, playerName);
    }

    // --- Snare Flea cling ---
    [HarmonyPatch(typeof(CentipedeAI), nameof(CentipedeAI.ClingToPlayerClientRpc))]
    [HarmonyPostfix]
    private static void SnareFleaCling(CentipedeAI __instance)
    {
        if (!CanSendEncounter(__instance)) return;

        string playerName = __instance.clingingToPlayer != null
            ? GetPlayerName(__instance.clingingToPlayer)
            : GetTargetPlayerName(__instance);

        SendEncounter(__instance, playerName);
    }

    // --- Spider web trip ---
    [HarmonyPatch(typeof(SandSpiderAI), nameof(SandSpiderAI.PlayerTripWebClientRpc))]
    [HarmonyPostfix]
    private static void SpiderWebTrip(SandSpiderAI __instance, int playerNum)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerNum));
    }

    // --- Forest Giant grab ---
    [HarmonyPatch(typeof(ForestGiantAI), nameof(ForestGiantAI.GrabPlayerClientRpc))]
    [HarmonyPostfix]
    private static void GiantGrab(ForestGiantAI __instance, int playerId)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerId));
    }

    // --- Thumper hit ---
    [HarmonyPatch(typeof(CrawlerAI), nameof(CrawlerAI.HitPlayerClientRpc))]
    [HarmonyPostfix]
    private static void ThumperHit(CrawlerAI __instance, int playerId)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerId));
    }

    // --- Baboon Hawk stab ---
    [HarmonyPatch(typeof(BaboonBirdAI), nameof(BaboonBirdAI.StabPlayerDeathAnimClientRpc))]
    [HarmonyPostfix]
    private static void BaboonHawkStab(BaboonBirdAI __instance, int playerObject)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerObject));
    }

    // --- Ghost Girl haunt ---
    [HarmonyPatch(typeof(DressGirlAI), nameof(DressGirlAI.ChooseNewHauntingPlayerClientRpc))]
    [HarmonyPostfix]
    private static void GhostGirlHaunt(DressGirlAI __instance)
    {
        if (!CanSendEncounter(__instance)) return;

        string playerName = __instance.hauntingPlayer != null
            ? GetPlayerName(__instance.hauntingPlayer)
            : GetTargetPlayerName(__instance);

        SendEncounter(__instance, playerName);
    }

    // --- Eyeless Dog kill (also serves as encounter since it's the chase) ---
    [HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.KillPlayerClientRpc))]
    [HarmonyPostfix]
    private static void EyelessDogEncounter(MouthDogAI __instance, int playerId)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerId));
    }

    // --- Blob contact ---
    [HarmonyPatch(typeof(BlobAI), nameof(BlobAI.SlimeKillPlayerEffectClientRpc))]
    [HarmonyPostfix]
    private static void BlobContact(BlobAI __instance, int playerKilled)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerKilled));
    }

    // --- Nutcracker shot ---
    [HarmonyPatch(typeof(NutcrackerEnemyAI), nameof(NutcrackerEnemyAI.FireGunClientRpc))]
    [HarmonyPostfix]
    private static void NutcrackerShot(NutcrackerEnemyAI __instance)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetTargetPlayerName(__instance));
    }

    // --- Butler stab ---
    [HarmonyPatch(typeof(ButlerEnemyAI), nameof(ButlerEnemyAI.StabPlayerClientRpc))]
    [HarmonyPostfix]
    private static void ButlerStab(ButlerEnemyAI __instance, int playerId)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerId));
    }

    // --- Barber (Clay Surgeon) approaching ---
    [HarmonyPatch(typeof(ClaySurgeonAI), nameof(ClaySurgeonAI.KillPlayerClientRpc))]
    [HarmonyPostfix]
    private static void BarberEncounter(ClaySurgeonAI __instance, int playerId)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerId));
    }

    // --- Maneater (Cave Dweller) ---
    [HarmonyPatch(typeof(CaveDwellerAI), nameof(CaveDwellerAI.SwitchToBehaviourClientRpc))]
    [HarmonyPostfix]
    private static void ManeaterEncounter(CaveDwellerAI __instance, int stateIndex)
    {
        if (stateIndex < 1) return;
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetTargetPlayerName(__instance));
    }

    // --- Hoarding Bug aggro ---
    [HarmonyPatch(typeof(HoarderBugAI), nameof(HoarderBugAI.SwitchToBehaviourClientRpc))]
    [HarmonyPostfix]
    private static void HoarderBugEncounter(HoarderBugAI __instance, int stateIndex)
    {
        // State 2 = aggressive/chasing
        if (stateIndex < 2) return;
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetTargetPlayerName(__instance));
    }

    // --- Earth Leviathan emerge ---
    [HarmonyPatch(typeof(SandWormAI), nameof(SandWormAI.EmergeClientRpc))]
    [HarmonyPostfix]
    private static void SandWormEmerge(SandWormAI __instance)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetTargetPlayerName(__instance));
    }

    // --- Old Bird (RadMech) targeting ---
    [HarmonyPatch(typeof(RadMechAI), nameof(RadMechAI.GrabPlayerClientRpc))]
    [HarmonyPostfix]
    private static void OldBirdGrab(RadMechAI __instance, int playerId)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerId));
    }

    // --- Old Bird shooting ---
    [HarmonyPatch(typeof(RadMechAI), nameof(RadMechAI.ShootGunClientRpc))]
    [HarmonyPostfix]
    private static void OldBirdShoot(RadMechAI __instance)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetTargetPlayerName(__instance));
    }

    // --- Circuit Bees aggro ---
    [HarmonyPatch(typeof(RedLocustBees), nameof(RedLocustBees.SwitchToBehaviourClientRpc))]
    [HarmonyPostfix]
    private static void CircuitBeesEncounter(RedLocustBees __instance, int stateIndex)
    {
        if (stateIndex < 1) return;
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetTargetPlayerName(__instance));
    }

    // --- Mask Hornets (Butler Bees) aggro ---
    [HarmonyPatch(typeof(ButlerBeesEnemyAI), nameof(ButlerBeesEnemyAI.SwitchToBehaviourClientRpc))]
    [HarmonyPostfix]
    private static void MaskHornetsEncounter(ButlerBeesEnemyAI __instance, int stateIndex)
    {
        if (stateIndex < 1) return;
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetTargetPlayerName(__instance));
    }

    // --- Turret firing ---
    [HarmonyPatch(typeof(Turret), nameof(Turret.SetToModeClientRpc))]
    [HarmonyPostfix]
    private static void TurretMode(Turret __instance, int mode)
    {
        if (!NetworkUtils.IsClientRpcExecution(__instance)) return;
        if (!ConfigManager.MonsterEncounterMessages.Value) return;

        // Only send on Firing (2) or Berserk (3) modes
        if (mode < 2) return;

        string message = EventMessages.GetTurretFiring();
        MessageSender.Send(message, MessageTier.Event);
    }

    // --- Generic encounter for modded/unknown enemies via behavior state change ---
    // This is handled in DiscoveryPatch.cs which also marks discovery
}
