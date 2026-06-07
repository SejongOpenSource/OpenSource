using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFXType
{
    Money, 
    Fridge, 
    Bell, 
    Barcode
}
public enum BGMType {
    Title1, 
    Title2,
    Main
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public struct SFXData { public SFXType type; public AudioClip clip; }

    [System.Serializable]
    public struct BGMData { public BGMType type; public AudioClip clip; }
    
    [Header("Sounds List")]
    [SerializeField] private List<SFXData> sfxList = new List<SFXData>();
    [SerializeField] private List<BGMData> bgmList = new List<BGMData>();
    
    private Dictionary<SFXType, AudioClip> _sfxDictionary = new Dictionary<SFXType, AudioClip>();
    private Dictionary<BGMType, AudioClip> _bgmDictionary = new Dictionary<BGMType, AudioClip>();
    private AudioSource _sfxSource;
    private AudioSource _bgmSource;

    private Coroutine _fadeCoroutine;
    
    // 옵션 창에서 볼륨 조절할 때 사용할 전역 변수 (0.0f ~ 1.0f)
    private float _bgmVolumeMaster = 1.0f;
    private float _sfxVolumeMaster = 1.0f;

    private void Awake()
    {
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
    
    private void InitManager()
    {
        // BGM 소스 생성
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.loop = true;
        _bgmSource.playOnAwake = false;
        _bgmSource.volume = _bgmVolumeMaster;
        
        // SFX 소스 생성
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.loop = false;
        _sfxSource.playOnAwake = false;
        _sfxSource.volume = _sfxVolumeMaster;

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

    public void StopBGM()
    {
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _bgmSource.Stop();
        _bgmSource.clip = null;
    }

    public void PlaySFX(SFXType type)
    {
        if (_sfxDictionary.TryGetValue(type, out AudioClip clip))
        {
            // PlayOneShot의 두 번째 인자로 볼륨을 직접 넘겨줍니다.
            _sfxSource.PlayOneShot(clip, _sfxVolumeMaster);
        }
    }

    /// <summary>
    /// AudioSource의 volume 변수를 직접 조절하는 페이드 코루틴
    /// </summary>
    private IEnumerator FadeTrackRoutine(AudioClip nextClip, float duration)
    {
        float halfDuration = duration * 0.5f;

        // Fade Out
        float startVolume = _bgmSource.volume;
        while (_bgmSource.volume > 0)
        {
            _bgmSource.volume -= startVolume * (Time.deltaTime / halfDuration);
            yield return null;
        }
        
        _bgmSource.Stop();
        _bgmSource.clip = nextClip;
        _bgmSource.Play();

        // Fade In
        while (_bgmSource.volume < _bgmVolumeMaster)
        {
            _bgmSource.volume += _bgmVolumeMaster * (Time.deltaTime / halfDuration);
            yield return null;
        }

        _bgmSource.volume = _bgmVolumeMaster;
    }
    
    // --- 나중에 설정창(UI) 만들 때 쓸 볼륨 조절 함수들 ---
    public void SetBGMVolume(float volume)
    {
        _bgmVolumeMaster = Mathf.Clamp01(volume);
        _bgmSource.volume = _bgmVolumeMaster; // 실시간 반영
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolumeMaster = Mathf.Clamp01(volume);
        _sfxSource.volume = _sfxVolumeMaster; // 실시간 반영
    }
}