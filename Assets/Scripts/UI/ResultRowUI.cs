using UnityEngine;
using UnityEngine.UI;

public class ResultRowUI : MonoBehaviour
{
    // 상품명 표시 텍스트
    // 예: 삼각김밥, 라면, 음료수
    public Text productNameText;

    // 해당 턴에 발주한 수량 표시 텍스트
    public Text orderedCountText;

    // 해당 턴에 실제로 판매된 수량 표시 텍스트
    public Text soldCountText;

    // 영업이 끝난 뒤 남아 있는 현재 재고 표시 텍스트
    public Text remainingStockText;

    // ResultView에서 상품별 결과 데이터를 넘겨받아 Row UI에 표시
    public void SetResult(string productName, int orderedCount, int soldCount, int remainingStock)
    {
        // 상품명 표시
        if (productNameText != null)
        {
            productNameText.text = productName;
        }

        // 발주 수량 표시
        if (orderedCountText != null)
        {
            orderedCountText.text = $"{orderedCount:N0}개";
        }

        // 판매 수량 표시
        if (soldCountText != null)
        {
            soldCountText.text = $"{soldCount:N0}개";
        }

        // 남은 재고 표시
        if (remainingStockText != null)
        {
            remainingStockText.text = $"{remainingStock:N0}개";
        }
    }
}