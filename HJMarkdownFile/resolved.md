# Resolved

## [Q-001] GameManager.cs ✓
- Conclusion: 싱글톤, TurnPhase enum(Upgrade/Order/Simulation/Result), AdvancePhase(), AddSales(), CalculateScore(remainingStockCost, remainingDebt), CheckWin/Lose, StoreManager·Loan·WeatherSystem 연동
- Key file: Assets/Scripts/Manager/GameManager.cs
- Date: 2026-05-06

## [Q-002] InventoryManager.cs ✓
- Conclusion: 상품 5종 재고 관리, 발주 시 StoreManager.SpendMoney 연동, 판매 시 GameManager.AddSales 및 StoreManager.AddMoney 연동, 잔여 재고 원가 합산(Penalty) 기능 구현.
- Key file: Assets/Scripts/Player/InventoryManager.cs, Assets/Scripts/Manager/GameManager.cs
- Date: 2026-05-14

---
