using Cysharp.Threading.Tasks.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemAndSkill : MonoBehaviour
{
    [SerializeField]
    ObjectFulling objectFulling;

    [SerializeField]
    GameObject buyWeaponPopUP;
    [SerializeField]
    GameObject buyThrowPopUP;
    [SerializeField]
    GameObject buySkillUPPopUP;
    [SerializeField]
    GameObject buySupportPopUP;
    [SerializeField]
    GameObject castleObj;
    [SerializeField]
    Transform castleEntrancePosition;

    [SerializeField]
    TextMeshProUGUI weaponBuyCommentText;
    [SerializeField]
    TextMeshProUGUI throwBuyCommentText;
    [SerializeField]
    TextMeshProUGUI skillUPBuyCommentText;
    [SerializeField]
    TextMeshProUGUI skillUPCommentText;
    [SerializeField]
    TextMeshProUGUI supportBuyComment;

    CastleManager castleManager;

    Image curWeaponImage;
    uint curWeaponPrice;
    int curWeaponAttack;

    uint curThrowPrice;
    int curbuyThrowIndex;
    int curThrowAttack;

    uint curSkillPrice;
    int curSkillIndex;
    int nextSkillLv;
    GameObject curSkillObj;

    uint curSupportPrice;
    int curSupportIndex;
    GameObject curSupportObj;

    const int maxPageNum = 4;

    void Awake()
    {
        castleManager = castleObj.GetComponent<CastleManager>();
        UpgradeButtonBinding();
    }
    /// <summary>
    /// 스킬 정보 초기화
    /// </summary>
    public void InitSkillInfo()
    {
        Debug.Log("여기는 간다");
        foreach (Button btn in gameObject.GetComponentsInChildren<Button>(true))
        {
            string btnName = btn.gameObject.transform.parent.name;
            if (btnName.Contains("Skill") && btnName != "Skill")
            {
                GameObject skillButton = btn.gameObject.transform.parent.gameObject;
                string skillName = skillButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
                switch (skillName)
                {
                    case "자벨린 부스터":
                        skillButton.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "500";
                        skillButton.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = "Lv 1";
                        break;
                    case "자벨린 증축":
                        skillButton.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "3000";
                        skillButton.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = "1개";
                        break;
                    case "체력 증가":
                        skillButton.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "1000";
                        skillButton.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = "HP +500";
                        break;
                    default:
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 모든 버튼 바인딩
    /// </summary>
    void UpgradeButtonBinding()
    {
        foreach(Button btn in gameObject.GetComponentsInChildren<Button>(true))
        {
            //페이지 이동
            if(btn.gameObject.name== "MoveLeft")
            {
                GameObject pageObj = btn.gameObject.transform.parent.GetChild(3).gameObject;
                string pageCategoty = btn.gameObject.transform.parent.gameObject.name;
                btn.onClick.AddListener(() => MoveLeftPage(pageObj, pageCategoty));

            }
            else if(btn.gameObject.name == "MoveRight")
            {
                GameObject pageObj = btn.gameObject.transform.parent.GetChild(3).gameObject;
                string pageCategoty = btn.gameObject.transform.parent.gameObject.name;
                btn.onClick.AddListener(() => MoveRightPage(pageObj, pageCategoty));
            }
            else//나머지
            {
                string btnName = btn.gameObject.transform.parent.name;
                if(btnName.Contains("Weapon") && btnName != "Weapon")
                {
                    Image weaponImage = btn.gameObject.transform.parent.GetChild(2).GetComponent<Image>();
                    uint weaponPrice = uint.Parse(btn.gameObject.transform.parent.GetChild(3).GetComponent<TextMeshProUGUI>().text);
                    string weaponAttackText = btn.gameObject.transform.parent.GetChild(5).GetComponent<TextMeshProUGUI>().text;
                    int weaponAttack = int.Parse(weaponAttackText.Substring(6));
                    btn.onClick.AddListener(()=>ClickWeaponButton(weaponImage,weaponPrice,weaponAttack));
                }
                else if (btnName.Contains("Throw") && btnName != "Throw")
                {
                    uint weaponPrice = uint.Parse(btn.gameObject.transform.parent.GetChild(3).GetComponent<TextMeshProUGUI>().text);
                    string throwIndexText = btn.gameObject.transform.parent.GetChild(6).GetComponent<TextMeshProUGUI>().text;
                    int throwIndex = int.Parse(throwIndexText);
                    btn.onClick.AddListener(() => ClickThrowButton(weaponPrice, throwIndex));
                }
                else if (btnName.Contains("Skill") && btnName != "Skill")
                {
                    btn.onClick.AddListener(() => ClickSkillUPButton(btn.gameObject.transform.parent.gameObject));
                }
                else if (btnName.Contains("Supporter") && btnName != "Supporter")
                {
                    uint supportPrice = uint.Parse(btn.gameObject.transform.parent.GetChild(3).GetComponent<TextMeshProUGUI>().text);
                    btn.onClick.AddListener(() => ClickSupportButton(supportPrice, btn.gameObject.transform.parent.gameObject));
                }
            }
        }
    }
    /// <summary>
    /// 페이지 왼쪽 이동
    /// </summary>
    public void MoveLeftPage(GameObject pageObj, string pageCategoty)
    {
        switch (pageCategoty)
        {
            case "Weapon":
                if(GameManager.Instance.CurrentWeaponPageNum >= 1)
                {
                    GameManager.Instance.CurrentWeaponPageNum -= 1;
                    pageObj.transform.GetChild(GameManager.Instance.CurrentWeaponPageNum + 1).gameObject.SetActive(false);
                    pageObj.transform.GetChild(GameManager.Instance.CurrentWeaponPageNum).gameObject.SetActive(true);
                }
                break;
            case "Throw":
                if (GameManager.Instance.CurrentThrowPageNum >= 1)
                {
                    GameManager.Instance.CurrentThrowPageNum -= 1;
                    pageObj.transform.GetChild(GameManager.Instance.CurrentThrowPageNum + 1).gameObject.SetActive(false);
                    pageObj.transform.GetChild(GameManager.Instance.CurrentThrowPageNum).gameObject.SetActive(true);
                }
                break;
            case "Supporter":
                if (GameManager.Instance.CurrentSupportPageNum >= 1)
                {
                    GameManager.Instance.CurrentSupportPageNum -= 1;
                    pageObj.transform.GetChild(GameManager.Instance.CurrentSupportPageNum + 1).gameObject.SetActive(false);
                    pageObj.transform.GetChild(GameManager.Instance.CurrentSupportPageNum).gameObject.SetActive(true);
                }
                break;
            default:
                break;
        }
       
    }

    /// <summary>
    /// 페이지 오른쪽 이동
    /// </summary>
    public void MoveRightPage(GameObject pageObj, string pageCategoty)
    {
        switch (pageCategoty)
        {
            case "Weapon":
                if (GameManager.Instance.CurrentWeaponPageNum < maxPageNum-1)
                {
                    GameManager.Instance.CurrentWeaponPageNum += 1;
                    pageObj.transform.GetChild(GameManager.Instance.CurrentWeaponPageNum - 1).gameObject.SetActive(false);
                    pageObj.transform.GetChild(GameManager.Instance.CurrentWeaponPageNum).gameObject.SetActive(true);
                }
                break;
            case "Throw":
                if (GameManager.Instance.CurrentThrowPageNum < maxPageNum - 1)
                {
                    GameManager.Instance.CurrentThrowPageNum += 1;
                    pageObj.transform.GetChild(GameManager.Instance.CurrentThrowPageNum - 1).gameObject.SetActive(false);
                    pageObj.transform.GetChild(GameManager.Instance.CurrentThrowPageNum).gameObject.SetActive(true);
                }
                break;
            case "Supporter":
                if (GameManager.Instance.CurrentSupportPageNum < maxPageNum - 1)
                {
                    GameManager.Instance.CurrentSupportPageNum += 1;
                    pageObj.transform.GetChild(GameManager.Instance.CurrentSupportPageNum - 1).gameObject.SetActive(false);
                    pageObj.transform.GetChild(GameManager.Instance.CurrentSupportPageNum).gameObject.SetActive(true);
                }
                break;
            default:
                break;
        }
        
    }

    #region 무기 구매
    /// <summary>
    /// 기능 : 무기 구매 버튼 클릭
    /// </summary>
    /// <param name="weaponImage"></param>
    /// <param name="weaponPrice"></param>
    /// <param name="weaponAttack"></param>
    void ClickWeaponButton(Image weaponImage, uint weaponPrice, int weaponAttack)
    {
        //어디에 설치할건가?
        curWeaponImage = weaponImage;
        curWeaponPrice = weaponPrice;
        curWeaponAttack = weaponAttack;

        //버튼 클릭 창 열림 및 초기화
        buyWeaponPopUP.SetActive(true);
        weaponBuyCommentText.text = string.Empty;
    }
    /// <summary>
    /// 구매 확정
    /// 매개변수 : 설치 위치
    /// </summary>
    public void OkWeaponButton(int num)
    {
        //현재 터렛수 검사
        if(num+1 > GameManager.Instance.MaxTurretCount)
        {
            weaponBuyCommentText.text = "터렛 자리 없음";
            return;
        }
        //선택한 터렛 번호
        GameManager.Instance.CurrentTurretIndex = num;

        //해당층에 이미 구매했는지도 검사

        //구매 가능 여부 검사
        if (GameManager.Instance.CurrentMeso < curWeaponPrice)
        {
            weaponBuyCommentText.text = "잔액 부족";
            return;
        }
    }

    /// <summary>
    /// 구매 확정
    /// castleAttack의 정보 바꿈
    /// </summary>
    public void OKWeaponButton()
    {
        if (GameManager.Instance.CurrentMeso>= curWeaponPrice)
        {
            GameManager.Instance.CurrentMeso -= curWeaponPrice;

            //Turret 정보 변경
            CastleAttack changeTurret = GameManager.Instance.CurrentTurretList[GameManager.Instance.CurrentTurretIndex];
            changeTurret.weaponAttack = curWeaponAttack;
            changeTurret.gameObject.GetComponent<SpriteRenderer>().sprite = curWeaponImage.sprite;
        }
        buyWeaponPopUP.SetActive(false);
    }

    /// <summary>
    /// 구매 취소
    /// </summary>
    public void CancleWeaponButton()
    {
        buyWeaponPopUP.SetActive(false);
    }
    #endregion

    #region 투사체 구매
    /// <summary>
    /// 기능 : 무기 구매 버튼 클릭
    /// </summary>
    /// <param name="weaponImage"></param>
    /// <param name="weaponPrice"></param>
    /// <param name="weaponAttack"></param>
    void ClickThrowButton(uint weaponPrice, int throwIndex)
    {
        curThrowPrice = weaponPrice;
        curbuyThrowIndex = throwIndex;

        //버튼 클릭 창 열림 및 초기화
        buyThrowPopUP.SetActive(true);
        throwBuyCommentText.text = string.Empty;
    }
    
    /// <summary>
    /// 구매 확정
    /// castleAttack의 정보 바꿈
    /// </summary>
    public void OKThrowButton()
    {
        //TODO : 구매 여부 검사

        if (GameManager.Instance.CurrentMeso >= curThrowPrice)
        {
            GameManager.Instance.CurrentMeso -= curThrowPrice;

            //투사체 변경
            GameManager.Instance.CurrentThrowIndex = curbuyThrowIndex;
            buyThrowPopUP.SetActive(false);
        }
        else
        {
            throwBuyCommentText.text = "잔액 부족";
        }
    }

    /// <summary>
    /// 구매 취소
    /// </summary>
    public void CancleThrowButton()
    {
        buyThrowPopUP.SetActive(false);
    }
    #endregion

    #region 스킬 업글
    /// <summary>
    /// 기능 : 스킬 업글 버튼 클릭
    /// </summary>
    /// <param name="weaponImage"></param>
    /// <param name="weaponPrice"></param>
    /// <param name="weaponAttack"></param>
    void ClickSkillUPButton(GameObject skillButton)
    {
        //버튼 클릭 창 열림 및 초기화
        buySkillUPPopUP.SetActive(true);
        skillUPBuyCommentText.text = string.Empty;
        DrawSkillUPButton(skillButton);
    }

    /// <summary>
    /// 기능 : 스킬 업 UI 정보 표시
    /// 버튼을 눌렀을 때 반영되는 로직
    /// </summary>
    void DrawSkillUPButton(GameObject skillButton)
    {
        string skillName = skillButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
        switch (skillName)
        {
            case "자벨린 부스터":
                curSkillObj = skillButton;
                curSkillIndex = 0;
                curSkillPrice = uint.Parse(skillButton.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text);
                nextSkillLv = Mathf.Min(GameManager.Instance.CurrentSkillLvArray[curSkillIndex] + 1,
                    GameManager.Instance.MaxSkillLvArray[curSkillIndex]);
                skillUPCommentText.text = $"{skillName}\n{GameManager.Instance.CurrentSkillLvArray[curSkillIndex]} " +
                    $"-> {nextSkillLv} \n가격 : {curSkillPrice}";
                break;
            case "자벨린 증축":
                curSkillObj = skillButton;
                curSkillIndex = 1;
                curSkillPrice = uint.Parse(skillButton.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text);
                nextSkillLv = Mathf.Min(GameManager.Instance.CurrentSkillLvArray[curSkillIndex] + 1,
                    GameManager.Instance.MaxSkillLvArray[curSkillIndex]);
                skillUPCommentText.text = $"{skillName}\n{GameManager.Instance.CurrentSkillLvArray[curSkillIndex]} " +
                    $"-> {nextSkillLv} \n가격 : {curSkillPrice}";
                break;
            case "체력 증가":
                curSkillObj = skillButton;
                curSkillIndex = 2;
                curSkillPrice = uint.Parse(skillButton.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text);
                nextSkillLv = Mathf.Min(GameManager.Instance.CurrentSkillLvArray[curSkillIndex] + 1,
                    GameManager.Instance.MaxSkillLvArray[curSkillIndex]);
                skillUPCommentText.text = $"{skillName}\n{GameManager.Instance.CurrentSkillLvArray[curSkillIndex]} " +
                    $"-> {nextSkillLv} \n가격 : {curSkillPrice}";
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 구매 확정
    /// castleAttack의 정보 바꿈
    /// </summary>
    public void OKSkillUPButton()
    {
        //TODO : 구매 여부 검사

        if (GameManager.Instance.CurrentSkillLvArray[curSkillIndex] < GameManager.Instance.MaxSkillLvArray[curSkillIndex])
        {
            if (GameManager.Instance.CurrentMeso >= curSkillPrice)
            {
                GameManager.Instance.CurrentMeso -= curSkillPrice;

                switch (curSkillIndex)
                {
                    case 0:
                        IncreaseAttackSpeed();
                        break;
                    case 1:
                        IncreaseTurretCount();
                        break;
                    case 2:
                        IncreaseCastleHP(GameManager.Instance.CurrentSkillLvArray[curSkillIndex]);
                        break;
                    default:
                        break;
                }

                ChangeSkillButtonInfo();
                buySkillUPPopUP.SetActive(false);
            }
            else
            {
                skillUPBuyCommentText.text = "잔액 부족";
            }
        }
        else
        {
            skillUPBuyCommentText.text = "최대 레벨";
        }
    }
    /// <summary>
    /// 기능 : 스킬창 정보 변경
    /// 3:가격,5:효과
    /// </summary>
    void ChangeSkillButtonInfo()
    {
        GameManager.Instance.CurrentSkillLvArray[curSkillIndex] += 1;
        int nextSkillLV= GameManager.Instance.CurrentSkillLvArray[curSkillIndex];
        switch (curSkillIndex)
        {
            case 0:
                ulong nextBoosterPrice = GameManager.Instance.BoosterPriceArray[nextSkillLV];
                curSkillObj.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = nextBoosterPrice.ToString();
                curSkillObj.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = 
                    $"Lv.{GameManager.Instance.CurrentSkillLvArray[curSkillIndex]}";
                break;
            case 1:
                ulong nextMasteryPrice = GameManager.Instance.MasteryPriceArray[nextSkillLV];
                curSkillObj.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = nextMasteryPrice.ToString();
                curSkillObj.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text =
                    $"{GameManager.Instance.CurrentSkillLvArray[curSkillIndex]}개";
                break;
            case 2:
                ulong nextHPPrice = GameManager.Instance.IncreaseCastleHPPrice[nextSkillLV];
                curSkillObj.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = nextHPPrice.ToString();
                curSkillObj.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text =
                    $"HP +{GameManager.Instance.IncreaseCastleHP[GameManager.Instance.CurrentSkillLvArray[curSkillIndex]]}";
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 구매 취소
    /// </summary>
    public void CancleSkillUPButton()
    {
        buySkillUPPopUP.SetActive(false);
    }

    /// <summary>
    /// 공격 속도 증가
    /// </summary>
    public void IncreaseAttackSpeed()
    {
        GameManager.Instance.AttackBetweenTime = Mathf.Max(GameManager.Instance.AttackBetweenTime-0.05f,0.2f);

    }
    /// <summary>
    /// 최대 터렛 개수 증가
    /// </summary>
    public void IncreaseTurretCount()
    {
        if(GameManager.Instance.MaxTurretCount < 4)
        {
            GameManager.Instance.CurrentTurretList[GameManager.Instance.MaxTurretCount].gameObject.SetActive(true);
            GameManager.Instance.MaxTurretCount += 1;
        }
    }
    /// <summary>
    /// 성 최대 HP 증가
    /// </summary>
    public void IncreaseCastleHP(int lv)
    {
        GameManager.Instance.FullCastleHP += GameManager.Instance.IncreaseCastleHP[lv];
        GameManager.Instance.CurrentCastleHP += GameManager.Instance.IncreaseCastleHP[lv];

        //HP 표시
        castleManager.ShowCastleHP();
    }
    #endregion

    #region 소환수 구매
    /// <summary>
    /// 기능 : 무기 구매 버튼 클릭
    /// </summary>
    /// <param name="weaponImage"></param>
    /// <param name="weaponPrice"></param>
    /// <param name="weaponAttack"></param>
    void ClickSupportButton(uint supportPrice, GameObject supportObj)
    {
        curSupportPrice = supportPrice;
        curSupportIndex = int.Parse(supportObj.transform.GetChild(8).GetComponent<TextMeshProUGUI>().text);
        curSupportObj = supportObj;

        //버튼 클릭 창 열림 및 초기화
        buySupportPopUP.SetActive(true);
        supportBuyComment.text = string.Empty;
    }

    /// <summary>
    /// 구매 확정
    /// castleAttack의 정보 바꿈
    /// </summary>
    public void OKSupportButton()
    {
        //TODO : 구매 여부 검사

        if (GameManager.Instance.CurrentMeso >= curSupportPrice)
        {
            GameManager.Instance.CurrentMeso -= curSupportPrice;
            //소환 효과음
            SoundManager._sound.PlaySfx((int)SFXSound.SupporterSpawn);
            //몬스터 소환
            GameManager.Instance.CurrentSupportIndex = curSupportIndex;
            GameObject supportObject = objectFulling.MakeSupportsObj(GameManager.Instance.CurrentSupportIndex);
            supportObject.transform.position = castleEntrancePosition.position - Vector3.up;
            //z좌표 0으로
            supportObject.transform.position = new Vector3(supportObject.transform.position.x, supportObject.transform.position.y, 0);
            buySupportPopUP.SetActive(false);
        }
        else
        {
            supportBuyComment.text = "잔액 부족";
        }
    }

    /// <summary>
    /// 구매 취소
    /// </summary>
    public void CancleSupportButton()
    {
        buySupportPopUP.SetActive(false);
    }


    #endregion 소환수 구매
}
