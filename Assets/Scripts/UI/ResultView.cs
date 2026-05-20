using UnityEngine;
using UnityEngine.UI;

public class ResultView : MonoBehaviour
{
    // 상품별 결과 Row UI 목록
    public ResultRowUI[] resultRows;

    // 오늘 총 매출 표시 텍스트
    public Text todayTotalSalesText;

    // 오늘 총 수익 표시 텍스트
    public Text todayRevenueText;

    // 대출 이자 차감액 표시 텍스트
    public Text interestCostText;

    // 최종 순이익 표시 텍스트
    public Text netProfitText;

    // 마감 자본금 표시 텍스트
    public Text finalMoneyText;

    private void Start()
    {
        // 실제 영업 결과 데이터 연동 전까지 임시 데이터 표시
        ShowTestResult();
    }

    private void ShowTestResult()
    {
        // 상품별 주문 수량 / 판매 수량 임시 표시
        if (resultRows != null && resultRows.Length >= 5)
        {
            resultRows[0].SetResult("삼각김밥", 10, 9);
            resultRows[1].SetResult("컵라면", 5, 5);
            resultRows[2].SetResult("음료수", 20, 20);
            resultRows[3].SetResult("도시락", 8, 2);
            resultRows[4].SetResult("우산", 10, 8);
        }

        // 정산 요약 임시값
        int todayTotalSales = 124500;
        int todayRevenue = 124500;
        int interestCost = 3000;
        int netProfit = todayRevenue - interestCost;
        int finalMoney = 471500;

        // 오늘 총 매출 표시
        if (todayTotalSalesText != null)
        {
            todayTotalSalesText.text = todayTotalSales.ToString("N0") + "원";
        }

        // 오늘 총 수익 표시
        if (todayRevenueText != null)
        {
            todayRevenueText.text = "오늘 총 수익 " + todayRevenue.ToString("N0") + "원";
        }

        // 이자 비용 표시
        if (interestCostText != null)
        {
            interestCostText.text = "대출 이자 차감액 -" + interestCost.ToString("N0") + "원";
        }

        // 최종 순이익 표시
        if (netProfitText != null)
        {
            netProfitText.text = "최종 순이익 " + netProfit.ToString("N0") + "원";
        }

        // 마감 자본금 표시
        if (finalMoneyText != null)
        {
            finalMoneyText.text = "마감 자본금 " + finalMoney.ToString("N0") + "원";
        }
    }
}