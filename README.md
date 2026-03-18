# LethalMessages

A BepInEx mod for Lethal Company that sends funny, randomized chat messages when players die, encounter monsters, or trigger in-game events.

## Features

- **Death Messages** — Randomized messages for all 16 causes of death, sent to in-game chat with red coloring
- **Monster Kill Messages** — Specific messages for 21+ monsters (Bracken, Jester, Forest Giant, Eyeless Dog, etc.)
- **Monster Encounter Messages** — Non-lethal encounter alerts (grabs, haunts, hits) with 15-second cooldown per enemy
- **Event Messages** — Situational alerts for critical damage, ship leaving, teleporter use, quota fulfilled, and more
- **Modded Enemy Support** — Generic fallback messages for custom/modded enemies
- **Enemy Blacklist** — Filter out passive creatures you don't want messages for
- **Extended Chat Duration** — Configurable chat fade time (default 6s vs game's ~4s)

All messages are **host/server only** and appear in the in-game text chat.

## Configuration

Config file location:
```
BepInEx/config/com.github.luckofthelefty.LethalMessages.cfg
```

| Section | Setting | Default | Description |
|---------|---------|---------|-------------|
| General | ChatMessageDuration | `6` | How long chat messages stay visible (seconds). Set ≤4 for game default. |
| Player Events | CriticalDamageMessages | `false` | Alert when a player's health drops to ≤20. |
| Ship Events | ShipLeavingMessages | `false` | Alert when the ship is leaving. |
| Ship Events | VoteToLeaveMessages | `false` | Alert when someone votes to leave early. |
| Ship Events | TeleporterMessages | `false` | Alert when the teleporter is used. |
| Ship Events | QuotaFulfilledMessages | `false` | Alert when the profit quota is met. |
| Monster Encounters | MonsterEncounterMessages | `true` | Show non-lethal monster encounter messages. |
| Monster Encounters | CustomEnemyEncounterMessages | `true` | Show fallback messages for modded/custom enemies. |
| Monster Encounters | EnemyBlacklist | *(passive creatures)* | Comma-separated enemy names to ignore. |

## Installation

1. Install [BepInEx 5](https://github.com/BepInEx/BepInEx) for Lethal Company
2. Place `com.github.luckofthelefty.LethalMessages.dll` in `BepInEx/plugins/`
3. Launch the game

## Building

```bash
dotnet restore
dotnet build
```

Output: `LethalMessages/bin/Debug/netstandard2.1/com.github.luckofthelefty.LethalMessages.dll`

## Support

This mod is provided as-is. **Support is limited to none.** Feel free to fork and modify for your own use.

## License

MIT
