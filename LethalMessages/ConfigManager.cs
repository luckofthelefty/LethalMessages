using BepInEx.Configuration;
using System;
using System.Collections.Generic;

namespace com.github.luckofthelefty.LethalMessages;

internal static class ConfigManager
{
    // Tier 2 — Situational (off by default)
    internal static ConfigEntry<bool> CriticalDamageMessages { get; private set; }
    internal static ConfigEntry<bool> ShipLeavingMessages { get; private set; }
    internal static ConfigEntry<bool> VoteToLeaveMessages { get; private set; }
    internal static ConfigEntry<bool> TeleporterMessages { get; private set; }

    // Tier 3a — Monster Encounters (on by default)
    internal static ConfigEntry<bool> MonsterEncounterMessages { get; private set; }
    internal static ConfigEntry<bool> CustomEnemyEncounterMessages { get; private set; }
    internal static ConfigEntry<string> EnemyBlacklist { get; private set; }

    // Chat Display
    internal static ConfigEntry<float> ChatMessageDuration { get; private set; }

    // Tier 3b — Fun (off by default)
    internal static ConfigEntry<bool> EmoteMessages { get; private set; }
    internal static ConfigEntry<bool> QuotaFulfilledMessages { get; private set; }

    // Parsed blacklist (cached)
    private static HashSet<string> _blacklistCache;

    internal static bool IsEnemyBlacklisted(string enemyName)
    {
        if (_blacklistCache == null) RebuildBlacklistCache();
        return _blacklistCache.Contains(enemyName.ToLowerInvariant());
    }

    private static void RebuildBlacklistCache()
    {
        _blacklistCache = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (EnemyBlacklist?.Value == null) return;

        foreach (string entry in EnemyBlacklist.Value.Split(','))
        {
            string trimmed = entry.Trim();
            if (trimmed.Length > 0)
                _blacklistCache.Add(trimmed.ToLowerInvariant());
        }
    }

    internal static void Initialize(ConfigFile config)
    {
        // Chat Display
        ChatMessageDuration = config.Bind(
            "General", "ChatMessageDuration", 6f,
            "How long chat messages stay visible before fading out, in seconds. The game default is ~4 seconds. Set to 4 or below to use default behavior.");

        // Tier 2
        CriticalDamageMessages = config.Bind(
            "Tier 2 - Situational", "CriticalDamageMessages", false,
            "Show a message when a player takes critical damage.");

        ShipLeavingMessages = config.Bind(
            "Tier 2 - Situational", "ShipLeavingMessages", false,
            "Show a message when the ship is leaving.");

        VoteToLeaveMessages = config.Bind(
            "Tier 2 - Situational", "VoteToLeaveMessages", false,
            "Show a message when someone votes to leave early.");

        TeleporterMessages = config.Bind(
            "Tier 2 - Situational", "TeleporterMessages", false,
            "Show a message when the teleporter is used.");

        // Tier 3a
        MonsterEncounterMessages = config.Bind(
            "Tier 3 - Monster Encounters", "MonsterEncounterMessages", true,
            "Show messages for non-lethal monster encounters (grabs, hits, haunts, etc.).");

        CustomEnemyEncounterMessages = config.Bind(
            "Tier 3 - Monster Encounters", "CustomEnemyEncounterMessages", true,
            "Show fallback messages for modded/custom enemies. Requires MonsterEncounterMessages to be enabled.");

        EnemyBlacklist = config.Bind(
            "Tier 3 - Monster Encounters", "EnemyBlacklist",
            "Docile Locust Bees,Manticoil,Roaming Locusts,Lasso Man,Puffer,FlowerSnake",
            "Comma-separated list of enemy names to ignore for encounter AND death messages. These are passive/ambient creatures that shouldn't trigger messages.");

        EnemyBlacklist.SettingChanged += (_, __) => RebuildBlacklistCache();
        RebuildBlacklistCache();

        // Tier 3b
        EmoteMessages = config.Bind(
            "Tier 3 - Fun", "EmoteMessages", false,
            "Show a message when a player emotes.");

        QuotaFulfilledMessages = config.Bind(
            "Tier 3 - Fun", "QuotaFulfilledMessages", false,
            "Show a message when the profit quota is met.");
    }
}
