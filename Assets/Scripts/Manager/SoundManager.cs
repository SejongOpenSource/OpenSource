using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    [Header("Audio Mixer & Groups")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    
    [Header("Audio Sources")]
    private AudioSource _bgmSource;
    private List<AudioSource> _sfxPool = new List<AudioSource>();
    
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private GameObject audioSourcePrefab;
    
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

        // SFX 오디오 풀 초기화
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewAudioSourceToPool();
        }
    }
    
    private AudioSource CreateNewAudioSourceToPool()
    {
        GameObject go = Instantiate(audioSourcePrefab, transform);
        AudioSource source = go.GetComponent<AudioSource>();
        source.outputAudioMixerGroup = sfxGroup;
        go.SetActive(false);
        _sfxPool.Add(source);
        return source;
    }

    public void PlayBGM(AudioClip clip, bool fade = true)
    {
        if (_bgmSource.clip == clip) return;
        
        _bgmSource.clip = clip;
        _bgmSource.Play();
        // TODO: 필요 시 코루틴을 활용한 Fade In/Out 로직 추가
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        
        AudioSource source = _sfxPool.Find(s => !s.gameObject.activeSelf);
        
        if (source == null)
        {
            source = CreateNewAudioSourceToPool();
        }

        source.gameObject.SetActive(true);
        source.clip = clip;
        source.Play();
        
        StartCoroutine(DisableSourceAfterPlayback(source));
    }

    private System.Collections.IEnumerator DisableSourceAfterPlayback(AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length);
        source.gameObject.SetActive(false);
    }
}
