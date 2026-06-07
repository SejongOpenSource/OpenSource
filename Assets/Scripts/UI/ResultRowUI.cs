using UnityEngine;
using UnityEngine.UI;

public class ResultRowUI : MonoBehaviour
{
    // 상품명 표시 텍스트
    public Text productNameText;

    // 주문 수량 표시 텍스트
    public Text orderedCountText;

    // 판매 수량 표시 텍스트
    public Text soldCountText;

    // 남은 재고 표시 텍스트
    public Text remainingStockText;

    public void SetResult(string productName, int orderedCount, int soldCount, int remainingStock)
    {
        if (productNameText != null)
        {
            productNameText.text = productName;
        }

        if (orderedCountText != null)
        {
            orderedCountText.text = $"{orderedCount:N0}개";
        }

        if (soldCountText != null)
        {
            soldCountText.text = $"{soldCount:N0}개";
        }

        if (remainingStockText != null)
        {
            remainingStockText.text = $"{remainingStock:N0}개";
        }
    }
}