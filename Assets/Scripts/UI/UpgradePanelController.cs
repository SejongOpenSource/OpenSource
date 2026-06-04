using UnityEngine;
using UnityEngine.UI;

public class OrderPanelController : MonoBehaviour
{
    // 영업 시작하기 버튼
    public Button startSalesButton;

    // 상품 Row 목록
    // Inspector에서 상품 Row들을 연결해야 함
    public OrderProductRowUI[] productRows;

    // 주문 합계 표시 텍스트
    public Text orderTotalValueText;

    // 대출 UI 컨트롤러
    public LoanPanelController loanPanelController;

    private void Start()
    {
        // 상품 Row가 제대로 연결되어 있는지 먼저 확인
        if (HasProductRows() == false)
        {
            Debug.LogError("OrderPanelController: productRows가 연결되지 않았습니다.");
        }
        else
        {
            // 상품 Row 초기 설정
            for (int i = 0; i < productRows.Length; i++)
            {
                if (productRows[i] == null)
                {
                    Debug.LogWarning($"OrderPanelController: productRows[{i}]가 비어 있습니다.");
                    continue;
                }

                productRows[i].orderPanelController = this;
                productRows[i].SetupRow();
            }
        }

        // 영업 시작 버튼 연결
        if (startSalesButton != null)
        {
            startSalesButton.onClick.AddListener(ConfirmOrder);
        }

        // 주문 합계 초기 표시
        UpdateOrderTotalText();
    }

    public void UpdateOrderTotalText()
    {
        int totalCost = 0;

        // 상품 Row가 없으면 주문 합계를 0원으로 표시하고 종료
        if (HasProductRows() == false)
        {
            if (orderTotalValueText != null)
            {
                orderTotalValueText.text = "0원";
            }

            return;
        }

        // 모든 상품 Row의 발주 금액 합산
        for (int i = 0; i < productRows.Length; i++)
        {
            if (productRows[i] == null)
            {
                continue;
            }

            totalCost += productRows[i].GetOrderCost();
        }

        // 주문 합계 텍스트 갱신
        if (orderTotalValueText != null)
        {
            orderTotalValueText.text = totalCost.ToString("N0") + "원";
        }
    }

    private void ConfirmOrder()
    {
        // 상품 Row가 연결되지 않았으면 발주 진행 불가
        if (HasProductRows() == false)
        {
            Debug.LogError("ConfirmOrder: productRows가 연결되지 않아 발주를 진행할 수 없습니다.");
            return;
        }

        // GameManager 확인
        if (GameManager.Instance == null)
        {
            Debug.LogError("ConfirmOrder: GameManager가 없습니다.");
            return;
        }

        // StoreManager 확인
        if (GameManager.Instance.storeManager == null)
        {
            Debug.LogError("ConfirmOrder: StoreManager가 연결되지 않았습니다.");
            return;
        }

        // InventoryManager 확인
        if (GameManager.Instance.inventoryManager == null)
        {
            Debug.LogError("ConfirmOrder: InventoryManager가 연결되지 않았습니다.");
            return;
        }

        int totalCost = 0;

        // 발주 총 비용 계산
        for (int i = 0; i < productRows.Length; i++)
        {
            if (productRows[i] == null)
            {
                continue;
            }

            totalCost += productRows[i].GetOrderCost();
        }

        int loanAmount = 0;

        // 선택한 대출 금액 가져오기
        if (loanPanelController != null)
        {
            loanAmount = loanPanelController.GetSelectedLoanAmount();
        }

        int currentMoney = GameManager.Instance.storeManager.currentMoney;
        int availableMoney = currentMoney + loanAmount;

        // 보유 자산 + 선택 대출금으로 발주 가능한지 먼저 확인
        if (availableMoney < totalCost)
        {
            Debug.LogWarning($"발주 실패: 자본금 부족 (보유+대출: {availableMoney:N0}원, 필요: {totalCost:N0}원)");
            return;
        }

        // 대출 금액이 있으면 먼저 대출 실행
        if (loanAmount > 0)
        {
            if (GameManager.Instance.loan == null)
            {
                Debug.LogError("ConfirmOrder: Loan이 연결되지 않았습니다.");
                return;
            }

            bool loanSuccess = GameManager.Instance.loan.TakeOutLoan(loanAmount);

            if (loanSuccess == false)
            {
                Debug.LogWarning("발주 실패: 대출 실행에 실패했습니다.");
                return;
            }
        }

        // 발주 수량을 InventoryManager에 임시 저장
        for (int i = 0; i < productRows.Length; i++)
        {
            if (productRows[i] == null)
            {
                continue;
            }

            GameManager.Instance.inventoryManager.SetOrder(
                productRows[i].GetItemType(),
                productRows[i].GetOrderQuantity()
            );
        }

        // 발주 확정
        // 비용 차감과 재고 반영은 InventoryManager.FinalizeOrder()에서 처리됨
        bool orderSuccess = GameManager.Instance.inventoryManager.FinalizeOrder();

        if (orderSuccess == false)
        {
            Debug.LogWarning("발주 실패: 발주 확정에 실패했습니다.");
            return;
        }

        // 재고 UI 갱신
        for (int i = 0; i < productRows.Length; i++)
        {
            if (productRows[i] == null)
            {
                continue;
            }

            productRows[i].RefreshStock();
        }

        // 주문 합계 다시 갱신
        UpdateOrderTotalText();

        Debug.Log($"발주 확정 완료. 지출: {totalCost:N0}원");

        // 다음 페이즈로 이동
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.AdvancePhase();
        }
        else
        {
            Debug.LogError("ConfirmOrder: TurnManager가 없습니다.");
        }
    }

    private bool HasProductRows()
    {
        // productRows 배열 자체가 없으면 false
        if (productRows == null)
        {
            return false;
        }

        // 배열은 있지만 비어 있으면 false
        if (productRows.Length == 0)
        {
            return false;
        }

        return true;
    }
}