using System.Collections.Generic;
using GameNetcodeStuff;
using HarmonyLib;

namespace com.github.luckofthelefty.LethalMessages.Patches;

internal static class MonsterKillPatch
{
    // Monster-specific patches store (playerId, enemyName) here before the generic death patch fires
    private static readonly Dictionary<int, string> _recentMonsterKills = new Dictionary<int, string>();

    internal static void RegisterMonsterKill(int playerId, string enemyName)
    {
        _recentMonsterKills[playerId] = enemyName;
    }

    internal static bool TryConsumeMonsterKill(int playerId, out string enemyName)
    {
        if (_recentMonsterKills.TryGetValue(playerId, out enemyName))
        {
            _recentMonsterKills.Remove(playerId);
            return true;
        }
        enemyName = null;
        return false;
    }

    private static string GetPlayerName(int playerObjectIndex)
    {
        if (StartOfRound.Instance?.allPlayerScripts == null
            || playerObjectIndex < 0
            || playerObjectIndex >= StartOfRound.Instance.allPlayerScripts.Length)
            return "Unknown";

        return StartOfRound.Instance.allPlayerScripts[playerObjectIndex]?.playerUsername ?? "Unknown";
    }

    // --- Bracken (Flowerman) ---
    [HarmonyPatch(typeof(FlowermanAI), nameof(FlowermanAI.KillPlayerAnimationClientRpc))]
    [HarmonyPostfix]
    private static void BrackenKill(int playerObjectId)
    {
        RegisterMonsterKill(playerObjectId, "Flowerman");
    }

    // --- Jester ---
    [HarmonyPatch(typeof(JesterAI), nameof(JesterAI.KillPlayerClientRpc))]
    [HarmonyPostfix]
    private static void JesterKill(int playerId)
    {
        RegisterMonsterKill(playerId, "Jester");
    }

    // --- Forest Giant ---
    [HarmonyPatch(typeof(ForestGiantAI), nameof(ForestGiantAI.GrabPlayerClientRpc))]
    [HarmonyPostfix]
    private static void GiantKill(int playerId)
    {
        RegisterMonsterKill(playerId, "ForestGiant");
    }

    // --- Eyeless Dog (MouthDog) ---
    [HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.KillPlayerClientRpc))]
    [HarmonyPostfix]
    private static void EyelessDogKill(int playerId)
    {
        RegisterMonsterKill(playerId, "MouthDog");
    }

    // --- Blob (Hygrodere) ---
    [HarmonyPatch(typeof(BlobAI), nameof(BlobAI.SlimeKillPlayerEffectClientRpc))]
    [HarmonyPostfix]
    private static void BlobKill(int playerKilled)
    {
        RegisterMonsterKill(playerKilled, "Blob");
    }

    // --- Baboon Hawk ---
    [HarmonyPatch(typeof(BaboonBirdAI), nameof(BaboonBirdAI.StabPlayerDeathAnimClientRpc))]
    [HarmonyPostfix]
    private static void BaboonHawkKill(int playerObject)
    {
        RegisterMonsterKill(playerObject, "BaboonHawk");
    }

    // --- Butler ---
    [HarmonyPatch(typeof(ButlerEnemyAI), nameof(ButlerEnemyAI.StabPlayerClientRpc))]
    [HarmonyPostfix]
    private static void ButlerKill(int playerId)
    {
        RegisterMonsterKill(playerId, "Butler");
    }

    // --- Barber (Clay Surgeon) ---
    [HarmonyPatch(typeof(ClaySurgeonAI), nameof(ClaySurgeonAI.KillPlayerClientRpc))]
    [HarmonyPostfix]
    private static void BarberKill(ClaySurgeonAI __instance)
    {
        var player = __instance.targetPlayer;
        if (player == null) return;
        int playerId = (int)player.playerClientId;
        RegisterMonsterKill(playerId, "ClaySurgeon");
    }

    // --- Old Bird (RadMech) ---
    [HarmonyPatch(typeof(RadMechAI), nameof(RadMechAI.GrabPlayerClientRpc))]
    [HarmonyPostfix]
    private static void OldBirdKill(int playerId)
    {
        RegisterMonsterKill(playerId, "RadMech");
    }
}
