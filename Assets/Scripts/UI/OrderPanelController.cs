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
        // 발주에 필요한 기본 연결 상태 확인
        if (CanConfirmOrder() == false)
        {
            return;
        }

        StoreManager storeManager = GameManager.Instance.storeManager;
        InventoryManager inventoryManager = GameManager.Instance.inventoryManager;
        Loan loan = GameManager.Instance.loan;

        int totalCost = GetTotalOrderCost();

        int loanAmount = 0;

        // 선택한 대출 금액 가져오기
        if (loanPanelController != null)
        {
            loanAmount = loanPanelController.GetSelectedLoanAmount();
        }

        int currentMoney = storeManager.currentMoney;
        int availableMoney = currentMoney + loanAmount;

        // 보유 자산 + 선택 대출금으로 발주 가능한지 먼저 확인
        if (availableMoney < totalCost)
        {
            Debug.LogWarning($"발주 실패: 자본금 부족 (보유+대출: {availableMoney:N0}원, 필요: {totalCost:N0}원)");
            return;
        }

        // 발주 수량을 InventoryManager에 임시 저장
        // 실제 비용 차감과 재고 반영은 FinalizeOrder에서 처리됨
        SetPendingOrders(inventoryManager);

        bool loanExecuted = false;

        // 대출 금액이 있으면 발주 확정 전에 대출 실행
        // FinalizeOrder가 StoreManager.SpendMoney를 사용하므로,
        // 대출금을 자산에 먼저 반영해야 발주 비용 차감이 가능함
        if (loanAmount > 0)
        {
            if (loan == null)
            {
                Debug.LogError("ConfirmOrder: Loan이 연결되지 않았습니다.");
                return;
            }

            bool loanSuccess = loan.TakeOutLoan(loanAmount);

            if (loanSuccess == false)
            {
                Debug.LogWarning("발주 실패: 대출 실행에 실패했습니다.");
                return;
            }

            loanExecuted = true;
        }

        // 발주 확정
        bool orderSuccess = inventoryManager.FinalizeOrder();

        // 대출은 성공했는데 발주 확정이 실패한 경우 상태 되돌리기
        if (orderSuccess == false)
        {
            if (loanExecuted)
            {
                RollbackLoan(storeManager, loanAmount);
            }

            Debug.LogWarning("발주 실패: 발주 확정에 실패했습니다.");
            return;
        }

        // 재고 UI 갱신
        RefreshProductRows();

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

    private bool CanConfirmOrder()
    {
        // 상품 Row가 연결되지 않았으면 발주 진행 불가
        if (HasProductRows() == false)
        {
            Debug.LogError("ConfirmOrder: productRows가 연결되지 않아 발주를 진행할 수 없습니다.");
            return false;
        }

        // GameManager 확인
        if (GameManager.Instance == null)
        {
            Debug.LogError("ConfirmOrder: GameManager가 없습니다.");
            return false;
        }

        // StoreManager 확인
        if (GameManager.Instance.storeManager == null)
        {
            Debug.LogError("ConfirmOrder: StoreManager가 연결되지 않았습니다.");
            return false;
        }

        // InventoryManager 확인
        if (GameManager.Instance.inventoryManager == null)
        {
            Debug.LogError("ConfirmOrder: InventoryManager가 연결되지 않았습니다.");
            return false;
        }

        return true;
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

    private int GetTotalOrderCost()
    {
        int totalCost = 0;

        // 모든 상품 Row의 발주 금액 합산
        for (int i = 0; i < productRows.Length; i++)
        {
            if (productRows[i] == null)
            {
                continue;
            }

            totalCost += productRows[i].GetOrderCost();
        }

        return totalCost;
    }

    private void SetPendingOrders(InventoryManager inventoryManager)
    {
        // 각 상품 Row의 발주 수량을 InventoryManager에 임시 저장
        for (int i = 0; i < productRows.Length; i++)
        {
            if (productRows[i] == null)
            {
                continue;
            }

            inventoryManager.SetOrder(
                productRows[i].GetItemType(),
                productRows[i].GetOrderQuantity()
            );
        }
    }

    private void RefreshProductRows()
    {
        // 발주 후 재고 UI 갱신
        for (int i = 0; i < productRows.Length; i++)
        {
            if (productRows[i] == null)
            {
                continue;
            }

            productRows[i].RefreshStock();
        }
    }

    private void RollbackLoan(StoreManager storeManager, int loanAmount)
    {
        // 방금 실행한 대출 금액이 없으면 되돌릴 필요 없음
        if (loanAmount <= 0)
        {
            return;
        }

        // 대출 실행으로 증가한 자산을 다시 차감
        bool moneyRollbackSuccess = storeManager.SpendMoney(loanAmount);

        if (moneyRollbackSuccess == false)
        {
            Debug.LogError("대출 롤백 실패: 대출로 증가한 자산을 되돌릴 수 없습니다.");
            return;
        }

        // 대출 실행으로 증가한 부채를 다시 감소
        storeManager.UpdateDebt(-loanAmount);

        Debug.Log($"대출 롤백 완료: {loanAmount:N0}원");
    }
}