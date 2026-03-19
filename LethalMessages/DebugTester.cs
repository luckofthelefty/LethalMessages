using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.InputSystem;

namespace com.github.luckofthelefty.LethalMessages;

internal class DebugTester : MonoBehaviour
{
    private static readonly CauseOfDeath[] DeathCauses = new[]
    {
        CauseOfDeath.Unknown,
        CauseOfDeath.Bludgeoning,
        CauseOfDeath.Gravity,
        CauseOfDeath.Blast,
        CauseOfDeath.Strangulation,
        CauseOfDeath.Suffocation,
        CauseOfDeath.Mauling,
        CauseOfDeath.Gunshots,
        CauseOfDeath.Crushing,
        CauseOfDeath.Drowning,
        CauseOfDeath.Abandoned,
        CauseOfDeath.Electrocution,
        CauseOfDeath.Kicking,
        CauseOfDeath.Burning,
        CauseOfDeath.Stabbing,
        CauseOfDeath.Fan
    };

    private static readonly string[] MonsterNames = new[]
    {
        "Flowerman", "Jester", "ForestGiant", "MouthDog", "Crawler",
        "Blob", "Centipede", "SandSpider", "BaboonHawk", "NutcrackerEnemy",
        "SpringMan", "MaskedPlayerEnemy", "DressGirl", "Butler",
        "ClaySurgeon", "CaveDweller", "HoarderBug", "SandWorm",
        "RadMech", "RedLocustBees", "ButlerBees"
    };

    private int _deathIndex;
    private int _monsterDeathIndex;
    private int _monsterEncounterIndex;
    private int _eventIndex;
    private const int EventTypeCount = 7;

    private InputAction _f5Action;
    private InputAction _f6Action;
    private InputAction _f7Action;
    private InputAction _f8Action;

    private string TestName
    {
        get
        {
            var localPlayer = GameNetworkManager.Instance?.localPlayerController;
            return localPlayer != null ? localPlayer.playerUsername : "TestPlayer";
        }
    }

    #pragma warning disable IDE0051
    private void Awake()
    #pragma warning restore IDE0051
    {
        _f5Action = new InputAction("DebugDeath", InputActionType.Button, "<Keyboard>/f5");
        _f6Action = new InputAction("DebugMonsterKill", InputActionType.Button, "<Keyboard>/f6");
        _f7Action = new InputAction("DebugEncounter", InputActionType.Button, "<Keyboard>/f7");
        _f8Action = new InputAction("DebugEvent", InputActionType.Button, "<Keyboard>/f8");

        _f5Action.Enable();
        _f6Action.Enable();
        _f7Action.Enable();
        _f8Action.Enable();

        Plugin.Log.LogInfo("[DebugTester] Awake — InputActions created and enabled.");
    }

    #pragma warning disable IDE0051
    private void OnDestroy()
    #pragma warning restore IDE0051
    {
        _f5Action?.Disable(); _f5Action?.Dispose();
        _f6Action?.Disable(); _f6Action?.Dispose();
        _f7Action?.Disable(); _f7Action?.Dispose();
        _f8Action?.Disable(); _f8Action?.Dispose();
    }

    #pragma warning disable IDE0051
    private void Update()
    #pragma warning restore IDE0051
    {
        if (!ConfigManager.EnableTestMode.Value) return;

        // F5 = Death message (cycles cause of death type)
        if (_f5Action.WasPressedThisFrame())
        {
            var cause = DeathCauses[_deathIndex % DeathCauses.Length];
            string msg = Messages.DeathMessages.Get(cause, TestName);
            Plugin.Log.LogInfo($"[TEST] Death ({cause}): {msg}");
            MessageSender.SendDirect(msg, MessageTier.Death);
            _deathIndex++;
        }

        // F6 = Monster kill message (cycles monster type)
        if (_f6Action.WasPressedThisFrame())
        {
            string monster = MonsterNames[_monsterDeathIndex % MonsterNames.Length];
            string msg = Messages.MonsterMessages.GetDeathMessage(monster, TestName);
            Plugin.Log.LogInfo($"[TEST] Monster Kill ({monster}): {msg}");
            MessageSender.SendDirect(msg, MessageTier.Death);
            _monsterDeathIndex++;
        }

        // F7 = Monster encounter message (cycles monster type)
        if (_f7Action.WasPressedThisFrame())
        {
            string monster = MonsterNames[_monsterEncounterIndex % MonsterNames.Length];
            string msg = Messages.MonsterMessages.GetEncounterMessage(monster, TestName);
            if (msg != null)
            {
                Plugin.Log.LogInfo($"[TEST] Encounter ({monster}): {msg}");
                MessageSender.SendDirect(msg, MessageTier.Event);
            }
            _monsterEncounterIndex++;
        }

        // F8 = Event messages (cycles through event types)
        if (_f8Action.WasPressedThisFrame())
        {
            string msg;
            string label;
            switch (_eventIndex % EventTypeCount)
            {
                case 0:
                    msg = Messages.EventMessages.GetCriticalDamage(TestName);
                    label = "CriticalDamage";
                    break;
                case 1:
                    msg = Messages.EventMessages.GetShipLeaving();
                    label = "ShipLeaving";
                    break;
                case 2:
                    msg = Messages.EventMessages.GetVoteToLeave();
                    label = "VoteToLeave";
                    break;
                case 3:
                    msg = Messages.EventMessages.GetTeleporter(false);
                    label = "Teleporter";
                    break;
                case 4:
                    msg = Messages.EventMessages.GetTeleporter(true);
                    label = "InverseTeleporter";
                    break;
                case 5:
                    msg = Messages.EventMessages.GetQuotaFulfilled();
                    label = "QuotaFulfilled";
                    break;
                case 6:
                    msg = Messages.EventMessages.GetTurretFiring();
                    label = "TurretFiring";
                    break;
                default:
                    return;
            }
            Plugin.Log.LogInfo($"[TEST] Event ({label}): {msg}");
            MessageSender.SendDirect(msg, MessageTier.Event);
            _eventIndex++;
        }
    }
}
