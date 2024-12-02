using Aspose.ThreeD.Render;
using Codice.Client.Common.GameUI;
using Cysharp.Threading.Tasks;
using MetacleAI;
using Model;
using ScreenShot;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TakeEditorScreenShot2DData : MonoBehaviour
{
    /// 2D 학습 파이프 데이터 리스트에 대한 스크린샷 촬영
    /// </summary>
    /// <typeparam name="T">촬영을 진행할 모델 오브젝트의 리스트</typeparam>
    /// <param name="screenshotData">스크린샷 설정 데이터</param>
    /// <param name="filter">스크린샷 필터 데이터</param>
    /// <param name="list">촬영을 진행할 오브젝트 리스트</param>
    /// <param name="cts">취소 토큰</param>
    /// <param name="progressAction">프로그래스 관련 이벤트</param>
    /// <param name="isNext">자동으로 다음 단계를 진행할지 여부</param>
    /// <param name="endNext">다음으로 넘어간 후 동작</param>
    /// <returns></returns>
    public static async UniTask Take2DDataScreenShot(
        ScreenshotSettingsData screenshotData,
        ScreenShotFilterData filter,
        List<FilteredPipeline> list, System.Threading.CancellationToken cts,
        System.Action<float, string, Bounds, GameObject, GameObject, Camera, GameObject, RenderTexture[]> progressAction = null,
        System.Func<bool> isNext = null,
        System.Action endNext = null
        )
    {
        // 나머지는 전체 끈다.
        await OffAllPipe();
      
        // 메인 카메라 캐싱
        Camera mainCamera = Camera.main;
        // 메인 카메라 비활성화
        mainCamera.enabled = false;
        // screenshotData를 통해 스크린샷 카메라 생성 및 초기화
        Camera cam = ScreenShotEditorUtility.InitCamera(screenshotData);
        ScreenshotUtility.InitScreenShotType(
            screenshotData.targetObject,
            filter.screenShotType,
            screenshotData.mainLight,
            cam);
       

        try // 예상치 못한 오류 발생 시 처리를 위한 try-catch-finally
        {
            // 리스트를 순회하며 스크린샷 촬영
            for (int index = 0; index < list.Count; index++)
            {
                var obj = list[index];
                if (obj is FilteredPipeline)
                {
                    await Take2DPipeDataScreenShot(cam, obj, screenshotData, filter,
                        (float) index / list.Count, cts, progressAction, isNext, endNext);
                }
                await UniTask.Yield(PlayerLoopTiming.Update);
                
            }
        }
        catch (System.Exception e)
        {
            throw e; // 에러 던지기
        }
        finally
        {
            // 랜더러 초기화
            ScreenshotUtility.InitScreenShotType(
                screenshotData.targetObject,
                EScreenShotType.Defalut,
                screenshotData.mainLight,
                cam);
            // 스크린샷 카메라 제거
            Object.DestroyImmediate(cam.gameObject);
            // 메인 카메라 활성화
            mainCamera.enabled = true;
            //전체 오브젝트 다시 켜기
            OnAllPipeObject();
        }
    }

    static async UniTask Take2DPipeDataScreenShot(
           Camera cam,
           FilteredPipeline pipeline,
           ScreenshotSettingsData screenshotData,
           ScreenShotFilterData filter,
           float prograss,
           System.Threading.CancellationToken cts,
           System.Action<float, string, Bounds, GameObject, GameObject, Camera, GameObject, RenderTexture[]> progressAction = null,
           System.Func<bool> isNext = null,
           System.Action endNext = null)
    {
        // 널 체크
        if (pipeline == null) return;
        if (pipeline.FromNozzle == null || pipeline.ToNozzle == null) return;

        // From, to 노즐이 같은 경우 제외
        if (pipeline.FromNozzle == pipeline.ToNozzle) return;
        
        HashSet<MeshRenderer> rendererHash = new();
        HashSet<MeshRenderer> pipeRenderHash = new();
        Dictionary<MeshRenderer, EObjectType> materialSwapaHash = new();
       
        try
        {
            // 전체 오브젝트 필터링
            ScreenshotUtility.FilterOtherObjects(screenshotData.rootObject, filter.otherObjectFilterType, rendererHash);
            // 장비 필터링
            ScreenshotUtility.FilterEquipmentObjects(filter.equipmentFilterType, rendererHash);
            //전체 파이프 켜기
            var pipeList = pipeline.GetAllPipe();
            Debug.Log(prograss + " @@@@@@@@@@@@@@@");

            // 메세지 생성
            string message = $"{pipeline.Name.Replace("\"", "")}";
            // 메세지 중 폴더 이름에 사용할 수 없는 문자 제거
            // 해당 동작으로 중복되는 캡처 저장 방지 (단 날짜 태그를 사용하면 파일명 이슈로 여러장 생성)
            string objFolderName = message.Replace('/', ' ').Replace('\\', ' ');

            await UniTask.Yield();
           
            // 파이프 필터링
            FilterAllPipelineObjects(pipeList, filter.pipeFilterType, rendererHash);
            //찍기 전 활성화
            await OnSelectPipeObject(pipeList);

            //From, to 색상 설정
            await SetNozzleColor(pipeline.FromNozzle, pipeline.ToNozzle);
            // 촬영 영역 설정
            Bounds bounds = PipePathGetBounds(pipeList, pipeline.FromNozzle, pipeline.ToNozzle);
            Bounds clipingAreaBound = bounds;
            //Bounds bounds = pipeline.GetPipelineBounds(pipeline.FromNozzle, pipeline.ToNozzle,pipeline.GetAllPipe());
            Debug.Log(bounds.size.x + " " + bounds.size.y + " @@@@ " + bounds.size.z);
            
           
            //확대율 고정 여부
            if (screenshotData.magnification)
            {
                float max = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
                bounds = new Bounds(bounds.center, Vector3.one * max);
            }

            // 저장할 파일 경로 설정
            string filePath = $"{screenshotData.folderPath}/Cliping {screenshotData.isCliping}/{objFolderName}";

            // 스크린샷 찍기
            RenderTexture[] textures = await Take2DScreenShotDirectionAsync(
                cam,
                filePath,
                screenshotData.fileExtension,
                bounds, clipingAreaBound, screenshotData.dataTag,
                screenshotData.captureWidth,
                screenshotData.captureHeight,
                screenshotData.captureMargin,
                screenshotData.isCliping
                ).AttachExternalCancellation(cts);

            // 디버깅 체크
            if (isNext != null)
            {
                await ScreenshotUtility.WaitUntil(isNext, bounds).AttachExternalCancellation(cts);
                endNext?.Invoke();
            }
            
            //찍은 후 파이프 지우기
            await OffSelectPipeObject(pipeList);
            //색상 초기화
            await InitNozzleColor(pipeline.FromNozzle, pipeline.ToNozzle);
            await UniTask.Yield();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            throw e; // 에러 던지기
        }
        finally // 무조건 실행 되어야하는 초기화 코드
        {
            // 필터링 초기화
            ScreenshotUtility.ResetNozzleObject(materialSwapaHash);
            ScreenshotUtility.ResetSelectedPipe(pipeRenderHash);
            ScreenshotUtility.ResetNozzleObject(materialSwapaHash);
            ScreenshotUtility.ResetFilterObjects(rendererHash);
        }
    }
    /// <summary>
    /// 기능 : 파이프 모두 끄기
    /// TODO : 처음에 전체 파이프 모두 끄기
    /// </summary>
    /// <returns></returns>
    static async UniTask OffAllPipe()
    {
        foreach(var pipe in ObjectManager.Instance.PipelineList)
        {
            pipe.gameObject.SetActive(false);
        }
        foreach(var pipeline in PipelineFilter.Instance.FilterPipelineList)
        {
            List<Pipe> allPipe = pipeline.GetAllPipe();
            foreach(var pipe in allPipe)
            {
                pipe.gameObject.SetActive(false);
            }
        }
        await UniTask.Yield();
    }
    /// <summary>
    /// 기능 : 바운드 영역 가져오기
    /// </summary>
    /// <param name="pipeline"></param>
    /// <returns></returns>
    static Bounds PipePathGetBounds(List<Pipe> pipeline, Nozzle from, Nozzle to)
    {
        Bounds newBound = default;
        foreach(Pipe pipe in pipeline)
        {
            //파이프
            foreach(MeshRenderer mesh in pipe.GetComponentsInChildren<MeshRenderer>(true))
            {
                if(mesh.gameObject.name == "Geometry")
                {
                    if (newBound == default)
                    {
                        newBound = mesh.bounds;
                    }
                    else
                    {
                        newBound.Encapsulate(mesh.bounds);
                    }
                }
            }
        }
        //From Nozzle
        foreach (MeshRenderer mesh in from.GetComponentsInChildren<MeshRenderer>(true))
        {
            if (mesh.gameObject.name == "Geometry")
            {
                if (newBound == default)
                {
                    newBound = mesh.bounds;
                }
                else
                {
                    newBound.Encapsulate(mesh.bounds);
                }
            }
        }
        //To Nozzle
        foreach (MeshRenderer mesh in to.GetComponentsInChildren<MeshRenderer>(true))
        {
            if (mesh.gameObject.name == "Geometry")
            {
                if (newBound == default)
                {
                    newBound = mesh.bounds;
                }
                else
                {
                    newBound.Encapsulate(mesh.bounds);
                }
            }
        }
        return newBound;
    }
    /// <summary>
    /// 파이프라인 오브젝트 필터링
    /// </summary>
    /// <param name="selectedPipeline"></param>
    /// <param name="filterType"></param>
    /// <param name="hashSet"></param>
    public static void FilterAllPipelineObjects(
        List<Pipe> selectedPipeline,
        ESelectedObjectFIlterType filterType,
        HashSet<MeshRenderer> hashSet)
    {
        foreach (Pipe obj in selectedPipeline)
        {
            bool active = filterType == ESelectedObjectFIlterType.All;

            foreach (var renderer in obj.GetComponentsInChildren<MeshRenderer>())
            {
                if (renderer.enabled == active) continue;

                renderer.enabled = active;

                if (hashSet.Contains(renderer)) hashSet.Remove(renderer);
                else hashSet.Add(renderer);
            }
        }
    }
    /// <summary>
    /// 기능 : 오브젝트 켜기
    /// </summary>
    static void OnAllPipeObject()
    {
        foreach (var pipe in ObjectManager.Instance.PipelineList)
        {
            pipe.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 기능 : 대상 오브젝트 켜기
    /// </summary>
    static async UniTask OnSelectPipeObject(List<Pipe> pipeList)
    {
        foreach (var pipe in pipeList)
        {
            var pipelineObj = pipe.MyPipeline.gameObject;
            pipelineObj.SetActive(true);
            pipe.gameObject.SetActive(true);
            Debug.Log(pipelineObj.gameObject.name+"  " +pipe.gameObject.name);
            foreach (var mesh in pipe.GetComponentsInChildren<MeshRenderer>(true))
            {
                mesh.enabled = true;
                mesh.material.color = new Color(255f/255f,102f/255f,51f/255f);
                mesh.gameObject.SetActive(true);
            }
        }
        await UniTask.Yield();
    }
    /// <summary>
    /// 기능 : 대상 오브젝트 끄기
    /// </summary>
    static async UniTask OffSelectPipeObject(List<Pipe> pipeList)
    {
        foreach (var pipe in pipeList)
        {
            pipe.MyPipeline.gameObject.SetActive(false);
            pipe.gameObject.SetActive(false);
        }
        await UniTask.Yield();
    }
    static async UniTask SetNozzleColor(Nozzle from, Nozzle to)
    {
        from.GetComponentInChildren<MeshRenderer>().material.color = new Color(153f/255f,204f/255f,0f);
        to.GetComponentInChildren<MeshRenderer>().material.color = new Color(153f / 255f, 102f / 255f, 0f);
        await UniTask.Yield();
    }
    static async UniTask InitNozzleColor(Nozzle from, Nozzle to)
    {
        from.GetComponentInChildren<MeshRenderer>().material.color = new Color(0f, 255f / 255f,204f / 255f);
        to.GetComponentInChildren<MeshRenderer>().material.color = new Color(0f, 255f / 255f, 204f/255f);
        await UniTask.Yield();
    }
    /// <summary>
    /// 기능 : 실제 촬영 진행
    /// </summary>
    /// <param name="cam"></param>
    /// <param name="filePath"></param>
    /// <param name="extension"></param>
    /// <param name="bounds"></param>
    /// <param name="dataTag"></param>
    /// <param name="captureWidth"></param>
    /// <param name="captureHeight"></param>
    /// <param name="margin"></param>
    /// <param name="isCliping"></param>
    /// <returns></returns>
    public static async UniTask<RenderTexture[]> Take2DScreenShotDirectionAsync(
            Camera cam,
            string filePath,
            string extension,
            Bounds bounds,Bounds clipingAreaBound, bool dataTag,
            int captureWidth, int captureHeight,
            float margin, bool isCliping)
    {
        RenderTexture[] renderTextures = new RenderTexture[(int) EDirectionType.Count];
        int len = (int) EDirectionType.Count;
        for (int i = 0; i < len; i++)
        {
            EDirectionType direction = (EDirectionType) i;
            string path = ScreenshotUtility.MakeScreenShotPath(filePath, direction.ToString(), extension, dataTag);
            await Move2DScreenShotCamera(cam, bounds, clipingAreaBound, direction, 1.0f + margin, isCliping);
            // 촬영 가능한 상태까지 대기
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            renderTextures[i] = ScreenshotUtility.TakeScreenShot(cam, captureWidth, captureHeight, path);
            //클리핑된 오브젝트 재활성화
            await ClipingObjectsInit();
        }
        return renderTextures;
    }
    /// <summary>
    /// 스크린샷을 찍기 위해 입력 받은 카메라의 위치를 이동한다.
    /// </summary>
    /// <param name="cam"></param>
    /// <param name="targetBounds"></param>
    /// <param name="direction"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    private static async UniTask<Camera> Move2DScreenShotCamera(Camera cam,
        Bounds targetBounds,Bounds clipingAreaBound,
        EDirectionType direction,
        float offset, bool isCliping)
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

        //이게 필요한가?
        switch (direction)
        {
            case EDirectionType.Top:
                float xPos = cam.transform.position.x - (float) (cam.orthographicSize / Screen.height) * (float) Screen.width;
                float xPosMax = cam.transform.position.x + (float) (cam.orthographicSize / Screen.height) * (float) Screen.width;
                ObjectManager.Instance.OPointX.Add(xPos);
                ObjectManager.Instance.OPointXMax.Add(xPosMax);
                float zPos = cam.transform.position.z - cam.orthographicSize;
                float zPosMax = cam.transform.position.z + cam.orthographicSize;
                ObjectManager.Instance.OPointZ.Add(zPos);
                ObjectManager.Instance.OPointZMax.Add(zPosMax);
                Debug.Log(xPos + " " + xPosMax + " " + zPos + " " + zPosMax + "@4444");
                break;
            case EDirectionType.Right:
                float yPos = cam.transform.position.y - cam.orthographicSize;
                float yPosMax = cam.transform.position.y + cam.orthographicSize;
                ObjectManager.Instance.OPointY.Add(yPos);
                ObjectManager.Instance.OPointYMax.Add(yPosMax);
                break;
        }

        //클리핑 여부
        if (isCliping)
        {
            switch (direction)
            {
                //y축 클리핑
                case EDirectionType.Top:
                case EDirectionType.Bottom:
                    float distanceToCenter = Mathf.Abs(position.y - boundsCenter.y);
                    cam.nearClipPlane = Mathf.Max(distanceToCenter - clipingAreaBound.size.y, 0);
                    cam.farClipPlane = distanceToCenter + clipingAreaBound.size.y;
                    break;
                //x축 클리핑
                case EDirectionType.Left:
                case EDirectionType.Right:
                    distanceToCenter = Mathf.Abs(position.x - boundsCenter.x);
                    cam.nearClipPlane = Mathf.Max(distanceToCenter - clipingAreaBound.size.x, 0);
                    cam.farClipPlane = distanceToCenter + clipingAreaBound.size.x;
                    break;
                //z축 클리핑
                case EDirectionType.Front:
                case EDirectionType.Back:
                    distanceToCenter = Mathf.Abs(position.z - boundsCenter.z);
                    cam.nearClipPlane = Mathf.Max(distanceToCenter - clipingAreaBound.size.z, 0);
                    cam.farClipPlane = distanceToCenter + clipingAreaBound.size.z;
                    break;
                default:
                    break;
            }
            await ClipingObjects(targetBounds, clipingAreaBound, direction);
        }
        else
        {
            // nearClipPlane과 farClipPlane 설정
            float distanceToCenter = Vector3.Distance(position, boundsCenter);
            cam.nearClipPlane = .099f;
            cam.farClipPlane = distanceToCenter + boundsSize.magnitude;
        }
        return cam;
    }
    /// <summary>
    /// 기능 : 오브젝트 클리핑
    /// 매개변수 : 촬영 대상 영역, 촬영 방향
    /// 1) 포인트 기준으로 클리핑
    /// 2) 탱크도 클리핑
    /// 3) 대상 영역만 남겨두고 나머지는 모두 클리핑
    /// 4) 각 축에 대해 클리핑 대상 : 전체(박스) - 대상
    /// 5) 확대율 고정모드이므로 각 박스는 3부분으로 나뉨(양옆, 개상)
    /// 6) 양옆 박스를 구해 클리핑
    /// </summary>
    /// <returns></returns>
    static private async UniTask ClipingObjects(Bounds allBound,Bounds bound, EDirectionType direction)
    {
        Bounds bound1 = default;
        Bounds bound2 = default;
        switch (direction)
        {
            //y축 클리핑
            case EDirectionType.Top:
            case EDirectionType.Bottom:
                bound1.size = new Vector3(bound.size.x,(allBound.size.y-bound.size.y)*0.5f,bound.size.z);
                bound1.center = bound.center + new Vector3(0, (allBound.size.y + bound.size.y) * 0.25f, 0);
                bound2.size = new Vector3(bound.size.x, (allBound.size.y - bound.size.y) * 0.5f, bound.size.z);
                bound2.center = bound.center - new Vector3(0, (allBound.size.y + bound.size.y) * 0.25f, 0);
                break;
            //x축 클리핑
            case EDirectionType.Left:
            case EDirectionType.Right:
                bound1.size = new Vector3((allBound.size.x - bound.size.x) * 0.5f, bound.size.y, bound.size.z);
                bound1.center = bound.center + new Vector3((allBound.size.x + bound.size.x) * 0.25f, 0, 0);
                bound2.size = new Vector3((allBound.size.x - bound.size.x) * 0.5f, bound.size.y, bound.size.z);
                bound2.center = bound.center - new Vector3((allBound.size.x + bound.size.x) * 0.25f, 0, 0);
                break;
            //z축 클리핑
            case EDirectionType.Front:
            case EDirectionType.Back:
                bound1.size = new Vector3(bound.size.x, bound.size.y, (allBound.size.z - bound.size.z) * 0.5f);
                bound1.center = bound.center + new Vector3(0, 0, (allBound.size.z + bound.size.z) * 0.25f);
                bound2.size = new Vector3(bound.size.x, bound.size.y, (allBound.size.z - bound.size.z) * 0.5f);
                bound2.center = bound.center - new Vector3(0, 0, (allBound.size.z + bound.size.z) * 0.25f);
                break;
            default:
                break;
        }
        Collider[] hit1Colliders = null;
        hit1Colliders = Physics.OverlapBox(bound1.center, bound1.size);
        Collider[] hit2Colliders = null;
        hit2Colliders = Physics.OverlapBox(bound2.center, bound2.size);
        foreach(var hit in hit1Colliders)
        {
            var hitObj = hit.GetComponent<GeometryObject>();
            if (hitObj.GetComponent<Structure>() != null || hitObj.GetComponent<Equipment>() != null || hitObj.GetComponent<Unknown>() != null)
            {
                hit.gameObject.SetActive(false);
                ObjectManager.Instance.clipingIObjectList.Add(hit.gameObject);
            }
        }
        foreach (var hit in hit2Colliders)
        {
            var hitObj = hit.GetComponent<GeometryObject>();
            if (hitObj.GetComponent<Structure>() != null || hitObj.GetComponent<Equipment>() != null || hitObj.GetComponent<Unknown>() != null)
            {
                hit.gameObject.SetActive(false);
                ObjectManager.Instance.clipingIObjectList.Add(hit.gameObject);
            }
        }
        await UniTask.Yield();
    }
    //클리핑된 오브젝트 재활성화
    static async UniTask ClipingObjectsInit()
    {
        foreach(var gm in ObjectManager.Instance.clipingIObjectList)
        {
            gm.SetActive(true);
        }
        ObjectManager.Instance.clipingIObjectList.Clear();
        await UniTask.Yield();
    }
}
