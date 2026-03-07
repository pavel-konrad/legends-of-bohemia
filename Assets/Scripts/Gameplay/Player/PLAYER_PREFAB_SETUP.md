# Player Prefab – Setup Guide

## Přehled

Player prefab je vytvářen dynamicky přes `PlayerFactory` z dat uložených v `PlayerData` ScriptableObject.
Nikdy se nevytváří ručně v scéně — vždy přes factory.

---

## 1. PlayerData ScriptableObject

Vytvoř asset: **Assets → Create → Player → PlayerData**

| Field | Popis |
|---|---|
| Name | Jméno postavy |
| Icon | Sprite pro UI |
| Race | Human / Undead / Angel / Devil |
| Class | Warrior / Mage / Rogue / ... |
| MoveSpeed | Rychlost pohybu po gridu |
| MaxHealth | Maximum HP |
| MaxEnergy | Maximum Energy |
| AttackPower | Poškození útoku |
| IsRanged | True = ranged, False = melee |
| FootstepSounds | Pole AudioClipů (náhodný výběr) |
| SelectSound | Zvuk při výběru v character select |
| AttackSound | Pole AudioClipů útoku (náhodný výběr) |
| PlayerPrefab | Reference na prefab GameObject |

---

## 2. Prefab Hierarchy

```
PlayerPrefab
 ├── [Mesh / Model]        ← 3D model s Animator komponentem
 ├── PlayerController      ← hlavní script
 ├── PlayerEventManager    ← observer subject pro player eventy
 ├── PlayerAnimatorController ← observer, řídí Animator
 ├── PlayerSoundController ← zvuky (footstep + combat)
 ├── SpellController       ← fronta a cast spellů
 ├── SpellEventManager     ← observer subject pro spell eventy
 ├── SpellSoundController  ← zvuky spellů
 ├── AttackController      ← logika útoku, input
 ├── AudioSource (Footstep) ← přiřadit do PlayerSoundController._footstepAudio
 └── AudioSource (Combat)   ← přiřadit do PlayerSoundController._combatAudio
```

---

## 3. Komponenty – Inspector Reference Setup

### PlayerController
| Field | Přiřadit |
|---|---|
| Input Actions | InputSystem_Actions asset |

*Ostatní reference (`GridSystem`, `PlayerData`, spawn point) jsou předány přes `Initialize()` z `PlayerFactory` — nepřiřazovat v Inspektoru.*

### PlayerAnimatorController
*Žádné SerializeField — vše přes `GetComponent` v Awake.*

### PlayerSoundController
| Field | Přiřadit |
|---|---|
| Footstep Audio | AudioSource (Footstep) |
| Combat Audio | AudioSource (Combat) |

*`PlayerData` předán přes `Initialize()` z `PlayerFactory`.*

### AttackController
*Žádné SerializeField — `InputAction` a `GridSystem` předány přes `Initialize()` z `PlayerController`.*

### SpellSoundController
| Field | Přiřadit |
|---|---|
| Hits Audio | AudioSource pro jednorázové zvuky |
| Loop Audio | AudioSource pro loopované efekty |

---

## 4. Animator Controller

Vytvoř nový **Animator Controller** a přiřaď na `Animator` komponent modelu.

### Parametry
| Název | Typ | Popis |
|---|---|---|
| Speed | Float | 0 = Idle, 1 = Walk, 2 = Run |
| Attack | Trigger | Spustí attack animaci |

### States
| State | Clip | Popis |
|---|---|---|
| Idle | Idle clip | Výchozí stav |
| Walk/Run | Blend Tree | Speed parametr |
| Attack | Attack clip | Jednorázový útok |

### Transitions
| Z → Do | Podmínka |
|---|---|
| Any State → Attack | Trigger: Attack, Exit Time OFF |
| Attack → Idle | Exit Time (po dohrání clipu) |

### Blend Tree (pohyb)
- Type: 1D, parametr `Speed`
- 0.0 → Idle clip
- 1.0 → Walk clip
- 2.0 → Run clip

---

## 5. Animation Events

### Attack clip
| Frame | Metoda | Objekt |
|---|---|---|
| Moment úderu | `OnAttack()` | `PlayerSoundController` |
| Poslední frame | `OnAttackFinished()` | `PlayerController` |

### Walk/Run clip
| Frame | Metoda | Objekt |
|---|---|---|
| Každý dopad nohy | `OnFootstep()` | `PlayerSoundController` |

---

## 6. Audio Mixer Setup

| AudioSource | Mixer Group |
|---|---|
| Footstep Audio | SFX/Footsteps |
| Combat Audio | SFX/Combat |

---

## 7. PlayerEvents Reference

Observery se registrují přes `playerController.RegisterObserver(this)` v `GameManager.HandlePlayerSpawned`.

| Event | Kdy | Value | MaxValue |
|---|---|---|---|
| Walking | Pohyb normální rychlostí | CurrentSpeed | MoveSpeed |
| Running | Pohyb se speed boostem | CurrentSpeed | MoveSpeed |
| Standing | Hráč se zastavil | 0 | MoveSpeed |
| Attacking | Hráč zaútočil | — | — |
| HealthChanged | HP se změnilo | CurrentHealth | MaxHealth |
| EnergyChanged | Energy se změnila | CurrentEnergy | MaxEnergy |

---

## 8. Registrace do PlayerDataRegistry

Aby byl hráč dostupný v character select, přidej `PlayerData` asset do:

**PlayerDataRegistry ScriptableObject → PlayerData list**

A přiřaď odpovídající `RaceData` do **Races list**.

---

## 9. Životní cyklus

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
