using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    [Header("씬 설정")]
    // 게임 시작 버튼을 눌렀을 때 이동할 게임 씬 이름
    // Build Profiles / Scene List에 등록된 씬 이름과 같아야 함
    [SerializeField] private string gameSceneName = "PlayerEconomy";

    public void StartGame()
    {
        // 씬 이름이 비어 있으면 이동하지 않음
        if (string.IsNullOrWhiteSpace(gameSceneName))
        {
            Debug.LogError("MainMenuController: gameSceneName이 비어 있습니다.");
            return;
        }

        // 게임 씬으로 이동
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료");

#if UNITY_EDITOR
        // Unity 에디터에서는 Play Mode 종료
        EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서는 애플리케이션 종료
        Application.Quit();
#endif
    }
}