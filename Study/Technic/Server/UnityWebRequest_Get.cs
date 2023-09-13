using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using UnityEngine;
using BestHTTP.JSON.LitJson;
using UnityEngine.Networking;
using UnityEditor;

public class IVLocalizationDataJson : MonoBehaviour
{
    public static IVLocalizationDataJson _instance = null;
    static string path = "D:\\LocalFile";

    static byte[] textFile;
    static string str;
    private void Awake()
    {
        #region 싱글톤
        if (_instance == null)
        {
            GameObject go = GameObject.Find("LocalizationManager");
            if (go == null)
            {
                go = new GameObject { name = "LocalizationManager" };
                go.AddComponent<IVLocalizationDataJson>();
            }
            DontDestroyOnLoad(go);
            _instance = go.GetComponent<IVLocalizationDataJson>();
        }
        #endregion
        StartCoroutine(DataGet());
        //LocalDataGet();
    }

    //저장
    public static void Save()
    {
        TextAsset textFile = Resources.Load("Localization_sqgolf") as TextAsset;
        JsonData textJsonFile = JsonMapper.ToJson(textFile);
        File.WriteAllText(path + "/Local.json", textJsonFile.ToString());
    }
    //로드
    public static string[] Load()
    {
        string[] textJsonFile = File.ReadAllLines(path + "/sqgolf.txt");
        
        return textJsonFile;
    }
    public static string[] Loads()
    {
        Debug.LogError(textFile.Length);
        str = System.Text.Encoding.UTF8.GetString(textFile);
        StringReader reader = new StringReader(str);
        string[] textAllFile = new string[272];//전체 파일
        Debug.LogError("총길이 "+str.Length);
        for (int i = 0; i < str.Length; i++)//한줄씩 탐색
        {
            if (i == 271) break;
            textAllFile[i] = reader.ReadLine();
        }
        return textAllFile;
    }

    //데이터 로드
    IEnumerator DataGet()
    {
        string url = "https://squaregolf.s3.ap-northeast-2.amazonaws.com/live/AssetData/common/sqgolf_localization.txt";
        //string urls = "https://squaregolf.s3.ap-northeast-2.amazonaws.com/live/AssetData/common/Localization_sqgolf.txt";
        UnityWebRequest webLocalFile = UnityWebRequest.Get(url);
        yield return webLocalFile.SendWebRequest();

        if (webLocalFile.error == null)
        {
            Debug.LogError("성공");
            textFile = webLocalFile.downloadHandler.data;
        }
        else Debug.LogError("실패");
    }
    void LocalDataGet()
    {
        string url = "https://squaregolf.s3.ap-northeast-2.amazonaws.com/live/AssetData/common/sqgolf_localization.txt";
        //string urls = "https://squaregolf.s3.ap-northeast-2.amazonaws.com/live/AssetData/common/Localization_sqgolf.txt";
        UnityWebRequest webLocalFile = UnityWebRequest.Get(url);
        webLocalFile.SendWebRequest();

        if (webLocalFile.error == null)
        {
            Debug.LogError("성공");
            textFile = webLocalFile.downloadHandler.data;
        }
        else Debug.LogError("실패");
    }
}
