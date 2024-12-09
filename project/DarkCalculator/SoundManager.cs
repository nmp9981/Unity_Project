using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] static GameObject _BGMManager;
    
    static public SoundManager _sound;

    [SerializeField] AudioClip[] _bgmClip;
    static AudioSource _audioBgmSource;


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

            _BGMManager = GameObject.Find("BGMManager");
            _audioBgmSource = _BGMManager.GetComponent<AudioSource>();
        }
    }
    void Awake()
    {
        Init();
    }

    void Update()
    {
        _audioBgmSource.volume = GameManager.Instance.BGMVolume;
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
}
