using UnityEngine;
using UnityEngine.UI;

public class OrderPanelController : MonoBehaviour
{
    // 영업 시작하기 버튼
    public Button startSalesButton;

    // 상품 Row 목록
    public OrderProductRowUI[] productRows;

    // 주문 합계 표시 텍스트
    public Text orderTotalValueText;

    // 외상/대출 UI 컨트롤러
    // 아직 실제 Loan.TakeOutLoan은 호출하지 않고 선택 금액만 가져옴
    public LoanPanelController loanPanelController;

    private void Start()
    {
        // 각 상품 Row에 OrderPanelController 연결
        for (int i = 0; i < productRows.Length; i++)
        {
            if (productRows[i] == null)
            {
                continue;
            }

            productRows[i].orderPanelController = this;
        }

        // 영업 시작하기 버튼 클릭 이벤트 연결
        if (startSalesButton != null)
        {
            startSalesButton.onClick.AddListener(CollectOrderDataForTest);
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

    private void CollectOrderDataForTest()
    {
        Debug.Log("=== 영업 시작하기 버튼 클릭 ===");

        // 상품별 발주 수량 확인
        for (int i = 0; i < productRows.Length; i++)
        {
            if (productRows[i] == null)
            {
                continue;
            }

            ItemType itemType = productRows[i].GetItemType();
            int quantity = productRows[i].GetOrderQuantity();

            Debug.Log(itemType + " 발주 수량: " + quantity);
        }

        // 선택한 외상/대출 금액 확인
        if (loanPanelController != null)
        {
            int loanAmount = loanPanelController.GetSelectedLoanAmount();
            Debug.Log("선택 대출 금액: " + loanAmount);
        }
        else
        {
            Debug.Log("LoanPanelController가 연결되지 않았습니다.");
        }

        // TODO:
        // ItemData, OrderSystem, InventoryManager 연동 방식이 확정되면
        // 여기에서 실제 발주 확정 로직을 호출한다.
    }
}