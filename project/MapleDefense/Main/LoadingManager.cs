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

    // Update is called once per frame
    void Update()
    {
        if (loadingObj.activeSelf)
        {
            loadingTime += Time.deltaTime;
        }
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

        //로딩바 채우기
        float loadingRate = Mathf.Min(1,loadingTime/ 5f);
        loadingBar.fillAmount = loadingRate;
        loadingText.text = $"{Mathf.Round(loadingRate*100)}%";

        Invoke("LoadingUIOff", 5f);
    }
    /// <summary>
    /// 로딩창 Off
    /// </summary>
    void LoadingUIOff()
    {
        loadingObj.SetActive(false);
    }
}
