using BepInEx;
using com.github.luckofthelefty.LethalMessages.Patches;
using HarmonyLib;

namespace com.github.luckofthelefty.LethalMessages;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
internal class Plugin : BaseUnityPlugin
{
    private readonly Harmony _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

    internal static Plugin Instance { get; private set; }
    internal static BepInEx.Logging.ManualLogSource Log => Instance.Logger;

    #pragma warning disable IDE0051
    private void Awake()
    #pragma warning restore IDE0051
    {
        Instance = this;

        ConfigManager.Initialize(Config);

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} loaded!");

        // Tier 1 — Death messages (always on)
        _harmony.PatchAll(typeof(DeathPatch));
        _harmony.PatchAll(typeof(MonsterKillPatch));

        // Tier 3a — Monster encounters (on by default, config toggle)
        _harmony.PatchAll(typeof(MonsterEncounterPatch));
        _harmony.PatchAll(typeof(DiscoveryPatch));

        // Tier 2 + 3b — Situational + fun events (off by default)
        _harmony.PatchAll(typeof(EventPatch));

        // Chat display
        _harmony.PatchAll(typeof(ChatFadePatch));

        // Debug tester
        gameObject.AddComponent<DebugTester>();
    }
}
