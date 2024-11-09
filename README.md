# Archipelago Mod

**Archipelago** is a framework for logic randomizers.
See [the Archipelago website](https://archipelago.gg/) for more details.

This repository is a BepInEx mod that allows Rain World to be randomized through Archipelago.
See [the setup guide on the Archipelago generator repository](
https://github.com/alphappy/ArchipelagoRW/blob/main/worlds/rain_world/docs/setup_en.md)
for an explanation of how to set up Archipelago for Rain World.

Archipelago Mod requires [Dev Console](https://github.com/SlimeCubed/DevConsole).
It adds three commands:
- `apconnect <HOST> <NAME> <PASSWORD>`: connect to an AP room.
  - `<HOST>` should include the port number.
  - `<NAME>` must *exactly* match the name in your settings YAML.
  - `<PASSWORD>` must *exactly* match the room password, but can be omitted if no password was set.
- `apdisconnect`: disconnect from current AP room.
- `apsay <MESSAGE>`: send `<MESSAGE>` to the room as a text message.
Because this is just a normal text message, server commands like `!hint` work here.
- `apcollect <LOCATION>`: tell the AP server that `<LOCATION>` has been checked.
This is a debug tool.

## Compatibility
Most other mods will be usable alongside the Archipelago mod,
but there are some important exceptions.

- **More Slugcats Expansion (Downpour)**: Currently *required* for Archipelago.
This will change in the future.
When enabled, all Passages, regions, new pearls, and new collectable tokens are checks.
- **Expedition (Downpour)**: Not compatible and not planned.
Having the Expedition mod enabled is fine,
but Archipelago won't function as intended in Expedition mode.
- **Jolly Co-op**:  Partially compatible and planned.
Archipelago mostly functions as intended with multiple players,
but currently all items are always delivered to player 1.
- **Room Randomizer**: Partially compatible.
As long as a path from each gate to each other gate exists, Archipelago logic should work.
However, one-way rooms like the Subterranean ravine or the Outskirts filtration area may break logic.
- **Enemy Randomizer**: Partially compatible and compatibility not planned.
Checks which require access to specific types of creatures
(food quest, Dragon Slayer, Friend, Chieftain)
may be logically impossible with Enemy Randomizer.
- *Most custom region mods*: Compatible but not supported.
Archipelago mod does not currently randomize checks in custom regions.
- *Most other mods*: Generally compatible.
If you discover an incompatibility that isn't already reported, [create an issue](
https://github.com/alphappy/ArchipelagoRWMod/issues).

## Building
The repo is a Visual Studio project (specifically made with VS Community 2022).
For now, it expects reference assemblies to be at `../.references`
(i.e., `.references` is a sibling directory to the repo root).
In addition to the assemblies that are shipped with Rain World 1.9,
[Archipelago.MultiClient.Net](https://www.nuget.org/packages/Archipelago.MultiClient.Net/6.3.1)
and [DevConsole]() must be available.