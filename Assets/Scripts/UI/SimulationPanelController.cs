using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SimulationPanelController : MonoBehaviour
{
    [Header("프로그레스 바")]
    // 자동으로 차오르는 진행바
    // Unity Slider 오브젝트를 연결하면 됨
    public Slider progressSlider;

    [Header("설정")]
    // 게이지가 0에서 1까지 차는 시간
    // PhasePanelManager의 Simulation 대기 시간과 맞추면 자연스러움
    public float duration = 1f;

    // 현재 실행 중인 코루틴 저장
    private Coroutine progressCoroutine;

    private void OnEnable()
    {
        // SimulationPanel이 켜질 때마다 게이지를 처음부터 시작
        StartProgress();
    }

    private void OnDisable()
    {
        // 패널이 꺼질 때 진행 중인 코루틴 정리
        if (progressCoroutine != null)
        {
            StopCoroutine(progressCoroutine);
            progressCoroutine = null;
        }
    }

    private void StartProgress()
    {
        // Slider 초기화
        if (progressSlider != null)
        {
            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.value = 0f;
            progressSlider.interactable = false;
        }

        // 게이지 진행 시작
        progressCoroutine = StartCoroutine(ProgressRoutine());
    }

    private IEnumerator ProgressRoutine()
    {
        float timer = 0f;

        // duration 시간 동안 게이지를 0에서 1까지 채움
        while (timer < duration)
        {
            timer += Time.deltaTime;

            // 현재 진행률 계산
            float progress = Mathf.Clamp01(timer / duration);

            // Slider 값 갱신
            if (progressSlider != null)
            {
                progressSlider.value = progress;
            }

            yield return null;
        }

        // 마지막 값 보정
        if (progressSlider != null)
        {
            progressSlider.value = 1f;
        }
    }
}