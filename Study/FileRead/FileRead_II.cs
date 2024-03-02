using Dummiesman;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Importer : MonoBehaviour
{
    string path = "C:/ObjectSet/ObjectSetCopy";
    string SuccessPath = "C:/ObjectSet/ObjectSetCopy" + "/Success";
    string FailPass = "C:/ObjectSet/ObjectSetCopy" + "/Fail";

    System.IO.DirectoryInfo di = new DirectoryInfo("C:/ObjectSet/ObjectSetCopy");
    private void Awake()
    {
        FileCreate();
        AutoImport();
    }
    void FileCreate()
    {
        if (!File.Exists(path + "/Success")) Directory.CreateDirectory(path + "/Success");
        if (!File.Exists(path + "/Fail")) Directory.CreateDirectory(path + "/Fail");
    }
    void AutoImport()
    {
        int cnt = 0;
        //해당 디렉토리를 순회하면서 파일이름 불러오기
        foreach (FileInfo file in di.GetFiles("*.rvm"))//rvm로 끝나는 파일만 검색
        {
            Debug.Log(file.FullName);
            Debug.Log(file.Name);

            bool prefabSuccess;
            string localPath = "Assets/Prefabs/" + gameObject.name + ".prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            GameObject pPrefab = PrefabUtility.SaveAsPrefabAsset(file, localPath, out prefabSuccess);
            Instantiate(pPrefab);
            
            //GameObject gm = new OBJLoader().Load(file.FullName);
            //Instantiate(gm);
            
            if (GameObject.Find(pPrefab.name))//성공
            {
                Debug.Log("성공");
                System.IO.File.Copy(file.FullName, SuccessPath+"/"+file.Name, true);//해당 경로로 파일 옮기기, true로 하면 덮어쓰기가 된다.
            }
            else//실패
            {
                Debug.Log("실패");
                System.IO.File.Copy(file.FullName, FailPass+"/"+file.Name, true);
            }
            
            AssetDatabase.Refresh();
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());//현재 씬 자동 저장
            cnt++;
            if (cnt > 5) break;
        }

    }
}
