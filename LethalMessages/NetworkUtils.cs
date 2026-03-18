using System.Collections.Generic;
using Unity.Netcode;

namespace com.github.luckofthelefty.LethalMessages;

internal static class NetworkUtils
{
    // Frame-based deduplication to prevent duplicate events on the host.
    // On the host, ClientRpc methods fire twice per call. This ensures
    // we only process each unique event once per frame.
    private static readonly Dictionary<string, int> _lastProcessedFrame = new Dictionary<string, int>();

    /// <summary>
    /// Returns true the first time a given event key is seen in a frame.
    /// Returns false on subsequent calls with the same key in the same frame.
    /// Use this in ClientRpc postfixes to prevent duplicate events on the host.
    /// </summary>
    public static bool ShouldProcess(string eventKey)
    {
        int frame = UnityEngine.Time.frameCount;
        if (_lastProcessedFrame.TryGetValue(eventKey, out int lastFrame) && lastFrame == frame)
            return false;

        _lastProcessedFrame[eventKey] = frame;
        return true;
    }
}
