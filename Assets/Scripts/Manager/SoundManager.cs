using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public enum SFXType
{
    Money,
    Fridge,
    Bell,
    Barcode
}

public enum BGMType
{
    Title1,
    Title2,
    Main
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    [Header("Audio Mixer & Groups")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [System.Serializable]
    public struct SFXData
    {
        public SFXType type;
        public AudioClip clip;
    }

    [System.Serializable]
    public struct BGMData
    {
        public BGMType type;
        public AudioClip clip;
    }
    
    [Header("Sounds List")]
    [SerializeField] private List<SFXData> sfxList = new List<SFXData>();
    [SerializeField] private List<BGMData> bgmList = new List<BGMData>();
    
    private Dictionary<SFXType, AudioClip> _sfxDictionary = new Dictionary<SFXType, AudioClip>();
    private Dictionary<BGMType, AudioClip> _bgmDictionary = new Dictionary<BGMType, AudioClip>();
    private AudioSource _sfxSource;
    private AudioSource _bgmSource;
    
    private void Awake()
    {
        // 싱글톤 중복 생성 방지 및 씬 전환 시 파괴 방지
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
        // BGM 전용 소스 생성 및 설정
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.outputAudioMixerGroup = bgmGroup;
        _bgmSource.loop = true;
        _bgmSource.playOnAwake = false;
        
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.outputAudioMixerGroup = sfxGroup;
        _sfxSource.loop = false;
        _sfxSource.playOnAwake = false;

        foreach (var data in sfxList)
        {
            if (data.clip != null && !_sfxDictionary.ContainsKey(data.type))
            {
                _sfxDictionary.Add(data.type, data.clip);
            }
        }
        
        foreach (var data in bgmList)
        {
            if (data.clip != null && !_bgmDictionary.ContainsKey(data.type))
            {
                _bgmDictionary.Add(data.type, data.clip);
            }
        }
    }
    
    public void PlayBGM(BGMType type, bool fade = true)
    {
        if (_bgmDictionary.TryGetValue(type, out AudioClip clip))
        {
            _bgmSource.clip = clip;
            _bgmSource.Play();
        }
        // TODO: 필요 시 코루틴을 활용한 Fade In/Out 로직 추가
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
        _bgmSource.clip = null;
    }

    public void PlaySFX(SFXType type)
    {
        if (_sfxDictionary.TryGetValue(type, out AudioClip clip))
        {
            _sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] {type}에 해당하는 사운드가 등록되지 않았습니다.");
        }
    }
}
