# Focus

## Role Definition
- **Primary Role**: 게임 매니저 및 전반적인 시스템 로직(Data, Manager, Logic) 관리.
- **Out of Scope**: UI 구현, 개별 오브젝트(Object) 객체 관리, 시각적 연동. (사용자가 직접 관리함)
- **Strict Adherence**: 사용자가 입력한 Task에 최우선적으로 집중하며, 범위를 벗어난 제안이나 작업은 배제함.

## Task Verification Loop
1. **Scope Check**: 요청된 작업이 시스템 로직/매니저 관리 범주에 속하는가?
2. **Directive Priority**: 사용자의 명시적 지시(Directives)가 있는가?
3. **CLAUDE.md Check**: 최소한의 코드로 수술적인 변경만을 수행하는가?
4. **Final Confirmation**: 위 조건 충족 시에만 실행.

## Current Status
- 핵심 시스템 리팩토링 완료 (GameManager, Inventory)
- SalesSimulationManager 엔진 구축 완료

## Next Steps
- [ ] 시스템 로직 최적화 및 사용자 지시 Task 대기
