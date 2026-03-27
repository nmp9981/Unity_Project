using Cysharp.Threading.Tasks;
using Ftp;
using Model;
using ScreenShot;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using System;
using PathFinding;

/// <summary>
/// 사진 저장 구조
/// </summary>
class ImageSetEntry
{
    public ulong[] imageHashes;   // 6장
    public string folderPath;     // 필요하면
}

public class ScreenShotSudoCode : MonoBehaviour
{
    public PipelineScreenShotDataHolder screenShotDataHolder;
    public Camera pictureCam;

    //파일 저장 경로
    [SerializeField]
    public string fileSavePath;

    //해상도
    int captureWidth = 1920;
    int captureHeight = 1920;

    //촬영 개수
    int pictureCountMax;
    int unitPictureSetCount = 1000;

    //각 이미지 세트 개수
    const int IMAGE_COUNT = 6;

    //땅
    [SerializeField]
    MeshRenderer groundJUPCMesh;

    //가상 노즐
    [SerializeField]
    MeshRenderer virtualFromnozzle;
    [SerializeField]
    MeshRenderer virtualTonozzle;

    //스크린샷 용 쉐이더
    Shader onlyColorShader;
    //원복용 일반 쉐이더
    Shader normalShader;

    //이번 세트는 제거된 사진인가?
    bool isPipeOnDuplicate;
    //해시값에 따른 이미지 저장 Dic
    Dictionary<ulong, List<ImageSetEntry>> setHashMap = new Dictionary<ulong, List<ImageSetEntry>>();

    //Nozzle 해시
    private HashSet<int> _shotHistory = new HashSet<int>();

