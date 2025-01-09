using Cysharp.Threading.Tasks;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CastleManager : MonoBehaviour
{
    [SerializeField]
    GameObject castleUI;
    [SerializeField]
    GameObject upgradeUI;

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
        ButtonBinding();
    }
    private void Start()
    {
        InitSettingCastleValue();
    }
    private void Update()
    {
        ShowCastleEXP();
        ShowCastleMeso();
        ShowCastleStage();
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
    /// 기능 : UI 버튼 바인딩
    /// </summary>
    void ButtonBinding()
    {
        foreach (Button btn in castleUI.GetComponentsInChildren<Button>(true))
        {
            string btnName = btn.gameObject.name;
            switch (btnName)
            {
                case "UnitSet":
                    btn.onClick.AddListener(OpenUnitUpgradeButton);
                    break;
                case "Setting":
                    btn.onClick.AddListener(OpenSettingButton);
                    break;
                case "ReturnMenu":
                    btn.onClick.AddListener(ReturnMainMenu);
                    break;
                case "CloseUnitUpgrade":
                    btn.onClick.AddListener(CloseUnitUpgradeButton);
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 기능 : 초기 세팅
    /// </summary>
    void InitSettingCastleValue()
    {
        //처음엔 풀피
        GameManager.Instance.CurrentCastleHP = GameManager.Instance.FullCastleHP;

        stageText.text = $"Stage {GameManager.Instance.CurrentStage}";
        mesoText.text = $"{GameManager.Instance.CurrentMeso }";
        hpRateText.text = $"HP. {GameManager.Instance.CurrentCastleHP} / {GameManager.Instance.FullCastleHP}";
        expRateText.text = $"Exp. {GameManager.Instance.CurrentExp} / {GameManager.Instance.RequireStageUPExp[GameManager.Instance.CurrentStage]} [{0.00}%]";

        hpBarImage.fillAmount = 1;
        expBarImage.fillAmount = 0;
    }
    /// <summary>
    /// 기능 : 메인메뉴로 돌아가기
    /// </summary>
    void ReturnMainMenu()
    {

    }
    /// <summary>
    /// 기능 : 세팅창 열기
    /// </summary>
    void OpenSettingButton()
    {

    }
    /// <summary>
    /// 기능 : 유닛 업그레이드 창 열기
    /// </summary>
    void OpenUnitUpgradeButton()
    {
        if (!upgradeUI.activeSelf)
        {
            upgradeUI.SetActive(true);
        }
    }
    void CloseUnitUpgradeButton()
    {
        if (upgradeUI.activeSelf)
        {
            upgradeUI.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("Enemy"))
        {
            StartCoroutine(DecreaseCastleHP(collision));
        }
    }
     IEnumerator DecreaseCastleHP(Collider2D collision)
    {
        GameManager.Instance.CurrentCastleHP -= collision.gameObject.GetComponent<EnemyUnit>().Attack;
        hpRateText.text = $"HP. {GameManager.Instance.CurrentCastleHP} / {GameManager.Instance.FullCastleHP}";
        hpBarImage.fillAmount = (float)GameManager.Instance.CurrentCastleHP / (float)GameManager.Instance.FullCastleHP;

        yield return new WaitForSeconds(1200);
    }
    /// <summary>
    /// 경험치 보이기
    /// </summary>
    void ShowCastleEXP()
    {
        expRateText.text = $"EXP. {GameManager.Instance.CurrentExp} / {GameManager.Instance.RequireStageUPExp[GameManager.Instance.CurrentStage]}";
        expBarImage.fillAmount = (float)GameManager.Instance.CurrentExp / (float) GameManager.Instance.RequireStageUPExp[GameManager.Instance.CurrentStage];
    }
    /// <summary>
    /// 메소 보이기
    /// </summary>
    void ShowCastleMeso()
    {
        mesoText.text = $"{GameManager.Instance.CurrentMeso}";
    }
    /// <summary>
    /// 스테이지 보이기
    /// </summary>
    void ShowCastleStage()
    {
        stageText.text = $"{GameManager.Instance.CurrentStage}";
    }
}
