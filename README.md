# ConvenienceStore — 세종 편의점 경영 시뮬레이션

> **Sejong University · OpenSource** 강의 팀 프로젝트  
> Unity 2D 턴제 편의점 경영 시뮬레이션

[![Repository](https://img.shields.io/badge/GitHub-SejongOpenSource%2FOpenSource-blue)](https://github.com/SejongOpenSource/OpenSource)

## 프로젝트 개요

| 항목 | 내용 |
|------|------|
| 과목 | OpenSource |
| 프로젝트 유형 | 팀 프로젝트 (4명) |
| 개발 기간 | 약 1개월 (2026.05 ~ 2026.06) |
| 버전 관리 | GitHub Flow |
| 엔진 | Unity 6.3 LTS (`6000.3.14f1`) |
| 저장소 | https://github.com/SejongOpenSource/OpenSource |

상권 투자, 상품 발주, 날씨 대응, 대출 관리를 통해 **30턴 이내 누적 매출 500만원**을 달성하는 것이 목표입니다.

## 팀원

| GitHub | 역할 | 담당 |
|--------|------|------|
| [westnowjin](https://github.com/westnowjin) | PM | 기획, 프로젝트 관리, 씬 통합, PR 리뷰 |
| [hlee0](https://github.com/hlee0) | 팀원 | 게임 코어 로직 (Manager, Data) |
| [JO SUBIN](https://github.com/JO-SUBIN) | 팀원 | UI 패널, HUD |
| 서현진 | 팀원 | Store/Player 시스템 |

> 팀원 GitHub ID·담당 영역은 실제 분담에 맞게 수정해 주세요.

## 게임 소개

### 목표

| 항목 | 내용 |
|------|------|
| 승리 조건 | 누적 매출 **500만원** 달성 |
| 패배 조건 | 자본금 **0원 미만** 또는 **30턴 소진** |
| 초기 자본금 | 50만원 |
| 최대 턴 | 30턴 (1턴 = 1영업일) |

**점수 공식:** `(30 - 달성 턴) × 10,000 - 잔여 재고 원가 - 잔여 대출 잔액`

### 턴 흐름

```
1. Upgrade    → 상권 투자 결정
2. Order      → 상품 발주, 날씨 확인, 대출
3. Simulation → 영업 시뮬레이션 (자동 진행)
4. Result     → 매출·재고 확인, 대출 상환
```

### 상품 (5종)

| 상품 | 원가 | 판매가 | 마진 |
|------|------|--------|------|
| 삼각김밥 | 800원 | 1,200원 | 400원 |
| 컵라면 | 700원 | 1,300원 | 600원 |
| 음료수 | 500원 | 1,000원 | 500원 |
| 도시락 | 3,500원 | 5,500원 | 2,000원 |
| 우산 | 2,000원 | 3,500원 | 1,500원 |

손님은 상권·날씨에 따른 **확률 가중치**로 상품을 선택합니다. 재고가 없으면 해당 손님의 구매는 무효 처리됩니다.

### 상권

| 상권 | 투자 비용 | 방문객 보너스 | 주요 효과 |
|------|-----------|---------------|-----------|
| 주택가 (기본) | 무료 | — | 시작 상권 |
| 학원가 | 50,000원 | +20% | 삼각김밥·컵라면 ×1.5 |
| 대학교 | 100,000원 | +50% | 음료수 ×1.4 |
| 오피스 | 150,000원 | +30% | 도시락 ×1.8 |
| 관광지 | 200,000원 | +60% | 전 상품 ×1.3 |

### 대출

| 항목 | 내용 |
|------|------|
| 1회 대출 한도 | 200,000원 |
| 최대 누적 대출 | 400,000원 |
| 이자율 | 잔액의 3% / 턴 |
| 상환 | Result 단계에서 자유 상환 |

## 실행 방법

```bash
git clone https://github.com/SejongOpenSource/OpenSource.git
```

1. [Unity Hub](https://unity.com/download)에서 **Unity 6000.3.14f1** 설치
2. 클론한 프로젝트를 Unity Hub로 열기
3. `Assets/Scenes/MainMenu.unity` 씬 실행
4. Play 버튼으로 게임 시작

### Unity Git 설정 (협업 시)

- **Version Control** → Visible Meta Files
- **Asset Serialization** → Force Text
- `Library/`, `Temp/`, `Logs/` 등은 `.gitignore`로 제외, `*.meta`는 반드시 커밋

## 기술 스택

| 분류 | 사용 기술 |
|------|-----------|
| 엔진 | Unity 6.3 LTS |
| 렌더링 | Universal Render Pipeline (URP) 2D |
| 입력 | Unity Input System |
| UI | Unity uGUI |
| 데이터 | ScriptableObject |

## 프로젝트 구조

```
Assets/
├── Scenes/
│   ├── MainMenu.unity          # 메인 메뉴
│   └── PlayerEconomy.unity     # 게임 플레이 씬
├── Scripts/
│   ├── Manager/                # GameManager, TurnManager, SalesAlgorithm 등
│   ├── Player/                 # StoreManager, InventoryManager, Loan, WeatherSystem
│   ├── Data/                   # ScriptableObject 데이터 정의
│   └── UI/                     # 페이즈별 UI 컨트롤러
├── Data/
│   ├── Products/               # 상품 ScriptableObject
│   └── CSV/District/           # 상권 ScriptableObject
├── Prefabs/UI/                 # UI 프리팹
└── Sprites/, Audio/            # 에셋
```

### 핵심 클래스

| 클래스 | 역할 |
|--------|------|
| `GameManager` | 전체 시스템 조율, 승패 판정 |
| `TurnManager` | 턴·페이즈 상태 머신 |
| `SalesAlgorithm` | 방문객 수 계산, 판매 시뮬레이션 |
| `StoreManager` | 자본금, 상권, 대출 잔액 관리 |
| `DataManager` | 상품·상권·날씨 데이터 조회 |

## 개발 프로세스 (GitHub Flow)

이 프로젝트는 **GitHub Flow** 기반으로 협업했습니다.

```
main (항상 실행 가능한 상태 유지)
  ↑
  PR (코드 리뷰 · 승인)
  ↑
feature/xxx (이슈 단위 작업 브랜치)
```

### 작업 순서

1. **Issue 생성** — `.github/ISSUE_TEMPLATE/issue_template.md` 양식 사용
2. **브랜치 생성** — `feature/기능명` 형식
3. **작업 및 커밋** — Conventional Commits 접두어 사용
4. **Pull Request** — `.github/pull_request_template.md` 작성
5. **코드 리뷰** — 팀원 리뷰 후 `main` 병합
6. **브랜치 삭제** — 병합 후 GitHub Actions로 자동 삭제 (`.github/workflows/delete-merged-branch.yml`)

### 커밋 컨벤션

| 접두어 | 용도 |
|--------|------|
| `feat:` | 새 기능 |
| `fix:` | 버그 수정 |
| `docs:` | 문서 변경 |
| `refactor:` | 리팩토링 |
| `test:` | 테스트 |

### 협업 규칙

- `main` 브랜치에 직접 push 금지
- PR은 최소 1명 이상 리뷰 후 병합
- 씬 파일(`.unity`)은 충돌 위험이 높아 담당자 1명이 관리
- 스크립트·프리팹 단위로 작업 분담

## 사용 에셋

- [Kenney.nl](https://kenney.nl/) (CC0)
- Unity Asset Store 무료 팩
- [OpenGameArt.org](https://opengameart.org/)

## 라이선스

본 프로젝트는 **세종대학교 OpenSource 강의** 팀 프로젝트입니다.  
게임 코드 및 프로젝트 구조에 대한 저작권은 팀원에게 있습니다.  
외부 에셋은 각 출처의 라이선스를 따릅니다.

### Sound & Background Music
* **Track:** CLEAR(Bit Shift)
* **Music from #Uppbeat (free for Creators!):** [https://uppbeat.io/t/kevin-macleod/bit-shift](https://uppbeat.io/t/kevin-macleod/bit-shift)
* **License code:** `4Z0YGUJO9WIOCLVQ`

* **Track:** CLEAR(Pixeltown)
* **Music from #Uppbeat (free for Creators!):** [https://uppbeat.io/t/color-parade/pixeltown](https://uppbeat.io/t/color-parade/pixeltown)
* **License code:** `BUJFOVLN1RGMSZO0`
