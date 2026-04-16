# CLAUDE.md
This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview
Unity 6.3 LTS project

## Running Tests

Use the Unity MCP tool (`/unity-mcp-orchestrator` skill) to run tests via the Unity Test Runner.

## Architecture

### ECS-Inspired Framework (non-DOTS)

Systems are **stateless** — they operate on injected entities/components but hold no state themselves. Data lives in component structs (`IComponentData`), referenced via `EntId` handles.

- **Entity** — base class for all entities (from `com.miguel.core`)
- **Component<T>** — generic component wrapper bound to an entity
- **BasicCompContainer<T>** — stores component data per entity ID

### Layered Tile Board

`BoardSystem` manages a 2D grid with 4 independent layers (dictionaries keyed by `Vector2Int`):

| Layer | Enum Flag | Tilemap |
|-------|-----------|---------|
| Floor | `WorldObjectType.Floor` | Ground |
| Surface | `WorldObjectType.Surface` | Surface |
| Object | `WorldObjectType.Object` | Objects |
| Air | `WorldObjectType.Air` | Air |

Entities not placed on the board use `Vector2Int(int.MaxValue, int.MaxValue)` (the "floating" sentinel).

**Data flow:** `BoardSystem` (model) → fires `OnTileChangedEvent` → `BoardViewSystem` (view) → updates Unity Tilemaps.

### Key Namespaces

| Namespace | Purpose |
|-----------|---------|
| `Game.Board` | Board grid logic and Tilemap rendering |
| `Game.Entities` | Entity spawning (`EntitySpawnSystem`) and `EntityConfig` SOs |
| `Game.UI` | Game HUD (`GameUISystem`, `GameUIView`, `GameUIMessenger`) |
| `Menus.MainMenu` | Main menu controller and scene transition |
| `Global.Logic` | Project bootstrap, DI installer (`GameProjectInstaller`) |
| `DebugUtils` | Debug spawn system for test tiles/entities |

### DI / Bootstrapping
Zenject wires everything. Each scene has a Bootstrap → Installer chain:

### Global Configs (bound in `GameProjectInstaller`)

- `GameConfig` — `TileSize` (Vector2)
- `MenusConfig` — main menu UI reference
- `DebugConfig` — test tile and test entity references
- `GameStatsContainer` — `DataTable` holding the `Day` stat

### Entity Lifecycle

1. `EntitySpawnSystem.Spawn(EntityConfig)` — creates a `WorldEntity` with component data set
2. `BoardSystem.AddEntity(entId, position)` — places it on the appropriate layer(s)
3. `BoardSystem.RemoveEntity(position, type)` — removes and fires `OnTileChangedEvent`

`EntityConfig` is a ScriptableObject defining `WorldObjectType` flags and the `TileBase` visual.

## Key Dependencies

- **Zenject** — DI (embedded in `Assets/Core/Runtime/Core/Zenject/` via `com.miguel.core`)
- **UniTask** — async/await (`com.cysharp.unitask`)
- **Odin Inspector** — editor tooling (`Assets/Plugins/Sirenix/`)
- **URP** — rendering pipeline (Unity 17.3.0)
- **xNode** — visual node editor (`com.miguel.xnode`, GitHub package)
- **Unity 2D Feature Pack** — `com.unity.feature.2d`

## Scene Names

Defined in `SceneNames.cs`: `PRELOAD`, `MAIN_MENU`, `PLAY_SCENE`.
