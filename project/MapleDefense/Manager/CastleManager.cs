using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CastleManager : MonoBehaviour
{
    [SerializeField]
    GameObject castleUI;

    TextMeshProUGUI expRateText;
    TextMeshProUGUI hpRateText;
    TextMeshProUGUI mesoText;
    TextMeshProUGUI stageText;

    Image expBarImage;
    Image hpBarImage;
    
    void Awake()
    {
        TextBinding();
        ImageBinding();
    }
    private void Start()
    {
        InitSettingCastleValue();
    }
    /// <summary>
    /// 기능 : UI 이미지 바인딩
    /// </summary>
    void ImageBinding()
    {
        foreach(Image img in castleUI.GetComponentsInChildren<Image>(true))
        {
            string imgName = img.gameObject.name;
            switch (imgName)
            {
                case "ExpRate":
                    expBarImage = img;
                    break;
                case "HPRate":
                    hpBarImage = img;
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 기능 : 텍스트 바인딩
    /// </summary>
    void TextBinding()
    {
        foreach (TextMeshProUGUI txt in castleUI.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            string imgName = txt.gameObject.name;
            switch (imgName)
            {
                case "StageUIText":
                    stageText = txt;
                    break;
                case "MesoText":
                    mesoText = txt;
                    break;
                case "HPText":
                    hpRateText = txt;
                    break;
                case "ExpText":
                    expRateText = txt;
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 초기 세팅
    /// </summary>
    void InitSettingCastleValue()
    {
        //처음엔 풀피
        GameManager.Instance.CurrentCastleHP = GameManager.Instance.FullCastleHP;

        stageText.text = $"Stage {GameManager.Instance.CurrentStage}";
        mesoText.text = $"{GameManager.Instance.CurrentMeso }";
        hpRateText.text = $"HP. {GameManager.Instance.CurrentCastleHP} / {GameManager.Instance.FullCastleHP}";
        expRateText.text = $"Exp. {GameManager.Instance.CurrentExp} / {4000} [{0.00}%]";

        hpBarImage.fillAmount = 1;
        expBarImage.fillAmount = 0;
    }
}
