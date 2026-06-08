using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndResultUI : MonoBehaviour
{
    [Header("결과 패널")]
    public GameObject clearPanel;
    public GameObject gameOverPanel;

    [Header("클리어 결과 텍스트")]
    public Text clearDayText;
    public Text clearMoneyText;
    public Text clearDebtText;
    public Text clearRepayText;

    [Header("게임오버 결과 텍스트")]
    public Text gameOverDayText;
    public Text gameOverMoneyText;
    public Text gameOverDebtText;
    public Text gameOverRepayText;

    [Header("클리어 버튼")]
    public Button clearMainMenuButton;
    public Button clearRetryButton;

    [Header("게임오버 버튼")]
    public Button gameOverMainMenuButton;
    public Button gameOverRetryButton;

    [Header("씬 이름")]
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "PlayerEconomy";

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += ShowResult;
        }

        ConnectButtons();
        HideAllPanels();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= ShowResult;
        }

        DisconnectButtons();
    }

    private void ConnectButtons()
    {
        if (clearMainMenuButton != null)
        {
            clearMainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        if (clearRetryButton != null)
        {
            clearRetryButton.onClick.AddListener(RetryGame);
        }

        if (gameOverMainMenuButton != null)
        {
            gameOverMainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        if (gameOverRetryButton != null)
        {
            gameOverRetryButton.onClick.AddListener(RetryGame);
        }
    }

    private void DisconnectButtons()
    {
        if (clearMainMenuButton != null)
        {
            clearMainMenuButton.onClick.RemoveListener(GoToMainMenu);
        }

        if (clearRetryButton != null)
        {
            clearRetryButton.onClick.RemoveListener(RetryGame);
        }

        if (gameOverMainMenuButton != null)
        {
            gameOverMainMenuButton.onClick.RemoveListener(GoToMainMenu);
        }

        if (gameOverRetryButton != null)
        {
            gameOverRetryButton.onClick.RemoveListener(RetryGame);
        }
    }

    private void HideAllPanels()
    {
        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void ShowResult(bool isClear)
    {
        HideAllPanels();
        UpdateResultTexts(isClear);

        if (isClear)
        {
            if (clearPanel != null)
            {
                clearPanel.SetActive(true);

                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayBGM(BGMType.Clear, fade: false);
                }
            }
        }
        else
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayBGM(BGMType.GameOver, fade: false);
                }
            }
        }

        // 2. 🚨 즉시 정지(Time.timeScale = 0)하지 않고, 
        // 사운드가 오디오 버퍼에 로드될 시간을 주기 위해 코루틴으로 1프레임 지연 정지시킵니다.
        StartCoroutine(FreezeGameNextFrame());
    }

    /// <summary>
    /// 사운드가 정상적으로 시작된 후 다음 프레임에 게임을 일시정지하는 코루틴
    /// </summary>
    private IEnumerator FreezeGameNextFrame()
    {
        // 현실 시간 기준으로 단 1프레임만 대기 (Time.timeScale의 영향을 받지 않음)
        yield return null; 
    
        // 사운드가 켜진 것을 확인한 후 안전하게 게임을 멈춥니다.
        Time.timeScale = 0f;
        Debug.Log("게임이 정상적으로 일시정지되었습니다. 사운드가 출력됩니다.");
        
    }

    private void UpdateResultTexts(bool isClear)
    {
        int currentTurn = GetCurrentTurn();
        int currentMoney = GetCurrentMoney();
        int currentDebt = GetCurrentDebt();
        int totalRepayAmount = GetTotalRepayAmount();

        if (isClear)
        {
            if (clearDayText != null)
            {
                clearDayText.text = currentTurn + "일차";
            }

            if (clearMoneyText != null)
            {
                clearMoneyText.text = currentMoney.ToString("N0") + "원";
            }

            if (clearDebtText != null)
            {
                clearDebtText.text = currentDebt.ToString("N0") + "원";
            }

            if (clearRepayText != null)
            {
                clearRepayText.text = totalRepayAmount.ToString("N0") + "원";
            }
        }
        else
        {
            if (gameOverDayText != null)
            {
                gameOverDayText.text = currentTurn + "일차";
            }

            if (gameOverMoneyText != null)
            {
                gameOverMoneyText.text = currentMoney.ToString("N0") + "원";
            }

            if (gameOverDebtText != null)
            {
                gameOverDebtText.text = currentDebt.ToString("N0") + "원";
            }

            if (gameOverRepayText != null)
            {
                gameOverRepayText.text = totalRepayAmount.ToString("N0") + "원";
            }
        }
    }

    private int GetCurrentTurn()
    {
        if (TurnManager.Instance == null)
        {
            return 0;
        }

        return TurnManager.Instance.CurrentTurn;
    }

    private int GetCurrentMoney()
    {
        if (GameManager.Instance == null || GameManager.Instance.storeManager == null)
        {
            return 0;
        }

        return GameManager.Instance.storeManager.currentMoney;
    }

    private int GetCurrentDebt()
    {
        if (GameManager.Instance == null || GameManager.Instance.storeManager == null)
        {
            return 0;
        }

        return GameManager.Instance.storeManager.currentDebt;
    }

    private int GetTotalRepayAmount()
    {
        if (GameManager.Instance == null || GameManager.Instance.loan == null)
        {
            return 0;
        }

        return GameManager.Instance.loan.TotalRepayAmount;
    }

    private void GoToMainMenu()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(BGMType.Title, fade: true, fadeDuration: 1.1f);
        }
        
        ResetGameSession();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void RetryGame()
    {
        ResetGameSession();
        SceneManager.LoadScene(gameSceneName);
    }

    private void ResetGameSession()
    {
        // 정지된 시간 복구
        Time.timeScale = 1f;

        // DontDestroyOnLoad로 유지된 이전 게임 세션 제거
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }

        // 혹시 TurnManager도 살아있을 경우 제거
        if (TurnManager.Instance != null)
        {
            Destroy(TurnManager.Instance.gameObject);
        }
    }
}