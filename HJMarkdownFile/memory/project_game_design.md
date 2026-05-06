# 세종 편의점 경영 시뮬레이션 — 기획 요약

## 기본 정보

| 항목 | 내용 |
|------|------|
| 장르 | 턴제 편의점 경영 시뮬레이션 (2D, Unity) |
| 엔진 | Unity 6.3 LTS (6000.3.14f1) |
| 제출 기한 | **2026-06-08 (월)** |
| 팀 구성 | PM 1명 + 팀원 3명 |
| 목표 | 누적 매출 500만원 달성 (자본금 고갈 전, 30턴 이내) |
| 초기 자본금 | 50만원 |
| 최대 턴 수 | 30턴 (1턴 = 1영업일) |

---

## 승리/패배 & 점수

- **승리**: 누적 매출 500만원 달성
- **패배**: 자본금 0원 이하 OR 30턴 미달성
- **점수** = (30 - 달성 턴) × 10,000 - 잔여 재고 × 원가 - 잔여 대출 잔액

---

## 상품 5종

| 상품 | 원가 | 판매가 | 마진 | 특성 |
|------|------|--------|------|------|
| 삼각김밥 | 800 | 1,200 | 400 | 학원가 ×1.5 |
| 컵라면 | 700 | 1,300 | 600 | 학원가 ×1.5, 흐린날 ×1.2 |
| 음료수 | 500 | 1,000 | 500 | 폭염 ×2.0, 대학교 ×1.4 |
| 도시락 | 3,500 | 5,500 | 2,000 | 오피스 ×1.8, 관광지 ×1.3 |
| 우산 | 2,000 | 3,500 | 1,500 | 비(오전) ×3.0, 비(오후) ×2.0, 맑음 0.1 |

---

## 날씨 시스템 (오전/오후 분리)

| 날씨 | 방문객 변화 | 주요 수요 변화 |
|------|------------|---------------|
| 화창 | 기본 | 없음 |
| 비 | -10% | 우산 ×3.0(오전) / ×2.0(오후) |
| 폭염 | +5% | 음료수 ×2.0 |
| 흐림 | -20% | 컵라면 ×1.2 |
| 눈 | -30% | 핫 음료 ×1.5 |

수요 가중치는 **확률 가중치** 방식 — 비 오는 날 모든 손님이 우산을 사는 게 아니라, 우산 손님 비율이 높아지는 구조.

---

## 상권 업그레이드 시스템 (누적 적용)

| 상권 | 비용 | 효과 |
|------|------|------|
| 주택가 (기본) | 무료 | 하루 방문객 20명 |
| 학원가 | 50,000 | 방문객 +20%, 삼각김밥·컵라면 ×1.5 |
| 대학교 | 100,000 | 방문객 +50%, 음료수 ×1.4 |
| 오피스 | 150,000 | 방문객 +30%, 도시락 ×1.8 |
| 관광지 | 200,000 | 방문객 +60%, 전체 수요 변동성 ↑ |

---

## 대출 시스템

| 항목 | 내용 |
|------|------|
| 1회 한도 | 200,000원 |
| 최대 누적 | 400,000원 |
| 이자율 | 잔액의 3% / 턴 |
| 이자 차감 | 매 턴 결과 단계 |
| 상환 | 결과 단계 자유 상환 |
| 잔여 대출 | 게임 종료 시 점수 차감 |

---

## 턴 구조 (4 Steps)

```
STEP 1: 상권 업그레이드 결정 (플레이어 조작 - UI Panel)
STEP 2: 발주 + 날씨 확인 + 외상 (플레이어 조작 - UI Panel)
STEP 3: 영업 시뮬레이션 (Unity Coroutine 자동 진행 - 2D 씬 애니메이션)
STEP 4: 결과 확인 + 대출 상환 (UI Panel)
```

---

## 손님 이동 흐름 (STEP 3)

```
입구 스폰 → 목표 진열대 이동 → 계산대 이동 → 출구 퇴장 → Destroy
```

