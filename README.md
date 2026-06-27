# PixelLeap

A complete **Unity 2D platformer** starter — built so it's playable the instant you open it.
There's no fragile scene wiring to fix: a single `GameBootstrap` object generates the whole
level (player, platforms, coins, enemies, goal) procedurally at runtime using solid-color
sprites created in code. No art assets required.

> Studio starter kit · C# / Unity · 2D platformer

## Requirements

- **Unity is not currently installed on this machine.** Install it first:
  1. Download **Unity Hub**: https://unity.com/download
  2. In Unity Hub → *Installs* → *Install Editor*, pick a **2021.3 LTS** or **2022.3 LTS** (or newer) editor.
     The project is pinned to `2022.3.40f1`, but any reasonably recent version will open it
     (Hub will offer to upgrade — that's fine).

## How to run it

1. Open **Unity Hub** → *Add* → *Add project from disk* → select this `PixelLeap` folder.
2. Click the project to open it (first import takes a minute).
3. In the Project window, open **`Assets/Scenes/Main.unity`**.
4. Press **Play**. 🎮

## Controls

| Action | Keys |
| --- | --- |
| Move | `A` / `D` or `←` / `→` |
| Jump | `Space`, `W`, or `↑` (hold for higher jumps) |
| Defeat enemy | Land on it from **above** (you bounce) |
| Restart after win/loss | `R` |

Collect coins for score, stomp the red enemies, avoid the pit, and reach the green flag.
You have 3 lives; falling in the pit or touching an enemy from the side costs one.

## Project layout

```
PixelLeap/
├─ Assets/
│  ├─ Scenes/Main.unity        ← the one scene; holds a single GameBootstrap object
│  └─ Scripts/
│     ├─ GameBootstrap.cs      ← builds the whole level at runtime (start here)
│     ├─ PlayerController2D.cs ← run + jump (coyote time, jump buffer, variable height)
│     ├─ CameraFollow.cs       ← smooth follow camera
│     ├─ GameManager.cs        ← score, lives, respawn, win/lose
│     ├─ Coin.cs               ← collectible pickup
│     ├─ Patroller.cs          ← pacing enemy, stompable
│     ├─ LevelGoal.cs          ← win trigger (the flag)
│     ├─ KillZone.cs           ← pit / fall death
│     ├─ Hud.cs                ← on-screen score/lives/banners (IMGUI, no canvas)
│     └─ SpriteFactory.cs      ← makes solid-color sprites in code
├─ ProjectSettings/            ← Unity version, "Ground" layer (index 8), build scene list
└─ Packages/manifest.json
```

## Where to go next

- **Tune the feel** — tweak `moveSpeed`, `jumpForce`, `coyoteTime` on `PlayerController2D`.
- **Redesign the level** — edit the coordinates in `GameBootstrap.BuildLevel()`.
- **Add real art** — drop sprites into `Assets/`, set them up as Sprites, and replace the
  `SpriteFactory.Solid(...)` calls with your imported sprites. Later, swap procedural
  generation for hand-authored scenes / Tilemaps.
- **More mechanics** — double jump, moving platforms, dashing, checkpoints, sound. Each is a
  small addition to the existing scripts.

Built as a studio starter — clone it per prototype and iterate.
