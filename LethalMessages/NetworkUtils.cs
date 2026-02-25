using System.Reflection;
using Unity.Netcode;

namespace com.github.luckofthelefty.LethalMessages;

internal static class NetworkUtils
{
    // Cached reflection for __rpc_exec_stage (internal field in NetworkBehaviour)
    private static readonly FieldInfo _rpcExecStageField =
        typeof(NetworkBehaviour).GetField("__rpc_exec_stage", BindingFlags.Instance | BindingFlags.NonPublic);

    /// <summary>
    /// Returns true if a ClientRpc is in the actual client execution stage.
    /// On the host, ClientRpc methods are called twice — this prevents duplicate events.
    /// </summary>
    public static bool IsClientRpcExecution(NetworkBehaviour instance)
    {
        if (instance == null || _rpcExecStageField == null) return true;

        // __RpcExecStage: None = 0, Server = 1, Client = 2
        int stage = (int)_rpcExecStageField.GetValue(instance);
        return stage == 2; // Client
    }
}
