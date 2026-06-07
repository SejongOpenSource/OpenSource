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
    // 게이지가차는 시간
    // PhasePanelManager의 Simulation 대기 시간과 맞추면 자연스러움
    public float duration = 4f;
    
    [Header("사운드 타이밍 설정")]
    [SerializeField] private float barcodeDelay = 0.8f;
    [Tooltip("바코드 소리가 반복되는 간격(초)")]
    [SerializeField] private float barcodeInterval = 1f;
    
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
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(SFXType.Bell);
            SoundManager.Instance.PlaySFX(SFXType.Fridge);
        }
        progressCoroutine = StartCoroutine(ProgressRoutine());
    }

    private IEnumerator ProgressRoutine()
    {
        // duration 값이 0 이하이면 나눗셈 계산을 하면 안 됨
        // 바로 100%로 처리하고 종료
        if (duration <= 0f)
        {
            if (progressSlider != null)
            {
                progressSlider.value = 1f;
            }

            yield break;
        }

        float timer = 0f;
        float barcodeTimer = barcodeInterval;

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
            
            if (timer >= barcodeDelay)
            {
                barcodeTimer += Time.deltaTime;
                
                if (barcodeTimer >= barcodeInterval)
                {
                    if (SoundManager.Instance != null)
                    {
                        SoundManager.Instance.PlaySFX(SFXType.Barcode);
                    }
                    barcodeTimer = 0f; // 타이머 리셋해서 다음 간격 재기
                }
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