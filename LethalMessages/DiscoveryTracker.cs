using System.Collections.Generic;

namespace com.github.luckofthelefty.LethalMessages;

internal static class DiscoveryTracker
{
    private static readonly HashSet<ulong> _discoveredEnemies = new HashSet<ulong>();

    internal static void MarkDiscovered(ulong networkObjectId)
    {
        _discoveredEnemies.Add(networkObjectId);
    }

    internal static bool IsDiscovered(ulong networkObjectId)
    {
        return _discoveredEnemies.Contains(networkObjectId);
    }

    internal static void Reset()
    {
        _discoveredEnemies.Clear();
    }
}
