using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;//파일 입출력

public class GameManager : MonoBehaviour
{
    public int stage;//스테이지
    public Animator stageAnim;//시작 애니메이션
    public Animator clearAnim;//종료 애니메이션
    public Animator fadeAnim;//종료 애니메이션

    public Transform playerPos;//플레이어 위치
    
    public string[] EnenyObjs;//적기 오브젝트

    public Transform[] spawnPoints;//스폰 위치

    public float nextSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;

    //UI관련
    public Text scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject gameOverSet;
    public ObjectManager objectManager;

    public List<Spawn> spawnList;//적 리스트
    public int spawnIndex;//어디서 생성?
    public bool spawnEnd;//생성이 끝났는가?

    private void Awake()
    {
        spawnList = new List<Spawn>();
        EnenyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL","EnemyB" };
        StageStart();
    }
    public void StageStart()
    {
        //stage UI
        stageAnim.SetTrigger("On");
        stageAnim.GetComponent<Text>().text = "Stage "+stage+"\nStart!!";
        clearAnim.GetComponent<Text>().text = "Stage " + stage + "\nClear!!";

        //적 스폰 파일 읽기
        ReadSpawnFile();

        //Fade In (밝아짐)
        fadeAnim.SetTrigger("In");
    }
    public void StageEnd()
    {
        //clear UI
        clearAnim.SetTrigger("On");

        //Fade out
        fadeAnim.SetTrigger("Out");
        //player 위치를 원래대로
        player.transform.position = playerPos.position;

        //스테이지 증가
        stage++;
        if(stage > 3)//올 클리어
        {
            GameOver();
        }else Invoke("StageStart", 4.0f);
    }
    void ReadSpawnFile()
    {
        //변수 초기화
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        //파일 열기, 스테이지대로 파일을 부름
        TextAsset textFile = Resources.Load("stage "+stage) as TextAsset;//TextAsset형이 맞는지 검사
        StringReader stringReader = new StringReader(textFile.text);//문자열 데이터 읽기

        while (stringReader != null)
        {
            string line = stringReader.ReadLine();//한줄씩 읽기
            if (line == null) break;

            ///리스폰 데이터 생성
            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);//지정한 구분자로 문자열 나눔
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);

        }
        //파일 닫기
        stringReader.Close();
        nextSpawnDelay = spawnList[0].delay;//첫번째 적 출현시간
    }
    private void Update()
    {
        curSpawnDelay += Time.deltaTime;
        if(curSpawnDelay > nextSpawnDelay && !spawnEnd)//생성 가능
        {
            SpawnEnemy();
            //nextSpawnDelay = Random.Range(0.5f,3.0f);//랜덤 시간
            curSpawnDelay = 0;
        }

        //UI
        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
        
    }
    //적 소환
    void SpawnEnemy()
    {
        //기존 적 생성 로직을 구조체를 활용한 로직으로 변경
        int enemyIndex = 0;
        switch (spawnList[spawnIndex].type)
        {
            case "S":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
            case "B":
                enemyIndex = 3;
                break;
        }
        int enemyPoint = spawnList[spawnIndex].point;//생성 위치
        
        //생성
        GameObject enemy = objectManager.MakeObj(EnenyObjs[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;
        
        //생성 위치에 따른 속도 조절
        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;
        enemyLogic.objectManager = objectManager;//넘겨줌

        if(enemyPoint == 5 || enemyPoint == 6)//왼쪽에서 생성
        {
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
            enemy.transform.Rotate(Vector3.back*90);
        }else if(enemyPoint == 7 || enemyPoint == 8)//오른쪽에서 생성
        {
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
            enemy.transform.Rotate(Vector3.forward * 90);
        }
        else
        {
            rigid.velocity = new Vector2(0,enemyLogic.speed*(-1));
        }

        //리스폰 인덱스 증가
        spawnIndex++;
        if (spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }
        //다음 리스폰 딜레이 갱신
        nextSpawnDelay = spawnList[spawnIndex].delay;
    }
    public void UpdateLifeIcon(int life)
    {
        //3개를 모두 끈 뒤 다시 활성화
        for(int i = 0; i < 3; i++)
        {
            lifeImage[i].color = new Color(1, 1, 1, 0);
        }
        for (int i = 0; i < life; i++)
        {
            lifeImage[i].color = new Color(1, 1, 1, 1);
        }
    }
    public void UpdateBoomIcon(int boom)
    {
        //3개를 모두 끈 뒤 다시 활성화
        for (int i = 0; i < 3; i++)
        {
            boomImage[i].color = new Color(1, 1, 1, 0);
        }
        for (int i = 0; i < boom; i++)
        {
            boomImage[i].color = new Color(1, 1, 1, 1);
        }
    }
    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe",2f);
    }
    //리스폰
    void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 3.5f;//생성 위치
        player.SetActive(true);//활성화

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }
    //게임 오버
    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }
    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
}
