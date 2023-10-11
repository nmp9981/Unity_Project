using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject menuCamera;
    public GameObject gameCamera;
    public Player player;
    public Boss boss;
    
    public int stage;
    public float playTime;
    
    public bool isBattle;//전투 중인가?
    public int enemyCntA;//남은 적의 수
    public int enemyCntB;
    public int enemyCntC;

    public GameObject menuPanel;
    public GameObject gamePanel;
    
    public Text maxScoreTxt;
    public Text scoreTxt;
    
    public Text stageTxt;
    public Text playTimeTxt;
    
    public Text playerHealthTxt;
    public Text playerAmmoTxt;
    public Text playerCoinTxt;

    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;

    public Text enemyATxt;
    public Text enemyBTxt;
    public Text enemyCTxt;

    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    // Start is called before the first frame update
    void Awake()
    {
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));//3자리 쉼표 주기

    }

    //메뉴관련 비활성화 게임관련 활성화
    public void GameStart()
    {
        menuCamera.SetActive(false);
        gameCamera.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }
    //플레이 타임
    private void Update()
    {
        if (isBattle) playTime += Time.deltaTime;//전투 중에만 시간 증가
    }
    //처리된 정보를 표시
    void LateUpdate()
    {
        //상단 UI
        scoreTxt.text = string.Format("{0:n0}",player.score);
        stageTxt.text = "STAGE " + stage;

        //시간
        int hour = (int)(playTime / 3600);
        int min = (int) ((playTime-(hour*3600)) / 60);
        int second = (int)(playTime % 60);
        playTimeTxt.text = string.Format("{0:00}", hour)+":"+ string.Format("{0:00}", min) + ":"+ string.Format("{0:00}", second);//초단위를 시,분,초로(각 요소는 2자리로 고정)

        playerHealthTxt.text = player.health + " / "+player.maxHealth;
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);

        //장착 무기에따라 다르게 표시
        if (player.equipWeapon == null) playerAmmoTxt.text = "- / " + player.ammo;
        else if(player.equipWeapon.type == Weapon.Type.Melee) playerAmmoTxt.text = "- / " + player.ammo;
        else playerAmmoTxt.text = player.equipWeapon.curAmmo+" / " + player.ammo;

        //단축키 색상(보유에 따라)
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, player.hasGrenades>0 ? 1 : 0);

        //적 마릿수
        enemyATxt.text = enemyCntA.ToString();
        enemyBTxt.text = enemyCntB.ToString();
        enemyCTxt.text = enemyCntC.ToString();

        //보스 체력
        bossHealthBar.localScale = new Vector3(Mathf.Max(0f,(float)boss.curHealth / (float)boss.maxHealth), 1, 1);
    }
}
