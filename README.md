# UnityRPG

Unity 6.3 LTS 기반 3D Single-Player Action RPG Vertical Slice입니다.

전투 시스템을 중심으로 Enemy AI, Boss Pattern, Inventory / Equipment,
Quest, Save / Load, UI, Audio / VFX를 연결하여

**MainMenu → Hub → Quest → Dungeon → Boss → Reward → Hub → Save / Continue**

까지 하나의 완전한 Gameplay Loop로 구현했습니다.

[UnityRPG 실제 게임플레이 보기 (YouTube)](https://youtu.be/m8P98AnVkJM)

---

## Overview

| 항목 | 내용 |
| --- | --- |
| Engine | Unity 6.3 LTS `6000.3.9f1` |
| Render Pipeline | URP |
| Language | C# |
| Platform | Windows |
| Development | 1인 개발 |
| Role | 기획 / Client Programming / UI / Data / Optimization / Build |

---

## Gameplay Loop

```text
MainMenu
↓
New Game / Continue
↓
Hub
↓
NPC Quest
↓
Dungeon
↓
Normal Enemy / Elite
↓
Boss
↓
Reward
↓
Hub Return
↓
Quest Turn-in
↓
Save / Continue
```

---

## Key Features

- CharacterController 기반 3D 이동 / Dodge / Lock-On
- 3타 기본 공격과 4종 Active Skill
- `DamageInfo → IDamageable` 기반 공통 Damage Pipeline
- Melee / Ranged / Elite Enemy AI
- Phase와 4종 Pattern을 사용하는 Boss Battle
- Inventory / Equipment / Consumable
- `RuntimeStat + StatModifier` 기반 능력치 시스템
- NPC Interaction / Quest / Growth
- Dungeon Encounter 및 완료 상태 관리
- JSON 기반 Save / Load / Continue / Checkpoint
- Inventory / HUD / Pause / Settings UI
- Persistent Audio / Scene Transition / VFX Feedback
- Editor / Development Build 전용 Developer Console

---

## Architecture Highlights

### Static Data와 Runtime State 분리

```text
ScriptableObject
→ 변하지 않는 원본 데이터

Plain C# Runtime
→ 플레이 중 변경되는 상태

MonoBehaviour
→ Unity Lifecycle / Scene / Component 연결
```

Item, Skill, Quest, Stat 등의 원본 데이터와
실제 플레이 중 변경되는 Runtime 상태를 분리했습니다.

### Combat

```text
Attack / Skill / Projectile
↓
DamageInfo
↓
IDamageable
↓
PlayerHealth / EnemyHealth
```

공격자와 피해 수신자의 구체 타입 의존성을 줄이고
기본 공격, Skill, Projectile이 동일한 Damage 흐름을 사용합니다.

### Boss Pattern

```text
BossCombatController
↓
사용 가능한 Pattern 후보 수집
↓
BossPatternBase
├ Heavy Slash
├ Ground Slam
├ Charge
└ Shockwave
```

`BossCombatController`는 실행 조건을 만족하는 Pattern 후보를 모아 무작위로 선택합니다.
직전 Pattern은 먼저 제외하고, 다른 후보가 없으면 직전 Pattern을 포함해 다시 후보를 모읍니다.
거리와 보스 Phase 등 패턴별 실행 조건과 공격 행동은 각 Pattern에서 처리합니다.

공통 `Windup → Active → Recovery → Cooldown` 생명주기는
`BossPatternBase`에서 관리합니다.

### Save / Load

```text
Gameplay Runtime
↓
Save Adapter
↓
Save DTO
↓
SaveGameController
↓
SaveFileService
↓
save_0.json
```

Runtime과 JSON File I/O를 분리했습니다.
`SaveFileService`가 JSON을 읽고 저장 버전과 필수 데이터를 검사한 뒤,
`SaveGameController`가 현재 씬을 확인하고 각 Adapter를 순서대로 호출합니다.

### Event-driven Presentation

```text
Gameplay State 변경
↓
Event
↓
HUD / UI / Visual
```

Gameplay Controller가 UI나 Visual을 직접 수정하지 않고
Runtime Event를 통해 Presentation 계층에 상태 변화를 전달합니다.

---

## Highlighted Code

### Boss Pattern Architecture

- [BossPatternBase.cs](Assets/_Project/Scripts/AI/Boss/BossPatternBase.cs)
- [BossCombatController.cs](Assets/_Project/Scripts/AI/Boss/BossCombatController.cs)

공통 Pattern 생명주기와 Pattern 선택 책임을 분리한 Boss 전투 구조입니다.

### Save / Load Architecture

- [SaveGameController.cs](Assets/_Project/Scripts/Infrastructure/Save/SaveGameController.cs)
- [PlayerItemSaveAdapter.cs](Assets/_Project/Scripts/Infrastructure/Save/PlayerItemSaveAdapter.cs)
- [SaveFileService.cs](Assets/_Project/Scripts/Infrastructure/Save/SaveFileService.cs)

Gameplay Runtime, Save DTO, File I/O의 책임을 분리했습니다.
`PlayerItemSaveAdapter`는 인벤토리와 장비를 비우기 전에 아이템 ID·수량, 장비 슬롯·중복 여부 등을 검사합니다.

### Equipment / Stat

- [PlayerEquipmentController.cs](Assets/_Project/Scripts/Item/PlayerEquipmentController.cs)
- [RuntimeStat.cs](Assets/_Project/Scripts/Character/Stats/RuntimeStat.cs)
- [StatModifier.cs](Assets/_Project/Scripts/Character/Stats/StatModifier.cs)

장비 교체 중 기존 장비를 인벤토리에 돌려놓지 못하면, 먼저 꺼낸 새 장비를 인벤토리에 다시 넣고 실패를 반환합니다.
정상적으로 교체할 때는 `StatModifier.Source`로 기존 장비가 적용한 능력치 보정만 제거합니다.

### Developer Console

- [IConsoleCommand.cs](Assets/_Project/Scripts/DeveloperTools/IConsoleCommand.cs)
- [CommandRegistry.cs](Assets/_Project/Scripts/DeveloperTools/CommandRegistry.cs)
- [PlayerDeveloperCommandRegistrar.cs](Assets/_Project/Scripts/DeveloperTools/PlayerDeveloperCommandRegistrar.cs)

Console Core와 Gameplay Dependency를 분리하고,
각 Command가 실제 Gameplay API를 재사용하도록 구성했습니다.

### Quest Runtime

- [PlayerQuestLog.cs](Assets/_Project/Scripts/Quest/PlayerQuestLog.cs)
- [RuntimeQuest.cs](Assets/_Project/Scripts/Quest/RuntimeQuest.cs)

`QuestDefinition`의 정적 데이터와
Player별 Quest 진행 상태를 분리하고 상태 전이를 관리합니다.

---

## Runtime Optimization

반복적으로 실행되는 Runtime 경로를 중심으로 필요한 부분만 최적화했습니다.

```text
Dash Slash
→ OverlapSphereNonAlloc
→ 고정 Collider Buffer 재사용

Projectile
→ SphereCastNonAlloc
→ 고정 RaycastHit Buffer
→ Array.Sort 제거

Boss HUD
→ Damage Event 기반 HP 갱신
→ Phase / Visibility 변경 시에만 갱신

Damage Flash
→ MaterialPropertyBlock
→ Material Instance 생성 방지
```

Object Pooling은 현재 Vertical Slice의 동시 객체 수와 생성 빈도를 기준으로
관리 복잡도 대비 성능 이득이 제한적이라고 판단하여 도입하지 않았습니다.

측정하지 않은 성능 향상 수치는 제시하지 않았으며,
최적화 후 전체 Gameplay Regression Test를 다시 수행했습니다.

---

## Troubleshooting Highlights

### 인벤토리·장비 복원 전 검사

저장된 인벤토리나 장비 데이터가 유효한지 확인하기 전에 기존 값을 비우면, 복원에 실패했을 때 해당 상태가 유실될 수 있습니다. `PlayerItemSaveAdapter`에서는 다음 순서로 처리합니다.

```text
인벤토리·장비 데이터 검사
↓
검사를 통과하면 인벤토리·장비 상태 초기화
↓
인벤토리·장비 복원
```

전체 로드는 성장, 인벤토리·장비, 퀘스트, 인카운터, 체크포인트, 위치 순으로 적용합니다. 중간 단계에서 실패하면 `RestoreFailed`를 반환합니다. 앞 단계에서 적용한 값은 남을 수 있습니다.

### Persistent Runtime의 Scene Lifecycle

`DontDestroyOnLoad` 객체와 Scene에 배치된 객체가 중복 생성되던 문제를 해결하기 위해
`GameRoot`, `GameplayRoot`, Scene Object의 수명을 분리했습니다.

### Windows Fullscreen 밝기 문제

최종 Windows Release Build에서 Windowed / Fullscreen의 밝기가 달라지는 문제를 확인하고
Graphics API를 분리 검증한 뒤 Direct3D11 구성으로 최종 확정했습니다.

---

## Project Structure

```text
Assets/
└─ _Project/
   ├─ Art/
   ├─ Audio/
   ├─ Data/
   ├─ Editor/
   ├─ Prefabs/
   ├─ Scenes/
   └─ Scripts/
      ├─ AI/
      ├─ Character/
      ├─ Core/
      ├─ DeveloperTools/
      ├─ Infrastructure/
      ├─ Item/
      ├─ Quest/
      └─ UI/
```

Gameplay 및 프로젝트 전용 Asset은 `_Project` 아래에 분리하여 관리했습니다.

---

## Development & Validation

개별 기능 구현 이후 관련 시스템과의 Integration / Regression Test를 반복했습니다.

최종적으로 다음 전체 흐름을 다시 검증했습니다.

```text
MainMenu
→ New Game
→ Hub
→ Quest Accept
→ Dungeon
→ Enemy / Elite
→ Boss
→ Return Portal
→ Hub
→ Quest Turn-in
→ Save
→ Game Restart
→ Continue
```

Windows Release Build에서도 Scene Transition, Combat, Skill,
Windowed / Fullscreen 및 재실행을 확인했습니다.

---

## Run

권장 환경

```text
Unity 6000.3.9f1
URP
Windows
Direct3D11
```

프로젝트를 Unity Hub에서 열고

```text
Assets/_Project/Scenes/Bootstrap.unity
```

Scene에서 Play하여 시작할 수 있습니다.

---

## Links

- [기술 포트폴리오 (Notion)](https://zinc-lipstick-51c.notion.site/UnityRPG-3cdd3addd2cb802281d9ef23c339acf6)
- [게임플레이 영상 (YouTube)](https://youtu.be/m8P98AnVkJM)
- Portfolio PDF 추후 연결
