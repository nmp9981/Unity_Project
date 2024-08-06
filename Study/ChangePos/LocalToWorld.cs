using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using static UnityEngine.GraphicsBuffer;
using Model;

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
    [SerializeField] GameObject cube;

    Vector3 fromTarget;
    Vector3 toTarget;
    Vector3 targetSize;
    float rate = 512f / 1920f;
    void Awake()
    {
        LoadData();
    }
    private void Start()
    {
        FindTarget();
        PipeSet();
    }
    void FindTarget()
    {
        foreach (var equip in ObjectManager.Instance.EquipmentList)
        {
            if (equip.name != "570V-001") continue;
            foreach (var nozzle in equip.NozzleList)
            {
                if (nozzle.name == "N9")
                {
                    Bounds bound = default;
                    foreach (var mesh in nozzle.gameObject.GetComponentsInChildren<MeshRenderer>())
                    {
                        if (bound == default) bound = mesh.bounds;
                        else bound.Encapsulate(mesh.bounds);
                    }
                    fromTarget = bound.center;
                    targetSize = bound.size;
                }
            }
        }
        foreach (var equip in ObjectManager.Instance.EquipmentList)
        {
            if (equip.name != "570TK-002A") continue;
            foreach (var nozzle in equip.NozzleList)
            {
                if (nozzle.name == "N1")
                {
                    Bounds bound = default;
                    foreach (var mesh in nozzle.gameObject.GetComponentsInChildren<MeshRenderer>())
                    {
                        if (bound == default) bound = mesh.bounds;
                        else bound.Encapsulate(mesh.bounds);
                    }
                    toTarget = bound.center;
                    targetSize = bound.size;
                }
            }
        }
    }
   
    public void PipeSet()
    {
        //지점 설치
        for (int i = 0; i < 4; i++)//각 파이프 별로
        {
            for(int j = 0; j < 5; j++)
            {
                GameObject gm = Instantiate(cube);
                gm.transform.parent = this.transform;
                Vector3 localPos = new Vector3(resultDataSet.AreaData[i].pos[j, 0], resultDataSet.AreaData[i].pos[j, 1], resultDataSet.AreaData[i].pos[j, 2]);
                gm.transform.position = localPos;

                gm.transform.position = LocalToWorldPosition(localPos);//여기에서 좌표 변환
                gm.transform.localScale = Vector3.one * rate;
            }
        }
        //사이에 설치
        for (int i = 0; i < 4; i++)//각 파이프 별로
        {
            string objName = "Pipe" + i;
            GameObject obj = new GameObject(objName);
            if (obj.GetComponent<Pipeline>() == null) obj.AddComponent<Pipeline>();
            obj.transform.parent = this.transform;
          
            for(int j = 0; j < 4; j++)
            {
                float middleX = (resultDataSet.AreaData[i].pos[j + 1, 0] + resultDataSet.AreaData[i].pos[j, 0])/2f;
                float middleY = (resultDataSet.AreaData[i].pos[j + 1, 1] + resultDataSet.AreaData[i].pos[j, 1])/2f;
                float middleZ = (resultDataSet.AreaData[i].pos[j + 1, 2] + resultDataSet.AreaData[i].pos[j, 2])/2f;
                Vector3 middle = new Vector3(middleX, middleY, middleZ);

                bool sameX = (resultDataSet.AreaData[i].pos[j + 1, 0] == resultDataSet.AreaData[i].pos[j, 0]) ? true : false;
                bool sameY = (resultDataSet.AreaData[i].pos[j + 1, 1] == resultDataSet.AreaData[i].pos[j, 1]) ? true : false;
                bool sameZ = (resultDataSet.AreaData[i].pos[j + 1, 2] == resultDataSet.AreaData[i].pos[j, 2]) ? true : false;

                float diffX = resultDataSet.AreaData[i].pos[j + 1, 0] - resultDataSet.AreaData[i].pos[j, 0];
                float diffY = resultDataSet.AreaData[i].pos[j + 1, 1] - resultDataSet.AreaData[i].pos[j, 1];
                float diffZ = resultDataSet.AreaData[i].pos[j + 1, 2] - resultDataSet.AreaData[i].pos[j, 2];
               
                GameObject gm = Instantiate(cube);
                gm.transform.parent = obj.transform;

                //각 케이스별로
                if(!sameX && sameY && sameZ)
                {
                    gm.transform.position = middle;
                    gm.transform.position = LocalToWorldPosition(gm.transform.position);
                    gm.transform.localScale = new Vector3(diffX, rate, rate);
                    if(gm.GetComponent<Pipe>()==null) gm.AddComponent<Pipe>();
                    obj.GetComponent<Pipeline>().PipeList.Add(gm.GetComponent<Pipe>());
                }
                else if (sameX && !sameY && sameZ)
                {
                    gm.transform.position = middle;
                    gm.transform.position = LocalToWorldPosition(gm.transform.position);
                    gm.transform.localScale = new Vector3(rate, diffY, rate);
                    if (gm.GetComponent<Pipe>() == null) gm.AddComponent<Pipe>();
                    obj.GetComponent<Pipeline>().PipeList.Add(gm.GetComponent<Pipe>());
                }
                else if (sameX && sameY && !sameZ)
                {
                    gm.transform.position = middle;
                    gm.transform.position = LocalToWorldPosition(gm.transform.position);
                    gm.transform.localScale = new Vector3(rate, rate, diffZ);
                    if (gm.GetComponent<Pipe>() == null) gm.AddComponent<Pipe>();
                    obj.GetComponent<Pipeline>().PipeList.Add(gm.GetComponent<Pipe>());
                }
                else if (!sameX && !sameY && sameZ)
                {
                    gm.transform.position = new Vector3(middleX, resultDataSet.AreaData[i].pos[j, 1], resultDataSet.AreaData[i].pos[j, 2]);
                    gm.transform.position = LocalToWorldPosition(gm.transform.position);
                    gm.transform.localScale = new Vector3(diffX, rate, rate);

                    GameObject gm2 = Instantiate(cube);
                    gm2.transform.parent = obj.transform;
                    gm2.transform.position = new Vector3( resultDataSet.AreaData[i].pos[j+1, 0],middleY, resultDataSet.AreaData[i].pos[j, 2]);
                    gm2.transform.position = LocalToWorldPosition(gm2.transform.position);
                    gm2.transform.localScale = new Vector3(rate, diffY, rate);

                    if (gm.GetComponent<Pipe>() == null) gm.AddComponent<Pipe>();
                    if (gm2.GetComponent<Pipe>() == null) gm2.AddComponent<Pipe>();
                    obj.GetComponent<Pipeline>().PipeList.Add(gm.GetComponent<Pipe>());
                    obj.GetComponent<Pipeline>().PipeList.Add(gm2.GetComponent<Pipe>());
                }
                else if (!sameX && sameY && !sameZ)
                {
                    gm.transform.position = new Vector3(middleX, resultDataSet.AreaData[i].pos[j, 1], resultDataSet.AreaData[i].pos[j, 2]);
                    gm.transform.position = LocalToWorldPosition(gm.transform.position);
                    gm.transform.localScale = new Vector3(diffX, rate, rate);

                    GameObject gm2 = Instantiate(cube);
                    gm2.transform.parent = obj.transform;
                    gm2.transform.position = new Vector3(resultDataSet.AreaData[i].pos[j+1, 0],  resultDataSet.AreaData[i].pos[j, 1],middleZ);
                    gm2.transform.position = LocalToWorldPosition(gm2.transform.position);
                    gm2.transform.localScale = new Vector3(rate, rate, diffZ);

                    if (gm.GetComponent<Pipe>() == null) gm.AddComponent<Pipe>();
                    if (gm2.GetComponent<Pipe>() == null) gm2.AddComponent<Pipe>();
                    obj.GetComponent<Pipeline>().PipeList.Add(gm.GetComponent<Pipe>());
                    obj.GetComponent<Pipeline>().PipeList.Add(gm2.GetComponent<Pipe>());
                }
                else if (sameX && !sameY && !sameZ)
                {
                    gm.transform.position = new Vector3( resultDataSet.AreaData[i].pos[j, 0],middleY, resultDataSet.AreaData[i].pos[j, 2]);
                    gm.transform.position = LocalToWorldPosition(gm.transform.position);
                    gm.transform.localScale = new Vector3(rate, diffY, rate);

                    GameObject gm2 = Instantiate(cube);
                    gm2.transform.parent = obj.transform;
                    gm2.transform.position = new Vector3(resultDataSet.AreaData[i].pos[j+1, 0], resultDataSet.AreaData[i].pos[j, 1], middleZ);
                    gm2.transform.position = LocalToWorldPosition(gm2.transform.position);
                    gm2.transform.localScale = new Vector3(rate, rate, diffZ);

                    if (gm.GetComponent<Pipe>() == null) gm.AddComponent<Pipe>();
                    if (gm2.GetComponent<Pipe>() == null) gm2.AddComponent<Pipe>();
                    obj.GetComponent<Pipeline>().PipeList.Add(gm.GetComponent<Pipe>());
                    obj.GetComponent<Pipeline>().PipeList.Add(gm2.GetComponent<Pipe>());
                }
                else if (!sameX && !sameY && !sameZ)
                {
                    gm.transform.position = new Vector3(middleX, resultDataSet.AreaData[i].pos[j, 1], resultDataSet.AreaData[i].pos[j, 2]);
                    gm.transform.position = LocalToWorldPosition(gm.transform.position);
                    gm.transform.localScale = new Vector3(diffX, rate, rate);

                    GameObject gm2 = Instantiate(cube);
                    gm2.transform.parent = obj.transform;
                    gm2.transform.position = new Vector3(resultDataSet.AreaData[i].pos[j+1, 0], middleY, resultDataSet.AreaData[i].pos[j, 2]);
                    gm2.transform.position = LocalToWorldPosition(gm2.transform.position);
                    gm2.transform.localScale = new Vector3(rate, diffY, rate);

                    GameObject gm3 = Instantiate(cube);
                    gm3.transform.parent = obj.transform;
                    gm3.transform.position = new Vector3(resultDataSet.AreaData[i].pos[j+1, 0], resultDataSet.AreaData[i].pos[j+1, 1], middleZ);
                    gm3.transform.position = LocalToWorldPosition(gm3.transform.position);
                    gm3.transform.localScale = new Vector3(rate, rate, diffZ);

                    if (gm.GetComponent<Pipe>() == null) gm.AddComponent<Pipe>();
                    if (gm2.GetComponent<Pipe>() == null) gm2.AddComponent<Pipe>();
                    if (gm3.GetComponent<Pipe>() == null) gm3.AddComponent<Pipe>();
                    obj.GetComponent<Pipeline>().PipeList.Add(gm.GetComponent<Pipe>());
                    obj.GetComponent<Pipeline>().PipeList.Add(gm2.GetComponent<Pipe>());
                    obj.GetComponent<Pipeline>().PipeList.Add(gm3.GetComponent<Pipe>());
                }
            }
            ObjectManager.Instance.PipelineList.Add(obj.GetComponent<Pipeline>());
        }
    }
    public void LoadData()
    {
        string path = "Assets/Resources/Table/PipeData/PipeArea.json";
        string data = File.ReadAllText(path);
        //resultDataSet = JsonUtility.FromJson<ResultDataSet>(data);
        resultDataSet = JsonConvert.DeserializeObject<ResultDataSet>(data);
    }
    
    Vector3 LocalToWorldPosition(Vector3 pos)
    {
        Vector3 O = fromTarget;
        Vector3 MaxPos = toTarget;
        //Vector3 O = new Vector3(2537.081f, 96.39884f, 2865.976f);//원점
        Vector3 boundSize = new Vector3(2542.543f, 99.47126f, 2869.048f) - O;
        //Vector3 worldPos = O + new Vector3(pos.x * boundSize.x, pos.y * boundSize.y, pos.z * boundSize.z) / 512f;
        Vector3 worldPos = O+pos;
        return worldPos;
    }
    
}
