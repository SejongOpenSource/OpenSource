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

    private void Start()
    {
        // Loan이 Inspector에서 연결되지 않았으면 자동 연결 시도
        AutoConnectLoan();

        // 대출 상환 버튼 클릭 이벤트 연결
        if (repayLoanButton != null)
        {
            repayLoanButton.onClick.AddListener(OnRepayLoanButtonClicked);
        }

        // 다음 날로 진행 버튼 클릭 이벤트 연결
        if (nextDayButton != null)
        {
            nextDayButton.onClick.AddListener(OnNextDayButtonClicked);
        }

        // 실제 결과 데이터 표시
        UpdateResultView();

        // 남은 대출금 텍스트 갱신
        UpdateRemainingDebtText();
    }

    private void OnEnable()
    {
        // ResultPanel이 켜질 때마다 최신 결과 데이터 표시
        AutoConnectLoan();
        UpdateResultView();
        UpdateRemainingDebtText();
    }

    private void AutoConnectLoan()
    {
        // 이미 연결되어 있으면 다시 찾지 않음
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
        // 상품별 발주량 / 판매량 표시
        UpdateProductResultRows();

        // 매출 정산 요약 표시
        UpdateSummaryTexts();
    }

    private void UpdateProductResultRows()
    {
        // 결과 Row가 없으면 표시할 수 없음
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

        // ItemType enum 순서대로 Result Row에 표시
        ItemType[] itemTypes = (ItemType[])System.Enum.GetValues(typeof(ItemType));

        int rowCount = Mathf.Min(resultRows.Length, itemTypes.Length);

        for (int i = 0; i < rowCount; i++)
        {
            if (resultRows[i] == null)
            {
                continue;
            }

            ItemType itemType = itemTypes[i];

            ItemData itemData = null;

            if (DataManager.Instance != null)
            {
                itemData = DataManager.Instance.GetItem(itemType);
            }

            string productName = GetProductName(itemType, itemData);
            int orderedCount = inventoryManager.GetLastOrder(itemType);
            int soldCount = inventoryManager.GetLastSold(itemType);

            resultRows[i].SetResult(productName, orderedCount, soldCount);
        }
    }

    private void UpdateSummaryTexts()
    {
        StoreManager storeManager = null;

        if (GameManager.Instance != null)
        {
            storeManager = GameManager.Instance.storeManager;
        }

        int todaySales = 0;

        // 오늘 매출은 실제 판매 알고리즘에서 계산된 마지막 매출 사용
        if (SalesAlgorithm.Instance != null)
        {
            todaySales = SalesAlgorithm.Instance.LastDailyRevenue;
        }

        int finalMoney = 0;

        // 마감 자본금은 현재 StoreManager의 실제 자산 사용
        if (storeManager != null)
        {
            finalMoney = storeManager.currentMoney;
        }

        // 시작 자본금은 마감 자본금에서 오늘 매출을 뺀 값으로 계산
        int startMoney = finalMoney - todaySales;

        if (startMoneyValueText != null)
        {
            startMoneyValueText.text = $"{startMoney:N0}원";
        }

        if (todaySalesValueText != null)
        {
            todaySalesValueText.text = $"{todaySales:N0}원";
        }

        // 현재 별도 이자 비용 계산값이 연결되어 있지 않으므로 0원 표시
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
        // ItemData에 상품명이 있으면 실제 데이터 이름 사용
        if (itemData != null && string.IsNullOrEmpty(itemData.itemName) == false)
        {
            return itemData.itemName;
        }

        // ItemData가 없을 때는 enum 이름 사용
        return itemType.ToString();
    }

    private void OnRepayLoanButtonClicked()
    {
        // InputField가 연결되지 않았으면 실행하지 않음
        if (repayInputField == null)
        {
            Debug.LogError("RepayInputField가 연결되지 않았습니다.");
            return;
        }

        // 입력값 가져오기
        string inputText = repayInputField.text;

        // 빈 값이면 조용히 무시
        if (string.IsNullOrWhiteSpace(inputText))
        {
            return;
        }

        // 쉼표나 원 표시가 들어가도 숫자로 처리할 수 있게 정리
        inputText = inputText.Replace(",", "");
        inputText = inputText.Replace("원", "");
        inputText = inputText.Trim();

        int repayAmount = 0;

        // 숫자로 변환할 수 없으면 실행하지 않음
        if (int.TryParse(inputText, out repayAmount) == false)
        {
            Debug.Log("상환 금액은 숫자로 입력해야 합니다.");
            return;
        }

        // 0원 이하이면 실행하지 않음
        if (repayAmount <= 0)
        {
            Debug.Log("상환 금액은 0원보다 커야 합니다.");
            return;
        }

        // Loan 자동 연결 시도
        AutoConnectLoan();

        // 실제 게임에서는 Loan 컴포넌트가 반드시 연결되어 있어야 함
        if (loan == null)
        {
            Debug.LogError("Loan 컴포넌트가 연결되지 않았습니다.");
            return;
        }

        // Loan 안에 StoreManager가 연결되어 있어야 실제 부채와 자산을 처리할 수 있음
        if (loan.storeManager == null)
        {
            Debug.LogError("Loan에 StoreManager가 연결되지 않았습니다.");
            return;
        }

        // 실제 대출 상환 처리
        loan.RepayLoan(repayAmount);

        // 입력칸 초기화
        repayInputField.text = "";

        // 남은 대출금 표시 갱신
        UpdateRemainingDebtText();

        // 상환 후 마감 자본금도 다시 표시
        UpdateSummaryTexts();
    }

    private void UpdateRemainingDebtText()
    {
        // 남은 대출금 텍스트가 연결되지 않았으면 표시할 수 없음
        if (remainingDebtText == null)
        {
            return;
        }

        // Loan 자동 연결 시도
        AutoConnectLoan();

        // 실제 게임에서는 Loan 컴포넌트가 반드시 연결되어 있어야 함
        if (loan == null)
        {
            Debug.LogError("Loan 컴포넌트가 연결되지 않았습니다.");
            remainingDebtText.text = "남은 대출금 정보 없음";
            return;
        }

        // Loan 안에 StoreManager가 연결되어 있어야 현재 부채를 가져올 수 있음
        if (loan.storeManager == null)
        {
            Debug.LogError("Loan에 StoreManager가 연결되지 않았습니다.");
            remainingDebtText.text = "남은 대출금 정보 없음";
            return;
        }

        // 실제 게임 데이터의 현재 부채만 사용
        int remainingDebt = loan.storeManager.currentDebt;

        // 남은 대출금 텍스트 표시
        remainingDebtText.text = $"남은 대출금 {remainingDebt:N0}원";
    }

    private void OnNextDayButtonClicked()
    {
        // TurnManager가 씬에 없으면 다음 턴으로 넘길 수 없음
        if (TurnManager.Instance == null)
        {
            Debug.LogError("TurnManager 인스턴스를 찾을 수 없습니다.");
            return;
        }

        // Result 페이즈에서 호출하면 다음 턴의 Upgrade 페이즈로 이동
        TurnManager.Instance.AdvancePhase();

        Debug.Log($"다음 날로 진행: Turn {TurnManager.Instance.CurrentTurn} / Phase {TurnManager.Instance.CurrentPhase}");
    }
}