using UnityEngine;
using UnityEngine.UI;

public class LoanPanelController : MonoBehaviour
{
    // 대출 금액을 조절하는 슬라이더
    public Slider loanSlider;

    // 선택된 대출 금액 표시 텍스트
    public Text loanAmountValueText;

    // 이자율 표시 텍스트
    public Text interestRateValueText;

    // 예상 이자 표시 텍스트
    public Text expectedInterestValueText;

    // 실제 대출 실행 로직이 있는 Loan 스크립트
    // 현재는 실제 대출 확정 전이면 비워둬도 됨
    public Loan loan;

    // 현재 선택된 대출 금액
    public int selectedLoanAmount = 0;

    private void Start()
    {
        // 슬라이더가 연결되어 있으면 값 변경 이벤트 등록
        if (loanSlider != null)
        {
            loanSlider.onValueChanged.AddListener(OnLoanSliderChanged);

            // 슬라이더 초기값을 현재 선택 금액으로 저장
            selectedLoanAmount = Mathf.RoundToInt(loanSlider.value);
        }

        // 처음 화면 텍스트 갱신
        UpdateLoanTexts();
    }

    private void OnLoanSliderChanged(float value)
    {
        // 슬라이더 값은 float이므로 반올림해서 int로 저장
        selectedLoanAmount = Mathf.RoundToInt(value);

        // 슬라이더 값이 바뀔 때마다 텍스트 갱신
        UpdateLoanTexts();

        Debug.Log("선택 대출 금액: " + selectedLoanAmount);
    }

    private void UpdateLoanTexts()
    {
        // 대출 금액 표시
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
            Debug.Log("대출 실행 완료: " + selectedLoanAmount);
        }
        else
        {
            Debug.Log("대출 실행 실패");
        }

        // 대출 실행 후 선택 금액 초기화
        selectedLoanAmount = 0;

        if (loanSlider != null)
        {
            loanSlider.value = 0;
        }

        UpdateLoanTexts();
    }

    public int GetSelectedLoanAmount()
    {
        // OrderPanelController에서 현재 선택된 대출 금액을 가져갈 때 사용
        return selectedLoanAmount;
    }
}