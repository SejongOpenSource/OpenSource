using UnityEngine;
using UnityEngine.UI;

public class OrderProductRowUI : MonoBehaviour
{
    // 이 Row가 담당하는 상품 종류
    public ItemType itemType;

    [Header("발주 수량 슬라이더")]
    // 발주 수량을 조절하는 슬라이더
    public Slider quantitySlider;

    // 슬라이더로 선택할 수 있는 최대 발주 수량
    public int maxOrderQuantity = 100;

    [Header("기존 +/- 버튼")]
    // 기존 버튼 UI를 아직 남겨둘 경우 사용 가능
    // 슬라이더만 사용할 거면 Inspector에서 비워둬도 됨
    public Button minusButton;

    public Button plusButton;

    [Header("텍스트 UI")]
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

    [Header("주문 패널")]
    // 주문 화면 전체 컨트롤러
    public OrderPanelController orderPanelController;

    // 현재 발주 수량
    public int orderQuantity = 0;

    // 이 상품의 원가
    private int itemCost = 0;

    private void Start()
    {
        // 슬라이더 이벤트 연결
        if (quantitySlider != null)
        {
            quantitySlider.onValueChanged.AddListener(OnQuantitySliderChanged);
        }

        // 기존 + 버튼이 연결되어 있으면 그대로 사용 가능
        if (plusButton != null)
        {
            plusButton.onClick.AddListener(IncreaseQuantity);
        }

        // 기존 - 버튼이 연결되어 있으면 그대로 사용 가능
        if (minusButton != null)
        {
            minusButton.onClick.AddListener(DecreaseQuantity);
        }
    }

    private void OnDestroy()
    {
        // 슬라이더 이벤트 해제
        if (quantitySlider != null)
        {
            quantitySlider.onValueChanged.RemoveListener(OnQuantitySliderChanged);
        }

        // 버튼 이벤트 해제
        if (plusButton != null)
        {
            plusButton.onClick.RemoveListener(IncreaseQuantity);
        }

        if (minusButton != null)
        {
            minusButton.onClick.RemoveListener(DecreaseQuantity);
        }
    }

    public void SetupRow()
    {
        ItemData itemData = DataManager.Instance?.GetItem(itemType);

        if (itemData == null)
        {
            Debug.LogWarning($"OrderProductRowUI: ItemData not found for {itemType}");
            return;
        }

        itemCost = itemData.cost;

        if (productNameText != null)
        {
            productNameText.text = itemData.itemName;
        }

        if (stockText != null)
        {
            stockText.text = (GameManager.Instance?.inventoryManager?.GetStock(itemType) ?? 0).ToString();
        }

        if (costText != null)
        {
            costText.text = itemCost.ToString("N0") + "원";
        }

        // 상품 데이터가 연결된 뒤 슬라이더, 버튼, 텍스트 상태를 한 번에 갱신
        InitializeSlider();
        UpdateButtonState();
        UpdateQuantityText();
        UpdateTotalPriceText();
    }

    private void InitializeSlider()
    {
        // 최대 수량이 음수로 들어오지 않도록 방지
        if (maxOrderQuantity < 0)
        {
            maxOrderQuantity = 0;
        }

        // 현재 수량도 범위 안으로 제한
        orderQuantity = System.Math.Max(0, System.Math.Min(orderQuantity, maxOrderQuantity));

        if (quantitySlider == null)
        {
            return;
        }

        // 슬라이더는 정수 단위로만 움직이게 설정
        quantitySlider.wholeNumbers = true;
        quantitySlider.minValue = 0;
        quantitySlider.maxValue = maxOrderQuantity;

        // 이벤트를 다시 발생시키지 않고 현재 발주 수량 반영
        quantitySlider.SetValueWithoutNotify(orderQuantity);
    }

    private void OnQuantitySliderChanged(float value)
    {
        // 슬라이더 값이 바뀌면 발주 수량 변경
        int newQuantity = Mathf.RoundToInt(value);
        SetOrderQuantity(newQuantity);
    }

    private void IncreaseQuantity()
    {
        // 기존 + 버튼을 사용할 경우 발주 수량 1 증가
        SetOrderQuantity(orderQuantity + 1);
    }

    private void DecreaseQuantity()
    {
        // 기존 - 버튼을 사용할 경우 발주 수량 1 감소
        SetOrderQuantity(orderQuantity - 1);
    }

    private void SetOrderQuantity(int newQuantity)
    {
        // 발주 수량을 0 ~ maxOrderQuantity 사이로 제한
        orderQuantity = System.Math.Max(0, System.Math.Min(newQuantity, maxOrderQuantity));

        // 슬라이더 위치도 현재 발주 수량과 맞춤
        if (quantitySlider != null)
        {
            quantitySlider.SetValueWithoutNotify(orderQuantity);
        }

        UpdateButtonState();
        UpdateQuantityText();
        UpdateTotalPriceText();

        // 주문 합계 갱신
        if (orderPanelController != null)
        {
            orderPanelController.UpdateOrderTotalText();
        }
    }

    public void RefreshStock()
    {
        if (stockText != null)
        {
            stockText.text = (GameManager.Instance?.inventoryManager?.GetStock(itemType) ?? 0).ToString();
        }
    }

    private void UpdateButtonState()
    {
        // 기존 - 버튼이 있으면 수량 0일 때 비활성화
        if (minusButton != null)
        {
            minusButton.interactable = orderQuantity > 0;
        }

        // 기존 + 버튼이 있으면 최대 수량일 때 비활성화
        if (plusButton != null)
        {
            plusButton.interactable = orderQuantity < maxOrderQuantity;
        }
    }

    private void UpdateQuantityText()
    {
        // 발주 수량 텍스트 갱신
        if (quantityText != null)
        {
            quantityText.text = orderQuantity.ToString("N0") + "개";
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