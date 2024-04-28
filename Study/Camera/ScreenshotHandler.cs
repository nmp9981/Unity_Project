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
using UnityEngine.UIElements;
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
        PipeOff
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

            _uniquePipelineNameList = ObjectManager.Instance.GetUniquePipelineNameList(PipingTable.Instance.DataList);
        }
        #region 키 입력
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                StartCoroutine(TakeScreenShotAll("All Object"));
            }

            // 테이블 데이터를 토대로 장비랑 파이프라인 촬영
            if (Input.GetKeyDown(KeyCode.F2))
            {
                _validPipelineDic = ObjectManager.Instance.GetValidPipelineDic(_uniquePipelineNameList);

                StartCoroutine(TakeScreenShotsPipeline(_uniquePipelineNameList));
            }
            //전체 촬영
            if (Input.GetKeyDown(KeyCode.F3))
            {
                if (ActiveType == EActiveType.AllObjectPipeOff || ActiveType == EActiveType.AllObjectPipeOn)
                {
                    StartCoroutine(TakeScreenShotAllObject("All Object"));
                }else if(ActiveType == EActiveType.PipeOff || ActiveType == EActiveType.PipeOn)
                {
                    StartCoroutine(TakeScreenShotsAllObjectPipe(ObjectManager.Instance.PipelineList));
                }
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
                        meshRenders[i].sharedMaterial = NoShadowMaterial;
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

        public void SetCameraPosition(EDirectionType type)
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

            ScreenShotCamera.transform.position = TargetPosition.position + forward * CameraDistanceOffset;
            ScreenShotCamera.transform.rotation = Quaternion.LookRotation(-forward, upwards);
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

            float cameraDistanceOffset = Mathf.Max(bound.size.x, bound.size.y, bound.size.z);
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

                            SetActiveOnlyObject(activeObjectList, ActiveType);

                            string fileName = $"{pipelineName}_{lineData.FromEquip.Replace("\"", "")}_{lineData.FromNozzle.Replace("\"", "")}_{lineData.ToEquip.Replace("\"", "")}_{lineData.ToNozzle.Replace("\"", "")}";

                            yield return StartCoroutine(TakeScreenShotAll(fileName));
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
        public IEnumerator TakeScreenShotsAllObjectPipe(List<Pipeline> pipeList)
        {
            SetScreenShotMode(ScreenShotMode);
            SetCameraSettings(Color.white, CameraClearFlags.SolidColor);
            SetActiveFalsePipe(ActiveType);

            foreach (Pipeline pipeobj in pipeList)
            {
                if (ActiveType == EActiveType.PipeOff) pipeobj.gameObject.SetActive(true);

                string fileName = $"{pipeobj.name}";
                if (fileName.Contains("\"")) fileName = fileName.Replace("\"", "@");

                Bounds bound = default;
                bound = GetPipeBound(pipeobj.gameObject, bound);
                yield return StartCoroutine(TakeScreenShotAllOnObject(fileName,bound,pipeobj));

                if (ActiveType == EActiveType.PipeOff) pipeobj.gameObject.SetActive(false);
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
            for (int i = 0; i < _directionAllTypes.Length; i++)
            {
                string path = $"{_folderPath}/{fileName}_{_directionAllTypes[i].ToString()}_{DateTime.Now.ToString("MMdd_HHmmss")}.{_extName}";
                SetCameraPosition(_directionAllTypes[i]);
                yield return new WaitForEndOfFrame();
                CaptureScreenAndSave(path);
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
                SetCameraPosition(_directionAllTypes[i]);
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
            SetCameraPosition(type);
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
                Debug.Log($"Screen Shot Saved : {path}");
            }
        }
        #endregion
        private List<GameObject> GetActiveObjectList(LineTableData lineData)
        {
            List<GameObject> activeObjList = new List<GameObject>();

            foreach (GameObject obj in ObjectManager.Instance.ObjectList)
            {
                if (obj.tag == "Equipment")
                {
                    if (obj.name == lineData.FromEquip)
                    {
                        Equipment equipment = obj.GetComponent<Equipment>();
                        activeObjList.Add(equipment.gameObject);

                        if (equipment.NozzleList != null)
                        {
                            foreach (Nozzle nozzle in equipment.NozzleList)
                            {
                                if (nozzle.name.Contains(lineData.FromNozzle))
                                {
                                    activeObjList.Add(nozzle.gameObject);
                                }
                            }
                        }
                    }
                    else if (obj.name == lineData.ToEquip)
                    {
                        Equipment equipment = obj.GetComponent<Equipment>();
                        activeObjList.Add(equipment.gameObject);

                        if (equipment.NozzleList != null)
                        {
                            foreach (Nozzle nozzle in equipment.NozzleList)
                            {
                                if (nozzle.name.Contains(lineData.ToNozzle))
                                {
                                    activeObjList.Add(nozzle.gameObject);
                                }
                            }
                        }
                    }
                }
            }
            return activeObjList;
        }

        private void SetActiveOnlyObject(List<GameObject> activeObjectList, EActiveType activeType)
        {
            if (activeObjectList.Count <= 0)
            {
                return;
            }

            foreach (var item in ObjectManager.Instance.ObjectList)
            {
                item.SetActive(false);
            }

            foreach (var item in activeObjectList)
            {
                string tag = item.tag;

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

                    default:
                        item.SetActive(true);
                        break;
                }
            }
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
            if(activeType == EActiveType.AllObjectPipeOff || activeType == EActiveType.PipeOff)
            {
                foreach (Pipeline obj in ObjectManager.Instance.PipelineList)
                {
                    obj.gameObject.SetActive(false);
                }
            }
            
        }
    }
}


