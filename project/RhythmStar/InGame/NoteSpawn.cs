using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawn : MonoBehaviour
{
    ObjectFulling _objectPulling;

    public int bpm;
    double currentTime = 0d;
    int noteLength;

    double waitLongNote1;
    double waitLongNote2;
    double waitLongNote3;
    double waitLongNote;

    double currentLongNote1 = 0d;
    double currentLongNote2 = 0d;
    double currentLongNote3 = 0d;
    double currentLongNote = 0d;
    void Awake()
    {
        _objectPulling = GameObject.Find("ObjectFulling").GetComponent<ObjectFulling>();
        GameManager.Instance.TotalNoteCount = 0;
        GameManager.Instance.IsPlayGame = false;
        GameManager.Instance.IsGameOver = false;
        InvokeRepeating("BPMChange", 0f, 1.5f);
    }
  
    void Update()
    {
        FlowTime();
        if (GameManager.Instance.IsPlayGame) BeatTimeCheck();
    }
    void FlowTime()
    {
        currentLongNote += Time.deltaTime;
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
        bpm = Random.Range(90, 300);
    }
    void NoteCreate()
    {
        int noteNum = Random.Range(-1, 3);//몇번 노트?
        if (noteNum == -1) return;//노트 생성X

        //노트 생성 불가
        if (waitLongNote > currentLongNote) return;
       
        if (IslongNoteMake())
        {
            noteLength = Random.Range(8, 50);
            StartCoroutine(LongNoteMake(noteLength, noteNum));
            return;
        }
        GameObject noteObj = _objectPulling.MakeObj(noteNum);//노트 생성
        currentTime -= 60d / bpm;//currentTime이 정확한 값이 아닌 부동 소수점 오차 존재

        NoteCreatePos(noteObj, noteNum);
        GameManager.Instance.TotalNoteCount += 1;
    }
    void NoteCreatePos(GameObject noteObj, int noteNum)
    {
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
    bool IslongNoteMake()
    {
        int longNoteNum = Random.Range(0, 100);
        if (longNoteNum >= 90)
        {
            return true;
        }
        return false;
    }
    IEnumerator LongNoteMake(int num, int noteNum)
    {
        bpm = 120;
        for(int i = 0; i < num; i++)
        {
            GameObject noteObj = _objectPulling.MakeObj(noteNum);
            NoteCreatePos(noteObj, noteNum);
            GameManager.Instance.TotalNoteCount += 1;
            yield return new WaitForSeconds(0.05f);
        }
        currentLongNote = 0d;
        waitLongNote = num * 0.25f;
    }
}
