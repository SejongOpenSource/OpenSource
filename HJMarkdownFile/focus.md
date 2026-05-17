# Focus

## [Q-003] TradeData ScriptableObject
- Source: 서현진 담당 파일
- Context: 상품 5종 (삼각김밥/컵라면/음료수/도시락/우산) 원가·판매가 데이터
- Question: TradeData.cs (ScriptableObject) 구현 및 .asset 파일 5종 생성
- Related files: Inventory.cs, ItemData.cs

## Goal
상품 5종의 데이터를 관리하는 ScriptableObject 시스템을 완성하고, 실제 데이터(.asset)를 생성하여 InventoryManager에 연결 가능한 상태로 만들기.

## Spec (기획서 기준)
| 상품 | 원가 | 판매가 |
|------|------|--------|
| 삼각김밥 | 800 | 1,200 |
| 컵라면 | 700 | 1,300 |
| 음료수 | 500 | 1,000 |
| 도시락 | 3,500 | 5,500 |
| 우산 | 2,000 | 3,500 |
