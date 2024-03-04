using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Pixyz;
using UnityEngine.Pixyz.API;
using Assimp;
using UnityEditor.PixyzPlugin4Unity.Import;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.PixyzPlugin4Unity.RuleEngine;
using UnityEditor.PixyzPlugin4Unity.Processing;
using UnityEngine.PixyzPlugin4Unity.Licensing;
using UnityEditor.PixyzPlugin4Unity.Toolbox;
using UnityEditor.PixyzPlugin4Unity.Settings;
using UnityEditor.PixyzPlugin4Unity.UI;
using UnityEngine.PixyzPlugin4Unity.Import;
using UnityEditor.PixyzPlugin4Unity.Extensions;
using UnityEditor.PixyzPlugin4Unity.UI.Import;
using UnityEditor.PixyzPlugin4Unity.Utilities;
using UnityEngine.PixyzPlugin4Unity.Import.Templates;
using UnityEngine.PixyzPlugin4Unity.Lod;
using RuntimeInspectorNamespace;
using UnityEngine.PixyzPlugin4Unity.Utilities;

public class RVMImporter : MonoBehaviour
{
    string path = "C:/Users/young/OneDrive/바탕 화면/RMVATT-BACKUP-240222/Outf20240227";
    string SuccessPath = "C:/Users/young/OneDrive/바탕 화면/RMVATT-BACKUP-240222/Outf20240227" + "/Success";
    string FailPass = "C:/Users/young/OneDrive/바탕 화면/RMVATT-BACKUP-240222/Outf20240227" + "/Fail";
    //DirectoryInfo로 폴더정보 불러오기
    System.IO.DirectoryInfo di = new DirectoryInfo("C:/Users/young/OneDrive/바탕 화면/RMVATT-BACKUP-240222/Outf20240227/TwoBlockCopy");

    protected ProgressBar _progressBar;
    ImportSettings _importSettings;
    Importers _importer;

    void Awake()
    {
        _importSettings = new ImportSettings();
        FileCreate();
    }
    void Start()
    {
        AutoImport();
    }
    void FileCreate()
    {
        if(!File.Exists(path + "/Success")) Directory.CreateDirectory(path + "/Success");
        if(!File.Exists(path + "/Fail")) Directory.CreateDirectory(path + "/Fail");
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
            Open(file.FullName);
            OnImportClicked(file.FullName);

            //GameObject pPrefab = PrefabUtility.SaveAsPrefabAsset(file, localPath, out prefabSuccess);
            //Instantiate(pPrefab);

            if (GameObject.Find(file.Name))//성공
            {
                Debug.Log("성공");
                System.IO.File.Copy(file.FullName, SuccessPath + "/" + file.Name, true);//해당 경로로 파일 옮기기, true로 하면 덮어쓰기가 된다.

                AssetDatabase.Refresh();
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());//현재 씬 자동 저장
            }
            else//실패
            {
                Debug.Log("실패");
                System.IO.File.Copy(file.FullName, FailPass + "/" + file.Name, true);
            }
            cnt++;
            if (cnt > 8) break;
        }

    }
    public static void Open(string file)
    {
        ImportSettings _ImportSettings = ScriptableObject.CreateInstance<ImportSettings>();
       
        _ImportSettings = AssetDatabase.LoadAssetAtPath<ImportSettings>(file);
        _ImportSettings.hasLODs = true;
        _ImportSettings.qualities = LodsGenerationSettings.Default();
        _ImportSettings.lodsMode = LodGroupPlacement.LEAVES;

        var lodSettings = new LodGenerationSettings();
        lodSettings.quality = LodQuality.LOW;
        var qualities = _ImportSettings.qualities;
        qualities.quality = lodSettings;
    }
   
    public void OnImportClicked(string _fileToImport)
    {

        _progressBar = new ProgressBar(importCanceled, $"Importing \"{Path.GetFileName(_fileToImport)}\"");

        Plugin4UnityProduct.API.Core.SetModuleProperty("IO", "AliasApiDllPath", Preferences.AliasExecutable.Replace("\\", "/"));
        Plugin4UnityProduct.API.Core.SetModuleProperty("IO", "VredExecutablePath", Preferences.VREDExecutable.Replace("\\", "/"));
        Plugin4UnityProduct.API.Core.SetModuleProperty("IO", "RecapSDKPath", Preferences.RecapSDKPath.Replace("\\", "/"));

        _importer = new Importers(_fileToImport, _importSettings);
        _importer.printMessageOnCompletion = Plugin4UnitySettings.LogImportTime;
        //_importer.ImportPrototypes = (ImportType == ImportType.NestedPrefab)?true:false;

        Plugin4UnityProduct.API.Core.PushAnalytic("ImportType", "Scene");
        _importer.run();

    }
    private void importCanceled()
    {
        _importer?.stop();
    }
}
