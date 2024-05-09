using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class NoteSpawn : MonoBehaviour
{
    ObjectFulling _objectPulling;
    FileInfo _songFile;
    StreamReader _reader;
    string filePath;
    string noteInfo;

    public int bpm;
    double currentTime = 0d;
    int noteIndex;
    int noteInfoLen;

    double waitLongNote;
    double currentLongNote = 0d;
    void Awake()
    {
        _objectPulling = GameObject.Find("ObjectFulling").GetComponent<ObjectFulling>();
        GameInfoInit();
        FileLoad();
        InvokeRepeating("BPMChange", 0f, 1.5f);
    }
    
    void Update()
    {
        FlowTime();
        if (GameManager.Instance.IsPlayGame) BeatTimeCheck();
    }
    //게임 정보 초기화
    void GameInfoInit()
    {
        GameManager.Instance.TotalNoteCount = 0;
        GameManager.Instance.IsPlayGame = false;
        GameManager.Instance.IsGameOver = false;
        GameManager.Instance.IsGameClear = false;

        GameManager.Instance.ComboCount = 0;
        GameManager.Instance.MaxCombo = 0;
        GameManager.Instance.MissCount = 0;
        GameManager.Instance.GoodCount = 0;
        GameManager.Instance.GreatCount = 0;
        GameManager.Instance.PerfectCount = 0;
    }
    void FileLoad()
    {
        filePath = $"C:\\Users\\tybna\\Songs\\{GameManager.Instance.MusicName}.txt";
        _songFile = new FileInfo(filePath);
        if (_songFile.Exists)
        {
            _reader = new StreamReader(filePath);
            noteInfo = _reader.ReadToEnd();
            noteInfoLen = noteInfo.Length;
            noteIndex = 0;
        }
        
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
         bpm = 120;
    }
    void NoteCreate()
    {
        if (noteIndex >= noteInfoLen && !GameManager.Instance.IsGameOver)//게임 클리어
        {
            GameManager.Instance.IsGameClear = true;
            return;
        }

        char noteNum = noteInfo[noteIndex];//몇번 노트?
        noteIndex++;
        if (noteNum == 'x') return;//노트 생성X

        /*
        if (IslongNoteMake(noteNum))
        {
            if (waitLongNote > currentLongNote) return;//롱노트 생성 불가
            char noteLength = noteInfo[noteIdx];//롱노트 길이
            noteIdx++;
            StartCoroutine(LongNoteMake(noteLength-'0', noteNum-'a'));
            return;
        }
        */
        GameObject noteObj = _objectPulling.MakeObj(noteNum-'0');//노트 생성
        noteObj.GetComponent<NoteFuction>().noteType = NoteType.General;//일반 노트
        currentTime -= 60d / bpm;//currentTime이 정확한 값이 아닌 부동 소수점 오차 존재

        NoteCreatePos(noteObj, noteNum-'0');
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
    bool IslongNoteMake(char noteNum)
    {
        if (noteNum == 'a') return true;
        if (noteNum == 'b') return true;
        if (noteNum == 'c') return true;
        return false;
    }
    IEnumerator LongNoteMake(int num, int noteNum)
    {
        bpm = 120;
        currentLongNote = 0d;
        waitLongNote = num * 0.25f;
        for (int i = 0; i < num; i++)
        {
            GameObject noteObj = _objectPulling.MakeObj(noteNum);
            noteObj.GetComponent<NoteFuction>().noteType = NoteType.Long;//롱 노트
            noteObj.GetComponent<NoteFuction>().longNoteLength = num;
            NoteCreatePos(noteObj, noteNum);
            GameManager.Instance.TotalNoteCount += 1;
            yield return new WaitForSeconds(0.045f);
        }
    }
}
