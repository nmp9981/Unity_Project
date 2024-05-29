using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UIElements;

namespace ScreenShotMain
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
        EquipPipeOnly,
        EquipPipeOn,
        EquipPipeOff
    }
    public class ScreenShotMain : MonoBehaviour
    {
        CSVReader CSVReader;//csv파일 읽기

        public Transform TargetPosition;
        public Transform TargetRootObject;
        public Transform TargetObject;
        public Light DirectionalLight;
        public Camera MainCamera;
        public Camera ScreenShotCamera;
        public Canvas UICanvas;

        public float CameraSizeOffset = 40;
        public float distRate;
        public float zoomRate;
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
        private Vector3 centerPosition;

        private List<Equipment> activeEquipment = new List<Equipment>();
        private List<Nozzle> activeNozzle = new List<Nozzle>();
        private List<string> _uniquePipelineNameList;
        private Dictionary<string, List<Pipeline>> _validPipelineDic;
        List<Dictionary<string, object>> data;

        EDirectionType[] _directionAllTypes = new EDirectionType[]
        {
            EDirectionType.Left,
            EDirectionType.Top,
            EDirectionType.Right,
            EDirectionType.Bottom,
            EDirectionType.Back,
            EDirectionType.Front,
        };

        void Start()
        {
            data = CSVReader.Read("List_240517");
       
            Debug.Log(data[3]["From"].ToString());
            Debug.Log(data[4]["From"].ToString());
            CameraSettingInit();
            AllObjectCenter();
            SetActiveFalsePipe(ActiveType);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))//전체
            {

            }
            if (Input.GetKeyDown(KeyCode.F2))//장비 - 파이프
            {
                StartCoroutine(TakeScreenShotPipeline(ObjectManager.Instance.PipelineList));
            }
        }
        #region 카메라 초기 세팅
        void CameraSettingInit()
        {
            _cameraBackOriginColor = ScreenShotCamera.backgroundColor;
            _cameraOriginClearFlag = ScreenShotCamera.clearFlags;
            _cameraOriginPosition = ScreenShotCamera.transform.position;
            _cameraOriginRotation = ScreenShotCamera.transform.rotation;
        }
        #endregion
        #region 전체 센터 잡기
        public void AllObjectCenter()
        {
            MeshRenderer[] children = TargetRootObject.GetComponentsInChildren<MeshRenderer>();
            Vector3 center = Vector3.zero;
            int subBoundCount = 0;
            foreach (MeshRenderer child in children)
            {
                Bounds newBound = child.GetComponent<MeshRenderer>().bounds;
                center += newBound.center;
                subBoundCount++;
            }
            centerPosition = center / subBoundCount;
        }
        #endregion
        public IEnumerator TakeScreenShotPipeline(List<Pipeline> pipeList)
        {
            SetScreenShotMode(ScreenShotMode);
            SetCameraSettings(Color.white, CameraClearFlags.SolidColor);

            foreach(var childList in pipeList)
            {
                if (ActiveType == EActiveType.EquipPipeOnly || ActiveType == EActiveType.EquipPipeOff) SetActiveOnPipe(childList.gameObject);

                Bounds newBound = GetBound(childList.gameObject);
                if (ActiveType == EActiveType.EquipPipeOnly) NearObjectActiveTrue(newBound);
                Debug.Log(childList.gameObject.name);
                string fileName = $"{childList.name.Replace("\"", "")}";
                yield return StartCoroutine(TakeScreenShotPipeEquipmentPair2(fileName, newBound));

                if (ActiveType == EActiveType.EquipPipeOnly) NearObjectActiveFalse();
                if (ActiveType == EActiveType.EquipPipeOnly || ActiveType == EActiveType.EquipPipeOff) SetActiveOffPipe(childList.gameObject);
            }
            ResetCameraSettings();
            SetScreenShotMode(EScreenShotType.Defalut);
            SetActiveAllObject();
        }
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
        private void SetActiveAllObject()
        {
            foreach (var obj in ObjectManager.Instance.PipelineList)
            {
                obj.gameObject.SetActive(true);
            }
        }
        private void SetActiveFalsePipe(EActiveType activeType)
        {
            if (activeType == EActiveType.EquipPipeOnly)
            {
                foreach (Transform obj in TargetRootObject.GetComponentsInChildren<Transform>())
                {
                    obj.gameObject.SetActive(false);
                }
            }
            else
            {
                foreach(var obj in ObjectManager.Instance.EquipmentList)
                {
                    obj.gameObject.SetActive(true);
                    foreach(var nozzle in obj.NozzleList)
                    {
                        nozzle.gameObject.SetActive(true);

                        foreach(var nozzleChild in nozzle.GetComponentsInChildren<MeshRenderer>())
                        {
                            nozzleChild.material.color = Color.yellow;
                        }
                    }
                }
            }
        }
        private void SetActiveOnPipe(GameObject pipeobj)
        {
            Transform currentObj = pipeobj.transform;
            while(currentObj.gameObject.name!= "3D Model Sample_240503")
            {
                currentObj = currentObj.transform.parent;
                currentObj.gameObject.SetActive(true);
            }
            foreach (Transform obj in pipeobj.GetComponentsInChildren<Transform>(true))
            {
                obj.gameObject.SetActive(true);
            }
        }
        private void SetActiveOffPipe(GameObject pipeobj)
        {
            foreach (Transform obj in pipeobj.GetComponentInChildren<Transform>(true))
            {
                obj.gameObject.SetActive(false);
            }
        }
        #endregion
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
        void NearObjectActiveTrue(Bounds newBound)
        {
            float newBoundDiagonal = Mathf.Sqrt(newBound.size.x * newBound.size.x + newBound.size.y * newBound.size.y + newBound.size.z * newBound.size.z)/2;
            foreach (var obj in ObjectManager.Instance.EquipmentList)
            {
                Bounds equipBound = GetBound(obj.gameObject);
                float equipBoundDiagonal = Mathf.Sqrt(equipBound.size.x * equipBound.size.x + equipBound.size.y * equipBound.size.y + equipBound.size.z * equipBound.size.z) / 2;
                if (Vector3.Distance(newBound.center, equipBound.center) < newBoundDiagonal+equipBoundDiagonal)//bound 근처
                {
                    activeEquipment.Add(obj);

                    Transform currentObj = obj.transform;
                    while (currentObj.gameObject.name != "3D Model Sample_240503")
                    {
                        currentObj = currentObj.transform.parent;
                        currentObj.gameObject.SetActive(true);
                    }
                    foreach (Transform child in obj.GetComponentsInChildren<Transform>(true))
                    {
                        child.gameObject.SetActive(true);//노즐 포함
                        Nozzle childNozzle = child.GetComponent<Nozzle>();
                        if (childNozzle != null) activeNozzle.Add(childNozzle);
                    }
                }
                foreach (Nozzle nozzleObj in obj.NozzleList)//노즐
                {
                    nozzleObj.gameObject.SetActive(true);
                    activeNozzle.Add(nozzleObj);

                    Transform currentObj = nozzleObj.transform;
                    while (currentObj.gameObject.name != "3D Model Sample_240503")
                    {
                        currentObj = currentObj.transform.parent;
                        currentObj.gameObject.SetActive(true);
                    }
                }
                
            }
        }
        void NearObjectActiveFalse()
        {
            foreach(var obj in activeEquipment)
            {
                obj.gameObject.SetActive(false);
            }
            foreach (var obj in activeNozzle)
            {
                obj.gameObject.SetActive(false);
            }
            activeEquipment.Clear();
            activeNozzle.Clear();
        }
        IEnumerator TakeScreenShotPipeEquipmentPair2(string fileName, Bounds bound)
        {
            for (int i = 0; i < _directionAllTypes.Length; i++)
            {
                if (!Directory.Exists($"{_folderPath}/{fileName}")) Directory.CreateDirectory($"{_folderPath}/{fileName}");
                string path = $"{_folderPath}/{fileName}/{_directionAllTypes[i].ToString()}_{DateTime.Now.ToString("MMdd_HHmmss")}.{_extName}";

                SetCameraPosition(_directionAllTypes[i], bound);
                yield return new WaitForEndOfFrame();
                CaptureScreenAndSave(path);
            }
        }
        Bounds GetBound(GameObject gm)
        {
            Bounds newBound = default;
            Vector3 newBoundCenter = Vector3.zero;
            int newBoundCount = 0;

            float minX = 10000;
            float minY = 10000;
            float minZ = 10000;
            float maxX = 0;
            float maxY = 0;
            float maxZ = 0;

            foreach (MeshRenderer child in gm.GetComponentsInChildren<MeshRenderer>(true))//촬영 대상
            {
                Bounds newSubBound = child.GetComponent<MeshRenderer>().bounds;

                newBoundCenter += newSubBound.center;
                newBoundCount++;

                if (minX > newSubBound.center.x - newSubBound.size.x / 2) minX = newSubBound.center.x - newSubBound.size.x / 2;
                if (minY > newSubBound.center.y - newSubBound.size.y / 2) minY = newSubBound.center.y - newSubBound.size.y / 2;
                if (minZ > newSubBound.center.z - newSubBound.size.z / 2) minZ = newSubBound.center.z - newSubBound.size.z / 2;
                if (maxX < newSubBound.center.x + newSubBound.size.x / 2) maxX = newSubBound.center.x + newSubBound.size.x / 2;
                if (maxY < newSubBound.center.y + newSubBound.size.y / 2) maxY = newSubBound.center.y + newSubBound.size.y / 2;
                if (maxZ < newSubBound.center.z + newSubBound.size.z / 2) maxZ = newSubBound.center.z + newSubBound.size.z / 2;
            }
            if (newBoundCount >= 1)
            {
                newBound.center = newBoundCenter / newBoundCount;
                newBound.size = new Vector3(maxX, maxY, maxZ) - new Vector3(minX, minY, minZ);
            }
            return newBound;
        }
        public void SetCameraPosition(EDirectionType type, Bounds bound)
        {
            /*
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
            */
            if (bound != default)
            {
                float cameraDistanceOffset = Mathf.Max(bound.size.x, bound.size.y, bound.size.z) * 1.5f;
                ScreenShotCamera.orthographicSize = cameraDistanceOffset;

                switch (type)
                {
                    case EDirectionType.Right:
                        ScreenShotCamera.transform.rotation = Quaternion.Euler(0, -90, 0);
                        ScreenShotCamera.orthographicSize = Mathf.Max(bound.size.y, bound.size.z)*zoomRate;
                        cameraDistanceOffset = bound.size.x * distRate;
                        ScreenShotCamera.transform.position = bound.center + Vector3.right * cameraDistanceOffset;
                        break;
                    case EDirectionType.Left:
                        ScreenShotCamera.transform.rotation = Quaternion.Euler(0, 90, 0);
                        ScreenShotCamera.orthographicSize = Mathf.Max(bound.size.y, bound.size.z)*zoomRate;
                        cameraDistanceOffset = bound.size.x * distRate;
                        ScreenShotCamera.transform.position = bound.center + Vector3.left * cameraDistanceOffset;
                        break;
                    case EDirectionType.Top:
                        ScreenShotCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
                        ScreenShotCamera.orthographicSize = Mathf.Max(bound.size.x, bound.size.z) * zoomRate;
                        cameraDistanceOffset = bound.size.y * distRate;
                        ScreenShotCamera.transform.position = bound.center + Vector3.up * cameraDistanceOffset;
                        break;
                    case EDirectionType.Bottom:
                        ScreenShotCamera.transform.rotation = Quaternion.Euler(-90, 0, 0);
                        ScreenShotCamera.orthographicSize = Mathf.Max(bound.size.x, bound.size.z) * zoomRate;
                        cameraDistanceOffset = bound.size.y * distRate;
                        ScreenShotCamera.transform.position = bound.center + Vector3.down * cameraDistanceOffset;
                        break;
                    case EDirectionType.Front:
                        ScreenShotCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
                        ScreenShotCamera.orthographicSize = Mathf.Max(bound.size.y, bound.size.x) * zoomRate;
                        cameraDistanceOffset = bound.size.z * distRate;
                        ScreenShotCamera.transform.position = bound.center - Vector3.forward * cameraDistanceOffset;
                        break;
                    case EDirectionType.Back:
                        ScreenShotCamera.transform.rotation = Quaternion.Euler(0, 180, 0);
                        ScreenShotCamera.orthographicSize = Mathf.Max(bound.size.y, bound.size.x) * zoomRate;
                        cameraDistanceOffset = bound.size.z * distRate;
                        ScreenShotCamera.transform.position = bound.center - Vector3.back * cameraDistanceOffset;
                        break;
                    default:
                        break;
                }
                if (ScreenShotCamera.orthographicSize <= 1f) ScreenShotCamera.orthographicSize *= 2;
                
                Debug.Log(type + " " + ScreenShotCamera.transform.rotation.x + " " + ScreenShotCamera.transform.rotation.y + " " + ScreenShotCamera.transform.rotation.z + " " + ScreenShotCamera.transform.position + "  " + ScreenShotCamera.orthographicSize);
               
            }
        }
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
    }
}
