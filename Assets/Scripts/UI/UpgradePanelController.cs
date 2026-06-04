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

    // 구매한 상권 상태 저장
    // 현재는 UI 컨트롤러 안에서 관리 중
    // 리뷰 반영 시 StoreManager 쪽으로 옮기는 것이 더 안전함
    private Dictionary<DistrictType, bool> purchasedDistricts = new Dictionary<DistrictType, bool>();

    private void Start()
    {
        // 기본 상권인 주거지는 처음부터 보유 상태
        InitializePurchasedDistricts();

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

        // 시작 시 보유 상권 텍스트 갱신
        UpdateOwnedDistrictText();

        // 시작 시 구매 완료 버튼 상태 갱신
        UpdateButtonStates();
    }

    private void InitializePurchasedDistricts()
    {
        // 모든 상권 초기 구매 상태 설정
        purchasedDistricts[DistrictType.Resident] = true;
        purchasedDistricts[DistrictType.Academy] = false;
        purchasedDistricts[DistrictType.Campus] = false;
        purchasedDistricts[DistrictType.Business] = false;
        purchasedDistricts[DistrictType.Tourist] = false;
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
        // 이미 구매한 상권이면 다시 구매하지 않음
        if (IsPurchased(districtType))
        {
            Debug.Log($"{GetDistrictName(districtType)} 상권은 이미 구매했습니다.");
            return;
        }

        // GameManager 확인
        if (GameManager.Instance == null || GameManager.Instance.storeManager == null)
        {
            Debug.LogError("UpgradePanelController: GameManager 또는 StoreManager가 없습니다.");
            return;
        }

        // DataManager 확인
        if (DataManager.Instance == null)
        {
            Debug.LogError("UpgradePanelController: DataManager가 없습니다.");
            return;
        }

        // 구매하려는 상권 데이터 가져오기
        DistrictData districtData = DataManager.Instance.GetDistrict(districtType);

        if (districtData == null)
        {
            Debug.LogError($"UpgradePanelController: {districtType} 상권 데이터를 찾을 수 없습니다.");
            return;
        }

        StoreManager storeManager = GameManager.Instance.storeManager;

        // 상권 구매 비용 차감
        bool success = storeManager.SpendMoney(districtData.investmentCost);

        // 돈이 부족하면 구매 실패
        if (success == false)
        {
            Debug.LogWarning($"{GetDistrictName(districtType)} 상권 구매 실패: 자산 부족");
            return;
        }

        // 구매 성공 처리
        purchasedDistricts[districtType] = true;

        // 기존 판매 계산 구조가 현재 상권 하나를 사용하므로
        // 마지막으로 구매한 상권을 현재 적용 상권으로도 설정
        storeManager.SetDistrict(districtType, districtData);

        // UI 갱신
        UpdateOwnedDistrictText();
        UpdateButtonStates();

        Debug.Log($"{GetDistrictName(districtType)} 상권 구매 완료");
    }

    private bool IsPurchased(DistrictType districtType)
    {
        // Dictionary에 없으면 미구매로 처리
        if (purchasedDistricts.ContainsKey(districtType) == false)
        {
            return false;
        }

        return purchasedDistricts[districtType];
    }

    private void UpdateOwnedDistrictText()
    {
        // 텍스트가 연결되지 않았으면 표시할 수 없음
        if (ownedDistrictText == null)
        {
            return;
        }

        List<string> ownedNames = new List<string>();

        // 구매한 상권만 목록에 추가
        if (IsPurchased(DistrictType.Resident))
        {
            ownedNames.Add("주거지");
        }

        if (IsPurchased(DistrictType.Academy))
        {
            ownedNames.Add("학원가");
        }

        if (IsPurchased(DistrictType.Campus))
        {
            ownedNames.Add("대학가");
        }

        if (IsPurchased(DistrictType.Business))
        {
            ownedNames.Add("오피스가");
        }

        if (IsPurchased(DistrictType.Tourist))
        {
            ownedNames.Add("관광지");
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

        // 구매 완료된 버튼은 다시 누르지 못하게 함
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

    private string GetDistrictName(DistrictType districtType)
    {
        // DistrictType enum 값을 한글 이름으로 변환
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