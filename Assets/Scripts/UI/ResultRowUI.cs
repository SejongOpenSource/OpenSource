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

    public void SetResult(string productName, int orderedCount, int soldCount)
    {
        // 상품명 표시
        if (productNameText != null)
        {
            productNameText.text = productName;
        }

        // 주문 수량 표시
        if (orderedCountText != null)
        {
            orderedCountText.text = orderedCount.ToString("N0") + "개";
        }

        // 판매 수량 표시
        if (soldCountText != null)
        {
            soldCountText.text = soldCount.ToString("N0") + "개";
        }
    }
}