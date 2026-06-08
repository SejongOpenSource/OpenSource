using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFXType
{
    Money,      // 돈 결제 소리
    Fridge,     // 냉장고 문 소리
    Bell,       // 손님 입장 벨소리
    Barcode     // 바코드 스캔 소리
}

public enum BGMType {
    Title,     // 타이틀 화면 음악 1
    Main,       // 메인 게임 진행 음악
    Clear,      // 스테이지 클리어 음악
    GameOver    // 게임 오버 음악
}

public class SoundManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static SoundManager Instance { get; private set; }

    // 인스펙터에서 SFX 타입과 클립을 매칭하기 위한 구조체
    [System.Serializable]
    public struct SFXData { public SFXType type; public AudioClip clip; }

    // 인스펙터에서 BGM 타입과 클립을 매칭하기 위한 구조체
    [System.Serializable]
    public struct BGMData { public BGMType type; public AudioClip clip; }
    
    [Header("Sounds List")]
    // 설정된 효과음 리스트
    [SerializeField] private List<SFXData> sfxList = new List<SFXData>();
    // 설정된 배경음 리스트
    [SerializeField] private List<BGMData> bgmList = new List<BGMData>();
    
    private Dictionary<SFXType, AudioClip> _sfxDictionary = new Dictionary<SFXType, AudioClip>();
    private Dictionary<BGMType, AudioClip> _bgmDictionary = new Dictionary<BGMType, AudioClip>();
    private AudioSource _sfxSource;
    private AudioSource _bgmSource;

    private Coroutine _fadeCoroutine;
    
    // 볼륨 조절을 위한 마스터 볼륨 (0.0f ~ 1.0f)
    private float _bgmVolumeMaster = 0.7f;
    private float _sfxVolumeMaster = 1.0f;

    private void Awake()
    {
        // 싱글톤 설정: 이미 존재하면 파괴, 없으면 유지
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // 게임 시작 시 초기 BGM 재생
        PlayBGM(BGMType.Title, fade: false);
    }
    
    // 사운드 시스템 초기화: AudioSource 생성 및 리스트를 조회용 사전으로 변환
    private void InitManager()
    {
        // BGM 재생기 설정
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.loop = true;
        _bgmSource.playOnAwake = false;
        _bgmSource.volume = _bgmVolumeMaster;
        
        // SFX 재생기 설정
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.loop = false;
        _sfxSource.playOnAwake = false;
        _sfxSource.volume = _sfxVolumeMaster;

        // 리스트 데이터를 딕셔너리로 옮겨 빠른 검색이 가능하게 함
        foreach (var data in sfxList)
        {
            if (data.clip != null && !_sfxDictionary.ContainsKey(data.type))
                _sfxDictionary.Add(data.type, data.clip);
        }
        
        foreach (var data in bgmList)
        {
            if (data.clip != null && !_bgmDictionary.ContainsKey(data.type))
                _bgmDictionary.Add(data.type, data.clip);
        }
    }
    
    // 배경음 재생 (페이드 인/아웃 지원)
    public void PlayBGM(BGMType type, bool fade = true, float fadeDuration = 1.0f)
    {
        if (!_bgmDictionary.TryGetValue(type, out AudioClip nextClip)) return;
        if (_bgmSource.clip == nextClip && _bgmSource.isPlaying) return;

        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);

        if (fade && _bgmSource.isPlaying)
        {
            _fadeCoroutine = StartCoroutine(FadeTrackRoutine(nextClip, fadeDuration));
        }
        else
        {
            _bgmSource.clip = nextClip;
            _bgmSource.volume = _bgmVolumeMaster;
            _bgmSource.Play();
        }
    }

    // 배경음 정지
    public void StopBGM()
    {
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _bgmSource.Stop();
        _bgmSource.clip = null;
    }

    // 효과음 일회성 재생
    public void PlaySFX(SFXType type)
    {
        if (_sfxDictionary.TryGetValue(type, out AudioClip clip))
        {
            _sfxSource.PlayOneShot(clip, _sfxVolumeMaster);
        }
    }

    /// <summary>
    /// 음악 전환 시 자연스럽게 전환하기 위한 페이드 아웃/인 코루틴
    /// </summary>
    private IEnumerator FadeTrackRoutine(AudioClip nextClip, float duration)
    {
        float halfDuration = duration * 0.5f;
        if (halfDuration <= 0f)
        {
            _bgmSource.clip = nextClip;
            _bgmSource.volume = _bgmVolumeMaster;
            _bgmSource.Play();
            yield break;
        }

        // 볼륨 낮추기 (Fade Out)
        float startVolume = _bgmSource.volume;
        while (_bgmSource.volume > 0)
        {
            _bgmSource.volume -= startVolume * (Time.unscaledDeltaTime / halfDuration);
            yield return null;
        }
        
        _bgmSource.Stop();
        _bgmSource.clip = nextClip;
        _bgmSource.volume = 0f;
        _bgmSource.Play();

        // 볼륨 높이기 (Fade In)
        while (_bgmSource.volume < _bgmVolumeMaster)
        {
            _bgmSource.volume += _bgmVolumeMaster * (Time.unscaledDeltaTime / halfDuration);
            yield return null;
        }

        _bgmSource.volume = _bgmVolumeMaster;
    }
}