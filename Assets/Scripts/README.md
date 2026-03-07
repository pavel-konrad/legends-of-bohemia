# Legends of Bohemia – Developer Documentation

## Table of Contents

1. [Architecture](#1-architecture)
2. [Grid System](#2-grid-system)
3. [Player System](#3-player-system)
4. [Combat System](#4-combat-system)
5. [Spell System](#5-spell-system)
6. [Main Menu & Character Select](#6-main-menu--character-select)
7. [Event Reference](#7-event-reference)
8. [How to Add a New Player](#8-how-to-add-a-new-player)

---

## 1. Architecture

### Observer Pattern

All cross-system communication uses a custom generic observer pattern. Producers (subjects) do not know who is listening — systems are fully decoupled.

```
ISubject<T>   — RegisterObserver, UnregisterObserver, NotifyObservers
IObserver<T>  — OnNotify(T eventData)
```

**Subjects in the project:**

| Subject | Event type | Lives on |
|---|---|---|
| `PlayerEventManager` | `PlayerEvent` | Player prefab |
| `SpellEventManager` | `SpellEvent` | Player prefab |
| `CharacterSelectEventManager` | `CharacterSelectEvent` | Main Menu scene |

**Typical observers:**
- UI Presenters (`SpellQueuePresenter`, `PlayerStatsPresenter`)
- `PlayerAnimatorController` — animates player based on events
- `PlayerSoundController` — plays sounds via Animation Events
- `UIAnimationObserver` — animates UI panels
- `UIAudioObserver` — plays UI click/confirm/cancel sounds

### How to Create an Observer

```csharp
public class MyObserver : MonoBehaviour, IObserver<PlayerEvent>
{
    private PlayerEventManager _manager;

    private void Start()
    {
        _manager = GetComponent<PlayerEventManager>();
        _manager.RegisterObserver(this);
    }

    private void OnDestroy()
    {
        _manager?.UnregisterObserver(this);
    }

    public void OnNotify(PlayerEvent eventData)
    {
        switch (eventData.Event)
        {
            case PlayerEventType.HealthChanged:
                // handle
                break;
        }
    }
}
```

> Register in `GameManager.HandlePlayerSpawned` for runtime-spawned players.
> Always unregister in `OnDestroy` to avoid memory leaks.

---

## 2. Grid System

The game world is a discrete grid. All entities (players, enemies, spells) occupy cells tracked by `GridSystem`.

### Key Methods

| Method | Description |
|---|---|
| `GridToWorld(Vector2Int)` | Converts grid position to world position |
| `WorldToGrid(Vector3)` | Converts world position to grid position |
| `IsValid(Vector2Int)` | Checks if position is within grid bounds |
| `IsCellFree(Vector2Int)` | Returns true if cell has no occupant |
| `GetOccupant(Vector2Int)` | Returns `IGridOccupant` at position or null |
| `Register(pos, occupant)` | Places entity on grid |
| `Unregister(pos)` | Removes entity from grid |
| `Move(from, to, occupant)` | Moves entity between cells |
| `GetFreeCells()` | Returns list of all free cells |

### IGridOccupant Interface

Any object that occupies a grid cell must implement `IGridOccupant`:

```csharp
public interface IGridOccupant
{
    Vector2Int GridPosition { get; }
}
```

### Configuration

`GridConfig` ScriptableObject — **Assets → Create → Grid → GridConfig**

| Field | Description |
|---|---|
| Width | Number of columns |
| Height | Number of rows |
| CellSize | Size of each cell in world units |

---

## 3. Player System

### Player Lifecycle

```
CharacterSelect → PlayerConfig.SelectedPlayer = PlayerData
SpawnManager.SpawnPlayer()
  → PlayerFactory.Create(PlayerData, spawnPoint)
      → Instantiate(data.PlayerPrefab)
      → PlayerController.Initialize(data, gridSystem, spawnPoint)
          → AttackController.Initialize(attackAction, gridSystem)
      → PlayerSoundController.Initialize(data)
  → OnPlayerSpawned event
      → GameManager.HandlePlayerSpawned(player)
          → SpellQueuePresenter.SetSpellEventManager()
          → PlayerStatsPresenter.SetPlayer()
```

### Player Prefab Structure

```
PlayerPrefab
 ├── [Mesh / Model]           ← 3D model with Animator component
 ├── PlayerController         ← movement, health, spell collection
 ├── PlayerEventManager       ← observer subject for player events
 ├── PlayerAnimatorController ← observer, drives Animator parameters
 ├── PlayerSoundController    ← footstep + combat audio
 ├── SpellController          ← spell queue and cast routines
 ├── SpellEventManager        ← observer subject for spell events
 ├── SpellSoundController     ← spell audio
 ├── AttackController         ← attack input and grid hit detection
 ├── AudioSource (Footstep)   → PlayerSoundController._footstepAudio
 └── AudioSource (Combat)     → PlayerSoundController._combatAudio
```

### Inspector Setup

**PlayerController**
| Field | Assign |
|---|---|
| Input Actions | `InputSystem_Actions` asset |

> All other references (`GridSystem`, `PlayerData`, spawn point) are injected via `Initialize()` from `PlayerFactory`. Do not assign in Inspector.

**PlayerSoundController**
| Field | Assign |
|---|---|
| Footstep Audio | AudioSource (Footstep) |
| Combat Audio | AudioSource (Combat) |

**SpellSoundController**
| Field | Assign |
|---|---|
| Hits Audio | AudioSource for one-shot sounds |
| Loop Audio | AudioSource for looping effects |

> `PlayerAnimatorController` and `AttackController` have no SerializeField — all references resolved via `GetComponent`.

### Animator Controller

Create a new **Animator Controller** and assign it to the model's `Animator` component.

**Parameters:**

| Name | Type | Description |
|---|---|---|
| Speed | Float | 0 = Idle, 1 = Walk, 2 = Run |
| Attack | Trigger | Triggers attack animation |

**Blend Tree (locomotion):**
- Type: 1D, parameter `Speed`
- 0.0 → Idle clip
- 1.0 → Walk clip
- 2.0 → Run clip

**Transitions:**

| From → To | Condition |
|---|---|
| Any State → Attack | Trigger: Attack, Has Exit Time OFF |
| Attack → Idle | Exit Time (after clip completes) |

### Animation Events

**Attack clip:**

| Frame | Method | Target Component |
|---|---|---|
| Impact frame | `OnAttack()` | `PlayerSoundController` |
| Last frame | `OnAttackFinished()` | `PlayerController` |

**Walk / Run clip:**

| Frame | Method | Target Component |
|---|---|---|
| Each footfall | `OnFootstep()` | `PlayerSoundController` |

### Audio Mixer Groups

| AudioSource | Mixer Group |
|---|---|
| Footstep Audio | SFX/Footsteps |
| Combat Audio | SFX/Combat |

### PlayerData ScriptableObject

Create: **Assets → Create → Player → PlayerData**

| Field | Description |
|---|---|
| Name | Character name |
| Icon | UI sprite |
| Race | Human / Undead / Angel / Devil |
| Class | Warrior / Mage / Rogue / ... |
| MoveSpeed | Movement speed across grid |
| MaxHealth | Maximum HP |
| MaxEnergy | Maximum Energy |
| AttackPower | Attack damage |
| IsRanged | True = ranged, False = melee |
| FootstepSounds | AudioClip array (random selection) |
| SelectSound | Sound played on character select screen |
| AttackSound | AudioClip array (random selection) |
| PlayerPrefab | Reference to the player prefab |

---

## 4. Combat System

### Attack Flow

```
Player presses Space
  → AttackController.Update()
      → FacingDirection != zero?
      → _attacker.SetAttacking(true)
      → _attacker.NotifyAttack()            → PlayerEventManager: Attacking
          → PlayerAnimatorController        → SetTrigger("Attack")
      → GridSystem.GetOccupant(target cell)
          → if IDemageable → TakeDamage(AttackPower)
              → PlayerEventManager: HealthChanged
```

### Interfaces

| Interface | Implemented by | Key members |
|---|---|---|
| `IAttacker` | PlayerController, EnemyController | `FacingDirection`, `AttackPower`, `IsAttacking`, `NotifyAttack()`, `SetAttacking()` |
| `IDemageable` | PlayerController, EnemyController | `TakeDamage(float)`, `MaxHealth` |

### Movement Lock

`IsAttacking = true` blocks `PlayerController.Move()` until `OnAttackFinished()` is called via Animation Event on the last frame of the attack clip.

---

## 5. Spell System

### Spell Lifecycle

**Spawn:**
```
SpawnManager (coroutine, interval from GameSettingsConfig)
  → SpellFactory.Create(SpellType)
  → GridSystem.Register(cell, spell)
```

**Collection and cast:**
```
PlayerController.CollectSpell()
  → GridSystem.Unregister()
  → SpellController.Enqueue(spell)
      → SpellEventManager: Collected
      → SpellEventManager: QueueChanged
      → SpellRoutine (coroutine)
          → SpellEventManager: ChargeTick    (Value 0→1)
          → spell.Cast(ISpellTarget)
          → SpellEventManager: Cast
          → SpellEventManager: DurationTick  (Value 1→0)
          → SpellEventManager: QueueChanged
  → Destroy(spell GameObject)
```

### Adding a New Spell

1. Create a class inheriting from `SpellBase`
2. Override `Cast(ISpellTarget target)`
3. Create a `SpellData` ScriptableObject — **Assets → Create → Spells → SpellsConfig**
4. Create a prefab with the new spell component + assign `SpellData`
5. Register prefab in `SpellFactory`

### SpellData Fields

| Field | Description |
|---|---|
| Name | Spell name |
| Icon | UI sprite |
| Color | Visual color on grid |
| Duration | Effect duration in seconds |
| ChargeTime | Time before cast in seconds |
| Type | SpellType enum |
| EffectValue | Magnitude of effect |
| CollectSound | Sound on pickup |
| CastSound | Sound on cast |
| EffectSound | Looping sound during effect |

---

## 6. Main Menu & Character Select

### UX Flow

```
[Start]
  │
  ▼
Player1Slot ("Add Player")          StartButton (off-screen)
  │
  ● Click Player1Slot
  │   → PreviewPanel slides IN
  ▼
PreviewPanel (Race selection)
  │
  ● Click Race
  │   → ClassPanel slides IN
  ▼
ClassPanel (Class selection)        [X] Close button
  │
  ● Click Class
  │   → Player stats populated, Placeholder hidden
  │
  ● Click Confirm (toggle)
  │   → [not confirmed] PlayerConfig set, PreviewPanel OUT, StartButton IN
  │   → [confirmed]     selection reset, PreviewPanel IN, StartButton OUT
  │
  ● Click Reset [X]
  │   → Full reset, Placeholder shown, workflow restarts
  ▼
StartButton
  │
  ● Click Start → Loading screen → LevelScene
```

### Scene Hierarchy

```
Canvas
 ├── Player1Slot              ← Animator: PlayerSlotAnimator
 │    ├── Placeholder         ← "Add Player" + Reset button [X]
 │    └── Preview
 │         ├── PlayerName     (TMP_Text)
 │         ├── PlayerIcon     (Image)
 │         ├── HealthSlider   (Slider)
 │         ├── EnergySlider   (Slider)
 │         ├── SpeedSlider    (Slider)
 │         └── AttackSlider   (Slider)
 ├── PreviewPanel             ← Animator: PreviewPanelAnimator
 │    ├── Human / Undead / Angel / Devil Buttons
 │    └── ConfirmButton       ← toggle confirm / unconfirm
 ├── ClassPanel               ← Animator: ClassPanelAnimator
 │    ├── RaceTitle           (TMP_Text)
 │    ├── RaceDescription     (TMP_Text)
 │    ├── ClassContainer      (VerticalLayoutGroup)
 │    └── CloseButton
 ├── StartButton              ← Animator: StartButtonAnimator
 ├── LoadingScreen            ← SetActive false by default
 │    └── LoadingBar          (Slider)
 ├── CharacterPresenter       ← CharacterSelectPresenter
 ├── CharacterSelectEventManager
 ├── UIAnimationObserver
 └── UIAudioObserver
```

### Animator Controllers

**PreviewPanelAnimator / ClassPanelAnimator / StartButtonAnimator**

Parameter: `IsOpen` (Bool)

| State | Description |
|---|---|
| Idle | Panel off-screen (default) |
| SlideIn | Panel animates on-screen |
| SlideOut | Panel animates off-screen |

Transitions:
- `Idle → SlideIn` : IsOpen = true, Has Exit Time OFF
- `SlideIn → SlideOut` : IsOpen = false, Has Exit Time OFF
- `SlideOut → Idle` : exit time, no condition

**PlayerSlotAnimator**

Parameters: `Select` (Trigger), `ShowPreview` (Trigger), `Reset` (Trigger), `Confirmed` (Bool)

| State | Description |
|---|---|
| Placeholder | "Add Player" shown (default) |
| Selected | Slot highlighted |
| Preview | Player stats shown |
| Confirmed | Selection locked |

Transitions:
- `Placeholder → Selected` : trigger Select
- `Selected → Preview` : trigger ShowPreview
- `Preview → Confirmed` : Confirmed = true, Has Exit Time OFF
- `Confirmed → Preview` : Confirmed = false, Has Exit Time OFF
- `Any State → Placeholder` : trigger Reset, Has Exit Time OFF

### Inspector Reference Setup

**CharacterSelectPresenter**

| Field | Assign |
|---|---|
| Registry | PlayerDataRegistry asset |
| Player Config | PlayerConfig asset |
| Settings | GameSettingsConfig asset |
| Event Manager | CharacterSelectEventManager |
| Class Container | ClassContainer Transform in ClassPanel |
| Class Button Prefab | Button prefab from Assets/Prefabs/UI/ |
| Race Title | TMP_Text in ClassPanel |
| Race Description | TMP_Text in ClassPanel |
| Placeholder | Placeholder GameObject in Player1Slot |
| Player Name | TMP_Text in Player1Slot/Preview |
| Player Icon | Image in Player1Slot/Preview |
| Health / Energy / Speed / Attack Slider | Sliders in Player1Slot/Preview |
| Start Button | StartButton GameObject |
| Loading Screen | LoadingScreen GameObject |
| Loading Bar | Slider in LoadingScreen |

**UIAnimationObserver**

| Field | Assign |
|---|---|
| Event Manager | CharacterSelectEventManager |
| Preview Panel Animator | Animator on PreviewPanel |
| Class Panel Animator | Animator on ClassPanel |
| Player Slot Animator | Animator on Player1Slot |
| Start Button Animator | Animator on StartButton |

**UIAudioObserver**

| Field | Assign |
|---|---|
| Event Manager | CharacterSelectEventManager |
| Registry | PlayerDataRegistry asset |
| Audio Source | AudioSource component |
| Click | Generic click sound |
| Confirm | Confirmation sound |
| Cancel | Cancel / reset sound |

### Button OnClick() Connections

| Button | Method |
|---|---|
| Player1Slot | `CharacterPresenter.OnPlayerSlotClicked()` |
| Human / Undead / Angel / Devil | `CharacterPresenter.OnSelectHuman()` etc. |
| Close Button (ClassPanel) | `CharacterPresenter.OnPanelClosed()` |
| Confirm Button | `CharacterPresenter.OnConfirm()` |
| Reset Button [X] | `CharacterPresenter.OnPlayerReset()` |
| Start Game Button | `CharacterPresenter.OnStartGame()` |

### Panel Whoosh Sounds

Attach `PanelSoundHandler` to each animated panel (PreviewPanel, ClassPanel, StartButton).

| Field | Assign |
|---|---|
| Audio Source | AudioSource on the panel |
| Whoosh In | Slide-in sound clip |
| Whoosh Out | Slide-out sound clip |

Add Animation Events on each clip:
- SlideIn clip frame 0 → `PlayWhooshIn()`
- SlideOut clip frame 0 → `PlayWhooshOut()`

---

## 7. Event Reference

### PlayerEvent

Subscribe via `playerController.RegisterObserver(this)` or in `GameManager.HandlePlayerSpawned`.

| Event | When | Value | MaxValue |
|---|---|---|---|
| Walking | Moving at normal speed | CurrentSpeed | MoveSpeed |
| Running | Moving with speed boost | CurrentSpeed | MoveSpeed |
| Standing | Player stopped | 0 | MoveSpeed |
| Attacking | Player attacked | — | — |
| HealthChanged | HP changed | CurrentHealth | MaxHealth |
| EnergyChanged | Energy changed | CurrentEnergy | MaxEnergy |

### SpellEvent

Subscribe via `spellEventManager.RegisterObserver(this)`.

| Event | When | Spell | Value | Queue |
|---|---|---|---|---|
| Collected | Spell picked up | the spell | — | — |
| QueueChanged | Spell added or removed | — | — | current queue |
| ChargeTick | Every frame while charging | — | 0→1 | — |
| Cast | Spell cast on target | the spell | — | — |
| DurationTick | Every frame during effect | the spell | 1→0 | — |

### CharacterSelectEvent

Subscribe via `CharacterSelectEventManager.RegisterObserver(this)`.

| Event | Triggered by | UI Effect |
|---|---|---|
| PlayerSlotClicked | `OnPlayerSlotClicked()` | PreviewPanel IN, StartButton OUT |
| RaceSelected | `OnRaceSelected()` | ClassPanel IN |
| ClassSelected | `OnClassSelected()` | PlayerSlot ShowPreview |
| PanelClosed | `OnPanelClosed()` | ClassPanel OUT |
| PlayerConfirmed | `OnConfirm()` 1st click | PreviewPanel OUT, StartButton IN |
| SelectionReset | `OnConfirm()` 2nd click | PreviewPanel IN, StartButton OUT |
| PlayerReset | `OnPlayerReset()` | All panels OUT, PlayerSlot reset |

---

## 8. How to Add a New Player

1. **Create PlayerData asset** — `Assets → Create → Player → PlayerData`
   - Fill in all fields (stats, audio, prefab reference)

2. **Create or duplicate a Player prefab**
   - Ensure all components listed in section 3 are present
   - Assign `InputSystem_Actions` to `PlayerController`
   - Assign AudioSources to `PlayerSoundController`

3. **Create an Animator Controller**
   - Add `Speed` (Float) and `Attack` (Trigger) parameters
   - Set up Blend Tree for locomotion
   - Set up Attack state with Animation Events

4. **Add Animation Events to clips**
   - Attack clip: `OnAttack()` at impact frame, `OnAttackFinished()` at last frame
   - Walk/Run clip: `OnFootstep()` at each footfall

5. **Register in PlayerDataRegistry**
   - Open `PlayerDataRegistry` ScriptableObject
   - Add the new `PlayerData` to the **PlayerData** list
   - Ensure correct `RaceData` exists in the **Races** list

6. **Assign audio clips** in `PlayerData`
   - FootstepSounds, AttackSound, SelectSound
