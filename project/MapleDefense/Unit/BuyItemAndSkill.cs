using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemAndSkill : MonoBehaviour
{
    [SerializeField]
    GameObject buyWeaponPopUP;
    [SerializeField]
    GameObject buyThrowPopUP;
    [SerializeField]
    GameObject buySkillUPPopUP;
    [SerializeField]
    GameObject castleObj;

    [SerializeField]
    TextMeshProUGUI weaponBuyCommentText;
    [SerializeField]
    TextMeshProUGUI throwBuyCommentText;
    [SerializeField]
    TextMeshProUGUI skillUPBuyCommentText;
    [SerializeField]
    TextMeshProUGUI skillUPCommentText;

    Image curWeaponImage;
    uint curWeaponPrice;
    int curWeaponAttack;

    uint curThrowPrice;
    int curbuyThrowIndex;
    int curThrowAttack;

    const int maxPageNum = 4;

    void Awake()
    {
        UpgradeButtonBinding();
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
                    btn.onClick.AddListener(() => ClickSkillUPButton(btn.gameObject));
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
            case "Skill":
                if (GameManager.Instance.CurrentSkillPageNum >= 1)
                {
                    GameManager.Instance.CurrentSkillPageNum -= 1;
                    pageObj.transform.GetChild(GameManager.Instance.CurrentSkillPageNum + 1).gameObject.SetActive(false);
                    pageObj.transform.GetChild(GameManager.Instance.CurrentSkillPageNum).gameObject.SetActive(true);
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
            case "Skill":
                if (GameManager.Instance.CurrentSkillPageNum < maxPageNum - 1)
                {
                    GameManager.Instance.CurrentSkillPageNum += 1;
                    pageObj.transform.GetChild(GameManager.Instance.CurrentSkillPageNum - 1).gameObject.SetActive(false);
                    pageObj.transform.GetChild(GameManager.Instance.CurrentSkillPageNum).gameObject.SetActive(true);
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
        DrawSkillUPButton();
    }

    /// <summary>
    /// 기능 : 스킬 업 UI 정보 표시
    /// </summary>
    void DrawSkillUPButton()
    {
        skillUPCommentText.text = "";
    }

    /// <summary>
    /// 구매 확정
    /// castleAttack의 정보 바꿈
    /// </summary>
    public void OKSkillUPButton()
    {
        //TODO : 구매 여부 검사

        if (GameManager.Instance.CurrentMeso >= curThrowPrice)
        {
            GameManager.Instance.CurrentMeso -= curThrowPrice;

           
            buySkillUPPopUP.SetActive(false);
        }
        else
        {
            skillUPBuyCommentText.text = "잔액 부족";
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
            GameManager.Instance.MaxTurretCount += 1;
        }
    }
    /// <summary>
    /// 성 최대 HP 증가
    /// </summary>
    public void IncreaseCastleHP()
    {
        GameManager.Instance.FullCastleHP += 800;
        GameManager.Instance.CurrentCastleHP += 800;
    }
    #endregion
}
