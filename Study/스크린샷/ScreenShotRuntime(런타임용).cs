using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEditor;
using UnityEngine;
using Model;

namespace ScreenShot
{
    public class ScreenShotRuntime : MonoBehaviour
    {
        EditorWindow editorWindow;
        System.Threading.CancellationToken cts;

        //스크린샷 데이터
        public Transform rootObj;
        public bool magnification = false;
        public string folderPath = "C:\\Users\\tybna\\OneDrive\\문서\\SE_ScreenShots";
        public string fileExtension = "png";
        public bool dataTag = false;
        public int captureWidth = 1920;
        public int captureHeight = 1920;
        public float captureMargin = 0.1f;
        public Light mainLight; 

        //스크린샷 필터 데이터
        public bool extension = false;
        // 스크린샷을 단계적으로 찍을지 여부
        [Header("스크린샷을 단계적으로 찍을지 여부")]
        public bool stepByStep = false;

        // 스크린샷 필터 설정
        public EScreenShotType screenShotType = EScreenShotType.NoShadow;
        public EObjectFilterType otherObjectFilterType = EObjectFilterType.All;
        public ESelectedObjectFIlterType pipeFilterType = ESelectedObjectFIlterType.All;
        public EObjectFilterType equipmentFilterType = EObjectFilterType.All;

        private GameObject selectedPipeline = null;
        private GameObject fromObj = null;
        private GameObject toObj = null;

        private bool isNext = false;

