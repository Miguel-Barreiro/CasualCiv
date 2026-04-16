## Project Overview
Unity 6.3 LTS project 

## Running Tests
To run tests we use the unity mcp 

## Architecture
The framework is entity-component-system inspired but not DOTS.
Systems cannot have state, and instead operate on entities (Entity) with specific components (Component<T>).

## Key Dependencies
- **Zenject** — DI (embedded in `Assets/Core/Runtime/Core/Zenject/`)
- **UniTask** — async/await (`com.cysharp.unitask`)
- **Odin Inspector** — editor tooling (`Assets/Plugins/Sirenix/`)
- **URP** — rendering pipeline (Unity 17.3.0)
- **xNode** — visual node editor (GitHub package)
- **Core** — entity-component-system framework inspired but not DOTS(GitHub package)

