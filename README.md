# Archipelago Mod

Archipelago is a framework for logic randomizers.
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

## Building
The repo is a Visual Studio project (specifically made with VS Community 2022).
For now, it expects reference assemblies to be at `../.references`
(i.e., `.references` is a sibling directory to the repo root).
In addition to the assemblies that are shipped with Rain World 1.9,
[Archipelago.MultiClient.Net](https://www.nuget.org/packages/Archipelago.MultiClient.Net/6.3.1)
and [DevConsole]() must be available.