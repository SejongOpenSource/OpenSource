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
        for (int i = 0; i < productRows.Length; i++)
        {
            if (productRows[i] == null) continue;
            productRows[i].orderPanelController = this;
            productRows[i].SetupRow();
        }

        if (startSalesButton != null)
            startSalesButton.onClick.AddListener(ConfirmOrder);

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

    private void ConfirmOrder()
    {
        int totalCost = 0;
        for (int i = 0; i < productRows.Length; i++)
        {
            if (productRows[i] == null) continue;
            totalCost += productRows[i].GetOrderCost();
        }

        // 대출 먼저 처리
        if (loanPanelController != null)
        {
            int loanAmount = loanPanelController.GetSelectedLoanAmount();
            if (loanAmount > 0)
                GameManager.Instance.loan.TakeOutLoan(loanAmount);
        }

        // 발주 비용 차감
        if (!GameManager.Instance.storeManager.SpendMoney(totalCost))
        {
            Debug.LogWarning("발주 실패: 자본금 부족");
            return;
        }

        // 발주 확정
        for (int i = 0; i < productRows.Length; i++)
        {
            if (productRows[i] == null) continue;
            InventoryManager.Instance.SetOrder(productRows[i].GetItemType(), productRows[i].GetOrderQuantity());
        }
        InventoryManager.Instance.FinalizeOrder();

        // 재고 UI 갱신
        for (int i = 0; i < productRows.Length; i++)
        {
            if (productRows[i] == null) continue;
            productRows[i].RefreshStock();
        }

        Debug.Log($"발주 확정 완료. 지출: {totalCost:N0}원");
    }
}