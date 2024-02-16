using JetBrains.Annotations;
using MGIZMO.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ESTest : MonoBehaviour
{
    List<GameObject> RootObjects = new List<GameObject>();
    List<string> RootNames = new List<string>();

    private void OnEnable()
    {
        Load();
    }
    private void Start()
    {
        Load();
    }
    private void OnDisable()
    {
        Save();
    }

    void GetRootObject()
    {
        List<GameObject> RootObjects = new List<GameObject>();
        SceneManager.GetActiveScene().GetRootGameObjects(RootObjects);
        this.RootObjects.Clear();
        this.RootObjects.AddRange(RootObjects.Where(x => x.GetComponent<RTComponent>() != null));
        RootNames = this.RootObjects.Select(x => x.name).ToList();
    }

    public void ClearSaveData()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (!ES3.KeyExists($"{sceneName}/roots")) return;
        RootNames = ES3.Load<List<string>>($"{sceneName}/roots");
        foreach (var item in RootNames)
        {
            ES3.DeleteKey($"{sceneName}/{item}");
        }
        ES3.DeleteKey($"{sceneName}/roots");
    }
    public void Save()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        ClearSaveData();
        GetRootObject();
        ES3.Save($"{sceneName}/roots", RootNames);
        foreach (var item in RootObjects)
        {
            ES3.Save($"{sceneName}/{item.name}", item);
        }
    }

    public void Load()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (!ES3.KeyExists($"{sceneName}/roots")) return;
        GetRootObject();
        RootNames = ES3.Load<List<string>>($"{sceneName}/roots");
        HashSet<string> nameSet = new HashSet<string>(RootNames);
        foreach(var item in RootObjects)
        {
            if (!nameSet.Contains(item.name))
            {
                Destroy(item);
            }
        }

        nameSet = null;

        foreach (var item in RootNames)
        {
            if (!ES3.KeyExists($"{sceneName}/{item}")) continue;
            GameObject go = ES3.Load<GameObject>($"{sceneName}/{item}");
            Debug.Log(go ? go.name : "null");
        }

        if (RTGizmoController.Instance) RTGizmoController.Instance.targetObject = null;
    }
}
