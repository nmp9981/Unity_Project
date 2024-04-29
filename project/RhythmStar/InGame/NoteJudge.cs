using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

public class NoteJudge : MonoBehaviour
{
    [SerializeField] GameObject judgeKey1;
    [SerializeField] GameObject judgeKey2;
    [SerializeField] GameObject judgeKey3;

    public ParticleSystem[] clickEffect = new ParticleSystem[3];
    public TextMeshProUGUI[] judgeText = new TextMeshProUGUI[3];

    float maxDistance=0.7f;//최대 판정 거리
    int judgeBonusScore;//판정 보너스 점수

    //롱노트를 누르는 중인가?
    bool isLongNote1;
    bool isLongNote2;
    bool isLongNote3;

    void Awake()
    {
        maxDistance = judgeKey1.transform.localScale.y * 6;//최대 판정 거리 설정
        for (int i = 0; i < 3; i++) judgeText[i].text = "";//글자 초기화

        isLongNote1 = false;
        isLongNote2 = false;
        isLongNote3 = false;
    }

    // Update is called once per frame
    void Update()
    {
        JudgeScore();
        JudgeLongNoteScore();
    }
    void JudgeScore()
    {
        //모바일에서는 키 입력이 아닌 버튼 형식으로 변환
        //1번
        if (Input.GetKey(KeyCode.Alpha1))
        {
            Vector2 startPoint = new Vector2(judgeKey1.transform.position.x, judgeKey1.transform.position.y) - new Vector2(0, 2.5f*judgeKey1.transform.localScale.y);
            RaycastHit2D hit = Physics2D.Raycast(startPoint, Vector2.up, maxDistance);
            clickEffect[0].Play();
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Red"))
                {
                    float diffDistance = 0;
                    if (hit.collider.gameObject.transform.localScale.y >= GameManager.Instance.LongNoteStandardScale)
                    {
                        diffDistance = Mathf.Abs(0.1f + hit.collider.gameObject.transform.position.y - hit.collider.gameObject.transform.localScale.y - judgeKey1.transform.position.y);
                    }else diffDistance = Mathf.Abs(hit.collider.gameObject.transform.position.y - judgeKey1.transform.position.y);
                   
                    JudgeScoreAndEffect(diffDistance, 0);
                    hit.collider.gameObject.SetActive(false);
                    GameManager.Instance.ComboCount += 1;
                    GameManager.Instance.Score += (10 * GameManager.Instance.ComboBonus);
                    SoundManager._sound.PlaySfx(0);
                }
            }   
        }

