using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEndResultUI : MonoBehaviour
{
    [Header("결과 패널")]
    // 게임 클리어 시 보여줄 패널
    public GameObject clearPanel;

    // 게임오버 시 보여줄 패널
    public GameObject gameOverPanel;

    [Header("클리어 결과 텍스트")]
    // 클리어 화면에 표시할 생존 일수
    public Text clearDayText;

    // 클리어 화면에 표시할 최종 자산
    public Text clearMoneyText;

    // 클리어 화면에 표시할 남은 부채
    public Text clearDebtText;

    // 클리어 화면에 표시할 총 상환 금액
    public Text clearRepayText;

    [Header("게임오버 결과 텍스트")]
    // 게임오버 화면에 표시할 생존 일수
    public Text gameOverDayText;

    // 게임오버 화면에 표시할 최종 자산
    public Text gameOverMoneyText;

    // 게임오버 화면에 표시할 남은 부채
    public Text gameOverDebtText;

    // 게임오버 화면에 표시할 총 상환 금액
    public Text gameOverRepayText;

    [Header("클리어 버튼")]
    // 클리어 화면의 메인 메뉴 이동 버튼
    public Button clearMainMenuButton;

    // 클리어 화면의 다시 시도 버튼
    public Button clearRetryButton;

    [Header("게임오버 버튼")]
    // 게임오버 화면의 메인 메뉴 이동 버튼
    public Button gameOverMainMenuButton;

    // 게임오버 화면의 다시 시도 버튼
    public Button gameOverRetryButton;

    [Header("씬 이름")]
    // 메인 메뉴 씬 이름
    public string mainMenuSceneName = "MainMenu";

    // 실제 게임 플레이 씬 이름
    public string gameSceneName = "PlayerEconomy";

    private void OnEnable()
    {
        // GameManager의 게임 종료 이벤트를 구독
        // 게임이 끝났을 때 ShowResult가 자동 호출됨
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver += ShowResult;
        }

        // 버튼 클릭 이벤트 연결
        ConnectButtons();

        // 처음 켜질 때는 두 결과 패널을 모두 숨김
        HideAllPanels();
    }

    private void OnDisable()
    {
        // 오브젝트가 비활성화될 때 이벤트 구독 해제
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= ShowResult;
        }

        // 버튼 클릭 이벤트 해제
        DisconnectButtons();
    }

    private void ConnectButtons()
    {
        // 클리어 화면 메인 메뉴 버튼 연결
        if (clearMainMenuButton != null)
        {
            clearMainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        // 클리어 화면 다시 시도 버튼 연결
        if (clearRetryButton != null)
        {
            clearRetryButton.onClick.AddListener(RetryGame);
        }

        // 게임오버 화면 메인 메뉴 버튼 연결
        if (gameOverMainMenuButton != null)
        {
            gameOverMainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        // 게임오버 화면 다시 시도 버튼 연결
        if (gameOverRetryButton != null)
        {
            gameOverRetryButton.onClick.AddListener(RetryGame);
        }
    }

    private void DisconnectButtons()
    {
        // 클리어 화면 메인 메뉴 버튼 이벤트 해제
        if (clearMainMenuButton != null)
        {
            clearMainMenuButton.onClick.RemoveListener(GoToMainMenu);
        }

        // 클리어 화면 다시 시도 버튼 이벤트 해제
        if (clearRetryButton != null)
        {
            clearRetryButton.onClick.RemoveListener(RetryGame);
        }

        // 게임오버 화면 메인 메뉴 버튼 이벤트 해제
        if (gameOverMainMenuButton != null)
        {
            gameOverMainMenuButton.onClick.RemoveListener(GoToMainMenu);
        }

        // 게임오버 화면 다시 시도 버튼 이벤트 해제
        if (gameOverRetryButton != null)
        {
            gameOverRetryButton.onClick.RemoveListener(RetryGame);
        }
    }

    private void HideAllPanels()
    {
        // 클리어 패널 숨김
        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }

        // 게임오버 패널 숨김
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void ShowResult(bool isClear)
    {
        // 기존에 켜져 있을 수 있는 결과 패널을 모두 숨김
        HideAllPanels();

        // 현재 게임 상태를 결과 텍스트에 반영
        UpdateResultTexts(isClear);

        // isClear가 true면 클리어 화면 표시
        if (isClear)
        {
            if (clearPanel != null)
            {
                clearPanel.SetActive(true);

                // 클리어 BGM 재생
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayBGM(BGMType.Clear, fade: false);
                }
            }
        }
        // isClear가 false면 게임오버 화면 표시
        else
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);

                // 게임오버 BGM 재생
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayBGM(BGMType.GameOver, fade: false);
                }
            }
        }

        // BGM이 바로 끊기지 않도록 한 프레임 뒤에 게임 정지
        StartCoroutine(FreezeGameNextFrame());
    }

    private IEnumerator FreezeGameNextFrame()
    {
        // 사운드 재생 요청이 처리될 수 있도록 한 프레임 대기
        yield return null;

        // 게임 진행을 정지
        // UI 버튼은 Time.timeScale이 0이어도 클릭 가능
        Time.timeScale = 0f;

        Debug.Log("게임이 정상적으로 일시정지되었습니다. 사운드가 출력됩니다.");
    }

    private void UpdateResultTexts(bool isClear)
    {
        // 결과 화면에 표시할 데이터 가져오기
        int currentTurn = GetCurrentTurn();
        int currentMoney = GetCurrentMoney();
        int currentDebt = GetCurrentDebt();
        int totalRepayAmount = GetTotalRepayAmount();

        // 클리어 화면 텍스트 갱신
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
        // 게임오버 화면 텍스트 갱신
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
        // TurnManager가 없으면 기본값 0 반환
        if (TurnManager.Instance == null)
        {
            return 0;
        }

        // 현재 턴 반환
        return TurnManager.Instance.CurrentTurn;
    }

    private int GetCurrentMoney()
    {
        // GameManager나 StoreManager가 없으면 기본값 0 반환
        if (GameManager.Instance == null || GameManager.Instance.storeManager == null)
        {
            return 0;
        }

        // 현재 보유 자산 반환
        return GameManager.Instance.storeManager.currentMoney;
    }

    private int GetCurrentDebt()
    {
        // GameManager나 StoreManager가 없으면 기본값 0 반환
        if (GameManager.Instance == null || GameManager.Instance.storeManager == null)
        {
            return 0;
        }

        // 현재 남은 부채 반환
        return GameManager.Instance.storeManager.currentDebt;
    }

    private int GetTotalRepayAmount()
    {
        // GameManager나 Loan이 없으면 기본값 0 반환
        if (GameManager.Instance == null || GameManager.Instance.loan == null)
        {
            return 0;
        }

        // 게임 중 실제로 상환한 총 금액 반환
        return GameManager.Instance.loan.TotalRepayAmount;
    }

    private void GoToMainMenu()
    {
        // 메인 메뉴로 이동하기 전에 타이틀 BGM 재생
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(BGMType.Title1, fade: true, fadeDuration: 1.1f);
        }

        // 이전 게임 세션 정리
        ResetGameSession();

        // 메인 메뉴 씬으로 이동
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void RetryGame()
    {
        // 이전 게임 세션 정리
        ResetGameSession();

        // 게임 씬을 다시 로드하여 새 게임 시작
        SceneManager.LoadScene(gameSceneName);
    }

    private void ResetGameSession()
    {
        // 결과 화면에서 멈춘 시간을 다시 정상 속도로 복구
        Time.timeScale = 1f;

        // DontDestroyOnLoad로 유지된 이전 GameManager 제거
        // 새 게임 시작 시 이전 자산, 부채, 턴 정보가 남지 않게 함
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }

        // TurnManager가 살아있을 경우 제거
        // 새 게임 시작 시 턴이 초기화되도록 함
        if (TurnManager.Instance != null)
        {
            Destroy(TurnManager.Instance.gameObject);
        }
    }
}