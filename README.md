# Wheel of Fortune

A mobile-first Wheel of Fortune game built with Unity. Spin the wheel, collect rewards, and survive as long as you can — but watch out for bombs.

![Platform](https://img.shields.io/badge/platform-Android-green)
![Unity](https://img.shields.io/badge/Unity-2021.3%20LTS-blue)
![Language](https://img.shields.io/badge/language-C%23-purple)

---

## Screenshots

![1024x768](Screenshots/1024x768.png)
![1920x1080](Screenshots/1920x1080.png)
![2560x1080](Screenshots/2560x1080.png)

---

## Gameplay

- Spin the wheel to collect item rewards
- Every **5th spin** is a Safe Zone — no bomb, guaranteed reward
- Every **30th spin** is a Super Zone — higher value rewards
- On Normal zones, bomb probability increases the longer you survive
- Collect enough rewards and **exit** on a Safe or Super zone
- Hit a bomb: spend gold to revive and keep going, or give up and end the run
- Reward multiplier scales with spin count — the longer you last, the more each item is worth

---

## Architecture

| Folder | Responsibility |
|---|---|
| `Core/` | `GameManager` (state machine), `GameContext` (shared state) |
| `Data/` | ScriptableObjects — `ItemData`, `SliceData`, `WheelConfig`, `GameConstants` |
| `Economy/` | `ICurrencyManager` interface, `PlayerPrefsCurrencyService` (PlayerPrefs-backed) |
| `Events/` | Typesafe `GameEvent<T>` pub/sub system, `GameEvents` registry |
| `Utilities/` | `ZoneHelper`, `RewardFormatter` — shared stateless helpers |
| `Wheel/` | `WheelController` (spin logic, zone transitions), `WheelSlice`, `WheelVisual`, `SpinResult` |
| `UI/` | `UIManager`, `RewardPoolUI`, `ZoneProgressUI`, `BombResultPanel`, supporting UI components |

### State Machine

```
Idle → Spinning → ShowingReward → Idle
                → BombHit → (Revive → Idle) / (GiveUp → GameOver)
Idle → Collected → GameOver
```

### Event System

All cross-system communication goes through `GameEvents` — a static registry of `GameEvent` and `GameEvent<T>` instances. Components subscribe in `OnEnable` and unsubscribe in `OnDisable`, preventing listener leaks.

### Zone System

Zone type is determined by spin count modulo configurable intervals (`safeZoneInterval = 5`, `superZoneInterval = 30`). Each zone has its own `WheelConfig` ScriptableObject with independent slice pools, weights, and reward multiplier curves.

### Economy

Gold earned during a run is tracked in `RewardPoolUI`. On exit, the gold amount is transferred to `PlayerPrefsCurrencyService` (persisted via PlayerPrefs). The revive button is gated behind the gold balance — disabled when the player cannot afford the cost.

### Weighted Random

Each `WheelConfig` holds a list of `WeightedSlice` entries. On each spin setup, `GetSlices()` draws `pickCount` items using weighted sampling (with replacement), then Fisher-Yates shuffles the result before filling the wheel. A separate `AnimationCurve` controls bomb probability over time.

---

## Tech

- **Unity 2021.3 LTS** — uGUI, Sprite Atlas, ScriptableObjects
- **DOTween** — wheel spin tween, particle paths (CatmullRom), UI animations
- **TextMeshPro** — all in-game text
- **Android** — Landscape Left, API 22+