        readonly RenderTexture[] _previewTexture = new RenderTexture[(int) EDirectionType.Count];
        
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5) && ObjectManager.Instance.PipelineList.Count > 0)
            {
                TakePicture();
            }
        }
       
        public void ShowWindow()
        {
            //EditorWindow.GetWindow<UnityEditor.ScreenshotWindow>();
        }
        async void TakePicture()
        {
            // 메인 카메라 캐싱
            Camera mainCamera = Camera.main;
            // 메인 카메라 비활성화
            mainCamera.enabled = false;
            // screenshotData를 통해 스크린샷 카메라 생성 및 초기화
            Camera camPicture = new GameObject("ScreenShotCamera").AddComponent<Camera>();
            Camera cam = InitCameraRuntime(camPicture);
            ScreenshotUtility.InitScreenShotType(
                null,
                screenShotType,
                mainLight,
                cam);

            ObjectInit("Start",pipeFilterType);
            // 리스트를 순회하며 스크린샷 촬영
            for (int i = 0; i < ObjectManager.Instance.PipelineList.Count; i++)
            {
                int index = i;
                var obj = ObjectManager.Instance.PipelineList[index];
                if (obj is Pipeline)
                {
                    await TakeScreenShotRuntime(cam, obj as Pipeline, cts, IsNext, EndNext);
                }
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            // 랜더러 초기화
            ScreenshotUtility.InitScreenShotType(
                rootObj,
                EScreenShotType.Defalut,
                mainLight,
                cam);
            // 스크린샷 카메라 제거
            UnityEngine.Object.DestroyImmediate(cam.gameObject);
            // 메인 카메라 활성화
            mainCamera.enabled = true;
            ObjectInit("End", pipeFilterType);
        }
        void ObjectInit(string sign, ESelectedObjectFIlterType filterType)
        {
            if(sign == "Start")
            {
                if(filterType == ESelectedObjectFIlterType.All)
                {
                    foreach (var pipe in ObjectManager.Instance.PipelineList)
                    {
                        foreach (var mesh in pipe.GetComponentsInChildren<MeshRenderer>()) mesh.enabled = true;
                    }
                }
                else
                {
                    foreach (var pipe in ObjectManager.Instance.PipelineList)
                    {
                        foreach (var mesh in pipe.GetComponentsInChildren<MeshRenderer>()) mesh.enabled = false;
                    }
                }
            }else if(sign == "End")
            {
                foreach(var pipe in ObjectManager.Instance.PipelineList)
                {
                    foreach(var mesh in pipe.GetComponentsInChildren<MeshRenderer>()) mesh.enabled = true;
                }
            }
        }
        public Camera InitCameraRuntime(Camera cam)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.white;
            cam.orthographic = true;
            //보여야 하는것들
            cam.cullingMask = default;
            cam.cullingMask |= 1 << LayerMask.NameToLayer("Obstacle");
            cam.cullingMask |= 1 << LayerMask.NameToLayer("CreatingObject");
            cam.cullingMask |= 1 << LayerMask.NameToLayer("Object");
            cam.aspect = (float) captureWidth / captureHeight;

            cam.GetComponent<Camera>().allowMSAA = false;
            cam.GetComponent<Camera>().allowHDR = true;

            Light ligth = cam.gameObject.GetComponent<Light>();
            if (ligth == null)
            {
                ligth = cam.gameObject.AddComponent<Light>();
                ligth.type = LightType.Directional;
                ligth.intensity = 1.0f;
                ligth.shadows = LightShadows.Soft;
                ligth.enabled = false;
            }

            return cam;
        }
        ModelObject SearchEquipmemt(string name)
        {
            ModelObject modelObj = null;
            foreach(var model in ObjectManager.Instance.EquipmentList)
            {
                if(model.gameObject.name == name)
                {
                    modelObj = model;
                    break;
                }
            }
            return modelObj;
        }
        
        async UniTask TakeScreenShotRuntime(
            Camera cam,
            Pipeline pipeline,
            System.Threading.CancellationToken cts,
          
            System.Func<bool> isNext = null,
            System.Action endNext = null)
        {
            // 널 체크
            if (pipeline == null)
                return;

            HashSet<MeshRenderer> rendererHash = new();
            HashSet<MeshRenderer> pipeRenderHash = new();
            Dictionary<MeshRenderer, EObjectType> materialSwapaHash = new();
            
            GameObject fromTemp = null;
            GameObject toTemp = null;

            try
            {
                // 전체 오브젝트 필터링
                ScreenshotUtility.FilterOtherObjects(rootObj, otherObjectFilterType, rendererHash);
                // 장비 필터링
                ScreenshotUtility.FilterEquipmentObjects(equipmentFilterType, rendererHash);
                FilterPipelineObjectsRuntime(pipeline, pipeFilterType, rendererHash);

                string[] pipeName = pipeline.gameObject.name.Split("_");              
                var fromObj = SearchEquipmemt(pipeName[2]);
                var fromNozzle = pipeline.GetComponent<Pipeline>().FromNozzle;
                var toObj = SearchEquipmemt(pipeName[4]);
                var toNozzle = pipeline.GetComponent<Pipeline>().ToNozzle;
                //Debug.Log(fromObj.gameObject.name + "  " + fromNozzle.gameObject.name + "  " + toObj.gameObject.name + "  " + toNozzle.gameObject.name + "  ");
                
                string message = $"{pipeline.name.Replace("\"", "")}_{GetFromToNames(fromObj, fromNozzle, toObj, toNozzle).Replace("\"", "")}";
                // 메세지 중 폴더 이름에 사용할 수 없는 문자 제거
                // 해당 동작으로 중복되는 캡처 저장 방지 (단 날짜 태그를 사용하면 파일명 이슈로 여러장 생성)
                string objFolderName = message.Replace('/', ' ').Replace('\\', ' ');
                fromTemp = fromNozzle.gameObject;
                toTemp = toNozzle.gameObject;

                await UniTask.Yield();

                string filePath = $"{folderPath}/{objFolderName}";
                Bounds bounds = default;
                bounds = getBoundPipe(pipeline, fromNozzle, toNozzle);
                // 스크린샷 찍기
                RenderTexture[] textures = await TakeScreenShotDirectionAsyncRuntime(
                    cam,
                    filePath,
                    fileExtension,
                    bounds, dataTag,
                    captureWidth,
                    captureHeight,
                    captureMargin
                    ).AttachExternalCancellation(cts);
                
                if (fromTemp != null && fromTemp.name == "tempNozzle")
                {
                    UnityEngine.Object.Destroy(fromTemp);
                }
                if (toTemp != null && toTemp.name == "tempNozzle")
                {
                    UnityEngine.Object.Destroy(toTemp);
                }
                ScreenshotUtility.ResetNozzleObject(materialSwapaHash);
                await UniTask.Yield();
                
                
            }
            catch (System.Exception e)
            {
                Debug.Log(pipeline.gameObject.name + "@@@@@@");
                Debug.LogError(e);
                throw e; // 에러 던지기
            }
            finally // 무조건 실행 되어야하는 초기화 코드
            {
                if (fromTemp != null && fromTemp.name == "tempNozzle")
                {
                    UnityEngine.Object.Destroy(fromTemp);
                }
                if (toTemp != null && toTemp.name == "tempNozzle")
                {
                    UnityEngine.Object.Destroy(toTemp);
                }
                FilterPipelineObjectsInitRuntime(pipeline, pipeFilterType, rendererHash);
                // 필터링 초기화
                ScreenshotUtility.ResetNozzleObject(materialSwapaHash);
                ScreenshotUtility.ResetSelectedPipe(pipeRenderHash);
                ScreenshotUtility.ResetNozzleObject(materialSwapaHash);
                ScreenshotUtility.ResetFilterObjects(rendererHash);
            }
        }
        void FilterPipelineObjectsRuntime(Pipeline selectedPipeline,
            ESelectedObjectFIlterType filterType,
            HashSet<MeshRenderer> hashSet)
        {
            foreach (var renderer in selectedPipeline.GetComponentsInChildren<MeshRenderer>())
            {
                if (filterType == ESelectedObjectFIlterType.Selected) renderer.enabled = true;
                else if (filterType == ESelectedObjectFIlterType.All) renderer.enabled = true;
                else renderer.enabled = false;

                if (hashSet.Contains(renderer)) hashSet.Remove(renderer);
                else hashSet.Add(renderer);
            }
        }
        void FilterPipelineObjectsInitRuntime(Pipeline selectedPipeline,
           ESelectedObjectFIlterType filterType,
           HashSet<MeshRenderer> hashSet)
        {
            foreach (var renderer in selectedPipeline.GetComponentsInChildren<MeshRenderer>())
            {
                if (filterType == ESelectedObjectFIlterType.All) renderer.enabled = true;
                else renderer.enabled = false;

                if (hashSet.Contains(renderer)) hashSet.Remove(renderer);
                else hashSet.Add(renderer);
            }
        }
        string GetFromToNames(ModelObject fromObj, Nozzle fromNozzle, ModelObject toObj, Nozzle toNozzle)
        {
            try
            { 
                string fromName = (fromNozzle == null) ? fromObj.name : fromObj.name + "-" + fromNozzle.name;
                string toName = (toNozzle == null) ? toObj.name : toObj.name + "-" + toNozzle.name;
                return $"{fromName}_{toName}";
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return string.Empty;
            }

        }
        public bool IsNext()
        {
            return !stepByStep || isNext;
        }
        public void EndNext()
        {
            isNext = false;
        }
        
        public async UniTask<RenderTexture[]> TakeScreenShotDirectionAsyncRuntime(
            Camera cam,
            string filePath,
            string extension,
            Bounds bounds, bool dataTag,
            int captureWidth, int captureHeight,
            float margin)
        {
            RenderTexture[] renderTextures = new RenderTexture[(int) EDirectionType.Count];
            int len = (int) EDirectionType.Count;
            for (int i = 0; i < len; i++)
            {
                EDirectionType direction = (EDirectionType) i;
                string path = MakeScreenShotPathRuntime(filePath, direction.ToString(), extension, dataTag);
                MoveScreenShotCameraRuntime(cam, bounds, direction, 1.0f + margin);
                // 촬영 가능한 상태까지 대기
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
                renderTextures[i] = TakeScreenShotRuntime(cam, captureWidth, captureHeight, path);
            }
            return renderTextures;
        }
         
        Bounds getBoundPipe(Pipeline pipeline, Nozzle fromNozzle, Nozzle toNozzle)
        {
            Bounds bound = new Bounds();
            foreach(var pipe in pipeline.GetComponentsInChildren<MeshRenderer>())
            {
                if (bound == default) bound = pipe.bounds;
                else bound.Encapsulate(pipe.bounds);
            }
            Debug.Log(bound.center + " " + bound.size+"  "+pipeline.gameObject.name);
            //bound.Encapsulate(fromNozzle.GetComponent<MeshRenderer>().bounds);
            //bound.Encapsulate(toNozzle.GetComponent<MeshRenderer>().bounds);
            return bound;
        }
        
        public void MoveScreenShotCameraRuntime(Camera cam,
            Bounds targetBounds,
            EDirectionType direction,
            float offset)
        {
            Vector3 boundsCenter = targetBounds.center;
            Vector3 boundsSize = targetBounds.size;

            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            float orthographicSize = 0;
            // 카메라 위치와 회전 설정
            switch (direction)
            {
                case EDirectionType.Top:
                    position = boundsCenter + Vector3.up * (boundsSize.y / 2 + 0.1f);
                    rotation = Quaternion.Euler(90f, 0f, 0f);
                    orthographicSize = Mathf.Max(boundsSize.x / cam.aspect, boundsSize.z) / 2;
                    break;
                case EDirectionType.Bottom:
                    position = boundsCenter + Vector3.down * (boundsSize.y / 2 + 0.1f);
                    rotation = Quaternion.Euler(-90f, 0f, 0f);
                    orthographicSize = Mathf.Max(boundsSize.x / cam.aspect, boundsSize.z) / 2;
                    break;
                case EDirectionType.Front:
                    position = boundsCenter + Vector3.forward * (boundsSize.z / 2 + 0.1f);
                    rotation = Quaternion.Euler(0f, 180f, 0f);
                    orthographicSize = Mathf.Max(boundsSize.y, boundsSize.x / cam.aspect) / 2;
                    break;
                case EDirectionType.Back:
                    position = boundsCenter + Vector3.back * (boundsSize.z / 2 + 0.1f);
                    rotation = Quaternion.Euler(0f, 0f, 0f);
                    orthographicSize = Mathf.Max(boundsSize.y, boundsSize.x / cam.aspect) / 2;
                    break;
                case EDirectionType.Left:
                    position = boundsCenter + Vector3.left * (boundsSize.x / 2 + 0.1f);
                    rotation = Quaternion.Euler(0f, 90f, 0f);
                    orthographicSize = Mathf.Max(boundsSize.y, boundsSize.z / cam.aspect) / 2;
                    break;
                case EDirectionType.Right:
                    position = boundsCenter + Vector3.right * (boundsSize.x / 2 + 0.1f);
                    rotation = Quaternion.Euler(0f, -90f, 0f);
                    orthographicSize = Mathf.Max(boundsSize.y, boundsSize.z / cam.aspect) / 2;
                    break;
            }
            // 설정된 카메라값 입력
            cam.transform.SetPositionAndRotation(position, rotation);
            cam.orthographicSize = orthographicSize * offset;

            // nearClipPlane과 farClipPlane 설정
            float distanceToCenter = Vector3.Distance(position, boundsCenter);
            cam.nearClipPlane = .099f;
            cam.farClipPlane = distanceToCenter + boundsSize.magnitude;
            
        }
        
        public string MakeScreenShotPathRuntime(string folderPath, string direction, string fileExtension, bool dataTag)
        {
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            if (dataTag)
                return $"{folderPath}/{direction}_{System.DateTime.Now.ToString("MMdd_HHmmss")}.{fileExtension}";
            else
                return $"{folderPath}/{direction}.{fileExtension}";
        }
        
        public RenderTexture TakeScreenShotRuntime(Camera cam, int captureWidth, int captureHeight, string filePath)
        {
            int originWidth = UnityEngine.Screen.width;
            int originHeight = UnityEngine.Screen.height;
            bool isfullscreen = UnityEngine.Screen.fullScreen;

            // 스크린 사이즈 변경
            UnityEngine.Screen.SetResolution(captureWidth, captureHeight, isfullscreen);

            // 렌더 텍스쳐 생성
            RenderTexture rt = new RenderTexture(captureWidth, captureHeight, 24);
            cam.targetTexture = rt;

            // 렌더 텍스쳐에 렌더링
            cam.Render();

            // 텍스쳐 읽기
            Texture2D screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);

            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
            screenShot.Apply();

            cam.targetTexture = null;
            RenderTexture.active = null;

            // 파일 저장
            byte[] bytes = screenShot.EncodeToPNG();
            UniTask.Void(async () =>
            {
                await System.IO.File.WriteAllBytesAsync(filePath, bytes);
                UnityEngine.Object.Destroy(screenShot);
            });
            // 스크린 사이즈 원복
            UnityEngine.Screen.SetResolution(originWidth, originHeight, isfullscreen);
            return rt;
        }
        
    }
}
