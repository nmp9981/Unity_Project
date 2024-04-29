using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    void Awake()
    {
        maxDistance = judgeKey1.transform.localScale.y * 6;//최대 판정 거리 설정
        for (int i = 0; i < 3; i++) judgeText[i].text = "";//글자 초기화
    }

    // Update is called once per frame
    void Update()
    {
        JudgeScore();
    }
    void JudgeScore()
    {
        //모바일에서는 키 입력이 아닌 버튼 형식으로 변환
        //1번
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Vector2 startPoint = new Vector2(judgeKey1.transform.position.x, judgeKey1.transform.position.y) - new Vector2(0, 2.5f*judgeKey1.transform.localScale.y);
            RaycastHit2D hit = Physics2D.Raycast(startPoint, Vector2.up, maxDistance);
            clickEffect[0].Play();
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Red"))
                {
                    float diffDistance = Mathf.Abs(hit.collider.gameObject.transform.position.y - judgeKey2.transform.position.y);
                    JudgeScoreAndEffect(diffDistance,0);
                    hit.collider.gameObject.SetActive(false);
                    GameManager.Instance.ComboCount += 1;
                    GameManager.Instance.Score += (10*GameManager.Instance.ComboBonus);
                    SoundManager._sound.PlaySfx(0);
                }
            }   
        }

        //2번
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Vector2 startPoint = new Vector2(judgeKey2.transform.position.x, judgeKey2.transform.position.y) - new Vector2(0, 2.5f*judgeKey2.transform.localScale.y);
            RaycastHit2D hit = Physics2D.Raycast(startPoint, Vector2.up, maxDistance);
            clickEffect[1].Play();

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Green"))
                {
                    if (hit.collider.gameObject.transform.localScale.y >= GameManager.Instance.LongNoteStandardScale)
                    {
                        JudgeLongNote(hit.collider.gameObject, 1);

                    }
                    else
                    {
                        float diffDistance = Mathf.Abs(hit.collider.gameObject.transform.position.y - judgeKey2.transform.position.y);
                        JudgeScoreAndEffect(diffDistance, 1);
                        hit.collider.gameObject.SetActive(false);
                        GameManager.Instance.ComboCount += 1;
                        GameManager.Instance.Score += (10 * GameManager.Instance.ComboBonus);
                        SoundManager._sound.PlaySfx(1);
                    }
                }
            }
        }
        //3번
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Vector2 startPoint = new Vector2(judgeKey3.transform.position.x, judgeKey3.transform.position.y) - new Vector2(0, 2.5f*judgeKey3.transform.localScale.y);
            RaycastHit2D hit = Physics2D.Raycast(startPoint, Vector2.up, maxDistance);
            clickEffect[2].Play();

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Blue"))
                {
                    float diffDistance = Mathf.Abs(hit.collider.gameObject.transform.position.y - judgeKey2.transform.position.y);
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
        int judgeBonusScore = 10;
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

    }
}
