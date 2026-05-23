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
#if UNITY_EDITOR
        // 에디터에서 UI 배치를 확인하기 위한 임시 결과 데이터
        // 실제 빌드에는 포함되지 않음
        ShowTestResult();
#endif

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

        // 남은 대출금 텍스트 갱신
        UpdateRemainingDebtText();
    }

#if UNITY_EDITOR
    private void ShowTestResult()
    {
        // 상품별 주문 수량 / 판매 수량 임시 표시
        // 실제 판매 결과 데이터 연동 전까지 에디터에서만 UI 확인용으로 사용
        if (resultRows != null && resultRows.Length >= 5)
        {
            resultRows[0].SetResult("삼각김밥", 10, 9);
            resultRows[1].SetResult("라면", 5, 5);
            resultRows[2].SetResult("음료수", 20, 20);
            resultRows[3].SetResult("도시락", 8, 2);
            resultRows[4].SetResult("우산", 10, 8);
        }

        // 정산 요약 임시값
        // 실제 매출 / 이자 / 자본금 계산 로직이 연결되면 제거 예정
        int startMoney = 350000;
        int todaySales = 124500;
        int interestCost = 3000;
        int finalMoney = startMoney + todaySales - interestCost;

        // 시작 자본금 표시
        if (startMoneyValueText != null)
        {
            startMoneyValueText.text = startMoney.ToString("N0") + "원";
        }

        // 오늘 매출 표시
        if (todaySalesValueText != null)
        {
            todaySalesValueText.text = todaySales.ToString("N0") + "원";
        }

        // 이자 비용 표시
        if (interestCostValueText != null)
        {
            interestCostValueText.text = "-" + interestCost.ToString("N0") + "원";
        }

        // 마감 자본금 표시
        if (finalMoneyValueText != null)
        {
            finalMoneyValueText.text = finalMoney.ToString("N0") + "원";
        }
    }
#endif

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

        // 빈 값이면 실행하지 않음
        if (string.IsNullOrEmpty(inputText))
        {
            Debug.Log("상환 금액이 입력되지 않았습니다.");
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
    }

    private void UpdateRemainingDebtText()
    {
        // 남은 대출금 텍스트가 연결되지 않았으면 표시할 수 없음
        if (remainingDebtText == null)
        {
            return;
        }

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
        remainingDebtText.text = "남은 대출금 " + remainingDebt.ToString("N0") + "원";
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

        Debug.Log("다음 날로 진행: Turn "
                  + TurnManager.Instance.CurrentTurn
                  + " / Phase "
                  + TurnManager.Instance.CurrentPhase);
    }
}