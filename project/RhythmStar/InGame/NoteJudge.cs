using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

public class NoteJudge : MonoBehaviour
{
    [SerializeField] GameObject judgeKey1;
    [SerializeField] GameObject judgeKey2;
    [SerializeField] GameObject judgeKey3;
    float maxDistance=0.7f;//최대 판정 거리
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        JudgeScore();
    }
    void JudgeScore()
    {
        //1번
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Vector2 startPoint = new Vector2(judgeKey1.transform.position.x, judgeKey1.transform.position.y) - new Vector2(0, judgeKey1.transform.localScale.y);
            RaycastHit2D hit = Physics2D.Raycast(startPoint, Vector2.up, maxDistance);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Red"))
                {
                    hit.collider.gameObject.SetActive(false);
                    SoundManager._sound.PlaySfx(0);
                }
            }   
        }

        //2번
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Vector2 startPoint = new Vector2(judgeKey2.transform.position.x, judgeKey2.transform.position.y) - new Vector2(0, judgeKey2.transform.localScale.y);
            RaycastHit2D hit = Physics2D.Raycast(startPoint, Vector2.up, maxDistance);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Green"))
                {
                    hit.collider.gameObject.SetActive(false);
                    SoundManager._sound.PlaySfx(1);
                }
            }
        }
        //3번
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Vector2 startPoint = new Vector2(judgeKey3.transform.position.x, judgeKey3.transform.position.y) - new Vector2(0, judgeKey3.transform.localScale.y);
            RaycastHit2D hit = Physics2D.Raycast(startPoint, Vector2.up, maxDistance);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Blue"))
                {
                    hit.collider.gameObject.SetActive(false);
                    SoundManager._sound.PlaySfx(2);
                }
            }
        }
    }
}
