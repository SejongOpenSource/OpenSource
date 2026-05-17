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
        if (GameManager.Instance == null || InventoryManager.Instance == null)
        {
            Debug.LogError("ConfirmOrder: GameManager 또는 InventoryManager 초기화되지 않음");
            return;
        }

        int totalCost = 0;
        for (int i = 0; i < productRows.Length; i++)
        {
            if (productRows[i] == null) continue;
            totalCost += productRows[i].GetOrderCost();
        }

        int loanAmount = 0;
        if (loanPanelController != null)
            loanAmount = loanPanelController.GetSelectedLoanAmount();

        // 대출 포함 잔액으로 발주 가능 여부 사전 검증
        int availableMoney = GameManager.Instance.storeManager.currentMoney + loanAmount;
        if (availableMoney < totalCost)
        {
            Debug.LogWarning($"발주 실패: 자본금 부족 (보유+대출: {availableMoney:N0}원, 필요: {totalCost:N0}원)");
            return;
        }

        // 검증 통과 후 대출 실행
        if (loanAmount > 0)
            GameManager.Instance.loan.TakeOutLoan(loanAmount);

        GameManager.Instance.storeManager.SpendMoney(totalCost);

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