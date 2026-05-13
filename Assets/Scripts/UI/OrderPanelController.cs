using UnityEngine;
using UnityEngine.UI;

public class OrderPanelController : MonoBehaviour
{
    // 아이템 데이터와 재고를 가져오기 위한 매니저
    public InventoryManager inventoryManager;

    // 상품 Row 목록
    public OrderProductRowUI[] productRows;

    // 주문 합계 표시 텍스트
    public Text orderTotalValueText;

    private void Start()
    {
        // 각 상품 Row에 초기 데이터 연결
        for (int i = 0; i < productRows.Length; i++)
        {
            OrderProductRowUI row = productRows[i];

            if (row == null)
            {
                continue;
            }

            // Row가 주문 합계를 갱신할 수 있도록 컨트롤러 연결
            row.orderPanelController = this;

            // 상품명, 재고, 원가 표시 세팅
            row.SetupRow(inventoryManager);
        }

        // 처음 주문 합계 표시
        UpdateOrderTotalText();
    }

    public void UpdateOrderTotalText()
    {
        int totalCost = 0;

        // 모든 상품 Row의 발주 금액 합산
        for (int i = 0; i < productRows.Length; i++)
        {
            if (productRows[i] == null)
            {
                continue;
            }

            totalCost = totalCost + productRows[i].GetOrderCost();
        }

        // 주문 합계 텍스트 갱신
        if (orderTotalValueText != null)
        {
            orderTotalValueText.text = totalCost.ToString("N0") + "원";
        }
    }
}