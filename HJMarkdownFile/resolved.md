# Resolved

## [Q-001] GameManager.cs ✓
- Conclusion: 싱글톤, TurnPhase enum(Upgrade/Order/Simulation/Result), AdvancePhase(), AddSales(), CalculateScore(remainingStockCost, remainingDebt), CheckWin/Lose, StoreManager·Loan·WeatherSystem 연동
- Key file: Assets/Scripts/Manager/GameManager.cs
- Date: 2026-05-06

## [Q-003] TradeData & Inventory Implementation ✓
- Conclusion: TradeData ScriptableObject 정의 및 5종 상품 Asset(삼각김밥, 컵라면, 음료수, 도시락, 우산) 생성 완료. Inventory.cs에서 발주(Order), 판매(Sell), 재고 원가 계산 로직 구현 및 데이터 검증 로직 추가.
- Key files: Assets/Scripts/Data/TradeData.cs, Assets/Scripts/Player/Inventory.cs, Assets/Data/Products/*.asset
- Date: 2026-05-11

## [Q-004] Sales Simulation Logic ✓
- Conclusion: SalesSimulationManager 구현 완료. 날씨(오전/오후)와 상권 데이터(DistrictData) 가중치를 적용하여 방문객 수 및 상품별 판매 확률 계산. TurnManager 연동을 통해 단계별 자동 실행(날씨 생성 -> 시뮬레이션).
- Key files: Assets/Scripts/Manager/SalesSimulationManager.cs, Assets/Scripts/Manager/TurnManager.cs, Assets/Scripts/Manager/GameManager.cs
- Date: 2026-05-11

---
