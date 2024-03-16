using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.RuntimeSceneSerialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MGIZMO.SAVE
{
    public static class RuntimeSceneSave
    {
        static Action<Scene> _onLoadedEnd;
        public static event Action<Scene> onLoadedEnd 
        { 
            add => _onLoadedEnd += value;
            remove => _onLoadedEnd -= value;
        }
        static Action<Scene> _onUnLoadedEnd;
        public static event Action<Scene> onUnLoadedEnd
        {
            add => _onUnLoadedEnd += value;
            remove => _onUnLoadedEnd -= value;
        }

        public static void SaveScene(string folderPath, string projectName)
        {
            Scene projectScene = SceneManager.GetSceneByName(projectName);
            if (!projectScene.isLoaded) return;

            string projectFolderPath = $"{folderPath}/Project/{projectName}";
            string resourceFolderPath = $"{folderPath}/Resource";

            SaveLoadUtility.FolderExistAndCreate(folderPath);
            SaveLoadUtility.FolderExistAndCreate(projectFolderPath);

            List<GameObject> roots = new();
            List<IResourceSaver> dataTests = new();

            DeleteFiles();
            void DeleteFiles()
            {
                List<string> fileList = Directory.GetFiles(projectFolderPath).ToList();

                foreach (var obj in roots)
                {
                    if (fileList.Contains(obj.name))
                    {
                        fileList.Remove(obj.name);
                    }
                }

                foreach (var file in fileList)
                {
                    File.Delete(file);
                }
            }

            projectScene.GetRootGameObjects(roots);
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.SceneManagement.EditorSceneManager.SaveScene(projectScene, projectFolderPath + $"/{projectName}.unity");
#endif

            foreach (var item in roots)
            {
                UnloadScene(item.name, OnLoadedScene);

                void OnLoadedScene()
                {
                    Scene temp = CreateScene(item.name);

                    string filePath = $"{projectFolderPath}/{item.name}.json";

                    GameObject clone = GameObject.Instantiate(item);
                    clone.name = item.name;

                    dataTests.Clear();
                    clone.GetComponentsInChildren(dataTests);
                    foreach (var saveData in dataTests)
                    {
                        saveData.Save();
                    }
                    SceneManager.MoveGameObjectToScene(clone, temp);
                    string json = SceneSerialization.SerializeScene(temp);

                    File.WriteAllText(filePath, json);

                    UnloadScene(item.name, CallGc);
                }
            }

            RuntimeResourceManager.ResourceSave(resourceFolderPath);

#if UNITY_EDITOR
            if (File.Exists(projectFolderPath + $"/{projectName}.unity"))
            {
                File.Delete(projectFolderPath + $"/{projectName}.unity");
                File.Delete(projectFolderPath + $"/{projectName}.unity.meta");
            }
#endif

            CallGc();
        }
        public static void LoadScene(string folderPath, string projectName)
        {
            string projectFolderPath = $"{folderPath}/Project/{projectName}";
            string resourceFolderPath = $"{folderPath}/Resource";
            
            RuntimeResourceManager.ResourceLoad(resourceFolderPath);

            if (!Directory.Exists(projectFolderPath)) return;

            var directory = new DirectoryInfo(projectFolderPath);

            UnloadScene(projectName, OnLoadedScene);

            void OnLoadedScene()
            {
                string[] roots = directory.GetFiles("*.json").OrderBy(p => p.CreationTime).Select(x => x.FullName).ToArray();

                if (roots.Length == 0) return;

                var renderSetting = SerializedRenderSettings.CreateFromActiveScene();
                Scene projectScene = CreateScene(projectName);
                projectScene.name = projectName;
                SceneManager.SetActiveScene(projectScene);
                renderSetting.ApplyValuesToRenderSettings();

                foreach (string root in roots)
                {
                    string sceneJson = File.ReadAllText(root);

                    SceneSerialization.ImportScene(sceneJson, null, (itmes) =>
                    {
                        List<IResourceSaver> dataTests = new();

                        foreach (var item in itmes)
                        {
                            dataTests.Clear();

                            item.GetComponentsInChildren(dataTests);
                            foreach (var loadObj in dataTests)
                            {
                                loadObj.Load();
                            }
                        }

                        CallGc();
                    });
                }

                if (_onLoadedEnd != null)
                {
                    _onLoadedEnd.Invoke(projectScene);
                }
                CallGc();
            }
        }

        public static Scene CreateScene(string sceneName, bool callLoadedAction = false)
        {
#if UNITY_EDITOR
            Scene temp = Application.isPlaying ? SceneManager.CreateScene(sceneName) :
                UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
                UnityEditor.SceneManagement.NewSceneMode.Additive);
#else
            Scene temp = SceneManager.CreateScene(sceneName);
#endif
            temp.name = sceneName;

            if (callLoadedAction)
            {
                if (_onLoadedEnd != null) 
                    _onLoadedEnd.Invoke(temp);

                SerializedRenderSettings activeScene = SerializedRenderSettings.CreateFromActiveScene();
                SceneManager.SetActiveScene(temp);
                activeScene.ApplyValuesToRenderSettings();
            }
            return temp;
        }
        public static void UnloadScene(string sceneName, Action unLoadedEndAction = null)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);

            if (scene.isLoaded)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    SceneManager.UnloadSceneAsync(scene).completed += 
                        _ =>
                        {
                            unLoadedEndAction?.Invoke();
                            _onUnLoadedEnd?.Invoke(scene);
                        };
                }
                else
                {
                    UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
                    unLoadedEndAction?.Invoke();
                    _onUnLoadedEnd?.Invoke(scene);
                }
#else
                SceneManager.UnloadSceneAsync(scene).completed += 
                    _ =>
                    {
                        unLoadedEndAction?.Invoke();
                        _onUnLoadedEnd?.Invoke(scene);
                    };
#endif

                return;
            }

            unLoadedEndAction?.Invoke();
        }
        public static void ClearAll()
        {
            ClientResrouceCache.Clear();
            RuntimeResourceManager.Clear();

            System.GC.Collect();
            System.GC.Collect();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
        static void CallGc()
        {
            System.GC.Collect();
            System.GC.Collect();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
        #region For Editor
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/RuntimeSceneSave/Create New Scene")]
        public static void CreateNewScene_InEdior()
        {
            UnloadScene("New Project", ()=> CreateScene("New Project", true));
        }
        [UnityEditor.MenuItem("Tools/RuntimeSceneSave/Close Scene")]
        public static void CloseNewScene_InEdior()
        {
            UnloadScene("New Project", null);
        }
        [UnityEditor.MenuItem("Tools/RuntimeSceneSave/Save Scene")]
        public static void SaveScene_InEditor()
        {
            string folderPath = $"D:/SaveData";
            string projectName = "New Project";

            SaveScene(folderPath, projectName);

            System.GC.Collect();
            System.GC.Collect();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        [UnityEditor.MenuItem("Tools/RuntimeSceneSave/Load Scene")]
        public static void LoadDefaultScene()
        {
            LoadScene("D:/SaveData", "New Project");
        }
        [UnityEditor.MenuItem("Tools/RuntimeSceneSave/Clear All")]
        public static void ClearAll_InEditor()
        {
            ClearAll();
        }
#endif
#endregion
    }
}
