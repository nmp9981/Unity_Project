using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting;

[System.Serializable]
public class UserRankData
{
    public int musicNum;//음악 번호
    public int score;//점수
    public string rank;//랭크
}
[System.Serializable]
public class MusicList
{
    public UserRankData[] musicList;
}

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager userData;
    public static MusicList musicListDatas;
    string path = "C:\\Users\\tybna\\RhythmStar\\Assets\\RhythmStar\\Data\\musicDataList.json";
    
    void Awake()
    {
        #region 싱글톤
        if (userData == null)
        {
            userData = this;
        }else if(userData != this)
        {
            Destroy(userData.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        #endregion

        LoadData();
    }

    public void SaveData(UserRankData[] musicList)
    {
        Debug.Log(musicList[0].rank);
        Debug.Log(musicList[1].rank);
        Debug.Log(musicList[2].rank);
        string data = JsonData.ToJson(musicList);
        Debug.Log(data);
        File.WriteAllText(path, data);
    }
    public void LoadData()
    {
        string data = File.ReadAllText(path);
        musicListDatas = JsonUtility.FromJson<MusicList>(data);
    }
}
