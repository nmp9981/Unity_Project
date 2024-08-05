using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

[System.Serializable]
public class ResultData
{
    public int areaNum;
    public int[,] pos;
}
[System.Serializable]
public class ResultDataSet
{
    public ResultData[] AreaData;
}

public class LocalToWorld : MonoBehaviour
{
    public static ResultDataSet resultDataSet;
  
    void Awake()
    {
        LoadData();
    }

    public void UseData()
    {
        for (int i = 1; i < 4; i++)
        {
            Debug.Log(resultDataSet.AreaData[i].pos[0,0] + " " + resultDataSet.AreaData[i].pos[0,1] + " " + resultDataSet.AreaData[i].pos[0,2]);
        }
    }
    public void LoadData()
    {
        string path = "Assets/Resources/Table/PipeData/PipeArea.json";
        string data = File.ReadAllText(path);
        //resultDataSet = JsonUtility.FromJson<ResultDataSet>(data);//2차원이상 배열 지원X

        resultDataSet = JsonConvert.DeserializeObject<ResultDataSet>(data);//2차원 이상 배열 지원 O
        UseData();
    }

}
