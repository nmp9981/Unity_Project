using System.Collections.Generic;
using UnityEngine;
using Table;
using ScreenShot;
using System.Data.SqlTypes;
using System.Collections;
using System.Xml.Linq;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class PipelineObjectSearch : MonoBehaviour
{
    List<string> _uniquePipelineList;
    List<string> _validPipelineNameList;

    Dictionary<string, List<GameObject>> _validPipelineDic;
    List<GameObject> _objectList;

    List<GameObject> _activeObjList;
    private int _activeObjIndex;
    private bool isAll = false;

    private string _folderPath => $"{Application.dataPath}/ScreenShots11";

    public EScreenShotType[] types = new EScreenShotType[]
                            {
                    EScreenShotType.Left,
                    EScreenShotType.Top,
                    EScreenShotType.Right,
                    EScreenShotType.Bottom,
                    EScreenShotType.Back,
                    EScreenShotType.Front,
                            };

    public int ActiveObjIndex
    {
        get
        {
            return _activeObjIndex;
        }
        set
        {
            _activeObjIndex = value;
            if (_activeObjIndex < 0)
            {
                _activeObjIndex = 0;
            }
            else if (_validPipelineNameList.Count - 1 < _activeObjIndex)
            {
                _activeObjIndex = _validPipelineNameList.Count - 1;
            }
        }
    }
    private void Awake()
    {
        LineTable.Instance.LoadData("LineTable");
        PipingTable.Instance.LoadData("PipingTable");
        Debug.Log(PipingTable.Instance.DataList.Count);
    }

    void Start()
    {
        SetUniquePipeLineList(PipingTable.Instance.DataList);
        SetValidPipelineDic(_uniquePipelineList);
        _activeObjList = new List<GameObject>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ActiveObjIndex++;

           StartCoroutine(TakeScreenShot(_validPipelineNameList[_activeObjIndex]));
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ActiveObjIndex--;

           StartCoroutine(TakeScreenShot(_validPipelineNameList[_activeObjIndex]));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SetActiveAllObject();
        }
    }
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            isAll = true;
            StartCoroutine(TakeScreenShotAll());
        }
    }
    IEnumerator TakeScreenShot(string pipelineName)
    {
        _activeObjList.Clear();
        string fileName = $"{pipelineName}_";
        List<LineTableData> activeLineDataList = GetActiveLineList(pipelineName);

        if (activeLineDataList.Count > 0)
        {
            foreach (var item in activeLineDataList)
            {
                foreach (GameObject obj in _objectList)
                {
                    if (obj.name.Contains(pipelineName))
                    {
                        _activeObjList.Add(obj);
                    }
                }

                SetActiveObjectList(item);
                fileName = $"{pipelineName}_{item.FromEquip.Replace("\"", "")}_{item.FromNozzle.Replace("\"", "")}_{item.ToEquip.Replace("\"", "")}_{item.ToNozzle.Replace("\"", "")}";

                if (_activeObjList.Count > 0)
                {
                    SetActiveOnlyObject(_activeObjList);
                        int i = 0;
                        while (i < types.Length)
                        {
                            string path = $"{_folderPath}/{fileName}_{types[i].ToString()}_{DateTime.Now.ToString("MMdd_HHmmss")}.png";
                        
                        ScreenshotHandler.Instance.SetCameraPosition(types[i]);
                        ScreenCapture.CaptureScreenshot(path);
                        yield return null;
                        i++;
                        }
                    //ScreenshotHandler.Instance.TakeScreenShotAll(fileName);//이게 문제
                    _activeObjList.Clear();
                }
               
            }
        }
        else
        {
            foreach (GameObject obj in _objectList)
            {
                if (obj.name.Contains(pipelineName))
                {
                    _activeObjList.Add(obj);
                }
            }
            if (_activeObjList.Count > 0)
            {
                SetActiveOnlyObject(_activeObjList);
               
                int i = 0;
                while (i < types.Length)
                {
                    string path = $"{_folderPath}/{fileName}_{types[i].ToString()}_{DateTime.Now.ToString("MMdd_HHmmss")}.png";
                   
                    ScreenshotHandler.Instance.SetCameraPosition(types[i]);
                    ScreenCapture.CaptureScreenshot(path);
                    yield return null;
                    i++;
                }
            }
        }
    }

    IEnumerator TakeScreenShotAll()
    {
        for(int Idx = 0; Idx < _validPipelineNameList.Count; Idx++)
        {
            string pipelineName = _validPipelineNameList[Idx];
            _activeObjList.Clear();
            string fileName = $"{pipelineName}_";
            List<LineTableData> activeLineDataList = GetActiveLineList(pipelineName);

            if (activeLineDataList.Count > 0)
            {
                foreach (var item in activeLineDataList)
                {
                    foreach (GameObject obj in _objectList)
                    {
                        if (obj.name.Contains(pipelineName))
                        {
                            _activeObjList.Add(obj);
                        }
                    }

                    SetActiveObjectList(item);
                    fileName = $"{pipelineName}_{item.FromEquip.Replace("\"", "")}_{item.FromNozzle.Replace("\"", "")}_{item.ToEquip.Replace("\"", "")}_{item.ToNozzle.Replace("\"", "")}";

                    if (_activeObjList.Count > 0)
                    {
                        SetActiveOnlyObject(_activeObjList);
                        int i = 0;
                        while (i < types.Length)
                        {
                            string path = $"{_folderPath}/{fileName}_{types[i].ToString()}_{DateTime.Now.ToString("MMdd_HHmmss")}.png";

                            ScreenshotHandler.Instance.SetCameraPosition(types[i]);
                            ScreenCapture.CaptureScreenshot(path);
                            yield return new WaitForSeconds(1f);
                            yield return null;
                            i++;
                        }
                        //ScreenshotHandler.Instance.TakeScreenShotAll(fileName);//이게 문제
                        _activeObjList.Clear();
                    }

                }
            }
            else
            {
                foreach (GameObject obj in _objectList)
                {
                    if (obj.name.Contains(pipelineName))
                    {
                        _activeObjList.Add(obj);
                    }
                }
                if (_activeObjList.Count > 0)
                {
                    SetActiveOnlyObject(_activeObjList);

                    int i = 0;
                    while (i < types.Length)
                    {
                        string path = $"{_folderPath}/{fileName}_{types[i].ToString()}_{DateTime.Now.ToString("MMdd_HHmmss")}.png";

                        ScreenshotHandler.Instance.SetCameraPosition(types[i]);
                        ScreenCapture.CaptureScreenshot(path);
                        yield return null;
                        i++;
                    }
                }
            }
        }
    }

    // 중복안되는거 리스트에 저장
    private void SetUniquePipeLineList(List<PipingTableData> pipeDataList)
    {
        _uniquePipelineList = new List<string>();

        foreach (var pipe in pipeDataList)
        {
            string tmpPipeline = _uniquePipelineList.Find(o => o.Equals(pipe.Pipeline));
            if (tmpPipeline == null)
            {
                _uniquePipelineList.Add(pipe.Pipeline);
            }
        }
    }

    // 유효한 오브젝트만
    private void SetValidPipelineDic(List<string> pipelineNameList)
    {
        _objectList = new List<GameObject>();
        GameObject[] equipArray = GameObject.FindGameObjectsWithTag("Equipment");
        GameObject[] nozzleArray = GameObject.FindGameObjectsWithTag("Nozzle");

        foreach (var equip in equipArray)
        {
            _objectList.Add(equip);
        }
        
        foreach (var nozzle in nozzleArray)
        {
            _objectList.Add(nozzle);
        }
       
        _validPipelineNameList = new List<string>();
        _validPipelineDic = new Dictionary<string, List<GameObject>>();
       
        foreach (var item in pipelineNameList)//2234
        {
            List<GameObject> objList = new List<GameObject>();
            foreach (GameObject obj in _objectList)//1437
            {
                if (obj.name.Contains(item))
                {
                    objList.Add(obj);
                }
            }
            
            if (objList.Count > 0)
            {
                _validPipelineNameList.Add(item);
                _validPipelineDic.Add(item, objList);
            }
        }
        Debug.Log("@@@@" + _validPipelineNameList.Count);
    }

    // 파이프 라인에 해당하는 라인 데이터 리턴
    private List<LineTableData> GetActiveLineList(string pipelineName)
    {
        List<LineTableData> _activeLineDataList = new List<LineTableData>();

        foreach (var item in LineTable.Instance.DataList)
        {
            if (item.LineNo_1 == pipelineName)
            {
                _activeLineDataList.Add(item);
            }
        }

        return _activeLineDataList;
    }

    private void SetActiveObjectList(LineTableData lineData)
    {
        foreach (GameObject obj in _objectList)
        {
            if (obj.tag == "Equipment")
            {
                if (obj.name == lineData.FromEquip)
                {
                    _activeObjList.Add(obj);
                }
                else if (obj.name == lineData.ToEquip)
                {
                    _activeObjList.Add(obj);
                }
            }

            if (obj.tag == "Nozzle")
            {
                if (lineData.FromNozzle != "")
                {
                    if (obj.name.Contains(lineData.FromNozzle))
                    {
                        _activeObjList.Add(obj);
                    }
                }

                if (lineData.ToNozzle != "")
                {
                    if (obj.name.Contains(lineData.ToNozzle))
                    {
                        _activeObjList.Add(obj);
                    }
                }
            }
        }
    }

    private void SetActiveOnlyObject(List<GameObject> activeObjectList)
    {
        if (activeObjectList.Count <= 0)
        {
            return;
        }

        foreach (var item in _objectList)
        {
            item.SetActive(false);
        }

        foreach (var item in activeObjectList)
        {
            item.SetActive(true);
        }
    }

    private void SetActiveAllObject()
    {
        foreach (GameObject obj in _objectList)
        {
            obj.SetActive(true);
        }
    }
}
