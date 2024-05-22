using Cysharp.Threading.Tasks;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Table;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

namespace ScreenShot
{
    public enum EDirectionType
    {
        Right,
        Left,
        Top,
        Bottom,
        Front,
        Back
    }

    public enum EScreenShotType
    {
        Defalut,
        NoShadow,
        CameraLight,
    }

    public enum EActiveType
    {
        All,
        EquipmentOnly,
        PipelineOnly,
        AllObjectPipeOn,
        AllObjectPipeOff,
        PipeOn,
        PipeOff,
        PipeOnly,
        PipeEquipmentOnly,
        PipeEquipmentPairOffInAll,
        PipeEquipmentPairOnInAll
    }

    public class ScreenshotHandler : MonoBehaviour
    {
        public Transform TargetPosition;
        public Transform TargetRootObject;
        public Light DirectionalLight;
        public Camera MainCamera;
        public Camera ScreenShotCamera;
        public Canvas UICanvas;
        public float CameraSizeOffset = 40;
        public EScreenShotType ScreenShotMode;
        public EActiveType ActiveType;
        public Material NoShadowMaterial;
        public string FolderName = "SE_ScreenShots";

        private string _fileName = "ScreenShot_";
        private string _extName = "png";
        private float CameraDistanceOffset = 100;

        private string _rootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string _folderPath => $"{_rootPath}/{FolderName}";
        
        private Color _cameraBackOriginColor;
        private CameraClearFlags _cameraOriginClearFlag;
        private Vector3 _cameraOriginPosition;
        private Quaternion _cameraOriginRotation;
        private Material[] _originMaterialArray;

        private List<string> _uniquePipelineNameList;
        private Dictionary<string, List<Pipeline>> _validPipelineDic;

        EDirectionType[] _directionAllTypes = new EDirectionType[]
        {
            EDirectionType.Left,
            EDirectionType.Top,
            EDirectionType.Right,
            EDirectionType.Bottom,
            EDirectionType.Back,
            EDirectionType.Front,
        };


        private void Start()
        {
            _cameraBackOriginColor = ScreenShotCamera.backgroundColor;
            _cameraOriginClearFlag = ScreenShotCamera.clearFlags;
            _cameraOriginPosition = ScreenShotCamera.transform.position;
            _cameraOriginRotation = ScreenShotCamera.transform.rotation;

            //_uniquePipelineNameList = ObjectManager.Instance.GetUniquePipelineNameList(PipingTable.Instance.DataList);
            _uniquePipelineNameList = null;
            AllObjectCenter();
        }
        #region 키 입력
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                if(ActiveType == EActiveType.All)
                {
                    StartCoroutine(TakeScreenShotAll("All Object"));
                }
            }

