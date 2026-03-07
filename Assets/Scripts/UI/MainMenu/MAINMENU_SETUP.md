# Main Menu – Character Select Setup Guide

## UX Flow

```
[Start]
  │
  ▼
Player1Slot (placeholder "Add Player")    StartButton (mimo scénu)
  │
  ● Click Player1Slot
  │   → PreviewPanel slides IN
  │   → Player1Slot: Select animace
  ▼
PreviewPanel (Race selection)
  │
  ● Click Race (Human / Undead / Angel / Devil)
  │   → ClassPanel slides IN
  │   → RaceTitle + RaceDescription se aktualizují
  ▼
ClassPanel (Class selection)              [X] Close button
  │
  ● Click Class
  │   → Player1Slot: ShowPreview animace
  │   → Placeholder schován
  │   → ClassPanel zůstává otevřený
  │
  ● Click Close (X) na ClassPanel
  │   → ClassPanel slides OUT
  │   → PreviewPanel zůstává otevřený
  │
  ● Click Confirm (toggle)
  │   → [není confirmed] PlayerConfig nastaven, Player1Slot: Confirmed stav
  │                       PreviewPanel slides OUT, StartButton slides IN
  │   → [je confirmed]   Reset výběru, Player1Slot: Preview stav
  │                       PreviewPanel slides IN, StartButton slides OUT
  │
  ● Click Reset [X]
  │   → Vše resetováno, Placeholder zobrazen, workflow od začátku
  ▼
StartButton
  │
  ● Click Start
  │   → Loading screen
  │   → LevelScene
```

---

## Hierarchy struktura

```
Canvas
 ├── Player1Slot              ← Animator: PlayerSlotAnimator
 │    ├── Placeholder         ← "Add Player" text/ikona + Reset button [X]
 │    └── Preview             ← statistiky (skryté v Placeholder stavu)
 │         ├── PlayerName     (TMP_Text)
 │         ├── PlayerIcon     (Image)
 │         ├── HealthSlider   (Slider)
 │         ├── EnergySlider   (Slider)
 │         ├── SpeedSlider    (Slider)
 │         └── AttackSlider   (Slider)
 ├── PreviewPanel             ← Animator: PreviewPanelAnimator (mimo scénu v Idle)
 │    ├── Human Button
 │    ├── Undead Button
 │    ├── Angel Button
 │    ├── Devil Button
 │    └── ConfirmButton       ← toggle: confirm / unconfirm
 ├── ClassPanel               ← Animator: ClassPanelAnimator (mimo scénu v Idle)
 │    ├── RaceTitle           (TMP_Text)
 │    ├── RaceDescription     (TMP_Text)
 │    ├── ClassContainer      ← VerticalLayoutGroup
 │    └── CloseButton
 ├── StartButton              ← Animator: StartButtonAnimator (mimo scénu v Idle)
 ├── LoadingScreen            ← defaultně neaktivní (SetActive false)
 │    └── LoadingBar          (Slider)
 ├── CharacterPresenter       ← CharacterSelectPresenter script
 ├── CharacterSelectEventManager
 └── UIAnimationObserver      ← UIAnimationObserver script
```

---

## Animator Controllers

### PreviewPanelAnimator / ClassPanelAnimator / StartButtonAnimator
**Parametry:** `IsOpen` (Bool)

| State    | Popis                      |
|----------|----------------------------|
| Idle     | Panel mimo scénu (default) |
| SlideIn  | Panel vjíždí do scény      |
| SlideOut | Panel odjíždí mimo scénu   |

**Transitions:**
- `Idle → SlideIn` : IsOpen = true, Has Exit Time = OFF
- `SlideIn → SlideOut` : IsOpen = false, Has Exit Time = OFF
- `SlideOut → Idle` : exit time, bez podmínky

---

### PlayerSlotAnimator
**Parametry:** `Select` (Trigger), `ShowPreview` (Trigger), `Reset` (Trigger), `Confirmed` (Bool)

