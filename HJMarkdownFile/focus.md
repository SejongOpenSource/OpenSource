# Focus

## Current Question
Inventory.cs 구현 — 상품 5종 재고 관리, 발주(Order), 판매(Sell), 잔여 재고 원가 합산

## Background Context
- Where this question came from: 서현진 담당 파일 목록
- Related files: StoreManager.cs, GameManager.cs, TradeData (ScriptableObject — 미구현)
- Already checked:
  - StoreManager.cs: SpendMoney(), AddMoney() 구현됨
  - GameManager.cs: AddSales(), CalculateScore(remainingStockCost, remainingDebt) — remainingStockCost는 Inventory에서 제공해야 함

## Goal
상품 5종의 재고를 추적하고, 발주 시 자본금 차감, 판매 시 매출 누적,
게임 종료 시 잔여 재고 원가를 GameManager.CalculateScore()에 넘길 수 있는 상태로 만들기

## Spec (기획서 기준)
| 상품 | 원가 | 판매가 |
|------|------|--------|
| 삼각김밥 | 800 | 1,200 |
| 컵라면 | 700 | 1,300 |
| 음료수 | 500 | 1,000 |
| 도시락 | 3,500 | 5,500 |
| 우산 | 2,000 | 3,500 |