- Coroutine + Vector2.MoveTowards 구현 (NavMesh 불필요)
- 4방향 걷기 Sprite Sheet 애니메이션 (Animator Controller)
- 방향에 따라 animator.SetFloat("DirX", dir.x) 로 자동 전환

---

## 핵심 클래스 구조

```
Assets/Scripts/
├── Core/
│   ├── GameManager.cs       # 싱글톤, 전체 상태, 씬 전환
│   ├── TurnManager.cs       # 턴 흐름 (STEP 1→2→3→4)
│   └── GameState.cs         # 자본금, 누적매출, 현재 턴 데이터
│
├── Store/
│   ├── StoreManager.cs      # 편의점 운영 (판매·수익 계산)
│   ├── InventoryManager.cs  # 재고·발주·가격 관리
│   ├── WeatherSystem.cs     # 날씨 결정·수요 가중치
│   ├── CommerceZone.cs      # 상권 업그레이드 상태
│   └── LoanSystem.cs        # 대출·이자·상환
│
├── Customer/
│   ├── CustomerSpawner.cs   # 확률 기반 스폰 (Coroutine)
│   ├── CustomerController.cs # 손님 이동
│   └── ShelfManager.cs      # 진열대 위치 레퍼런스
│
├── UI/
│   ├── UpgradeScreen.cs     # STEP 1 패널
│   ├── OrderScreen.cs       # STEP 2 패널
│   ├── GameHUD.cs           # STEP 3 실시간 HUD
│   └── ResultScreen.cs      # STEP 4 패널
│
└── Data/
    ├── ProductData.cs       # ScriptableObject - 상품 데이터
    └── ZoneData.cs          # ScriptableObject - 상권 데이터
```

---

## Git Flow 전략

```
master (직접 push 금지)
  └─ develop (직접 push 금지)
       ├─ setting              초기 Unity 세팅 + .gitignore
       ├─ feature/core         GameManager·TurnManager·GameState
       ├─ feature/store        StoreManager·Inventory·Weather·Loan·Zone
       ├─ feature/customer     CustomerSpawner·CustomerController·Animator
       ├─ feature/ui           4개 UI 패널 + HUD
       └─ release/v1.0
master ← hotfix/*
```

**필수 규칙**: Issue 생성 → feature 브랜치 작업 → PR → PM 리뷰·승인 → develop 병합
**커밋 접두어**: feat: / fix: / docs: / refactor: / test:

**Unity Git 필수 설정**:
- .gitignore: Library/, Temp/, Logs/, obj/, Build/ 제외 (*.meta는 반드시 커밋)
- Editor: Version Control → Visible Meta Files
- Editor: Asset Serialization → Force Text

---

## 전체 일정

| 주차 | 기간 | 목표 |
|------|------|------|
| W1 | 5/3~5/9 | 기획 확정, Unity 초기화, gitignore+Editor 설정, setting 브랜치 병합 |
| W2 | 5/10~5/16 | feature/core (GameManager·TurnManager·GameState) + feature/store (Weather·Inventory·Loan) |
| W3 | 5/17~5/23 | feature/store 완성 (Zone·StoreManager) + feature/customer (Spawner·Controller·Animator) |
| W4 | 5/24~5/30 | feature/ui (4개 패널·HUD) + 에셋 교체 + 씬 통합 + 전체 연결 테스트 |
| W5 | 5/31~6/7 | QA, 밸런스 조정, release/v1.0, 보고서 |
| **D-Day** | **6/8 (월)** | **최종 제출** |

---

## 기타 참고

- 수요 가중치 수치는 QA 단계에서 밸런스 조정 예정
- 씬 파일은 1인 담당 원칙 (Game.unity = PM), 팀원은 Script·Prefab 단위 작업
- 에셋: kenney.nl (CC0), Unity Asset Store 무료팩, itch.io, opengameart.org 활용
- 소나기 같은 이벤트 발생 기능 추가 고려 중 (기획서 마지막 메모)
