using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

public class UserRankData
{
    public int musicNum;//음악 번호
    public int score;//점수
    public char rank;//랭크
}
public class UserDataManager : MonoBehaviour
{
    public static UserDataManager userData;
    UserRankData nowPlayer = new UserRankData();

    public static List<UserRankData> userRankData = new List<UserRankData>();
    string path;
    string filename = "musicRankdata";

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

        path = Application.persistentDataPath+"/";

        userRankData.Add(nowPlayer);
        userRankData.Add(nowPlayer);
        userRankData.Add(nowPlayer);
    }

    public void SaveData()
    {
        string data = JsonUtility.ToJson(userRankData);
        File.WriteAllText(path+filename, data);
    }
    public void LoadData()
    {
        string data = File.ReadAllText(path+filename);
        userRankData = JsonUtility.FromJson<List<UserRankData>>(data);
    }
}
