using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

//직렬화
[System.Serializable]
public class MobInfo
{
    public string name;
    public int lv;
    public int xp;
}
[Serializable]
public class MobData
{
    public List<MobInfo> mobInfo = new List<MobInfo>();
}
public class JsonData : MonoBehaviour
{
    public static JsonData Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this.gameObject) Instance = null;
    }
    // Start is called before the first frame update
    void Start()
    {
        TextAsset Text = Resources.Load<TextAsset>("monsterData");
        MobData data = JsonUtility.FromJson<MobData>(Text.text);
        Debug.Log(data.mobInfo[1].lv);
        /*
        //클래스 인스턴스 생성 후 값 할당
        MobData[] mobData = new MobData[3];

        //데이터 넣어주기
        mobData[0] = new MobData();
        mobData[0].name = "블러드 하프";
        mobData[0].lv = 83;
        //mobData[0].info[0] = 31000;
        //mobData[0].info[1] = 1100;

        mobData[1] = new MobData();
        mobData[1].name = "켄타우로스";
        mobData[1].lv = 88;
        //mobData[1].info[0] = 37000;
        //mobData[1].info[1] = 1600;

        mobData[2] = new MobData();
        mobData[2].name = "본피쉬";
        mobData[2].lv = 92;
        //mobData[2].info[0] = 40000;
        //mobData[2].info[1] = 2000;

        //인스턴스를 Json포맷으로 직렬화
        string toJson0 = JsonUtility.ToJson(mobData[0]);
        string toJson1 = JsonUtility.ToJson(mobData[1]);
        string toJson2 = JsonUtility.ToJson(mobData[2]);

        //Json포맷에서 클래스나 구조체로 역 직렬화
        MobData fromJson = JsonUtility.FromJson<MobData>(toJson0);

        //파일 저장
        File.WriteAllText(@"C:\JsonSample.json", toJson1);

        //파일 읽기
        string readJson = File.ReadAllText(@"C:\JsonSample.json");
        Debug.Log(readJson);
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
