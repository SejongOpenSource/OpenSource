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

    // 상품명 표시 텍스트
    public Text productNameText;

    // 현재 재고 표시 텍스트
    public Text stockText;

    // 발주 수량 표시 텍스트
    public Text quantityText;

    // 원가 표시 텍스트
    public Text costText;

    // 상품별 발주 금액 표시 텍스트
    public Text totalPriceText;

    // 주문 화면 전체 컨트롤러
    public OrderPanelController orderPanelController;

    // 현재 발주 수량
    public int orderQuantity = 0;

    // 이 상품의 원가
    private int itemCost = 0;

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

        // 처음 화면 상태 갱신
        UpdateButtonState();
        UpdateQuantityText();
        UpdateTotalPriceText();
    }

    public void SetupRow(InventoryManager inventoryManager)
    {
        // InventoryManager가 없으면 상품 정보를 가져올 수 없음
        if (inventoryManager == null)
        {
            return;
        }

        int itemIndex = (int)itemType;

        // Items 배열이 없거나 인덱스가 잘못되면 중단
        if (inventoryManager.Items == null || itemIndex < 0 || itemIndex >= inventoryManager.Items.Length)
        {
            return;
        }

        ItemData itemData = inventoryManager.Items[itemIndex];

        // 해당 상품 데이터가 연결되지 않았으면 중단
        if (itemData == null)
        {
            return;
        }

        // ItemData에서 원가 저장
        itemCost = itemData.cost;

        // 상품명 표시
        if (productNameText != null)
        {
            productNameText.text = itemData.itemName;
        }

        // 현재 재고 표시
        if (stockText != null)
        {
            stockText.text = inventoryManager.GetStock(itemType).ToString();
        }

        // 원가 표시
        if (costText != null)
        {
            costText.text = itemCost.ToString("N0") + "원";
        }

        // 수량과 금액 표시 갱신
        UpdateQuantityText();
        UpdateTotalPriceText();
    }

    private void IncreaseQuantity()
    {
        // 발주 수량 1 증가
        orderQuantity = orderQuantity + 1;

        Debug.Log(itemType + " 발주 수량 증가: " + orderQuantity);

        UpdateButtonState();
        UpdateQuantityText();
        UpdateTotalPriceText();

        // 주문 합계 갱신
        if (orderPanelController != null)
        {
            orderPanelController.UpdateOrderTotalText();
        }
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

        Debug.Log(itemType + " 발주 수량 감소: " + orderQuantity);

        UpdateButtonState();
        UpdateQuantityText();
        UpdateTotalPriceText();

        // 주문 합계 갱신
        if (orderPanelController != null)
        {
            orderPanelController.UpdateOrderTotalText();
        }
    }

    private void UpdateButtonState()
    {
        // 수량이 0이면 - 버튼 비활성화
        if (minusButton != null)
        {
            minusButton.interactable = orderQuantity > 0;
        }
    }

    private void UpdateQuantityText()
    {
        // 발주 수량 텍스트 갱신
        if (quantityText != null)
        {
            quantityText.text = orderQuantity.ToString();
        }
    }

    private void UpdateTotalPriceText()
    {
        // 상품별 발주 금액 갱신
        if (totalPriceText != null)
        {
            int totalPrice = itemCost * orderQuantity;
            totalPriceText.text = totalPrice.ToString("N0") + "원";
        }
    }

    public int GetOrderCost()
    {
        // 상품 원가 * 발주 수량
        return itemCost * orderQuantity;
    }

    public int GetOrderQuantity()
    {
        // 현재 발주 수량 반환
        return orderQuantity;
    }

    public ItemType GetItemType()
    {
        // 현재 Row의 상품 종류 반환
        return itemType;
    }
}