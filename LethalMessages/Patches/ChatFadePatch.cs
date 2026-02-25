using HarmonyLib;

namespace com.github.luckofthelefty.LethalMessages.Patches;

/// <summary>
/// Extends the duration chat messages stay visible before fading out.
/// The game calls PingHUDElement(Chat, delay) when a message is received.
/// This patch intercepts that call and replaces the delay with our configured value.
/// </summary>
[HarmonyPatch(typeof(HUDManager))]
internal static class ChatFadePatch
{
    /// <summary>
    /// Intercepts PingHUDElement calls. When the element being pinged is the
    /// Chat element, override the delay with our configured duration.
    /// </summary>
    [HarmonyPatch(nameof(HUDManager.PingHUDElement))]
    [HarmonyPrefix]
    private static void ExtendChatDuration(HUDManager __instance, HUDElement element, ref float delay)
    {
        if (element == null || __instance.Chat == null) return;

        // Only modify the delay for the Chat HUD element
        if (element != __instance.Chat) return;

        float configuredDuration = ConfigManager.ChatMessageDuration.Value;
        if (configuredDuration > delay)
        {
            delay = configuredDuration;
        }
    }
}
