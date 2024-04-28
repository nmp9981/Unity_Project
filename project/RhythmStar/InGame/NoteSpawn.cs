using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawn : MonoBehaviour
{
    ObjectFulling _objectPulling;

    public int bpm;
    double currentTime = 0d;


    void Awake()
    {
        _objectPulling = GameObject.Find("ObjectFulling").GetComponent<ObjectFulling>();
        GameManager.Instance.TotalNoteCount = 0;
        GameManager.Instance.IsPlayGame = false;
        GameManager.Instance.IsGameOver = false;
        InvokeRepeating("BPMChange", 0f, 1.5f);
        //StartCoroutine(NoteCreates());
    }
    /*
    IEnumerator NoteCreates()
    {
        for(int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.3f);
            int noteNum = Random.Range(0, 3);
            GameObject noteObj = _objectPulling.MakeObj(noteNum);

            switch (noteNum)
            {
                case 0:
                    noteObj.transform.position = new Vector2(-1, 6);
                    break;
                case 1:
                    noteObj.transform.position = new Vector2(0, 6);
                    break;
                case 2:
                    noteObj.transform.position = new Vector2(1, 6);
                    break;
            }
        }
    }
    */
    void Update()
    {
        if (GameManager.Instance.IsPlayGame) BeatTimeCheck();
    }
    void BeatTimeCheck()
    {
        currentTime += Time.deltaTime;
        if (currentTime > 60d / bpm)//현재 시간 >= 1beat 시간(노트 생성 속도)
        {
            if (!GameManager.Instance.IsGameOver) NoteCreate();
        }
    }
    void BPMChange()
    {
        bpm = Random.Range(60, 300);
    }
    void NoteCreate()
    {
        int noteNum = Random.Range(0, 3);
        GameObject noteObj = _objectPulling.MakeObj(noteNum);
        currentTime -= 60d / bpm;//currentTime이 정확한 값이 아닌 부동 소수점 오차 존재

        switch (noteNum)
        {
            case 0:
                noteObj.transform.position = new Vector2(-1, 6);
                break;
            case 1:
                noteObj.transform.position = new Vector2(0, 6);
                break;
            case 2:
                noteObj.transform.position = new Vector2(1, 6);
                break;
        }
        GameManager.Instance.TotalNoteCount += 1;
    }
}
