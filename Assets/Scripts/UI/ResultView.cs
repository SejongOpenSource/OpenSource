using UnityEngine;
using UnityEngine.UI;

public class ResultView : MonoBehaviour
{
    // 상품별 결과 Row UI 목록
    public ResultRowUI[] resultRows;

    // 시작 자본금 표시 텍스트
    public Text startMoneyValueText;

    // 오늘 매출 표시 텍스트
    public Text todaySalesValueText;

    // 대출 이자 비용 표시 텍스트
    public Text interestCostValueText;

    // 마감 자본금 표시 텍스트
    public Text finalMoneyValueText;

    // 남은 대출금 표시 텍스트
    public Text remainingDebtText;

    // 상환 금액 입력 필드
    public InputField repayInputField;

    // 대출 상환 버튼
    public Button repayLoanButton;

    // 다음 날로 진행 버튼
    public Button nextDayButton;

    // 실제 대출 상환 로직이 있는 Loan 스크립트
    public Loan loan;

    // 결과 화면 UI Row 순서와 동일한 상품 순서
    private readonly ItemType[] resultItemTypes =
    {
        ItemType.Onigiri,
        ItemType.Noodle,
        ItemType.Drink,
        ItemType.Bento,
        ItemType.Umbrella
    };

    private void Start()
    {
        AutoConnectLoan();

        if (repayLoanButton != null)
        {
            repayLoanButton.onClick.AddListener(OnRepayLoanButtonClicked);
        }

        if (nextDayButton != null)
        {
            nextDayButton.onClick.AddListener(OnNextDayButtonClicked);
        }

        UpdateResultView();
        UpdateRemainingDebtText();
    }

    private void OnEnable()
    {
        AutoConnectLoan();
        UpdateResultView();
        UpdateRemainingDebtText();
    }

    private void AutoConnectLoan()
    {
        if (loan != null)
        {
            return;
        }

        if (GameManager.Instance != null)
        {
            loan = GameManager.Instance.loan;
        }
    }

    private void UpdateResultView()
    {
        UpdateProductResultRows();
        UpdateSummaryTexts();
    }

    private void UpdateProductResultRows()
    {
        if (resultRows == null)
        {
            return;
        }

        if (GameManager.Instance == null || GameManager.Instance.inventoryManager == null)
        {
            Debug.LogError("ResultView: InventoryManager를 찾을 수 없습니다.");
            return;
        }

        InventoryManager inventoryManager = GameManager.Instance.inventoryManager;

        int rowCount = System.Math.Min(resultRows.Length, resultItemTypes.Length);

        for (int i = 0; i < rowCount; i++)
        {
            if (resultRows[i] == null)
            {
                continue;
            }

            ItemType itemType = resultItemTypes[i];

            ItemData itemData = null;

            if (DataManager.Instance != null && DataManager.Instance.itemDataManager != null)
            {
                itemData = DataManager.Instance.itemDataManager.GetItem(itemType);
            }

            string productName = GetProductName(itemType, itemData);

            int orderedCount = inventoryManager.GetLastOrder(itemType);
            int soldCount = inventoryManager.GetLastSold(itemType);

            // InventoryManager에 저장된 현재 재고값 사용
            int remainingStock = inventoryManager.GetStock(itemType);

            resultRows[i].SetResult(productName, orderedCount, soldCount, remainingStock);
        }
    }

    private void UpdateSummaryTexts()
    {
        if (GameManager.Instance == null || GameManager.Instance.storeManager == null)
        {
            Debug.LogError("ResultView: StoreManager를 찾을 수 없습니다.");
            return;
        }

        if (GameManager.Instance.inventoryManager == null)
        {
            Debug.LogError("ResultView: InventoryManager를 찾을 수 없습니다.");
            return;
        }

        StoreManager storeManager = GameManager.Instance.storeManager;
        InventoryManager inventoryManager = GameManager.Instance.inventoryManager;

        int todaySales = 0;

        if (SalesAlgorithm.Instance != null)
        {
            todaySales = SalesAlgorithm.Instance.LastDailyRevenue;
        }

        int startMoney = inventoryManager.GetLastStartMoney();
        int finalMoney = storeManager.currentMoney;

        if (startMoneyValueText != null)
        {
            startMoneyValueText.text = $"{startMoney:N0}원";
        }

        if (todaySalesValueText != null)
        {
            todaySalesValueText.text = $"{todaySales:N0}원";
        }

        if (interestCostValueText != null)
        {
            interestCostValueText.text = "0원";
        }

        if (finalMoneyValueText != null)
        {
            finalMoneyValueText.text = $"{finalMoney:N0}원";
        }
    }

    private string GetProductName(ItemType itemType, ItemData itemData)
    {
        if (itemData != null && string.IsNullOrEmpty(itemData.itemName) == false)
        {
            return itemData.itemName;
        }

        return itemType.ToString();
    }

    private void OnRepayLoanButtonClicked()
    {
        if (repayInputField == null)
        {
            Debug.LogError("RepayInputField가 연결되지 않았습니다.");
            return;
        }

        string inputText = repayInputField.text;

        if (string.IsNullOrWhiteSpace(inputText))
        {
            return;
        }

        inputText = inputText.Replace(",", "");
        inputText = inputText.Replace("원", "");
        inputText = inputText.Trim();

        int repayAmount = 0;

        if (int.TryParse(inputText, out repayAmount) == false)
        {
            Debug.Log("상환 금액은 숫자로 입력해야 합니다.");
            return;
        }

        if (repayAmount <= 0)
        {
            Debug.Log("상환 금액은 0원보다 커야 합니다.");
            return;
        }

        AutoConnectLoan();

        if (loan == null)
        {
            Debug.LogError("Loan 컴포넌트가 연결되지 않았습니다.");
            return;
        }

        if (loan.storeManager == null)
        {
            Debug.LogError("Loan에 StoreManager가 연결되지 않았습니다.");
            return;
        }

        loan.RepayLoan(repayAmount);

        repayInputField.text = "";

        UpdateRemainingDebtText();
        UpdateSummaryTexts();
    }

    private void UpdateRemainingDebtText()
    {
        if (remainingDebtText == null)
        {
            return;
        }

        AutoConnectLoan();

        if (loan == null)
        {
            Debug.LogError("Loan 컴포넌트가 연결되지 않았습니다.");
            remainingDebtText.text = "남은 대출금 정보 없음";
            return;
        }

        if (loan.storeManager == null)
        {
            Debug.LogError("Loan에 StoreManager가 연결되지 않았습니다.");
            remainingDebtText.text = "남은 대출금 정보 없음";
            return;
        }

        int remainingDebt = loan.storeManager.currentDebt;

        remainingDebtText.text = $"남은 대출금 {remainingDebt:N0}원";
    }

    private void OnNextDayButtonClicked()
    {
        if (TurnManager.Instance == null)
        {
            Debug.LogError("TurnManager 인스턴스를 찾을 수 없습니다.");
            return;
        }

        TurnManager.Instance.AdvancePhase();

        Debug.Log($"다음 날로 진행: Turn {TurnManager.Instance.CurrentTurn} / Phase {TurnManager.Instance.CurrentPhase}");
    }
}