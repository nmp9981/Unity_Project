using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemAndSkill : MonoBehaviour
{
    [SerializeField]
    GameObject buyWeaponPopUP;
    [SerializeField]
    GameObject castleObj;

    [SerializeField]
    TextMeshProUGUI weaponBuyCommentText;

    Image curWeaponImage;
    uint curWeaponPrice;
    int curWeaponAttack;

    void Awake()
    {
        UpgradeButtonBinding();
    }
    /// <summary>
    /// 모든 버튼 바인딩
    /// </summary>
    void UpgradeButtonBinding()
    {
        foreach(Button btn in gameObject.GetComponentsInChildren<Button>())
        {
            //페이지 이동
            if(btn.gameObject.name== "MoveLeft")
            {

            }else if(btn.gameObject.name == "MoveRight")
            {

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
            }
        }
    }
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
        Debug.Log(curWeaponPrice + " " + curWeaponAttack);
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
}
