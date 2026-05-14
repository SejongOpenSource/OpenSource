# Resolved

## [Q-001] GameManager.cs ✓
- Conclusion: 싱글톤, TurnPhase enum(Upgrade/Order/Simulation/Result), AdvancePhase(), AddSales(), CalculateScore(remainingStockCost, remainingDebt), CheckWin/Lose, StoreManager·Loan·WeatherSystem 연동
- Key file: Assets/Scripts/Manager/GameManager.cs
- Date: 2026-05-06

## [Q-002] InventoryManager.cs ✓
- Conclusion: 상품 5종 재고 관리, 발주 시 StoreManager.SpendMoney 연동, 판매 시 GameManager.AddSales 및 StoreManager.AddMoney 연동, 잔여 재고 원가 합산(Penalty) 기능 구현.
- Key file: Assets/Scripts/Player/InventoryManager.cs, Assets/Scripts/Manager/GameManager.cs
- Date: 2026-05-14

## [Q-003] TradeData ScriptableObject ✓
- Conclusion: TradeData.cs (ScriptableObject) 구현 완료. 상품 5종(삼각김밥, 컵라면, 음료수, 도시락, 우산)에 대한 .asset 파일 생성 및 InventoryManager/ItemDataManager 연동 완료.
- Key file: Assets/Scripts/Player/TradeData.cs, Assets/Data/TradeData/*.asset, Assets/Scripts/Player/InventoryManager.cs
- Date: 2026-05-14

## [Q-004] DistrictDataManager Consolidation ✓
- Conclusion: 중복된 DistrictDataManager(Manager 폴더)를 제거하고 ScriptableObject 버전(Data 폴더)으로 통합. Commerce Enum 중복 제거 및 DataManager 연동 방식 변경(GetComponent -> Inspector 할당).
- Key file: Assets/Scripts/Data/DistrictDataManager.cs, Assets/Scripts/Manager/DataManager.cs, Assets/Scripts/Player/DistrictData.cs
- Date: 2026-05-14

---
