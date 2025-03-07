using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BGMSound
{
    Main,
    Map1,
    TraingMap,
    Map2
}
public enum SFXSound
{
    Ready,
    ReadyGo,
    Motor,
    Drift,
    Booster
}
public class SoundManger : MonoBehaviour
{
    [SerializeField] static GameObject _BGMManager;
    [SerializeField] static GameObject _SFXManager;

    static public SoundManger _sound;

    [SerializeField] AudioClip[] _bgmClip;
    [SerializeField] AudioClip[] _sfxClip;
    static AudioSource _audioBgmSource;
    static AudioSource _audioSfxSource;

    static void Init()
    {
        if (_sound == null)
        {
            GameObject gm = GameObject.Find("SoundManager");
            if (gm == null)
            {
                gm = new GameObject { name = "SoundManager" };

                gm.AddComponent<SoundManger>();
            }
            DontDestroyOnLoad(gm);
            _sound = gm.GetComponent<SoundManger>();

            _BGMManager = GameObject.Find("BGMManager");
            _SFXManager = GameObject.Find("SFXManager");
            _audioBgmSource = _BGMManager.GetComponent<AudioSource>();
            _audioSfxSource = _SFXManager.GetComponent<AudioSource>();
        }
    }
    void Awake()
    {
        Init();
    }

    void Update()
    {
        _audioBgmSource.volume = GameManager.Instance.BGMVolume;
        _audioSfxSource.volume = GameManager.Instance.SFXVolume;
    }
    public void PlayBGM(int musicNum)
    {
        _audioBgmSource.clip = _bgmClip[musicNum];
        _audioBgmSource.volume = GameManager.Instance.BGMVolume;//상시 조절되게 바깥으로 뺌
        _audioBgmSource.loop = true;
        _audioBgmSource.Play();
    }
    public void StopBGM(int musicNum)
    {
        _audioBgmSource.clip = _bgmClip[musicNum];
        if (_audioBgmSource.isPlaying)
        {
            _audioBgmSource.Stop();
        }
    }
    public void PlaySfx(int soundNum)
    {
        _audioSfxSource.volume = GameManager.Instance.SFXVolume;//변수 값으로 조절
        _audioSfxSource.playOnAwake = true;
        _audioSfxSource.loop = false;
        _audioSfxSource.PlayOneShot(_sfxClip[soundNum]);//한개만 적용
    }
}