    #region Unity 함수
    private void Awake()
    {
        screenShotDataHolder = GetComponent<PipelineScreenShotDataHolder>();
    }
    private void Start()
    {
        onlyColorShader = Shader.Find("Unlit/Color");
        normalShader = Shader.Find("Standard");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            //오브젝트 설정
            ObjectInit(true);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //사진 촬영 로직
            AllFlow_TakeScreenShot();
        }
    }
    #endregion
    #region 전체 촬영 Flow
    /// <summary>
    /// 전체 촬영
    /// </summary>
    async UniTask AllFlow_TakeScreenShot()
    {
        //카메라 세팅
        await CameraOptionSetting(pictureCam);
        //오브젝트 끄기
        await ObjectOff();

        //촬영
        int num = 0;
        pictureCountMax = screenShotDataHolder.RackFromToList.Count;
        foreach (var pathPipe in screenShotDataHolder.RackFromToList)
        {
            num+=1;
            isPipeOnDuplicate = false;
            ScreenShotFromToData curPath = pathPipe;

            Debug.Log($"<color=green>현재 진도 {num} / {pictureCountMax} </color>");
            // 경로 데이터가 없음
            if (curPath == null) continue;
            if (curPath.PipeList.Count == 0) continue;
            if (curPath.PipeBoundList.Count == 0) continue;

            //From. To가 둘다 없음
            if(curPath.From == null &&  curPath.To == null) continue;

            // 리스트 개수 불일치 검사
            if (pathPipe.PipeList.Count != pathPipe.PipeBoundList.Count) continue;

            // 실제 오브젝트와 Bounds 위치 매칭 검사
            if (IsMisMatchPipeBountList(pathPipe)) continue;

            // 고유 키로 중복 검사
            if(CreateCharacteristPathValue(pathPipe, out int key))
            {
                if (!_shotHistory.Add(key))
                {
                    Debug.Log($"[Skip] {num}번은 이미 촬영된 노즐 조합입니다: {pathPipe.PathName}");
                    continue;
                }
            }
            //경로내 파이프가 없음
            if (IsNotPipe_InPath(curPath.PipeList)) continue;

            //실제 노즐과 붙어있지 않음
            (Vector3 startPos, Vector3 endPos) sidePoint = GetPipeEndpoints(curPath.PipeBoundList);
            await UniTask.Yield();
            if (!IsAttachPipe_Nozzle(curPath, sidePoint.startPos, sidePoint.endPos)) continue;

            // 데이터 자체가 똑같은지 확인하는 로그
            string pipeNames = string.Join(", ", curPath.PipeList.Select(p => p.name));
            Debug.Log($"[DataCheck] {curPath.PathName} | Pipes: {pipeNames} | PipeCount: {curPath.PipeList.Count}" +
                $" | FirstBoundCenter: {curPath.PipeBoundList[0].center} | PipeName : {curPath.PipeList[0].name} | PipeId : {curPath.PipeList[0].GetInstanceID()}");

            //촬영 시작
            await ScreenShotFromTo(curPath, sidePoint.startPos, sidePoint.endPos, num,true);//파이프 On
            if(!isPipeOnDuplicate) await ScreenShotFromTo(curPath, sidePoint.startPos, sidePoint.endPos, num,false);//파이프 Off
            else Debug.Log($"[PipeSkip] {num} : {curPath.PathName} OFF skipped (ON duplicate)");
        }

        //원래대로
        await ObjectOn();
        pictureCam.gameObject.SetActive(false);//카메라 끄기
    }
    #endregion
    #region 사전 준비 관련 함수
    /// <summary>
    /// 오브젝트 끄기
    /// </summary>
    /// <returns></returns>
    async UniTask ObjectOff()
    {
        //파이프 끄기
        foreach (var pipeline in ObjectManager.Instance.PipelineList)
        {
            foreach (var pipe in pipeline.PipeList)
            {
                pipe.gameObject.SetActive(false);
            }
        }
        //가상노즐끄기
        virtualFromnozzle.gameObject.SetActive(false);
        virtualTonozzle.gameObject.SetActive(false);
    }

    /// <summary>
    /// 촬영전 오브젝트 설정
    /// </summary>
    async UniTask ObjectInit(bool isTaking)
    {
        //파이프라인
        Color pipeColor = new Color(1, 102f / 255f, 51f / 255f);
        foreach (var pipeline in ObjectManager.Instance.PipelineList)
        {
            foreach (var pipe in pipeline.PipeList)
            {
                foreach (var pipeMesh in pipe.GetComponentsInChildren<MeshRenderer>(true))
                {
                    Color changePipeColor = pipeColor;
                    ChangeColor_ShadowOff(pipeMesh, changePipeColor, isTaking);
                }
            }
        }
        foreach (var pathPipe in screenShotDataHolder.RackFromToList)
        {
            foreach (var pipe in pathPipe.PipeList)
            {
                MeshRenderer pipeMesh = null;
                if (!pipe.TryGetComponent(out pipeMesh))
                {
                    pipeMesh = pipe.gameObject.AddComponent<MeshRenderer>();
                }
                Color changePipeColor = pipeColor;
                ChangeColor_ShadowOff(pipeMesh, changePipeColor, isTaking);
            }
        }
        //구조물
        Color stColor = new Color(0, 0, 102f / 255f);
        foreach (var structure in ObjectManager.Instance.StructureList)
        {
            MeshRenderer stMesh = null;
            if (!structure.TryGetComponent(out stMesh))
            {
                stMesh = structure.gameObject.AddComponent<MeshRenderer>();
            }
            Color changeStColor = stColor;
            ChangeColor_ShadowOff(stMesh, changeStColor, isTaking);

            foreach (var childSt in structure.GetComponentsInChildren<MeshRenderer>(true))
            {
                Color changeStChildColor = stColor;
                ChangeColor_ShadowOff(childSt, changeStChildColor, isTaking);
            }
        }
        //메인 rack, 땅
        OnOff_MainRackAndGround(false);

        //장비
        GameObject equipmentSet = GameObject.Find("Equipments");
        Color equipColor = new Color(0, 1, 204f / 255f);
        foreach (var equip in equipmentSet.GetComponentsInChildren<Equipment>(true))
        {
            MeshRenderer rootMesh = null;
            if (!equip.TryGetComponent(out rootMesh))
            {
                rootMesh = equip.gameObject.AddComponent<MeshRenderer>();
            }
            Color changeColor = equipColor;
            ChangeColor_ShadowOff(rootMesh, changeColor, isTaking);

            // 2. 장비 루트 아래 모든 자식 오브젝트의 MeshRenderer에 적용 (핵심 수정)
            foreach (var childMesh in equip.GetComponentsInChildren<MeshRenderer>(true)) // true: 비활성화된 자식도 포함
            {
                // 이미 처리된 루트 오브젝트 MeshRenderer를 건너뛸 수 있음 (선택 사항)
                if (childMesh == rootMesh) continue;

                Color changeChildColor = equipColor;
                ChangeColor_ShadowOff(childMesh, changeChildColor, isTaking);
            }

            //노즐
            if (equip.NozzleList == null || equip.NozzleList.Count == 0) continue;

            foreach (var noz in equip.NozzleList)
            {
                // null 체크 추가
                if (noz == null) continue;

                // MeshRenderer 캐싱
                var nozMeshes = noz.GetComponentsInChildren<MeshRenderer>(true);
                foreach (MeshRenderer nozMesh in nozMeshes)
                {
                    if (nozMesh == null) continue;
                    ChangeColor_ShadowOff(nozMesh, equipColor, isTaking);
                }
            }
        }
      
        //가상노즐
        Color fromColor = new Color(153f / 255f, 204f / 255f, 0);
        ChangeColor_ShadowOff(virtualFromnozzle, fromColor, isTaking);
        Color toColor = new Color(153f / 255f, 102f / 255f, 0);
        ChangeColor_ShadowOff(virtualTonozzle, toColor, isTaking);

        //기즈모 날리기
        RuntimeVisualizer.Instance.ClearAll();
        Debug.Log("오브젝트 색상 변경 완료");
    }

    /// <summary>
    /// 카메라 옵션 설정
    /// </summary>
    /// <param name="cam">카메라</param>
    /// <returns></returns>
    async UniTask CameraOptionSetting(Camera cam)
    {
        //1. 카메라 on
        cam.gameObject.SetActive(true);
        //2. 카메라 모드 orthographic으로 변경
        cam.orthographic = true;
        //3. 배경 색상 변경
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.white;
        //4. 안티앨리어싱 비활성화 (엣지 블렌딩으로 인한 색상 증가 방지)
        cam.allowMSAA = false;

        await UniTask.Yield();
    }

    /// <summary>
    /// 메시 색상 변경 , 그림자 제거
    /// </summary>
    /// <param name="mesh">Mesh</param>
    /// <returns></returns>
    void ChangeColor_ShadowOff(MeshRenderer mesh, Color color, bool istaking)
    {
        // 1. 그림자 주고 받기 해제, TwoSide모드
        mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        mesh.receiveShadows = !istaking;

        // 2. material 한 번만 참조 후 재사용 ← 핵심 수정
        Material mat = mesh.material;
        mat.shader = istaking ? onlyColorShader : normalShader;
        mat.color = color;

        // 3. Light Probes
        mesh.lightProbeUsage = istaking
            ? UnityEngine.Rendering.LightProbeUsage.Off
            : UnityEngine.Rendering.LightProbeUsage.BlendProbes;

        // 4. Reflection Probe
        mesh.reflectionProbeUsage = istaking
            ? UnityEngine.Rendering.ReflectionProbeUsage.Off
            : UnityEngine.Rendering.ReflectionProbeUsage.BlendProbes;
    }
    #endregion

    #region 데이터 중복 검사

    bool IsMisMatchPipeBountList(ScreenShotFromToData pathPipe)
    {
        // 파이프 오브젝트의 실제 위치와 데이터로 넘어온 Bounds의 중심점이 너무 멀면 데이터 오염임
        for (int i = 0; i < pathPipe.PipeList.Count; i++)
        {
            if (i >= pathPipe.PipeBoundList.Count) break;

            Vector3 realPos = pathPipe.PipeList[i].transform.position;
            Vector3 dataBoundsPos = pathPipe.PipeBoundList[i].center;
            float distance = Vector3.Distance(realPos, dataBoundsPos);

            if (distance > 0.1f) // 10cm 이상 차이날 경우 (환경에 따라 조정)
            {
                Debug.Log($"<color=red>[Mismatch] {pathPipe.PathName} - Pipe[{i}]({pathPipe.PipeList[i].name}) " +
                    $"is too far from its DataBounds! Dist: {distance}</color>");
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 고유 경로값 생성
    /// </summary>
    /// <param name="curPath"></param>
    /// <returns></returns>
    bool CreateCharacteristPathValue(ScreenShotFromToData pathPipe, out int key)
    {
        key = -1;
        //양쪽 다 있는 경우
        if(pathPipe.From!=null && pathPipe.To != null)
        {
            key = HashCode.Combine(pathPipe.From.GetInstanceID(),pathPipe.To.GetInstanceID());
            return true;
        }
        //둘다 없음
        if (pathPipe.From == null && pathPipe.To == null)
        {
            return false;
        }
        //한쪽만 있음
        var target = pathPipe.From ?? pathPipe.To;
        key = target.GetInstanceID();
        return true;
    }
    #endregion

    #region 파이프 무결성
    /// <summary>
    /// 경로내 파이프가 없는가?
    /// </summary>
    /// <returns></returns>
    bool IsNotPipe_InPath(List<Pipe> pipePath)
    {
        int countPipe = 0;
        foreach (Pipe pipe in pipePath)
        {
            //파이프
            if(pipe.OriginalName == "Pipe" ||  pipe.OriginalName == "Degree")
            {
                countPipe += 1;
            }
        }
        //경로 내 파이프가 없음
        if (countPipe == 0)
        {
            Debug.Log($"[PipeSkip] Not pipe in Path");
            return true;
        }
        return false;
    }
    /// <summary>
    /// 실제 노즐과 파이프가 붙어있는지 검사
    /// </summary>
    /// <param name="pathinfo">파이프라인 경로</param>
    /// <param name="startPos">시작점</param>
    /// <param name="endPos">끝점</param>
    /// <returns></returns>
    bool IsAttachPipe_Nozzle(ScreenShotFromToData pathinfo,Vector3 startPos, Vector3 endPos)
    {
        float threshold = GridManager.Instance.NozzleOffset + 0.1f; // 허용 오차

        if (pathinfo.From != null)
        {
            Vector3 fromPipeStart = pathinfo.From.PipeStartPosition;
            if (Vector3.Distance(startPos, fromPipeStart) <= threshold ||
                Vector3.Distance(endPos, fromPipeStart) <= threshold)
                return true;
        }

        if (pathinfo.To != null)
        {
            Vector3 toPipeStart = pathinfo.To.PipeStartPosition;
            if (Vector3.Distance(startPos, toPipeStart) <= threshold ||
                Vector3.Distance(endPos, toPipeStart) <= threshold)
                return true;
        }
        return false;
    }
    #endregion

    #region 스크린샷 촬영
    /// <summary>
    /// From, to 있는 버전
    /// 각 Pipe에 bounds가 있겠지만 없을 경우를 위해 따로 매개변수 분리
    /// </summary>
    /// <param name="pipeList"></param>
    /// <param name=""></param>
    async UniTask ScreenShotFromTo(ScreenShotFromToData pathinfo,Vector3 startPos, Vector3 endPos, int num, bool isPipeOn)
    {
        //파이프 라인 on
        foreach (var pipe in pathinfo.PipeList)
        {
            pipe.gameObject.SetActive(isPipeOn);
        }
        await UniTask.Yield();
        //From, to On
        MeshRenderer fromMesh = null;
        if (pathinfo.From != null)
        {
            if(!pathinfo.From.TryGetComponent<MeshRenderer>(out fromMesh))
            {
                fromMesh = pathinfo.From.AddComponent<MeshRenderer>();
            }
            Color fromColor = new Color(153f / 255f, 204f / 255f, 0);
            await ChangeNozzleColor(pathinfo.From, fromMesh, fromColor);
        }
        MeshRenderer toMesh = null;
        if (pathinfo.To != null)
        {
            if (!pathinfo.To.TryGetComponent<MeshRenderer>(out toMesh))
            {
                toMesh = pathinfo.To.AddComponent<MeshRenderer>();
            }
            Color toColor = new Color(153f / 255f, 102f / 255f, 0);
            await ChangeNozzleColor(pathinfo.To, toMesh, toColor);
        }

        //파이프의 끝점 구하기
        //(Vector3 startPos, Vector3 endPos)sidePoint = GetPipeEndpoints(pathinfo.PipeBoundList);
        await UniTask.Yield();

        //전체 촬영영역 구하기
        //From ~ To
        //첫 bounds
        Bounds picturebounds = new Bounds();
        if (pathinfo.From != null)
        {
            picturebounds = fromMesh.bounds;
        }
        else
        {
            virtualFromnozzle.gameObject.SetActive(true);
            picturebounds = pathinfo.PipeBoundList[0];
            AttachVirtualNozzle(virtualFromnozzle, pathinfo.PipeBoundList[0],startPos);
            picturebounds.Encapsulate(virtualFromnozzle.bounds);
        }
        //나머지 pipeline
        foreach (var pipe in pathinfo.PipeBoundList)
        {
            Bounds bounds = pipe;
            if (picturebounds == null) picturebounds = bounds;
            else picturebounds.Encapsulate(bounds);
        }
        //to
        if (pathinfo.To != null)
        {
            picturebounds.Encapsulate(toMesh.bounds);
        }
        else
        {
            virtualTonozzle.gameObject.SetActive(true);
            int lastindex = pathinfo.PipeBoundList.Count - 1;
            //여기서 가상 노즐 부착
            AttachVirtualNozzle(virtualTonozzle, pathinfo.PipeBoundList[lastindex],endPos);
            picturebounds.Encapsulate(virtualTonozzle.bounds);
        }

        //확대율 고정
        float maxBoundSize = Mathf.Max(picturebounds.size.x, Mathf.Max(picturebounds.size.y, picturebounds.size.z));
        picturebounds.size = Vector3.one * maxBoundSize;
        
        //6면체 촬영
        int len = (int) EDirectionType.Count;

        //저장 폴더 생성
        int unitNum = num / unitPictureSetCount;
        string folderName = fileSavePath +"\\"+isPipeOn.ToString()+ "\\"+unitNum.ToString()  +"\\"+ pathinfo.PathName;
        if (!System.IO.Directory.Exists(folderName))
        {
            System.IO.Directory.CreateDirectory(folderName);
        }
        //이미지 해시 저장
        ulong[] imageHashes = new ulong[IMAGE_COUNT];
        byte[][] imageBytes = new byte[IMAGE_COUNT][];

        for (int i = 0; i < len; i++)
        {
            //촬영 방향
            EDirectionType direction = (EDirectionType) i;

            //촬영을 위해 카메라 이동
            MoveScreenShotCameraRuntime(pictureCam, picturebounds, direction, 1.1f);
            // 촬영 가능한 상태까지 대기
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
           
            //파일명 설정
            string fileName = SettingFilename(pathinfo, direction);
            //파일 경로 설정
            string filePath = folderName + "\\" + fileName + ".png";

            // 촬영 및 저장 (await로 파일 저장이 완료되기를 기다립니다)
            var fileContent = await TakeScreenShotRuntime(pictureCam, captureWidth, captureHeight, filePath);
            FtpAPIManager.FtpUploadFormat uploadFormat = new FtpAPIManager.FtpUploadFormat()
            {
                content = fileContent.bytes,
                fileName = fileName
            };

            //이미지와 해시값 저장
            imageBytes[i] = fileContent.bytes;
            imageHashes[i] = fileContent.hash;
        }
        await UniTask.Yield();

        //중복이미지 검사 후 중복 이미지는 제거, PipeOn일때만 검사
        if (isPipeOn)
        {
            ulong setHash = ImageHash.ComputeSetHash(imageHashes);
            Debug.Log($"[HashCheck] {pathinfo.PathName} - Generated Hash: {setHash}");
            //isPipeOnDuplicate = RemoveSameImage(setHash, imageHashes, folderName, pathinfo);
            //중복 이미지가 아니면 신규 이미지 등록
            //if (!isPipeOnDuplicate) EnrollNewFile(setHash, imageHashes, folderName);
        }
        
        //파이프라인 off
        foreach (var pipe in pathinfo.PipeList)
        {
            pipe.gameObject.SetActive(false);
        }
        await UniTask.Yield();

        //색상 정상화
        Color equipmentColor = new Color(0, 1, 204f / 255f);
        await ChangeNozzleColor(pathinfo.From, fromMesh, equipmentColor, true);
        await ChangeNozzleColor(pathinfo.To, toMesh, equipmentColor, true);

        //가상노즐끄기
        virtualFromnozzle.gameObject.SetActive(false);
        virtualTonozzle.gameObject.SetActive(false);

        //개수 세기
        Debug.Log($"<color=yellow>현재 촬영 개수 {num} / {pictureCountMax} </color>");
    }

    /// <summary>
    /// 노즐 색상 변경
    /// </summary>
    /// <param name="noz">노즐</param>
    /// <param name="nozMesh">노즐메시</param>
    /// <param name="color">변경할 색상</param>
    async UniTask ChangeNozzleColor(Nozzle noz, MeshRenderer nozMesh, Color color, bool isTaking = true)
    {
        if (noz != null && nozMesh != null)
        {
            ChangeColor_ShadowOff(nozMesh, color, isTaking);
            foreach (var child in noz.gameObject.GetComponentsInChildren<MeshRenderer>(true))
            {
                ChangeColor_ShadowOff(child, color, isTaking);
            }
        }
        await UniTask.Yield();
    }

    /// <summary>
    /// 가상노즐 부착
    /// </summary>
    void AttachVirtualNozzle(MeshRenderer nozMesh, Bounds endpipeBounds, Vector3 attachPoint)
    {
        //가상 노즐 크기
        float minEndBoundSize =Mathf.Min(endpipeBounds.size.z,  Mathf.Min(endpipeBounds.size.x, endpipeBounds.size.y));
        nozMesh.gameObject.transform.localScale = minEndBoundSize * Vector3.one;
       
        //가상 노즐 붙이는 위치
        nozMesh.gameObject.transform.position = attachPoint;
    }

    /// <summary>
    /// 파이프라인의 끝점 구하기
    /// </summary>
    /// <returns></returns>
    public (Vector3 startPoint, Vector3 endPoint) GetPipeEndpoints(List<Bounds> boundsList)
    {
        if (boundsList.Count == 1)
        {
            Debug.LogError("최소 2개 이상의 파이프가 리스트에 있어야 합니다.");
            return (boundsList[0].min, boundsList[0].max);
        }

        // 1. 시작 파이프의 끝점 계산 (0번과 1번의 방향 활용)
        Bounds startBounds = boundsList[0];
        Vector3 startDir = (boundsList[0].center - boundsList[1].center).normalized;
        Vector3 startPoint = GetExtremityPoint(startBounds, startDir);

        // 2. 마지막 파이프의 끝점 계산 (끝번과 그 직전 번호의 방향 활용)
        int lastIdx = boundsList.Count - 1;
        Bounds endBounds = boundsList[lastIdx];
        Vector3 endDir = (boundsList[lastIdx].center - boundsList[lastIdx - 1].center).normalized;
        Vector3 endPoint = GetExtremityPoint(endBounds, endDir);

        return (startPoint, endPoint);
    }

    /// <summary>
    /// 특정 방향(dir)으로 Bounds의 가장 끝 표면 중앙점을 찾는 함수
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    private Vector3 GetExtremityPoint(Bounds bounds, Vector3 dir)
    {
        // 방향 성분 중 가장 큰 축을 찾아 해당 면의 중앙을 리턴
        // AABB이므로 X, Y, Z 축 중 하나를 선택해야 합니다.
        Vector3 extents = bounds.extents;
        Vector3 center = bounds.center;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y) && Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
        {
            return center + new Vector3(Mathf.Sign(dir.x) * extents.x, 0, 0);
        }
        else if (Mathf.Abs(dir.y) > Mathf.Abs(dir.z))
        {
            return center + new Vector3(0, Mathf.Sign(dir.y) * extents.y, 0);
        }
        else
        {
            return center + new Vector3(0, 0, Mathf.Sign(dir.z) * extents.z);
        }
    }
    /// <summary>
    /// 기능 : 촬영지점으로 카메라 이동
    /// 1) 촬영지점 설정
    /// 2) 각 촬영 방향에 따른 카메라 위치와 회전 설정
    /// </summary>
    /// <param name="cam">카메라</param>
    /// <param name="targetBounds">촬영 영역</param>
    /// <param name="direction">촬영 방향</param>
    /// <param name="offset">촬영 중심으로부터의 거리</param>
    public void MoveScreenShotCameraRuntime(Camera cam,
        Bounds targetBounds,
        EDirectionType direction,
        float offset)
    {
        // 촬영 지점을 boundbox로 설정
        Vector3 boundsCenter = targetBounds.center;
        Vector3 boundsSize = targetBounds.size;
        float boundLength = 0;

        //카메라 위치, 회전 정보
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        float orthographicSize = 0;
        float margin = 5f;
        // 카메라 위치와 회전 설정
        switch (direction)
        {
            //위
            case EDirectionType.Top:
                position = boundsCenter + Vector3.up * (boundsSize.y / 2 + margin);
                rotation = Quaternion.Euler(90f, 0f, 0f);
                orthographicSize = Mathf.Max(boundsSize.x / cam.aspect, boundsSize.z) / 2;
                boundLength = boundsSize.y / 2;
                break;
            //아래
            case EDirectionType.Bottom:
                position = boundsCenter + Vector3.down * (boundsSize.y / 2 + margin);
                rotation = Quaternion.Euler(-90f, 0f, 0f);
                orthographicSize = Mathf.Max(boundsSize.x / cam.aspect, boundsSize.z) / 2;
                boundLength = boundsSize.y / 2;
                break;
            //앞
            case EDirectionType.Front:
                position = boundsCenter + Vector3.forward * (boundsSize.z / 2 + margin / 2);
                rotation = Quaternion.Euler(0f, 180f, 0f);
                orthographicSize = Mathf.Max(boundsSize.y, boundsSize.x / cam.aspect) / 2;
                boundLength = boundsSize.z / 2;
                break;
            //뒤
            case EDirectionType.Back:
                position = boundsCenter + Vector3.back * (boundsSize.z / 2 + margin);
                rotation = Quaternion.Euler(0f, 0f, 0f);
                orthographicSize = Mathf.Max(boundsSize.y, boundsSize.x / cam.aspect) / 2;
                boundLength = boundsSize.z / 2;
                break;
            //왼쪽
            case EDirectionType.Left:
                position = boundsCenter + Vector3.left * (boundsSize.x / 2 + margin / 2);
                rotation = Quaternion.Euler(0f, 90f, 0f);
                orthographicSize = Mathf.Max(boundsSize.y, boundsSize.z / cam.aspect) / 2;
                boundLength = boundsSize.x / 2;
                break;
            //오른쪽
            case EDirectionType.Right:
                position = boundsCenter + Vector3.right * (boundsSize.x / 2 + margin);
                rotation = Quaternion.Euler(0f, -90f, 0f);
                orthographicSize = Mathf.Max(boundsSize.y, boundsSize.z / cam.aspect) / 2;
                boundLength = boundsSize.x / 2;
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

    /// <summary>
    /// 파일명 설정
    /// </summary>
    /// <param name="pathinfo">파이프라인 경로 정보</param>
    /// <param name="direction">촬영 방향</param>
    /// <returns></returns>
    string SettingFilename(ScreenShotFromToData pathinfo ,EDirectionType direction)
    {
        string fileName = string.Empty;
        if (pathinfo.From != null && pathinfo.To != null)
        {
            fileName = $"{pathinfo.From.name}_{pathinfo.To.name}_{direction.ToString()}";
        }
        else if (pathinfo.From == null && pathinfo.To != null)
        {
            fileName = $"Not_{pathinfo.To.name}_{direction.ToString()}";
        }
        else if (pathinfo.From != null && pathinfo.To == null)
        {
            fileName = $"{pathinfo.From.name}_Not_{direction.ToString()}";
        }
        else if (pathinfo.From == null && pathinfo.To == null)
        {
            fileName = $"Not_{direction.ToString()}";
        }
        return fileName;
    }

    /// <summary>
    /// 기능 : 카메라로 촬영한 사진을 이미지화
    /// 1) 촬영한 영역을 이미지화
    /// 2) 렌더 텍스처로 렌더링
    /// 3) 이미지 파일을 저장
    /// </summary>
    /// <param name="cam">카메라</param>
    /// <param name="captureWidth">해상도 - 가로</param>
    /// <param name="captureHeight">해상도 - 세로</param>
    /// <param name="filePath">파일 경로</param>
    /// <returns>파일 리스트</returns>
    public async UniTask<(byte[] bytes, ulong hash)> TakeScreenShotRuntime(Camera cam, int captureWidth, int captureHeight, string filePath)
    {
        //해상도 설정
        int originWidth = UnityEngine.Screen.width;
        int originHeight = UnityEngine.Screen.height;
        bool isfullscreen = UnityEngine.Screen.fullScreen;

        // 1. 기존 타겟 백업 및 렌더 텍스쳐 설정
        RenderTexture oldRT = cam.targetTexture;
        RenderTexture rt = new RenderTexture(captureWidth, captureHeight, 24);
        rt.antiAliasing = 1; // AA 비활성화 (1 = no MSAA)
        cam.targetTexture = rt;

        // 2. 텍스쳐 준비
        Texture2D screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);

        // 3. 렌더링 및 대기 (피드백 반영 핵심 구간)
        cam.Render();
        await UniTask.WaitForEndOfFrame();

        // 4. 텍스쳐 픽셀 읽기
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        screenShot.Apply();

        // 5. 상태 복구 및 리소스 정리
        cam.targetTexture = oldRT;
        RenderTexture.active = null;

        //6. Hash 계산
        ulong hash = ImageHash.ComputeDHash(screenShot);

        //7. 파일 저장
        byte[] bytes = screenShot.EncodeToPNG();
        UniTask.Void(async () =>
        {
            await System.IO.File.WriteAllBytesAsync(filePath, bytes);
            UnityEngine.Object.Destroy(screenShot as UnityEngine.Object);
        });
        //메모리 누수 방지
        Destroy(rt);
        Destroy(screenShot);
        //8. byte 파일 반환
        return (bytes, hash);
    }
    #endregion
    #region 촬영 후 원복
    async UniTask ObjectOn()
    {
        //파이프 켜기
        foreach (var pipeline in ObjectManager.Instance.PipelineList)
        {
            foreach (var pipe in pipeline.PipeList)
            {
                pipe.gameObject.SetActive(true);
            }
        }
        return;
        //오브젝트 초기화
        await ObjectInit(false);

        //가상 노즐
        virtualFromnozzle.gameObject.SetActive(false);
        virtualTonozzle.gameObject.SetActive(false);
        //땅, 메인 rack
        OnOff_MainRackAndGround(true);
    }
    /// <summary>
    /// 땅, 메인 rack 오브젝트 활/비활성화
    /// </summary>
    void OnOff_MainRackAndGround(bool isActive)
    {
        //메인 Rack
        foreach (var structure in ObjectManager.Instance.MainRackList)
        {
            structure.gameObject.SetActive(isActive);
        }
        foreach (var structure in ObjectManager.Instance.AccessibleAreaList)
        {
            structure.gameObject.SetActive(isActive);
        }
        //땅
        groundJUPCMesh.gameObject.SetActive(isActive);
        foreach (var ground in ObjectManager.Instance.GroundList)
        {
            ground.gameObject.SetActive(isActive);
        }
    }
    #endregion

    /// <summary>
    /// 중복 이미지 제거
    /// </summary>
    bool RemoveSameImage(ulong setHash, ulong[] imageHashes, string folderName, ScreenShotFromToData pathinfo)
    {
        bool isDuplicate = false;
        if (setHashMap.TryGetValue(setHash, out var candidates))
        {
            foreach (var entry in candidates)
            {
                // 자기 자신 비교 방지
                if (entry.folderPath == folderName) continue;
                
                //중복 검사
                if (ImageHash.IsDuplicateSet(imageHashes, entry.imageHashes))
                {
                    isDuplicate = true;
                    break;
                }
            }
        }
        //중복시 방금 저장한 파일 삭제
        if (isDuplicate)
        {
            // 방금 저장한 파일들 삭제
            for (int i = 0; i < IMAGE_COUNT; i++)
            {
                string fileName = SettingFilename(pathinfo, (EDirectionType) i);
                string filePath = folderName + "\\" + fileName + ".png";
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            Debug.Log($"[Duplicate] SetHash={setHash}, Path={pathinfo.PathName}");
        }
        return isDuplicate;
    }
    /// <summary>
    /// 신규 파일 등록
    /// </summary>
    /// <param name="setHash"></param>
    /// <param name="imageHashes"></param>
    void EnrollNewFile(ulong setHash, ulong[] imageHashes, string folderName)
    {
        var newEntry = new ImageSetEntry
        {
            imageHashes = (ulong[]) imageHashes.Clone(),
            folderPath = folderName
        };
        //기존 리스트 조회
        //같은 setHash를 가진 이미지 세트가 이미 등록돼 있으면 → 그 리스트에 추가, 없으면 → 새 리스트 생성
        if (!setHashMap.TryGetValue(setHash, out var list))
        {
            list = new List<ImageSetEntry>();
            setHashMap[setHash] = list;
        }
        list.Add(newEntry);
    }
}
