# UnityRPG

Unity 6.3 LTS 기반 3D Single-Player Action RPG Vertical Slice입니다.

전투 시스템을 중심으로 Enemy AI, Boss Pattern, Inventory  Equipment,
Quest, Save  Load, UI, Audio  VFX를 연결하여

MainMenu → Hub → Quest → Dungeon → Boss → Reward → Hub → Save  Continue

까지 하나의 완전한 Gameplay Loop로 구현했습니다.

---

## Overview

 항목  내용 
 ---  --- 
 Engine  Unity 6.3 LTS `6000.3.9f1` 
 Render Pipeline  URP 
 Language  C# 
 Platform  Windows 
 Development  1인 개발 
 Role  기획  Client Programming  UI  Data  Optimization  Build 

---

## Gameplay Loop

```text
MainMenu
↓
New Game  Continue
↓
Hub
↓
NPC Quest
↓
Dungeon
↓
Normal Enemy  Elite
↓
Boss
↓
Reward
↓
Hub Return
↓
Quest Turn-in
↓
Save  Continue
```

---

## Key Features

- CharacterController 기반 3D 이동  Dodge  Lock-On
- 3타 기본 공격과 4종 Active Skill
- `DamageInfo → IDamageable` 기반 공통 Damage Pipeline
- Melee  Ranged  Elite Enemy AI
- Phase와 4종 Pattern을 사용하는 Boss Battle
- Inventory  Equipment  Consumable
- `RuntimeStat + StatModifier` 기반 능력치 시스템
- NPC Interaction  Quest  Growth
- Dungeon Encounter 및 완료 상태 관리
- JSON 기반 Save  Load  Continue  Checkpoint
- Inventory  HUD  Pause  Settings UI
- Persistent Audio  Scene Transition  VFX Feedback
- Editor  Development Build 전용 Developer Console

---

## Architecture Highlights

### Static Data와 Runtime State 분리

```text
ScriptableObject
→ 변하지 않는 원본 데이터

Plain C# Runtime
→ 플레이 중 변경되는 상태

MonoBehaviour
→ Unity Lifecycle  Scene  Component 연결
```

Item, Skill, Quest, Stat 등의 원본 데이터와
실제 플레이 중 변경되는 Runtime 상태를 분리했습니다.

### Combat

```text
Attack  Skill  Projectile
↓
DamageInfo
↓
IDamageable
↓
PlayerHealth  EnemyHealth
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

`BossCombatController`는 Pattern 선택을 담당하고,
각 Pattern은 자신의 거리  Phase 조건과 실제 공격 행동을 담당합니다.

공통 `Windup → Active → Recovery → Cooldown` 생명주기는
`BossPatternBase`에서 관리합니다.

### Save  Load

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

Runtime과 JSON File IO를 분리했으며,
복원 시 Save Data를 먼저 검증한 뒤 Runtime에 적용합니다.

### Event-driven Presentation

```text
Gameplay State 변경
↓
Event
↓
HUD  UI  Visual
```

Gameplay Controller가 UI나 Visual을 직접 수정하지 않고
Runtime Event를 통해 Presentation 계층에 상태 변화를 전달합니다.

---

## Highlighted Code

### Boss Pattern Architecture

- [BossPatternBase.cs](Assets_ProjectScriptsAIBossBossPatternBase.cs)
- [BossCombatController.cs](Assets_ProjectScriptsAIBossBossCombatController.cs)

공통 Pattern 생명주기와 Pattern 선택 책임을 분리한 Boss 전투 구조입니다.

### Save  Load Architecture

- [SaveGameController.cs](Assets_ProjectScriptsInfrastructureSaveSaveGameController.cs)
- [PlayerItemSaveAdapter.cs](Assets_ProjectScriptsInfrastructureSavePlayerItemSaveAdapter.cs)
- [SaveFileService.cs](Assets_ProjectScriptsInfrastructureSaveSaveFileService.cs)

Gameplay Runtime, Save DTO, File IO의 책임을 분리하고
복원 전 데이터 검증을 수행합니다.

### Equipment  Stat

- [PlayerEquipmentController.cs](Assets_ProjectScriptsItemPlayerEquipmentController.cs)
- [RuntimeStat.cs](Assets_ProjectScriptsCharacterStatsRuntimeStat.cs)
- [StatModifier.cs](Assets_ProjectScriptsCharacterStatsStatModifier.cs)

장비 교체 실패 시 Rollback을 수행하며,
`StatModifier.Source`를 이용해 특정 장비가 적용한 능력치만 제거합니다.

### Developer Console

- [IConsoleCommand.cs](Assets_ProjectScriptsDeveloperToolsIConsoleCommand.cs)
- [CommandRegistry.cs](Assets_ProjectScriptsDeveloperToolsCommandRegistry.cs)
- [PlayerDeveloperCommandRegistrar.cs](Assets_ProjectScriptsDeveloperToolsPlayerDeveloperCommandRegistrar.cs)

Console Core와 Gameplay Dependency를 분리하고,
각 Command가 실제 Gameplay API를 재사용하도록 구성했습니다.

### Quest Runtime

- [PlayerQuestLog.cs](Assets_ProjectScriptsQuestPlayerQuestLog.cs)
- [RuntimeQuest.cs](Assets_ProjectScriptsQuestRuntimeQuest.cs)

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
→ Phase  Visibility 변경 시에만 갱신

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

### Save Restore 중 기존 Runtime 유실 방지

기존 Runtime을 먼저 초기화하지 않고

```text
Load
↓
Validate
↓
복원 가능한 상태 구성
↓
검증 성공
↓
Runtime 적용
```

순서로 변경하여 잘못된 Save Data가 기존 상태를 손상시키지 않도록 했습니다.

### Persistent Runtime의 Scene Lifecycle

`DontDestroyOnLoad` 객체와 Scene에 배치된 객체가 중복 생성되던 문제를 해결하기 위해
`GameRoot`, `GameplayRoot`, Scene Object의 수명을 분리했습니다.

### Windows Fullscreen 밝기 문제

최종 Windows Release Build에서 Windowed  Fullscreen의 밝기가 달라지는 문제를 확인하고
Graphics API를 분리 검증한 뒤 Direct3D11 구성으로 최종 확정했습니다.

---

## Project Structure

```text
Assets
└─ _Project
   ├─ Art
   ├─ Audio
   ├─ Data
   ├─ Editor
   ├─ Prefabs
   ├─ Scenes
   └─ Scripts
      ├─ AI
      ├─ Character
      ├─ Core
      ├─ DeveloperTools
      ├─ Infrastructure
      ├─ Item
      ├─ Quest
      └─ UI
```

Gameplay 및 프로젝트 전용 Asset은 `_Project` 아래에 분리하여 관리했습니다.

---

## Development & Validation

개별 기능 구현 이후 관련 시스템과의 Integration  Regression Test를 반복했습니다.

최종적으로 다음 전체 흐름을 다시 검증했습니다.

```text
MainMenu
→ New Game
→ Hub
→ Quest Accept
→ Dungeon
→ Enemy  Elite
→ Boss
→ Return Portal
→ Hub
→ Quest Turn-in
→ Save
→ Game Restart
→ Continue
```

Windows Release Build에서도 Scene Transition, Combat, Skill,
Windowed  Fullscreen 및 재실행을 확인했습니다.

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
Assets_ProjectScenesBootstrap.unity
```

Scene에서 Play하여 시작할 수 있습니다.

---

## Links

- 기술 포트폴리오 (Notion) 추후 연결
- Gameplay Video 추후 연결
- Portfolio PDF 추후 연결