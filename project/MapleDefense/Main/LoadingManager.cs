using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField]
    GameObject loadingObj;

    [SerializeField]
    Image loadingBar;
    [SerializeField]
    TextMeshProUGUI loadingText;

    float loadingTime = 0;
    void Awake()
    {
        loadingObj.SetActive(false);
    }

    void Update()
    {
        GageFill();
    }
    /// <summary>
    /// 로딩창 On
    /// </summary>
    public void LoadingPrograssBar()
    {
        loadingTime = 0;
        loadingText.text = "0%";
        loadingBar.fillAmount = 0;
        loadingObj.SetActive(true);
       
        Invoke("LoadingUIOff", 5f);
    }
    /// <summary>
    /// 게이지 채우기
    /// </summary>
    void GageFill()
    {
        loadingTime += Time.deltaTime;
        //로딩바 채우기
        float loadingRate = Mathf.Min(1, loadingTime / 4.5f);
        loadingBar.fillAmount = loadingRate;
        loadingText.text = $"{Mathf.Round(loadingRate * 100)}%";
    }

    /// <summary>
    /// 로딩창 Off
    /// </summary>
    void LoadingUIOff()
    {
        InitCastleHP();
        loadingObj.SetActive(false);
    }
    /// <summary>
    /// 성 HP 초기화
    /// </summary>
    void InitCastleHP()
    {
        GameManager.Instance.FullCastleHP = 900;
        GameManager.Instance.CurrentCastleHP = GameManager.Instance.FullCastleHP;
    }
}
