# Slayer

Slayer is a 2D action-platformer prototype built with Unity 6. The project uses a
state-machine-driven player and enemy architecture, 2D physics, the Unity Input
System, and Animator controllers for combat and movement.

## Current features

- Player movement, jumping, wall sliding, wall jumping, and dashing
- Ground and aerial combo attacks
- Counterattacks and enemy stun windows
- Skeleton enemy patrol, detection, battle, attack, stun, and death states
- Health, damage, hit VFX, parallax backgrounds, and interactive chests
- Animator-driven state transitions and combat events

## Requirements

- Unity 6.5 (`6000.5.5f1`)
- A desktop platform supported by Unity

## Getting started

1. Clone the repository.
2. Open the repository root in Unity Hub using Unity `6000.5.5f1`.
3. Open `Assets/Scenes/Debug.unity`.
4. Enter Play mode to test the current gameplay prototype.

Unity will regenerate ignored folders such as `Library`, `Temp`, `Logs`, and IDE
project files when the project is opened.

## Project structure

- `Assets/Scripts/States` contains the entity, player, and enemy state machines.
- `Assets/Scripts/Entity` contains shared entity, combat, health, and VFX logic.
- `Assets/Scripts/Player` and `Assets/Scripts/Enemy` contain actor-specific logic.
- `Assets/Animations` contains player, enemy, object clips, and controllers.
- `Assets/Scenes/Debug.unity` is the current development and test scene.
