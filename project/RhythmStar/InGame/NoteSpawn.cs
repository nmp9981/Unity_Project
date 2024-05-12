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

        //처음엔 모두 일반 노트
        GerneralNoteChange(0);
        GerneralNoteChange(1);
        GerneralNoteChange(2);
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
        switch (GameManager.Instance.MusicNumber)
        {
            case 0:
                bpm = 126;
                break;
            case 1:
                bpm = 148;//2배
                break;
            case 2:
                bpm = 240;//2배
                break;
        }
    }
    void NoteCreate()
    {
        if (noteIndex >= noteInfoLen && !GameManager.Instance.IsGameOver)//게임 클리어
        {
            Invoke("GameClearJudge", 2.5f);
            return;
        }

        char noteNum = noteInfo[noteIndex];//몇번 노트?
        Debug.Log(noteNum);
        noteIndex++;
        if (noteNum == 'x') return;//노트 생성X

        if (IslongNoteMake(noteNum))
        {
            if (waitLongNote > currentLongNote) return;//노트 생성 불가
            char noteLength = noteInfo[noteIndex];//롱노트 길이
            noteIndex++;
            StartCoroutine(LongNoteMake(noteLength-'0', noteNum-'a'));
            return;
        }
        
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
    //롱노트 생성
    IEnumerator LongNoteMake(int num, int noteNum)
    {
        currentLongNote = 0d;
        waitLongNote = num * 0.1f;
        int noteLenRivision = noteNum * (noteNum/2) * 3;
        for (int i = 0; i < noteLenRivision; i++)
        {
            GameObject noteObj = _objectPulling.MakeObj(noteNum);
            noteObj.GetComponent<NoteFuction>().noteType = NoteType.Long;//롱 노트
            noteObj.GetComponent<NoteFuction>().longNoteLength = num;
            NoteCreatePos(noteObj, noteNum);
            GameManager.Instance.TotalNoteCount += 1;
            yield return new WaitForSeconds(0.045f);
        }
        yield return new WaitForSeconds(2f);
        GerneralNoteChange(noteNum);//다시 일반 노트로
    }
    void LongNoteChange(int noteNum)
    {
        switch (noteNum)
        {
            case 0:
                GameManager.Instance.IsLongNote1 = true;
                break;
            case 1:
                GameManager.Instance.IsLongNote2 = true;
                break;
            case 2:
                GameManager.Instance.IsLongNote3 = true;
                break;
        }
    }
    void GerneralNoteChange(int noteNum)
    {
        switch (noteNum)
        {
            case 0:
                GameManager.Instance.IsLongNote1 = false;
                break;
            case 1:
                GameManager.Instance.IsLongNote2 = false;
                break;
            case 2:
                GameManager.Instance.IsLongNote3 = false;
                break;
        }
    }

    void GameClearJudge()
    {
        GameManager.Instance.IsGameClear = true;
    }
}
