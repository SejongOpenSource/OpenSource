using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanelController : MonoBehaviour
{
    [Header("보유 상권 표시")]
    // 구매한 상권 목록을 보여주는 텍스트
    // 예: 보유 상권: 주거지, 학원가
    public Text ownedDistrictText;

    [Header("상권 구매 버튼")]
    // 학원가 구매 버튼
    public Button academyBuyButton;

    // 대학가 구매 버튼
    public Button campusBuyButton;

    // 오피스가 구매 버튼
    public Button businessBuyButton;

    // 관광지 구매 버튼
    public Button touristBuyButton;

    [Header("상권 가격 텍스트")]
    // 학원가 가격 텍스트
    // DistrictData.investmentCost 값을 표시함
    public Text academyPriceText;

    // 대학가 가격 텍스트
    // DistrictData.investmentCost 값을 표시함
    public Text campusPriceText;

    // 오피스가 가격 텍스트
    // DistrictData.investmentCost 값을 표시함
    public Text businessPriceText;

    // 관광지 가격 텍스트
    // DistrictData.investmentCost 값을 표시함
    public Text touristPriceText;

    private void Start()
    {
        // 버튼 클릭 이벤트 연결
        if (academyBuyButton != null)
        {
            academyBuyButton.onClick.AddListener(BuyAcademy);
        }

        if (campusBuyButton != null)
        {
            campusBuyButton.onClick.AddListener(BuyCampus);
        }

        if (businessBuyButton != null)
        {
            businessBuyButton.onClick.AddListener(BuyBusiness);
        }

        if (touristBuyButton != null)
        {
            touristBuyButton.onClick.AddListener(BuyTourist);
        }

        // 시작 시 UI 갱신
        RefreshUpgradePanel();
    }

    private void OnEnable()
    {
        // 패널이 다시 켜질 때 최신 데이터로 UI 갱신
        RefreshUpgradePanel();
    }

    private void RefreshUpgradePanel()
    {
        // 보유 상권 목록 갱신
        UpdateOwnedDistrictText();

        // 구매 완료 버튼 상태 갱신
        UpdateButtonStates();

        // 상권 가격 텍스트를 백엔드 데이터 기준으로 갱신
        UpdatePriceTexts();
    }

    private void BuyAcademy()
    {
        // 학원가 구매
        BuyDistrict(DistrictType.Academy);
    }

    private void BuyCampus()
    {
        // 대학가 구매
        BuyDistrict(DistrictType.Campus);
    }

    private void BuyBusiness()
    {
        // 오피스가 구매
        BuyDistrict(DistrictType.Business);
    }

    private void BuyTourist()
    {
        // 관광지 구매
        BuyDistrict(DistrictType.Tourist);
    }

    private void BuyDistrict(DistrictType districtType)
    {
        // StoreManager 확인
        StoreManager storeManager = GetStoreManager();

        if (storeManager == null)
        {
            Debug.LogError("UpgradePanelController: StoreManager를 찾을 수 없습니다.");
            return;
        }

        // StoreManager에서 실제 상권 구매 처리
        bool success = storeManager.PurchaseDistrict(districtType);

        // 성공/실패와 관계없이 UI를 최신 상태로 갱신
        RefreshUpgradePanel();

        if (success)
        {
            Debug.Log($"{GetDistrictName(districtType)} 상권 구매 UI 갱신 완료");
        }
    }

    private StoreManager GetStoreManager()
    {
        // GameManager가 없으면 StoreManager를 가져올 수 없음
        if (GameManager.Instance == null)
        {
            return null;
        }

        return GameManager.Instance.storeManager;
    }

    private bool IsPurchased(DistrictType districtType)
    {
        StoreManager storeManager = GetStoreManager();

        if (storeManager == null)
        {
            return false;
        }

        return storeManager.IsDistrictPurchased(districtType);
    }

    private void UpdateOwnedDistrictText()
    {
        // 텍스트가 연결되지 않았으면 표시할 수 없음
        if (ownedDistrictText == null)
        {
            return;
        }

        StoreManager storeManager = GetStoreManager();

        // 주거지는 기본 보유 상권이므로 항상 표시
        List<string> ownedNames = new List<string>();
        ownedNames.Add("주거지");

        if (storeManager != null)
        {
            List<DistrictType> purchasedDistricts = storeManager.GetPurchasedDistricts();

            // StoreManager에 저장된 구매 상권을 화면에 추가
            for (int i = 0; i < purchasedDistricts.Count; i++)
            {
                DistrictType districtType = purchasedDistricts[i];

                // 주거지는 이미 맨 앞에 넣었으므로 중복 방지
                if (districtType == DistrictType.Resident)
                {
                    continue;
                }

                string districtName = GetDistrictName(districtType);

                // 중복 표시 방지
                if (ownedNames.Contains(districtName) == false)
                {
                    ownedNames.Add(districtName);
                }
            }
        }

        // 보유 상권 목록 표시
        ownedDistrictText.text = "보유 상권: " + string.Join(", ", ownedNames);
    }

    private void UpdateButtonStates()
    {
        // 구매 완료된 상권 버튼은 비활성화
        SetButtonPurchasedState(academyBuyButton, IsPurchased(DistrictType.Academy));
        SetButtonPurchasedState(campusBuyButton, IsPurchased(DistrictType.Campus));
        SetButtonPurchasedState(businessBuyButton, IsPurchased(DistrictType.Business));
        SetButtonPurchasedState(touristBuyButton, IsPurchased(DistrictType.Tourist));
    }

    private void SetButtonPurchasedState(Button button, bool isPurchased)
    {
        if (button == null)
        {
            return;
        }

        // 이미 구매한 버튼은 다시 누르지 못하게 함
        button.interactable = isPurchased == false;

        // 버튼 안의 Text를 찾아서 문구 변경
        Text buttonText = button.GetComponentInChildren<Text>();

        if (buttonText != null)
        {
            if (isPurchased)
            {
                buttonText.text = "구매 완료";
            }
            else
            {
                buttonText.text = "구매하기";
            }
        }
    }

    private void UpdatePriceTexts()
    {
        // 각 가격 Text를 DistrictData의 investmentCost 기준으로 갱신
        SetPriceText(academyPriceText, DistrictType.Academy);
        SetPriceText(campusPriceText, DistrictType.Campus);
        SetPriceText(businessPriceText, DistrictType.Business);
        SetPriceText(touristPriceText, DistrictType.Tourist);
    }

    private void SetPriceText(Text priceText, DistrictType districtType)
    {
        // 가격 Text가 연결되지 않았으면 표시할 수 없음
        if (priceText == null)
        {
            return;
        }

        // DataManager가 없으면 가격 데이터를 가져올 수 없음
        if (DataManager.Instance == null)
        {
            priceText.text = "-";
            return;
        }

        // 백엔드 상권 데이터 가져오기
        DistrictData districtData = DataManager.Instance.GetDistrict(districtType);

        if (districtData == null)
        {
            priceText.text = "-";
            return;
        }

        // 실제 백엔드 가격 표시
        priceText.text = $"{districtData.investmentCost:N0}원";
    }

    private string GetDistrictName(DistrictType districtType)
    {
        // DistrictType enum 값을 화면용 한글 이름으로 변환
        switch (districtType)
        {
            case DistrictType.Resident:
                return "주거지";

            case DistrictType.Academy:
                return "학원가";

            case DistrictType.Campus:
                return "대학가";

            case DistrictType.Business:
                return "오피스가";

            case DistrictType.Tourist:
                return "관광지";

            default:
                return "알 수 없음";
        }
    }
}