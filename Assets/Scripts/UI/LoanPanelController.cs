using UnityEngine;
using UnityEngine.UI;

public class LoanPanelController : MonoBehaviour
{
    [Header("대출 금액 슬라이더")]
    // 대출 금액을 조절하는 슬라이더
    public Slider loanSlider;

    [Header("대출 정보 표시")]
    // 선택된 대출 금액 표시 텍스트
    public Text loanAmountValueText;

    // 이자율 표시 텍스트
    public Text interestRateValueText;

    // 예상 이자 표시 텍스트
    public Text expectedInterestValueText;

    [Header("대출 처리")]
    // 실제 대출 실행 로직이 있는 Loan 스크립트
    public Loan loan;

    // 현재 선택된 대출 금액
    public int selectedLoanAmount = 0;

    private void Start()
    {
        AutoConnectLoan();

        // 슬라이더 이벤트 연결
        if (loanSlider != null)
        {
            loanSlider.onValueChanged.AddListener(OnLoanSliderChanged);
        }

        // 처음에는 현재 대출 가능 금액 기준으로 슬라이더 초기화
        RefreshLoanSlider();
    }

    private void OnEnable()
    {
        AutoConnectLoan();

        // 발주 화면이 다시 켜질 때마다
        // 현재 부채 기준으로 슬라이더 최대값을 다시 계산
        RefreshLoanSlider();
    }

    private void OnDestroy()
    {
        // 슬라이더 이벤트 해제
        if (loanSlider != null)
        {
            loanSlider.onValueChanged.RemoveListener(OnLoanSliderChanged);
        }
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

    private void RefreshLoanSlider()
    {
        // 현재 선택 가능한 최대 대출 금액 계산
        int availableLoanAmount = GetAvailableLoanAmount();

        // 화면에 들어올 때마다 선택 대출 금액은 0원으로 초기화
        selectedLoanAmount = 0;

        if (loanSlider != null)
        {
            // 슬라이더 기본 설정
            loanSlider.wholeNumbers = true;
            loanSlider.minValue = 0;
            loanSlider.maxValue = availableLoanAmount;

            // 현재 화면 진입 시 항상 0원에서 시작
            loanSlider.SetValueWithoutNotify(0);

            // 대출 가능 금액이 없으면 슬라이더 비활성화
            loanSlider.interactable = availableLoanAmount > 0;
        }

        // 텍스트 갱신
        UpdateLoanTexts();
    }

    private int GetAvailableLoanAmount()
    {
        AutoConnectLoan();

        if (loan == null)
        {
            return 0;
        }

        // Loan.cs에서 계산한 값 사용
        // Min(1회 대출 한도, 전체 한도 - 현재 부채)
        return loan.GetAvailableLoanAmount();
    }

    private void OnLoanSliderChanged(float value)
    {
        int availableLoanAmount = GetAvailableLoanAmount();

        // 슬라이더 값은 float이므로 int로 변환
        int sliderAmount = Mathf.RoundToInt(value);

        // 혹시 모를 값 초과 방지
        if (sliderAmount > availableLoanAmount)
        {
            sliderAmount = availableLoanAmount;
        }

        if (sliderAmount < 0)
        {
            sliderAmount = 0;
        }

        selectedLoanAmount = sliderAmount;

        // 텍스트 갱신
        UpdateLoanTexts();
    }

    private void UpdateLoanTexts()
    {
        // 선택 대출 금액 표시
        if (loanAmountValueText != null)
        {
            loanAmountValueText.text = selectedLoanAmount.ToString("N0") + "원";
        }

        // Loan.cs에 정의된 공통 이자율 사용
        float currentInterestRate = Loan.InterestRate;

        // 이자율 표시
        if (interestRateValueText != null)
        {
            int ratePercent = Mathf.RoundToInt(currentInterestRate * 100f);
            interestRateValueText.text = ratePercent + "% / 회차";
        }

        // 예상 이자 표시
        if (expectedInterestValueText != null)
        {
            int expectedInterest = Mathf.RoundToInt(selectedLoanAmount * currentInterestRate);
            expectedInterestValueText.text = expectedInterest.ToString("N0") + "원 / 회차";
        }
    }

    public void ConfirmLoan()
    {
        // 아직 대출 금액이 0원이면 실행하지 않음
        if (selectedLoanAmount <= 0)
        {
            Debug.Log("대출 금액이 0원입니다.");
            return;
        }

        AutoConnectLoan();

        // Loan 스크립트 연결 확인
        if (loan == null)
        {
            Debug.LogError("Loan 스크립트가 연결되지 않았습니다.");
            return;
        }

        // 실제 대출 실행
        bool success = loan.TakeOutLoan(selectedLoanAmount);

        if (success)
        {
            Debug.Log("대출 실행 완료: " + selectedLoanAmount.ToString("N0") + "원");
        }
        else
        {
            Debug.Log("대출 실행 실패");
        }

        // 대출 실행 후 현재 부채 기준으로 슬라이더 최대값 다시 갱신
        RefreshLoanSlider();
    }

    public int GetSelectedLoanAmount()
    {
        // OrderPanelController에서 현재 선택된 대출 금액을 가져갈 때 사용
        return selectedLoanAmount;
    }
}