| State       | Popis                            |
|-------------|----------------------------------|
| Placeholder | "Add Player" zobrazeno (default) |
| Selected    | Slot zvýrazněn                   |
| Preview     | Zobrazeny statistiky hráče       |
| Confirmed   | Výběr potvrzen                   |

**Transitions:**
- `Placeholder → Selected` : trigger Select
- `Selected → Preview` : trigger ShowPreview
- `Preview → Confirmed` : Confirmed = true, Has Exit Time = OFF
- `Confirmed → Preview` : Confirmed = false, Has Exit Time = OFF
- `Any State → Placeholder` : trigger Reset, Has Exit Time = OFF

---

## Inspector Reference Setup

### CharacterSelectPresenter
| Field | Přiřadit |
|---|---|
| Registry | PlayerDataRegistry asset |
| Player Config | PlayerConfig asset |
| Settings | GameSettingsConfig asset |
| Event Manager | CharacterSelectEventManager GameObject |
| Class Container | ClassContainer Transform v ClassPanel |
| Class Button Prefab | Button prefab z Assets/Prefabs/UI/ |
| Race Title | TMP_Text v ClassPanel |
| Race Description | TMP_Text v ClassPanel |
| Placeholder | Placeholder GameObject v Player1Slot |
| Player Name | TMP_Text v Player1Slot/Preview |
| Player Icon | Image v Player1Slot/Preview |
| Health Slider | Slider v Player1Slot/Preview |
| Energy Slider | Slider v Player1Slot/Preview |
| Speed Slider | Slider v Player1Slot/Preview |
| Attack Slider | Slider v Player1Slot/Preview |
| Start Button | StartButton GameObject |
| Loading Screen | LoadingScreen GameObject |
| Loading Bar | Slider v LoadingScreen |

### UIAnimationObserver
| Field | Přiřadit |
|---|---|
| Event Manager | CharacterSelectEventManager GameObject |
| Preview Panel Animator | Animator na PreviewPanel |
| Class Panel Animator | Animator na ClassPanel |
| Player Slot Animator | Animator na Player1Slot |
| Start Button Animator | Animator na StartButton |

---

## Button OnClick() Napojení

| Tlačítko | Metoda |
|---|---|
| Player1Slot | `CharacterPresenter.OnPlayerSlotClicked()` |
| Human Button | `CharacterPresenter.OnSelectHuman()` |
| Undead Button | `CharacterPresenter.OnSelectUndead()` |
| Angel Button | `CharacterPresenter.OnSelectAngel()` |
| Devil Button | `CharacterPresenter.OnSelectDevil()` |
| Close Button (ClassPanel) | `CharacterPresenter.OnPanelClosed()` |
| Confirm Button (PreviewPanel) | `CharacterPresenter.OnConfirm()` |
| Reset Button [X] (Player1Slot) | `CharacterPresenter.OnPlayerReset()` |
| Start Game Button | `CharacterPresenter.OnStartGame()` |

---

## Events přehled

| Event | Spouští | Animace |
|---|---|---|
| PlayerSlotClicked | OnPlayerSlotClicked() | PreviewPanel IN, StartButton OUT, PlayerSlot Select |
| RaceSelected | OnRaceSelected() | ClassPanel IN |
| ClassSelected | OnClassSelected() | PlayerSlot ShowPreview |
| PanelClosed | OnPanelClosed() | ClassPanel OUT |
| PlayerConfirmed | OnConfirm() [1. klik] | PreviewPanel OUT, StartButton IN, PlayerSlot Confirmed=true |
| SelectionReset | OnConfirm() [2. klik] | PreviewPanel IN, StartButton OUT, PlayerSlot Confirmed=false |
| PlayerReset | OnPlayerReset() | vše OUT, PlayerSlot Reset trigger |

---

## Audio (UIAudioObserver)

| Event | Zvuk |
|---|---|
| PlayerSlotClicked | UI click / whoosh in |
| RaceSelected | UI select |
| ClassSelected | Character select / fanfare |
| PlayerConfirmed | Confirm / seal |
| SelectionReset | UI click |
| PlayerReset | Whoosh out |
| PanelClosed | Whoosh out |
