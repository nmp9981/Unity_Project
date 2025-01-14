using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemAndSkill : MonoBehaviour
{
    [SerializeField]
    GameObject buyWeaponPopUP;
    [SerializeField]
    GameObject castleObj;

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
        buyWeaponPopUP.SetActive(true);
    }
    /// <summary>
    /// 구매 확정
    /// 매개변수 : 설치 위치
    /// </summary>
    void OkWeaponButton(int num)
    {
        //최대 터렛수 검사

        //해당층에 이미 구매했는지도 검사

        //구매 및 설치 완료
        //castleAttack의 정보 바꿈
        GameManager.Instance.CurrentMeso -= curWeaponPrice;
        buyWeaponPopUP.SetActive(false);
    }
    /// <summary>
    /// 구매 취소
    /// </summary>
    void CancleWeaponButton()
    {
        buyWeaponPopUP.SetActive(false);
    }
}
