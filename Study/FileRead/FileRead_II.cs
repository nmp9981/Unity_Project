using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Pixyz;
using UnityEngine.Pixyz.API;
using Assimp;
using UnityEditor.PixyzPlugin4Unity.Toolbox;

public class RVMImporter : MonoBehaviour
{
    string path = "C:\\Users\\young\\OneDrive\\바탕 화면\\RMVATT-BACKUP-240222\\Outf20240227";
    //DirectoryInfo로 폴더정보 불러오기
    System.IO.DirectoryInfo di = new DirectoryInfo("C:\\Users\\young\\OneDrive\\바탕 화면\\RMVATT-BACKUP-240222\\Outf20240227\\TwoBlockCopy");
    void Awake()
    {
        FileCreate();
        AutoImport();
    }

    void FileCreate()
    {
        if(!File.Exists(path + "/Success")) Directory.CreateDirectory(path + "/Success");
        if(!File.Exists(path + "/Fail")) Directory.CreateDirectory(path + "/Fail");
    }
    void AutoImport()
    {
        string pullPath = "C:\\Users\\young\\OneDrive\\바탕 화면\\RMVATT-BACKUP-240222\\Outf20240227\\TwoBlockCopy";

        int cnt = 0;
        //해당 디렉토리를 순회하면서 파일이름 불러오기
        foreach (FileInfo file in di.GetFiles("*.rvm"))//rvm로 끝나는 파일만 검색
        {
            Debug.Log(file.FullName);
            
            cnt++;
            if(cnt>10) break;
        }
       
    }

}
