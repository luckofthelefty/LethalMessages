using Unity.Netcode;

namespace com.github.luckofthelefty.LethalMessages;

internal enum MessageTier
{
    Death,
    Event
}

internal static class MessageSender
{
    private const string DeathColor = "#E11919";
    private const string EventColor = "#FFD700";

    internal static void Send(string message, MessageTier tier = MessageTier.Death)
    {
        if (NetworkManager.Singleton == null) return;
        if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer) return;
        if (HUDManager.Instance == null) return;

        string color = tier == MessageTier.Death ? DeathColor : EventColor;
        HUDManager.Instance.AddTextToChatOnServer($"<color={color}>{message}</color>");
    }

    /// <summary>
    /// Sends a chat message directly, bypassing the host/server check. Used by DebugTester.
    /// </summary>
    internal static void SendDirect(string message, MessageTier tier = MessageTier.Death)
    {
        if (HUDManager.Instance == null) return;

        string color = tier == MessageTier.Death ? DeathColor : EventColor;
        HUDManager.Instance.AddTextToChatOnServer($"<color={color}>{message}</color>");
    }
}
