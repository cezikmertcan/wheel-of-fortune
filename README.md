# Wheel of Fortune

A mobile-first Wheel of Fortune game built with Unity. Spin the wheel, collect rewards, and survive as long as you can — but watch out for bombs.

![Platform](https://img.shields.io/badge/platform-Android-green)
![Unity](https://img.shields.io/badge/Unity-6-blue)
![Language](https://img.shields.io/badge/language-C%23-purple)

---

## Gameplay

- Spin the wheel to collect item rewards
- Every **5th spin** is a Safe Zone — no bomb, guaranteed reward
- Every **30th spin** is a Super Zone — higher value rewards
- On Normal zones, bomb probability increases the longer you survive
- Collect enough rewards and **exit** on a Safe or Super zone
- Hit a bomb: revive and keep going, or give up and end the run
- Reward multiplier scales with spin count — the longer you last, the more each item is worth

---

## Architecture

| Folder | Responsibility |
|---|---|
| `Core/` | `GameManager` (state machine), `GameContext` (shared state) |
| `Data/` | ScriptableObjects — `ItemData`, `SliceData`, `WheelConfig` |
| `Events/` | Typesafe `GameEvent<T>` pub/sub system, `GameEvents` registry |
| `Wheel/` | `WheelController` (spin logic, zone transitions), `WheelSlice`, `WheelVisual`, `SpinResult` |
| `UI/` | `UIManager`, `RewardPoolUI`, `ZoneProgressUI`, `BombResultPanel`, supporting UI components |

### State Machine

```
Idle → Spinning → ShowingReward → Idle
                → BombHit → (Revive → Idle) / (GiveUp → GameOver)
Idle → Collected → GameOver
```

### Event System

All cross-system communication goes through `GameEvents` — a static registry of `GameEvent` and `GameEvent<T>` instances. Components subscribe in `OnEnable` and unsubscribe in `OnDisable`, preventing listener leaks across scene reloads.

### Zone System

Zone type is determined by spin count modulo configurable intervals (`safeZoneInterval = 5`, `superZoneInterval = 30`). Each zone has its own `WheelConfig` ScriptableObject with independent slice pools, weights, and reward multiplier curves.

### Weighted Random

Each `WheelConfig` holds a list of `WeightedSlice` entries. On each spin setup, `GetSlices()` draws `pickCount` items using weighted sampling (with replacement), then Fisher-Yates shuffles the result before filling the wheel. A separate `AnimationCurve` controls bomb probability over time.

---

## Tech

- **Unity 6** — UI Toolkit (uGUI), Sprite Atlas, ScriptableObjects
- **DOTween** — wheel spin tween, particle paths (CatmullRom), UI animations
- **TextMeshPro** — all in-game text
- **Android** — Landscape Left, API 22+
