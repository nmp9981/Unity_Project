using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    [SerializeField] static GameObject _BGMManager;
    [SerializeField] static GameObject _SFXManager;

    static SoundManager _sound;

    [SerializeField] AudioClip[] _bgmClip;
    [SerializeField] AudioClip[] _sfxClip;
    static AudioSource _audioBgmSource;
    static AudioSource _audioSfxSource;
    public static SoundManager Sound { get { Init(); return _sound; } }
    static void Init()
    {
        if (_sound == null)
        {
            GameObject gm = GameObject.Find("SoundManager");
            if (gm == null)
            {
                gm = new GameObject { name = "SoundManager" };

                gm.AddComponent<SoundManager>();
            }
            DontDestroyOnLoad(gm);
            _sound = gm.GetComponent<SoundManager>();
            _audioBgmSource = _BGMManager.AddComponent<AudioSource>();
            _audioSfxSource = _SFXManager.AddComponent<AudioSource>();
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        PlayBGM();
    }
    void PlayBGM()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            _audioBgmSource.clip = _bgmClip[0];
        }
        else if (SceneManager.GetActiveScene().name == "FishDomInGame")
        {
            _audioBgmSource.clip = _bgmClip[1];
        }

        if (!_audioBgmSource.isPlaying)
        {
            _audioBgmSource.volume = GameManager.Instance.BGMVolume;
            _audioBgmSource.playOnAwake = true;
            _audioBgmSource.loop = true;
            _audioBgmSource.Play();//한개만 적용
        }
    }
}