        //2번
        if (Input.GetKey(KeyCode.Alpha2))
        {
            Vector2 startPoint = new Vector2(judgeKey2.transform.position.x, judgeKey2.transform.position.y) - new Vector2(0, 2.5f*judgeKey2.transform.localScale.y);
            RaycastHit2D hit = Physics2D.Raycast(startPoint, Vector2.up, maxDistance);
            clickEffect[1].Play();

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Green"))
                {
                    float diffDistance = 0;
                    if (hit.collider.gameObject.transform.localScale.y >= GameManager.Instance.LongNoteStandardScale)
                    {
                        diffDistance = Mathf.Abs(0.1f + hit.collider.gameObject.transform.position.y - hit.collider.gameObject.transform.localScale.y - judgeKey1.transform.position.y);
                    }
                    else diffDistance = Mathf.Abs(hit.collider.gameObject.transform.position.y - judgeKey1.transform.position.y);
                   
                    JudgeScoreAndEffect(diffDistance, 1);
                    hit.collider.gameObject.SetActive(false);
                    GameManager.Instance.ComboCount += 1;
                    GameManager.Instance.Score += (10 * GameManager.Instance.ComboBonus);
                    SoundManager._sound.PlaySfx(1);
                }
            }
        }
        //3번
        if (Input.GetKey(KeyCode.Alpha3))
        {
            Vector2 startPoint = new Vector2(judgeKey3.transform.position.x, judgeKey3.transform.position.y) - new Vector2(0, 2.5f*judgeKey3.transform.localScale.y);
            RaycastHit2D hit = Physics2D.Raycast(startPoint, Vector2.up, maxDistance);
            clickEffect[2].Play();

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Blue"))
                {
                    float diffDistance = 0;
                    if (hit.collider.gameObject.transform.localScale.y >= GameManager.Instance.LongNoteStandardScale)
                    {
                        diffDistance = Mathf.Abs(0.1f + hit.collider.gameObject.transform.position.y - hit.collider.gameObject.transform.localScale.y - judgeKey1.transform.position.y);
                    }
                    else diffDistance = Mathf.Abs(hit.collider.gameObject.transform.position.y - judgeKey1.transform.position.y);
                   
                    JudgeScoreAndEffect(diffDistance,2);
                    hit.collider.gameObject.SetActive(false);
                    GameManager.Instance.ComboCount += 1;
                    
                    SoundManager._sound.PlaySfx(2);
                }
            }
        }
    }
    void JudgeScoreAndEffect(float diff, int key)
    {
        float judgeSize = judgeKey1.transform.localScale.y/2;
        judgeBonusScore = 10;
        if (diff < judgeSize)
        {
            judgeBonusScore = 12;
            StartCoroutine(JudgeTextOn(judgeText[key],"Perfect", Color.magenta));
            GameManager.Instance.HealthPoint += 3.0f;
        }else if(diff>=judgeSize && diff < 2*judgeSize)
        {
            judgeBonusScore = 10;
            StartCoroutine(JudgeTextOn(judgeText[key], "Great",Color.green));
            GameManager.Instance.HealthPoint += 2.0f;
        }
        else
        {
            judgeBonusScore = 8;
            StartCoroutine(JudgeTextOn(judgeText[key], "Good",Color.yellow));
            GameManager.Instance.HealthPoint += 1.0f;
        }

        GameManager.Instance.Score += (GameManager.Instance.ComboBonus*judgeBonusScore);
    }
    IEnumerator JudgeTextOn(TextMeshProUGUI textObj, string restext, Color color)
    {
        textObj.text = restext;
        textObj.color = color;
        yield return new WaitForSeconds(0.3f);
        textObj.text = "";
    }

    void JudgeLongNote(GameObject gmNote, int keyNum)
    {
        float diffLongDistance = Mathf.Abs(0.1f + gmNote.transform.position.y - gmNote.transform.localScale.y - judgeKey1.transform.position.y);
        float judgeSize = 0.1f;
        if (diffLongDistance < judgeSize)
        {
            judgeBonusScore = 12;
            StartCoroutine(JudgeTextOn(judgeText[keyNum], "Perfect", Color.magenta));
            GameManager.Instance.HealthPoint += 3.0f;
        }
        else if (diffLongDistance >= judgeSize && diffLongDistance < 2 * judgeSize)
        {
            judgeBonusScore = 10;
            StartCoroutine(JudgeTextOn(judgeText[keyNum], "Great", Color.green));
            GameManager.Instance.HealthPoint += 2.0f;
        }
        else
        {
            judgeBonusScore = 8;
            StartCoroutine(JudgeTextOn(judgeText[keyNum], "Good", Color.yellow));
            GameManager.Instance.HealthPoint += 1.0f;
        }
        isLongNote1 = true;
    }
   
    void JudgeLongNoteScore()
    {
        /*
        //1번 롱노트
        if (isLongNote1)
        {
            if (Input.GetKey(KeyCode.Alpha1))
            {
                Debug.Log(111);
                GameManager.Instance.ComboCount += 1;
                collision.gameObject.transform.localScale -= new Vector3(0, GameManager.Instance.NoteSpeed * 0.5f * Time.deltaTime, 0);
                GameManager.Instance.Score += (GameManager.Instance.ComboBonus * judgeBonusScore);

                if (collision.gameObject.transform.localScale.y <= GameManager.Instance.OriginNoteScale / 4)//롱노트 종료
                {
                    isLongNote1 = false;
                    return;
                }
            }
        }
        Debug.Log(collision.gameObject.transform.localScale.y);
        */
    }
}
