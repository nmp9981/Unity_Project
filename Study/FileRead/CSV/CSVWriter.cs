using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

struct TalkStruct
{
    public string key { get; set; }
    public string talker { get; set; }
    public string talk { get; set; }
}
public class CSVWriter : MonoBehaviour
{
    // CSV 파일로 내보내기
    private List<string[]> rowData = new List<string[]>();
    List<TalkStruct> TalkAddText = new List<TalkStruct>();//최초 데이터

    private void Awake()
    {
        Talk();
        Write();
    }
    void Talk()
    {
        //데이터 넣기
        TalkAddText = new List<TalkStruct>()
        {
            new TalkStruct {key = "a",talker = "2",talk = "3" },
            new TalkStruct {key = "b",talker = "5",talk = "6" },
            new TalkStruct {key = "c",talker = "8",talk = "9" },
            new TalkStruct {key = "d",talker = "11",talk = "12" }
        };
    }
    void Write()
    {
        //첫줄 제목
        string[] rowDataTemp = new string[3];
        rowDataTemp[0] = "key";
        rowDataTemp[1] = "talker";
        rowDataTemp[2] = "talk";
        rowData.Add(rowDataTemp);

        //각 줄의 내용 추가
        for (int i = 0; i < TalkAddText.Count; i++)
        {
            rowDataTemp = new string[3];
            rowDataTemp[0] = TalkAddText[i].key + "dialogue" + i; // name
            rowDataTemp[1] = TalkAddText[i].talker; // name
            rowDataTemp[2] = TalkAddText[i].talk; // ID
            rowData.Add(rowDataTemp);
        }

        string[][] output = new string[TalkAddText.Count + 1][];//엑셀에 반영될 데이터

        //rowData의 데이터 넣기
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);//행 길이
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
        {
            sb.AppendLine(string.Join(delimiter, output[index]));//데이터 추가 및 문자열 이어 붙이기
        }

        string filePath = getPath();//저장할 파일 경로 가져오기

        StreamWriter outStream = System.IO.File.CreateText(filePath);//해당 경로에 파일 생성
        outStream.WriteLine(sb);//파일에 한줄씩 쓰기
        outStream.Close();//파일 닫기

        Debug.Log(filePath);
    }
    private void Read()
    {
        
    }
    private string getPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/TalkData.csv";
#elif UNITY_ANDROID
        return Application.persistentDataPath+"TalkData.csv";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+"TalkData.csv";
#else
        return Application.dataPath +"/"+"TalkData.csv";
#endif
    }
}
