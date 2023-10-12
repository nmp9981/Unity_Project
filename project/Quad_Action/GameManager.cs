using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject menuCamera;
    public GameObject gameCamera;
    public Player player;
    public Boss boss;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;
    
    public int stage;
    public float playTime;
    
    public bool isBattle;//전투 중인가?
    public int enemyCntA;//남은 적의 수
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;

    public Transform[] enemyZones;//적 스폰 위치
    public GameObject[] enemies;//적들
    public List<int> enemyList;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    
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

    public Text curScoreText;
    public Text bestScoreText;
    // Start is called before the first frame update
    void Awake()
    {
        enemyList = new List<int>();
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));//3자리 쉼표 주기

        //첫 시작시 최고점 세팅
        if (PlayerPrefs.HasKey("MaxScore"))
        {
            PlayerPrefs.SetInt("MaxScore", 0);
        }
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
    //스테이지 관리
    public void StageStart()
    {
        //전투가 시작되면 상점과 시작 존은 비활성화
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        foreach (Transform zone in enemyZones) zone.gameObject.SetActive(true);//스폰 존은 활성화

        isBattle = true;
        StartCoroutine(InBattle());
    } 
    public void StageEnd()
    {
        isBattle = false;
        player.transform.position = Vector3.up*1f;//플레이어 원위치

        //전투가 끝났으므로 다시 활성화
        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);
        stage++;

        foreach (Transform zone in enemyZones) zone.gameObject.SetActive(false);//스폰 존은 비활성화
    }
    //게임 오버
    public void GameOver()
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScoreText.text = scoreTxt.text;//현재 점수

        //최고점 갱신
        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if(player.score > maxScore)
        {
            bestScoreText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
    }
    //메인으로
    public void ReStart()
    {
        SceneManager.LoadScene(0);
    }
    IEnumerator InBattle()
    {
        //보스 
        if (stage % 5 == 0)
        {
            enemyCntD++;
            GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;//타겟은 플레이어
            enemy.manager = this;//자기 자신, manager변수 채워주기
            boss = instantEnemy.GetComponent<Boss>();//보스 변수 채워주기
        }
        else//일반 몹
        {
            //몬스터 정보 저장
            for (int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                switch (ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }
            //몬스터 소환
            while (enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;//타겟은 플레이어
                enemy.manager = this;//자기 자신, manager변수 채워주기
                enemyList.RemoveAt(0);//맨 앞 삭제

                yield return new WaitForSeconds(4f);//4초에 한번씩
            }
        }
        //남은 몬스터 숫자 검사
        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(4f);
        boss = null;//다시 원래대로
        StageEnd();
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

        //보스 체력(보스가 있을때만)
        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3(Mathf.Max(0f, (float)boss.curHealth / (float)boss.maxHealth), 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }
    }
}
