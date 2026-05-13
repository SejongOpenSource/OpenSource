using UnityEngine;
using UnityEngine.UI;

public class OrderProductRowUI : MonoBehaviour
{
    // 이 Row가 담당하는 상품 종류
    public ItemType itemType;

    // 수량 감소 버튼
    public Button minusButton;

    // 수량 증가 버튼
    public Button plusButton;

    // 현재 발주 수량
    public int orderQuantity = 0;

    private void Start()
    {
        // + 버튼 클릭 이벤트 연결
        if (plusButton != null)
        {
            plusButton.onClick.AddListener(IncreaseQuantity);
        }

        // - 버튼 클릭 이벤트 연결
        if (minusButton != null)
        {
            minusButton.onClick.AddListener(DecreaseQuantity);
        }

        // 처음 수량은 0이므로 - 버튼 상태 갱신
        UpdateButtonState();
    }

    private void IncreaseQuantity()
    {
        // 발주 수량 1 증가
        orderQuantity = orderQuantity + 1;

        // 화면 텍스트가 없으므로 Console에서 수량 확인
        Debug.Log(itemType + " 발주 수량 증가: " + orderQuantity);

        UpdateButtonState();
    }

    private void DecreaseQuantity()
    {
        // 수량이 0이면 더 이상 감소하지 않음
        if (orderQuantity <= 0)
        {
            return;
        }

        // 발주 수량 1 감소
        orderQuantity = orderQuantity - 1;

        // 화면 텍스트가 없으므로 Console에서 수량 확인
        Debug.Log(itemType + " 발주 수량 감소: " + orderQuantity);

        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        // 수량이 0이면 - 버튼 비활성화
        if (minusButton != null)
        {
            minusButton.interactable = orderQuantity > 0;
        }
    }

    public int GetOrderQuantity()
    {
        // 외부에서 현재 발주 수량을 가져갈 때 사용
        return orderQuantity;
    }

    public ItemType GetItemType()
    {
        // 외부에서 상품 종류를 가져갈 때 사용
        return itemType;
    }
}