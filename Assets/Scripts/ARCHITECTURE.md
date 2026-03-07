# Legends of Bohemia – Architecture Overview

## Observer Pattern

The project uses a custom generic observer pattern instead of C# events for cross-system communication. This keeps systems decoupled — producers (subjects) do not know who is listening.

**Interfaces:**
- `ISubject<T>` — registers/unregisters observers, notifies them with event data
- `IObserver<T>` — receives event data via `OnNotify(T eventData)`

**Subjects in the project:**
| Subject | Event type | Lives on |
|---|---|---|
| `PlayerEventManager` | `PlayerEvent` | Player prefab |
| `SpellEventManager` | `SpellEvent` | Player prefab |

**Typical observers:**
- UI Presenters (`SpellQueuePresenter`, `PlayerStatsPresenter`)
- Animator controllers
- Audio systems
- VFX systems

---

## Player Lifecycle

```
SpawnManager.SpawnPlayer()
    → PlayerFactory.Create(PlayerData, spawnPoint)
        → Instantiate(_playerPrefab)
        → PlayerController.Initialize()   // stats, grid position
        → Animator.runtimeAnimatorController = data.AnimatorController
        → PlayerSoundController.Initialize()
    → OnPlayerSpawned event
        → GameManager.HandlePlayerSpawned()
            → SpellQueuePresenter.SetSpellEventManager()
            → PlayerStatsPresenter.SetPlayer()
```

**Player prefab components:**
- `PlayerController` — input, movement, spell collection, stat changes
- `PlayerEventManager` — broadcasts player events
- `SpellController` — spell queue, charge/duration routines
- `SpellEventManager` — broadcasts spell events

---

## Spell Lifecycle

### Spawn on grid
```
SpawnManager (coroutine, interval from GameSettingsConfig)
    → SpellFactory.Create(SpellType)
        → Instantiate spell prefab
    → GridSystem.Register(cell, spell)
```

### Collection and cast
```
PlayerController.CollectSpell()
    → GridSystem.Unregister()
    → SpellController.Enqueue(spell)
        → SpellEventManager: Collected    // spell picked up
        → _spellQueue.Enqueue(spell)
        → SpellEventManager: QueueChanged // UI update
        → SpellRoutine (coroutine)
            → SpellEventManager: ChargeTick   // every frame, Value 0→1
            → spell.Cast(target)
            → SpellEventManager: Cast
            → SpellEventManager: DurationTick // every frame, Value 1→0
            → _spellQueue.Dequeue()
            → SpellEventManager: QueueChanged
    → Destroy(spell GameObject)
```

---

## PlayerEvent Reference

Subscribe via: `playerController.RegisterObserver(this)` or `playerEventManager.RegisterObserver(this)`

| Event | When | Value | MaxValue |
|---|---|---|---|
| `Walking` | Player moves at normal speed | CurrentSpeed | MoveSpeed |
| `Running` | Player moves with speed boost | CurrentSpeed | MoveSpeed |
| `Standing` | Player stops moving | 0 | MoveSpeed |
| `HealthChanged` | HP changes (heal, poison, damage) | CurrentHealth | MaxHealth |
| `EnergyChanged` | Energy changes | CurrentEnergy | MaxEnergy |

---

## SpellEvent Reference

Subscribe via: `spellEventManager.RegisterObserver(this)`

| Event | When | Spell | Value | Queue |
|---|---|---|---|---|
| `Collected` | Player picks up a spell | the spell | — | — |
| `QueueChanged` | Spell added or removed from queue | — | — | current queue |
| `ChargeTick` | Every frame during charging | — | 0→1 normalized | — |
| `Cast` | Spell is cast on target | the spell | — | — |
| `DurationTick` | Every frame during active effect | — | 1→0 normalized | — |

---

## How to Subscribe

### 1. Implement the interface

```csharp
public class MyAudioObserver : MonoBehaviour, IObserver<SpellEvent>
{
    public void OnNotify(SpellEvent eventData)
    {
        switch (eventData.Event)
        {
            case SpellEventType.Collected:
                PlaySound(eventData.Spell.Data.Type);
                break;
            case SpellEventType.Cast:
                PlayCastSound(eventData.Spell.Data.Type);
                break;
        }
    }
}
```

### 2. Register after player is spawned

Use `GameManager.HandlePlayerSpawned` as the entry point — this is where the player reference is available. Get the manager via `GetComponent` and register.

```csharp
SpellEventManager spellManager = player.GetComponent<SpellEventManager>();
spellManager.RegisterObserver(myObserver);
```

### 3. Always unregister on destroy

```csharp
private void OnDestroy()
{
    _spellManager?.UnregisterObserver(this);
}
```
