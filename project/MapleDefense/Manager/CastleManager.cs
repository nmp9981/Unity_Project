using Cysharp.Threading.Tasks;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CastleManager : MonoBehaviour
{
    [SerializeField]
    GameObject castleUI;
    [SerializeField]
    GameObject upgradeUI;
    [SerializeField]
    public GameObject monsterEntranceObj;
    [SerializeField]
    GameObject gameOverUI;
    [SerializeField]
    GameObject victoryUI;
    [SerializeField]
    GameObject soundSettingUI;
    [SerializeField]
    GameObject mainUI;
    [SerializeField]
    BackGroundManager backGroundManager;
    [SerializeField]
    BuyItemAndSkill buyItemAndSkill;

    [SerializeField]
    LoadingManager loadingManager;

    [SerializeField]
    Sprite initWeaponSprite;

    TextMeshProUGUI expRateText;
    TextMeshProUGUI hpRateText;
    TextMeshProUGUI mesoText;
    TextMeshProUGUI stageText;
    TextMeshProUGUI gameOverStageText;

    Image expBarImage;
    Image hpBarImage;
    SpriteRenderer castleSprite;

    SoundManager soundManager;
    
    void Awake()
    {
        TextBinding();
        ImageBinding();
        ButtonBinding();
        SliderBinding();
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
        CastleDestroy();
    }

    /// <summary>
    /// 기능 : UI 이미지 바인딩
    /// </summary>
    void ImageBinding()
    {
        castleSprite = GetComponent<SpriteRenderer>();
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
                case "CastleHPText":
                    hpRateText = txt;
                    break;
                case "ExpText":
                    expRateText = txt;
                    break;
                case "GameOverTxt":
                    gameOverStageText = txt;
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
                case "CloseVictoryUI":
                    btn.onClick.AddListener(ReturnMainMenu);
                    break;
                case "CloseUnitUpgrade":
                    btn.onClick.AddListener(CloseUnitUpgradeButton);
                    break;
                case "CloseGameOverUI":
                    btn.onClick.AddListener(CloseGameOverUI);
                    break;
                case "SettingClose":
                    btn.onClick.AddListener(CloseSettingButton);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 기능 : 슬라이더 바인딩
    /// </summary>
    void SliderBinding()
    {
        foreach (Slider sl in soundSettingUI.GetComponentsInChildren<Slider>(true))
        {
            string sliderName = sl.gameObject.name;
            switch (sliderName)
            {
                case "BGMSlider":
                    sl.value = 0.7f;
                    sl.onValueChanged.AddListener(delegate { SettingBGMVolume(sl); });
                    break;
                case "SFXSlider":
                    sl.value = 0.7f;
                    sl.onValueChanged.AddListener(delegate { SettingSFXVolume(sl); });
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 기능 : 초기 세팅
    /// </summary>
    public void InitSettingCastleValue()
    {
        //맵 초기화
        backGroundManager.ChangeBackground();
        //처음엔 풀피
        GameManager.Instance.CurrentCastleHP = GameManager.Instance.FullCastleHP;
        
        stageText.text = $"Stage {GameManager.Instance.CurrentStage}";
        mesoText.text = $"{GameManager.Instance.CurrentMeso }";
        hpRateText.text = $"HP. {GameManager.Instance.CurrentCastleHP} / {GameManager.Instance.FullCastleHP}";
        expRateText.text = $"Exp. {GameManager.Instance.CurrentExp} / {GameManager.Instance.RequireStageUPExp[GameManager.Instance.CurrentStage]} [{0.00}%]";
       
        hpBarImage.fillAmount = 1;
        expBarImage.fillAmount = 0;

        //터렛 개수는 1개
        for (int turretIdx=0; turretIdx< GameManager.Instance.CurrentTurretList.Count;turretIdx++)
        {
            GameObject turretObj = GameManager.Instance.CurrentTurretList[turretIdx].gameObject;
            turretObj.GetComponent<CastleAttack>().weaponAttack = 3;
            turretObj.GetComponent<SpriteRenderer>().sprite = initWeaponSprite;
            turretObj.SetActive(false);
        }
        GameManager.Instance.CurrentTurretList[0].gameObject.SetActive(true);
    }
    /// <summary>
    /// 기능 : 메인메뉴로 돌아가기
    /// </summary>
    void ReturnMainMenu()
    {
        loadingManager.LoadingPrograssBar();//로딩창
        InitGameInfomation();
        SoundManager._sound.PlayBGM(GameManager.Instance.MainBGMIndex);
        mainUI.SetActive(true);
        GameManager.Instance.IsGamePlaying = false;
        //비활성화
        //gameObject.SetActive(false);
        monsterEntranceObj.SetActive(false);
        victoryUI.SetActive(false);
    }
    /// <summary>
    /// 게임 정보 초기화
    /// 테렛 개수, 돈, 스테이지, 소환수 등 초기화
    /// </summary>
    public void InitGameInfomation()
    {
        //성 정보 초기화
        GameManager.Instance.CurrentStage = 1;
        GameManager.Instance.CurrentMeso = 0;
        GameManager.Instance.CurrentExp = 0;

        GameManager.Instance.FullCastleHP = 900;
        GameManager.Instance.CurrentCastleHP = GameManager.Instance.FullCastleHP;

        //터렛 초기화
        GameManager.Instance.CurrentTurretIndex = 1;
        GameManager.Instance.MaxTurretCount = 1;

        //표창 초기화
        GameManager.Instance.CurrentThrowIndex = 0;

        foreach (var unit in GameManager.Instance.ActiveUnitList)
        {
            unit.gameObject.SetActive(false);
        }
        for (int turretIdx = 0; turretIdx < GameManager.Instance.CurrentTurretList.Count; turretIdx++)
        {
            GameManager.Instance.CurrentTurretList[turretIdx].gameObject.SetActive(false);
        }

        GameManager.Instance.ActiveUnitList.Clear();

        //소환수 정보 초기화
        foreach(var unit in GameManager.Instance.ActiveSupportUnitList)
        {
            unit.gameObject.SetActive(false);
        }
        GameManager.Instance.ActiveSupportUnitList.Clear();

        //성 정보 초기화 UI
        InitSettingCastleValue();
        //스킬 정보 초기화
        for (int i = 0; i < 3; i++)
        {
            GameManager.Instance.CurrentSkillLvArray[i] = 1;
        }
        buyItemAndSkill.InitSkillInfo();
    }

    /// <summary>
    /// 기능 : 세팅창 열기
    /// </summary>
    void OpenSettingButton()
    {
        soundSettingUI.SetActive(true);
    }
    /// <summary>
    /// 기능 : 세팅창 닫기
    /// </summary>
    void CloseSettingButton()
    {
        soundSettingUI.SetActive(false);
    }
    /// <summary>
    /// 기능 : 유닛 업그레이드 창 열기
    /// 일시 정지 
    /// </summary>
    void OpenUnitUpgradeButton()
    {
        if (!upgradeUI.activeSelf)
        {
            foreach(Transform building in this.gameObject.GetComponentInChildren<Transform>())
            {
                building.gameObject.SetActive(false);
            }
            monsterEntranceObj.SetActive(false);
            castleSprite.enabled = false;
            upgradeUI.SetActive(true);
            GameManager.Instance.IsOpenUpgradeUI = true;
        }
    }
    /// <summary>
    /// 기능 : 유닛 업그레이드 창 닫기
    /// 일시 정지 해제
    /// </summary>
    void CloseUnitUpgradeButton()
    {
        if (upgradeUI.activeSelf)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            for(int turretIdx=0; turretIdx< GameManager.Instance.MaxTurretCount;turretIdx++)
            {
                GameManager.Instance.CurrentTurretList[turretIdx].gameObject.SetActive(true);
            }
            monsterEntranceObj.SetActive(true);
            castleSprite.enabled = true;
            upgradeUI.SetActive(false);
            GameManager.Instance.IsOpenUpgradeUI = false;
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
        ShowCastleHP();

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
    /// HP 보이기
    /// </summary>
    public void ShowCastleHP()
    {
        hpRateText.text = $"HP. {GameManager.Instance.CurrentCastleHP} / {GameManager.Instance.FullCastleHP}";
        hpBarImage.fillAmount = (float)GameManager.Instance.CurrentCastleHP / (float)GameManager.Instance.FullCastleHP;
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
        stageText.text = $"Stage {GameManager.Instance.CurrentStage}";
    }
    
    /// <summary>
    /// 기능 : 사망 판정
    /// </summary>
    void CastleDestroy()
    {
        if(GameManager.Instance.CurrentCastleHP <= 0)
        {
            //사망 처리
            GameManager.Instance.IsDie = true;
            gameOverUI.gameObject.SetActive(true);
            //기록 처리
            float score = Mathf.Round(100*(float)GameManager.Instance.CurrentStage / GameManager.Instance.MaxStage);
            gameOverStageText.text = $"스테이지 : {GameManager.Instance.CurrentStage}\n성취도 : {score}\n등급 : {Grade(score)}";
        }
    }
    /// <summary>
    /// 등급 판정
    /// </summary>
    /// <param name="score">점수</param>
    /// <returns></returns>
    string Grade(float score)
    {
        if (score >= 96)
        {
            return "Challenger";
        }
        if (score >= 88)
        {
            return "Master";
        }
        if (score >= 82)
        {
            return "Ruby";
        }
        if (score >= 77)
        {
            return "Diamond";
        }
        if (score >= 67)
        {
            return "Emerald";
        }
        if (score >= 60)
        {
            return "Platinum";
        }
        if (score >= 50)
        {
            return "Gold";
        }
        if (score >= 40)
        {
            return "Silver";
        }
        if (score >= 32)
        {
            return "Bronze";
        }
        if (score >= 25)
        {
            return "Iron";
        }
        if (score >= 18)
        {
            return "Stone";
        }
        if (score >= 9)
        {
            return "Wood";
        }
        return "Unrank";
    }
    /// <summary>
    /// 승리 UI열기
    /// </summary>
    public void OpenVictoryUI()
    {
        victoryUI.gameObject.SetActive(true);
    }

    /// <summary>
    /// 게임 오버 후 메인으로 이동
    /// </summary>
    public void CloseGameOverUI()
    {
        InitGameInfomation();
        loadingManager.LoadingPrograssBar();//로딩창
        SoundManager._sound.PlayBGM(GameManager.Instance.MainBGMIndex);
        mainUI.SetActive(true);
        gameOverUI.gameObject.SetActive(false);
        GameManager.Instance.IsGamePlaying = false;
        //비활성화
        //gameObject.SetActive(false);
        monsterEntranceObj.SetActive(false);
    }

    public void SettingBGMVolume(Slider sl)
    {
        GameManager.Instance.BGMVolume = sl.value;
    }
    public void SettingSFXVolume(Slider sl)
    {
        GameManager.Instance.SFXVolume = sl.value;
    }
}
