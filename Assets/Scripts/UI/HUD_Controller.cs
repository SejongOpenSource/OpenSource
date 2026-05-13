using UnityEngine;
using UnityEngine.UI;

public class HUD_Controller : MonoBehaviour
{
    // 보유 자금을 관리하는 StoreManager
    public StoreManager storeManager;

    // 보유 자금 표시 Text
    public Text moneyValueText;

    private void Start()
    {
        // 화면이 처음 켜졌을 때 현재 자금 표시
        UpdateMoneyText();

        // 결제 성공 이벤트가 발생하면 자금 표시 갱신
        if (storeManager != null)
        {
            storeManager.OnPaymentSuccess += OnMoneyChanged;
        }
    }

    private void OnDestroy()
    {
        // 오브젝트가 사라질 때 이벤트 연결 해제
        if (storeManager != null)
        {
            storeManager.OnPaymentSuccess -= OnMoneyChanged;
        }
    }

    private void OnMoneyChanged(int currentMoney)
    {
        // 이벤트로 받은 현재 자금을 표시
        if (moneyValueText != null)
        {
            moneyValueText.text = currentMoney.ToString("N0") + "원";
        }
    }

    private void UpdateMoneyText()
    {
        // StoreManager나 Text가 연결되지 않았으면 실행하지 않음
        if (storeManager == null || moneyValueText == null)
        {
            return;
        }

        // 현재 보유 자금 표시
        moneyValueText.text = storeManager.currentMoney.ToString("N0") + "원";
    }
}