            // 테이블 데이터를 토대로 장비랑 파이프라인 촬영
            if (Input.GetKeyDown(KeyCode.F2))
            {
                //_validPipelineDic = ObjectManager.Instance.GetValidPipelineDic(_uniquePipelineNameList);
                //StartCoroutine(TakeScreenShotsPipeline(_uniquePipelineNameList));
                if (ActiveType == EActiveType.PipeEquipmentOnly) ObjSetting(ObjectManager.Instance.pipeNozzleObj);
                StartCoroutine(TakeScreenShotsNewDataPipeline(ObjectManager.Instance.pipeNozzleObj));
            }
            //전체 촬영
            if (Input.GetKeyDown(KeyCode.F3))
            {
                if (ActiveType == EActiveType.AllObjectPipeOff || ActiveType == EActiveType.AllObjectPipeOn)
                {
                    StartCoroutine(TakeScreenShotAllObject("All Object"));
                }else if(ActiveType == EActiveType.PipeOff || ActiveType == EActiveType.PipeOn || ActiveType == EActiveType.PipeOnly)
                {
                    StartCoroutine(TakeScreenShotsAllObjectPipe(ObjectManager.Instance.PipelineList));
                }
            }
        }
        void ObjSetting(List<Transform> nozzlePipeList)
        {
            foreach (Transform nozzlechildList in nozzlePipeList)
            {
                nozzlechildList.gameObject.SetActive(false);
            }
        }
        #endregion
        #region 스샷 모드 세팅
        public void SetScreenShotMode(EScreenShotType screenShotMode)
        {
            MeshRenderer[] meshRenders = TargetRootObject.GetComponentsInChildren<MeshRenderer>();

            if (screenShotMode == EScreenShotType.NoShadow)
            {
                _originMaterialArray = new Material[meshRenders.Length];
            }

            switch (screenShotMode)
            {
                case EScreenShotType.Defalut:

                    if (_originMaterialArray != null)
                    {
                        for (int i = 0; i < meshRenders.Length; i++)
                        {
                            meshRenders[i].sharedMaterial = _originMaterialArray[i];
                        }
                    }

                    SetLight(false);
                    break;

                case EScreenShotType.NoShadow:

                    for (int i = 0; i < meshRenders.Length; i++)
                    {
                        Color color = meshRenders[i].sharedMaterial.color;

                        _originMaterialArray[i] = meshRenders[i].sharedMaterial;
                        //meshRenders[i].sharedMaterial = NoShadowMaterial;
                        meshRenders[i].sharedMaterial.color = color;
                    }
                    break;

                case EScreenShotType.CameraLight:
                    SetLight(true);
                    break;
            }
        }

        public void SetLight(bool isCameraLightMode)
        {
            DirectionalLight.gameObject.SetActive(!isCameraLightMode);
            ScreenShotCamera.GetComponent<Light>().enabled = isCameraLightMode;
        }
        #endregion

        public void SetCameraPosition(EDirectionType type, Bounds bound)
        {
            Vector3 forward = Vector3.zero;
            Vector3 upwards = Vector3.zero;

            switch (type)
            {
                case EDirectionType.Right:
                    forward = TargetPosition.right;
                    upwards = TargetPosition.up;
                    break;
                case EDirectionType.Left:
                    forward = -TargetPosition.right;
                    upwards = TargetPosition.up;
                    break;
                case EDirectionType.Top:
                    forward = TargetPosition.up;
                    upwards = TargetPosition.forward;
                    break;
                case EDirectionType.Bottom:
                    forward = -TargetPosition.up;
                    upwards = -TargetPosition.forward;
                    break;
                case EDirectionType.Front:
                    forward = TargetPosition.forward;
                    upwards = TargetPosition.up;
                    break;
                case EDirectionType.Back:
                    forward = -TargetPosition.forward;
                    upwards = TargetPosition.up;
                    break;
                default:
                    break;
            }

            _cameraOriginPosition = ScreenShotCamera.transform.position;
            _cameraOriginRotation = ScreenShotCamera.transform.rotation;

            ScreenShotCamera.transform.rotation = Quaternion.LookRotation(-forward, upwards);
            ScreenShotCamera.transform.position = TargetPosition.position + forward * CameraDistanceOffset;
            ScreenShotCamera.transform.position = TargetPosition.position + forward * 30;

            if (bound != default)
            {
                float cameraDistanceOffset = Mathf.Max(bound.size.x, bound.size.y, bound.size.z)*1.5f;
                ScreenShotCamera.orthographicSize = cameraDistanceOffset;
               
                switch (type)
                {
                    case EDirectionType.Right:
                        ScreenShotCamera.orthographicSize = bound.size.x;
                        cameraDistanceOffset = bound.size.x * 1.5f;
                        ScreenShotCamera.transform.position = bound.center + Vector3.right * cameraDistanceOffset;
                        break;
                    case EDirectionType.Left:
                        ScreenShotCamera.orthographicSize = bound.size.x;
                        cameraDistanceOffset = bound.size.x * 1.5f;
                        ScreenShotCamera.transform.position = bound.center + Vector3.left * cameraDistanceOffset;
                        break;
                    case EDirectionType.Top:
                        ScreenShotCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
                        ScreenShotCamera.orthographicSize = bound.size.y;
                        cameraDistanceOffset = bound.size.y*1.5f;
                        ScreenShotCamera.transform.position = bound.center + Vector3.up * cameraDistanceOffset;
                        break;
                    case EDirectionType.Bottom:
                        ScreenShotCamera.transform.rotation = Quaternion.Euler(-90, 0, 0);
                        ScreenShotCamera.orthographicSize = bound.size.y;
                        cameraDistanceOffset = bound.size.y*1.2f;
                        ScreenShotCamera.transform.position = bound.center + Vector3.down * cameraDistanceOffset;
                        break;
                    case EDirectionType.Front:
                        ScreenShotCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
                        ScreenShotCamera.orthographicSize = bound.size.z;
                        cameraDistanceOffset = bound.size.z * 1.5f;
                        ScreenShotCamera.transform.position = bound.center - Vector3.forward * cameraDistanceOffset;
                        break;
                    case EDirectionType.Back:
                        ScreenShotCamera.transform.rotation = Quaternion.Euler(0, 180, 0);
                        ScreenShotCamera.orthographicSize = bound.size.z;
                        cameraDistanceOffset = bound.size.z * 1.3f;
                        ScreenShotCamera.transform.position = bound.center - Vector3.back * cameraDistanceOffset;
                        break;
                    default:
                        break;
                }
                if (ScreenShotCamera.orthographicSize <= 1f) ScreenShotCamera.orthographicSize *= 2;
                Debug.Log(type+" " + ScreenShotCamera.transform.rotation.x+ " " + ScreenShotCamera.transform.rotation.y+" " + ScreenShotCamera.transform.rotation.z + " "+ScreenShotCamera.transform.position + "  " + ScreenShotCamera.orthographicSize);
                if (ActiveType == EActiveType.PipeEquipmentOnly || ActiveType == EActiveType.PipeEquipmentPairOffInAll || ActiveType == EActiveType.PipeEquipmentPairOnInAll)
                {
                   
                }
            }
        }
        public void SetCameraAllObjectPosition(EDirectionType type)
        {
            CameraDistanceOffset = 300;
            switch (type)
            {
                case EDirectionType.Right:
                    CameraDistanceOffset = 800;
                    ScreenShotCamera.orthographicSize = CameraSizeOffset;
                    ScreenShotCamera.transform.rotation = Quaternion.Euler(0, 90, 0);
                    ScreenShotCamera.transform.position = TargetPosition.transform.position - Vector3.right * CameraDistanceOffset;
                    Debug.Log(ScreenShotCamera.transform.position + " 타겟 포지션 " + type);
                    Debug.Log(ScreenShotCamera.transform.rotation.x + " " + ScreenShotCamera.transform.rotation.y + " " + ScreenShotCamera.transform.rotation.z);
                    break;
                case EDirectionType.Left:
                    CameraDistanceOffset = 800;
                    ScreenShotCamera.orthographicSize = CameraSizeOffset;
                    ScreenShotCamera.transform.rotation = Quaternion.Euler(0, -90, 0);
                    ScreenShotCamera.transform.position = TargetPosition.transform.position - Vector3.left * CameraDistanceOffset;
                    Debug.Log(ScreenShotCamera.transform.position + " 타겟 포지션 " + type);
                    Debug.Log(ScreenShotCamera.transform.rotation.x + " " + ScreenShotCamera.transform.rotation.y + " " + ScreenShotCamera.transform.rotation.z);
                    break;
                case EDirectionType.Top:
                    ScreenShotCamera.orthographicSize = 130;
                    ScreenShotCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
                    ScreenShotCamera.transform.position = TargetPosition.transform.position + Vector3.up * CameraDistanceOffset;
                    Debug.Log(ScreenShotCamera.transform.position + " 타겟 포지션 " + type);
                    Debug.Log(ScreenShotCamera.transform.rotation.x + " " + ScreenShotCamera.transform.rotation.y + " " + ScreenShotCamera.transform.rotation.z);
                    break;
                case EDirectionType.Bottom:
                    ScreenShotCamera.orthographicSize = 130;
                    ScreenShotCamera.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    ScreenShotCamera.transform.position = TargetPosition.transform.position + Vector3.down * CameraDistanceOffset;
                    Debug.Log(ScreenShotCamera.transform.position + " 타겟 포지션 " + type);
                    Debug.Log(ScreenShotCamera.transform.rotation.x + " " + ScreenShotCamera.transform.rotation.y + " " + ScreenShotCamera.transform.rotation.z);
                    break;
                case EDirectionType.Front:
                    ScreenShotCamera.orthographicSize = CameraSizeOffset;
                    ScreenShotCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
                    ScreenShotCamera.transform.position = TargetPosition.transform.position - Vector3.forward * CameraDistanceOffset;
                    Debug.Log(ScreenShotCamera.transform.position + " 타겟 포지션 " + type);
                    Debug.Log(ScreenShotCamera.transform.rotation.x + " " + ScreenShotCamera.transform.rotation.y + " " + ScreenShotCamera.transform.rotation.z);
                    break;
                case EDirectionType.Back:
                    ScreenShotCamera.orthographicSize = CameraSizeOffset;
                    ScreenShotCamera.transform.rotation = Quaternion.Euler(0, 180, 0);
                    ScreenShotCamera.transform.position = TargetPosition.transform.position - Vector3.back * CameraDistanceOffset;
                    Debug.Log(ScreenShotCamera.transform.position + " 타겟 포지션 " + type);
                    Debug.Log(ScreenShotCamera.transform.rotation.x + " " + ScreenShotCamera.transform.rotation.y + " " + ScreenShotCamera.transform.rotation.z);
                    break;
                default:
                    break;
            }
        }
        //bounds 사용
        public void SetCameraPositionOnObject(EDirectionType type, Bounds bound, Pipeline pipeobj)
        {
            Vector3 forward = Vector3.zero;
            Vector3 upwards = Vector3.zero;

            switch (type)
            {
                case EDirectionType.Right:
                    forward = pipeobj.transform.right;
                    upwards = pipeobj.transform.up;
                    break;
                case EDirectionType.Left:
                    forward = -pipeobj.transform.right;
                    upwards = pipeobj.transform.up;
                    break;
                case EDirectionType.Top:
                    forward = pipeobj.transform.up;
                    upwards = pipeobj.transform.forward;
                    break;
                case EDirectionType.Bottom:
                    forward = -pipeobj.transform.up;
                    upwards = -pipeobj.transform.forward;
                    break;
                case EDirectionType.Front:
                    forward = pipeobj.transform.forward;
                    upwards = pipeobj.transform.up;
                    break;
                case EDirectionType.Back:
                    forward = -pipeobj.transform.forward;
                    upwards = pipeobj.transform.up;
                    break;
                default:
                    break;
            }

            _cameraOriginPosition = ScreenShotCamera.transform.position;
            _cameraOriginRotation = ScreenShotCamera.transform.rotation;

            float cameraDistanceOffset = 3*Mathf.Max(bound.size.x, bound.size.y, bound.size.z);
            ScreenShotCamera.orthographicSize = cameraDistanceOffset;
            ScreenShotCamera.transform.position = bound.center + forward * cameraDistanceOffset;
            ScreenShotCamera.transform.rotation = Quaternion.LookRotation(-forward, upwards);
        }
        #region 카메라 세팅
        public void SetCameraSettings(Color backgroundColor, CameraClearFlags clearFlag)
        {
            MainCamera.enabled = false;
            UICanvas.enabled = false;

            ScreenShotCamera.gameObject.SetActive(true);
            ScreenShotCamera.clearFlags = clearFlag;
            ScreenShotCamera.backgroundColor = backgroundColor;
            ScreenShotCamera.orthographic = true;
            ScreenShotCamera.orthographicSize = CameraSizeOffset;
        }

        public void ResetCameraSettings()
        {
            ScreenShotCamera.clearFlags = _cameraOriginClearFlag;
            ScreenShotCamera.backgroundColor = _cameraBackOriginColor;
            ScreenShotCamera.orthographic = false;
            ScreenShotCamera.transform.position = _cameraOriginPosition;
            ScreenShotCamera.transform.rotation = _cameraOriginRotation;
            ScreenShotCamera.gameObject.SetActive(false);

            MainCamera.enabled = true;
            UICanvas.enabled = true;
        }
        #endregion
        #region 파이프 장비 쌍 전체 촬영
        public IEnumerator TakeScreenShotsPipeline(List<string> uniquePipelineNameList)
        {
            SetScreenShotMode(ScreenShotMode);
            SetCameraSettings(Color.white, CameraClearFlags.SolidColor);

            foreach (string pipelineName in uniquePipelineNameList)
            {
                if (_validPipelineDic.ContainsKey(pipelineName))
                {
                    List<LineTableData> lineDataList = LineTable.Instance.GetLineDataList(pipelineName);
                    List<Pipeline> pipelineList = _validPipelineDic[pipelineName];

                    foreach (LineTableData lineData in lineDataList)
                    {
                        List<GameObject> activeObjectList = GetActiveObjectList(lineData);

                        if (activeObjectList.Count > 0)
                        {
                            for (int i = 0; i < pipelineList.Count; i++)
                            {
                                activeObjectList.Add(pipelineList[i].gameObject);
                            }

                            Bounds newBound = default;
                            newBound = SetActiveOnlyObject(activeObjectList, ActiveType);

                            string fileName = $"{pipelineName}_{lineData.FromEquip.Replace("\"", "")}_{lineData.FromNozzle.Replace("\"", "")}_{lineData.ToEquip.Replace("\"", "")}_{lineData.ToNozzle.Replace("\"", "")}";

                            yield return StartCoroutine(TakeScreenShotPipeEquipmentPair(fileName, pipelineName, newBound));
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            ResetCameraSettings();
            SetScreenShotMode(EScreenShotType.Defalut);
            SetActiveAllObject();
        }
        #endregion
        public IEnumerator TakeScreenShotsNewDataPipeline(List<Transform> nozzlePipeList)
        {
            SetScreenShotMode(ScreenShotMode);
            SetCameraSettings(Color.white, CameraClearFlags.SolidColor);

            foreach (Transform nozzlechildList in nozzlePipeList)
            {
                Bounds newBound1 = default;
                Vector3 newBound1Center = Vector3.zero;
                Vector3 newBound1Size = Vector3.zero;
                int newBound1Count = 0;

                Bounds newBound2 = default;
                Vector3 newBound2Center = Vector3.zero;
                Vector3 newBound2Size = Vector3.zero;
                int newBound2Count = 0;

                if (ActiveType == EActiveType.PipeEquipmentOnly) nozzlechildList.gameObject.SetActive(true);
                foreach (Transform child in nozzlechildList.GetComponentsInChildren<Transform>())//촬영 대상
                {
                    
                    if (child.gameObject.tag == "PipeNozzle")
                    {
                        if (child.GetComponent<MeshRenderer>() != null)
                        {
                            Bounds newSubBound = child.GetComponent<MeshRenderer>().bounds;
                            
                            newBound1Center += newSubBound.center;
                            newBound1Size += newSubBound.size;
                            newBound1Count++;
                        }
                    }
                    else if (child.gameObject.tag == "PipeNozzle2")
                    {
                        if (child.GetComponent<MeshRenderer>() != null)
                        {
                            Bounds newSubBound = child.GetComponent<MeshRenderer>().bounds;
                          
                            newBound2Center += newSubBound.center;
                            newBound2Size += newSubBound.size;
                            newBound2Count++;
                        }
                    }
                }
                if (newBound1Count >= 1)
                {
                    newBound1.center = newBound1Center / newBound1Count;
                    newBound1.size = newBound1Size;
                }
                if (newBound2Count >= 1)
                {
                    newBound2.center = newBound2Center / newBound2Count;
                    newBound2.size = newBound2Size;
                }
                string fileName = $"{nozzlechildList.name.Replace("\"", "")}";
                Debug.Log(nozzlechildList.name);
                yield return StartCoroutine(TakeScreenShotPipeEquipmentPair2(fileName, newBound1,newBound2));

                if (ActiveType == EActiveType.PipeEquipmentOnly) nozzlechildList.gameObject.SetActive(false);
            }
            ResetCameraSettings();
            SetScreenShotMode(EScreenShotType.Defalut);
            SetActiveAllObject();
        }
        public IEnumerator TakeScreenShotsAllObjectPipe(List<Pipeline> pipeList)
        {
            SetScreenShotMode(ScreenShotMode);
            SetCameraSettings(Color.white, CameraClearFlags.SolidColor);
            SetActiveFalsePipe(ActiveType);

            foreach (Pipeline pipeobj in pipeList)
            {
                if (ActiveType == EActiveType.PipeOff || ActiveType == EActiveType.PipeOnly) pipeobj.gameObject.SetActive(true);

                string fileName = $"{pipeobj.name}";
                if (fileName.Contains("\"")) fileName = fileName.Replace("\"", "@");

                Bounds bound = default;
                bound = GetPipeBound(pipeobj.gameObject, bound);
                yield return StartCoroutine(TakeScreenShotAllOnObject(fileName,bound,pipeobj));

                if (ActiveType == EActiveType.PipeOnly) pipeobj.gameObject.SetActive(false);
            }
            ResetCameraSettings();
            SetScreenShotMode(EScreenShotType.Defalut);
            SetActiveAllObject();
        }
        public Bounds GetPipeBound(GameObject gm, Bounds subBound)
        {
            Transform[] children = gm.GetComponentsInChildren<Transform>();
            foreach(Transform child in children)
            {
                MeshRenderer meshRender = child.GetComponent<MeshRenderer>();
                if (meshRender != null)
                {
                    if (subBound == default) subBound = meshRender.bounds;
                    else subBound.Encapsulate(meshRender.bounds);
                }
            }
            return subBound;
        }
        #region 모든 방향 촬영
        // 모든 방향(6면) 촬영
        IEnumerator TakeScreenShotAll(string fileName)
        {
            SetScreenShotMode(ScreenShotMode);
            SetCameraSettings(Color.white, CameraClearFlags.SolidColor);

            for (int i = 0; i < _directionAllTypes.Length; i++)
            { 
                string path = $"{_folderPath}/{fileName}_{_directionAllTypes[i].ToString()}_{DateTime.Now.ToString("MMdd_HHmmss")}.{_extName}";
                SetCameraAllObjectPosition(_directionAllTypes[i]);
                yield return new WaitForEndOfFrame();
                CaptureScreenAndSave(path);
            }

            ResetCameraSettings();
            SetScreenShotMode(EScreenShotType.Defalut);
        }
        //장비 - 파이프 촬영
        IEnumerator TakeScreenShotPipeEquipmentPair(string fileName, string obj, Bounds bound)
        {
            for (int i = 0; i < _directionAllTypes.Length; i++)
            {
                if (!Directory.Exists($"{_folderPath}/{obj}")) Directory.CreateDirectory($"{_folderPath}/{obj}");
                string path = $"{_folderPath}/{obj}/{fileName}_{_directionAllTypes[i].ToString()}_{DateTime.Now.ToString("MMdd_HHmmss")}.{_extName}";
                SetCameraPosition(_directionAllTypes[i],bound);
                yield return new WaitForEndOfFrame();
                CaptureScreenAndSave(path);
            }
        }
        IEnumerator TakeScreenShotPipeEquipmentPair2(string fileName, Bounds bound1,Bounds bound2)
        {
            for(int k = 0; k < 2; k++)
            {
                string subname = k == 0 ? "first" : "second";
                if (k == 1 && bound2 == default) break;

                for (int i = 0; i < _directionAllTypes.Length; i++)
                {
                    if (!Directory.Exists($"{_folderPath}/{fileName}")) Directory.CreateDirectory($"{_folderPath}/{fileName}");
                    string path = $"{_folderPath}/{fileName}/{subname}_{_directionAllTypes[i].ToString()}_{DateTime.Now.ToString("MMdd_HHmmss")}.{_extName}";

                    if(k==0 && bound1 != default) SetCameraPosition(_directionAllTypes[i], bound1);
                    else if(k==1 && bound2 != default) SetCameraPosition(_directionAllTypes[i], bound2);
                    yield return new WaitForEndOfFrame();
                    CaptureScreenAndSave(path);
                }
            }
        }
        IEnumerator TakeScreenShotAllOnObject(string fileName, Bounds bound,Pipeline pipeobj)
        {
            for (int i = 0; i < _directionAllTypes.Length; i++)
            {
                string subPath = $"{_folderPath}/{fileName}";
                if (!Directory.Exists(subPath)) Directory.CreateDirectory(subPath);

                string path = $"{subPath}/{fileName}_{_directionAllTypes[i].ToString()}_{DateTime.Now.ToString("MMdd_HHmmss")}.{_extName}";
                SetCameraPositionOnObject(_directionAllTypes[i], bound, pipeobj);
                if (ActiveType == EActiveType.PipeOff) pipeobj.gameObject.SetActive(false);
                yield return new WaitForEndOfFrame();
                CaptureScreenAndSave(path);
            }
        }
        #endregion
        #region 전체 오브젝트 촬영
        IEnumerator TakeScreenShotAllObject(string fileName)
        {
            SetScreenShotMode(ScreenShotMode);
            SetCameraSettings(Color.white, CameraClearFlags.SolidColor);
            SetActiveFalsePipe(ActiveType);

            for (int i = 0; i < _directionAllTypes.Length; i++)
            {
                string path = $"{_folderPath}/{fileName}_{_directionAllTypes[i].ToString()}_{DateTime.Now.ToString("MMdd_HHmmss")}.{_extName}";
                SetCameraPosition(_directionAllTypes[i],default);
                yield return new WaitForEndOfFrame();
                CaptureScreenAndSave(path);
            }
            ResetCameraSettings();
            SetScreenShotMode(EScreenShotType.Defalut);
            SetActiveAllObject();
        }
        #endregion
        #region 지정 방향 촬영
        public IEnumerator TakeScreenShot(EDirectionType type, string fileName)
        {
            string path = $"{_folderPath}/{fileName + type.ToString()}_{DateTime.Now.ToString("MMdd_HHmmss")}.{_extName}";
            SetCameraSettings(Color.white, CameraClearFlags.SolidColor);
            SetCameraPosition(type,default);
            yield return new WaitForEndOfFrame();
            Task.Run(() => CaptureScreenAndSave(path));
            ResetCameraSettings();
        }
        #endregion
        #region 스샷 캡처 및 저장
        private void CaptureScreenAndSave(string path)
        {
            Texture2D screenTex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            Rect area = new Rect(0f, 0f, Screen.width, Screen.height);

            screenTex.ReadPixels(area, 0, 0);

            bool succeeded = true;
            try
            {
                if (Directory.Exists(_folderPath) == false)
                {
                    Directory.CreateDirectory(_folderPath);
                }

                File.WriteAllBytes(path, screenTex.EncodeToPNG());
            }
            catch (Exception e)
            {
                succeeded = false;
                Debug.LogWarning($"Screen Shot Save Failed : {path}");
                Debug.LogWarning(e);
            }

            Destroy(screenTex);

            if (succeeded)
            {
                //Debug.Log($"Screen Shot Saved : {path}");
            }
        }
        #endregion
        private List<GameObject> GetActiveObjectList(LineTableData lineData)
        {
            List<GameObject> activeObjList = new List<GameObject>();

            bool hasFrom = false;
            bool hasTo = false;

            foreach (GameObject obj in ObjectManager.Instance.ObjectList)
            {
                if (obj.tag == "Equipment")
                {
                    if (obj.name.Contains(lineData.FromEquip))
                    {
                        hasFrom = true;
                        Equipment equipment = obj.GetComponent<Equipment>();
                        activeObjList.Add(equipment.gameObject);

                        if (equipment.NozzleList != null)
                        {
                            foreach (Nozzle nozzle in equipment.NozzleList)
                            {
                                // 노즐 다 띄우기로 요청해서 주석처리
                                //if (nozzle.name.Contains(lineData.FromNozzle))
                                //{
                                    activeObjList.Add(nozzle.gameObject);
                                //}
                            }
                        }
                    }
                    else if (obj.name.Contains(lineData.ToEquip))
                    {
                        hasTo = true;
                        Equipment equipment = obj.GetComponent<Equipment>();
                        activeObjList.Add(equipment.gameObject);

                        if (equipment.NozzleList != null)
                        {
                            foreach (Nozzle nozzle in equipment.NozzleList)
                            {
                                //if (nozzle.name.Contains(lineData.ToNozzle))
                                //{
                                    activeObjList.Add(nozzle.gameObject);
                                //}
                            }
                        }
                    }
                }
            }
            if (hasTo && hasFrom)
            {
                return activeObjList;
            }
            else
            {
                activeObjList.Clear();
                return activeObjList;
            }


        }

        private Bounds SetActiveOnlyObject(List<GameObject> activeObjectList, EActiveType activeType)
        {
            Bounds bound = default;
            Vector3 boundSize = Vector3.zero;
            Vector3 boundPos = Vector3.zero;
            int subBoundCount = 0;
            int subPipeCount = 0;

            if (activeObjectList.Count <= 0)
            {
                return bound;
            }

            foreach (var item in ObjectManager.Instance.ObjectList)
            {
                if (activeType == EActiveType.PipeEquipmentOnly) item.SetActive(false);
                else item.SetActive(true);
            }

            foreach (var item in activeObjectList)
            {
                string tag = item.tag;
                
                if (tag == "Pipeline" || tag == "Equipment" )
                {
                    if (tag == "Pipeline") subPipeCount++;
                    Transform[] children = item.GetComponentsInChildren<Transform>();
                    foreach (Transform child in children)
                    {
                        if (child.GetComponent<MeshRenderer>() != null)
                        {
                            Bounds newBound = child.GetComponent<MeshRenderer>().bounds;
                            bound.Encapsulate(newBound);
                            boundPos += newBound.center;
                            if (tag == "Pipeline") boundSize += newBound.size;
                            subBoundCount++;
                        }
                    }
                }
                switch (activeType)
                {
                    case EActiveType.EquipmentOnly:
                        if (tag == "Equipment" || tag == "Nozzle")
                        {
                            item.SetActive(true);
                        }
                        break;

                    case EActiveType.PipelineOnly:
                        if (tag == "Pipeline")
                        {
                            item.SetActive(true);
                        }
                        break;
                    case EActiveType.PipeEquipmentOnly:
                        if (tag == "Equipment" || tag == "Pipeline" || tag == "Nozzle")
                        {
                            item.SetActive(true);
                        }
                        break;
                    case EActiveType.PipeEquipmentPairOnInAll:
                        item.SetActive(true);
                        break;
                    case EActiveType.PipeEquipmentPairOffInAll:
                        if (tag == "Pipeline")
                        {
                            item.SetActive(false);
                        }
                        break;
                    default:
                        item.SetActive(true);
                        break;
                }
                
            }
            bound.size = boundSize / subPipeCount;
            bound.center = boundPos / subBoundCount;
            return bound;
        }
       
        private void SetActiveAllObject()
        {
            foreach (GameObject obj in ObjectManager.Instance.ObjectList)
            {
                obj.SetActive(true);
            }
        }
        private void SetActiveFalsePipe(EActiveType activeType)
        {
            if(activeType == EActiveType.AllObjectPipeOff || activeType == EActiveType.PipeOff || activeType == EActiveType.PipeOnly)
            {
                foreach (Pipeline obj in ObjectManager.Instance.PipelineList)
                {
                    obj.gameObject.SetActive(false);
                }
            }
            
        }
        public void AllObjectCenter()
        {
            Transform[] children = TargetRootObject.GetComponentsInChildren<Transform>();
            Vector3 center = Vector3.zero;
            int subBoundCount = 0;
            foreach (Transform child in children)
            {
                if (child.GetComponent<MeshRenderer>() != null)
                {
                    Bounds newBound = child.GetComponent<MeshRenderer>().bounds;
                    center += newBound.center;
                    subBoundCount++;
                }
            }
            center = center / subBoundCount;
            Debug.Log(center);
        }
    }
}


