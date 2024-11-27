using Aspose.ThreeD.Render;
using DA_Assets.Shared.Extensions;
using Metacle;
using Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

namespace MetacleAI
{
    public class FilteredPipeline
    {
        // 메인버스를 경유하는지 장비로 바로가는 파이프라인인지 판단용
        private string _name;
        public string Name
        {
            get
            {
                _name = $"{0}Inch_PipeRun_{FromNozzle.AttachedEquipment.name}[{FromNozzle.name}]_{ToNozzle.AttachedEquipment.name}[{ToNozzle.name}]";
                return _name;
            }
        }

        public Transform RootObject { get; set; }
        public bool IsMainBusRoute { get; set; }
        public Nozzle FromNozzle { get; set; }
        public Nozzle ToNozzle { get; set; }
        public List<Pipe> FromPipeList { get; set; }
        public List<Pipe> ToPipeList { get; set; }
        public List<Pipe> MainBusPipeList { get; set; }

        /// <summary> 전체 파이프를 반환하는 함수 </summary>
        public List<Pipe> GetAllPipe()
        {
            List<Pipe> result = new();

            if (FromPipeList != null)
                result.AddRange(FromPipeList);

            if (ToPipeList != null)
                result.AddRange(ToPipeList);

            if (MainBusPipeList != null)
                result.AddRange(MainBusPipeList);

            if (result.Count > 0)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary> From, To 노즐을 포함한 파이프라인의 Bounds 영역을 반환 </summary>
        public Bounds GetPipelineBounds(Nozzle fromNozzle, Nozzle toNozzle, List<Pipe> pipes)
        {
            if (pipes == null || pipes.Count == 0)
                return new Bounds(); // 빈 Bounds 반환

            Bounds combinedBounds = pipes[0].MyGeometryObject.MyCollider.bounds;

            for (int i = 1; i < pipes.Count; i++)
            {
                combinedBounds.Encapsulate(pipes[i].MyGeometryObject.MyCollider.bounds);
            }

            // from, to Bounds까지 포함
            combinedBounds.Encapsulate(fromNozzle.MyGeometryObject.MyMeshRender.bounds);
            combinedBounds.Encapsulate(toNozzle.MyGeometryObject.MyMeshRender.bounds);

            return combinedBounds;
        }
    }

    public class PipelineFilter : SingletonMonoBehaviour<PipelineFilter>
    {
        /// <summary> 오브젝트 분류 후 새로 생성된 파이프라인 리스트 </summary>
        public List<Pipeline> PipelineList;

        /// <summary> 필터링된 파이프라인 리스트 </summary>
        public List<FilteredPipeline> FilterPipelineList = new();

        /// <summary> 주변 파이프를 검사할 때 Bounds 박스 오프셋 </summary>
        public Vector3 PipeFindOffset = new Vector3(0.01f, 0.01f, 0.01f);

        /// <summary> 노즐에서 붙어있는 파이프를 찾을 때 오프셋 </summary>
        public Vector3 NozzleFindOffset = new Vector3(0.1f, 0.1f, 0.1f);

        /// <summary> 실제 Mesh 충돌 거리 오프셋 </summary>
        public float OverlapDistanceOffset = 0.01f;

        private List<GameObject> _hideObjectList = new();

        #region 디버그용
        public Nozzle TempNozzle;
        private List<List<Collider>> _displayColliders = new();
        #endregion

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                AllNozzlePipelineGenerate();
            }

            #region 디버그용
            if (Input.GetKeyDown(KeyCode.C))
            {
                var filterList = GetFilteredPipeline(TempNozzle);

                // 시각화용
                //if (filterList != null)
                //{
                //    _displayColliders.Clear();
                //    foreach (var pipeline in filterList)
                //    {
                //        var pipeList = pipeline.GetAllPipe();

                //        List<Collider> cols = new();
                //        foreach (var item in pipeList)
                //        {
                //            cols.Add(item.MyGeometryObject.MyCollider);
                //        }
                //        _displayColliders.Add(cols);
                //    }
                //}
            }
            #endregion
        }

#if UNITY_EDITOR
        #region 디버그용
        void OnDrawGizmos()
        {
            if (_displayColliders.Count < 0) return;

            Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow, Color.cyan };
            int colorIndex = 0;

            foreach (var pipeline in _displayColliders)
            {

                // 색상 배열에서 순차적으로 색상 선택
                Gizmos.color = colors[colorIndex % colors.Length];
                colorIndex++;

                foreach (var item in pipeline)
                {
                    Gizmos.DrawWireCube(item.bounds.center, item.bounds.size);
                }
            }
        }
        #endregion
#endif

        /// <summary>
        /// 매개변수로 전달 받은 노즐에서부터 연결된 파이프라인 그룹 반환
        /// 그룹 요소: From 노즐 -> 연결된 파이프들 -> To 노즐
        /// </summary>
        private List<List<ModelObject>> GetPipelineGroup(Nozzle fromNozzle)
        {
            // 노즐에서 첫번째로 연결된 파이프를 찾음
            Pipe firstPipe = FindConnectedPipeForNozzle(fromNozzle);
            if (firstPipe == null)
            {
                return null;
            }

            List<ModelObject> pipelineGroup = new List<ModelObject>
        {
            fromNozzle,
            firstPipe
        };

            List<List<ModelObject>> findPipelineList = TraverseConnectedPipe(pipelineGroup);

            if (findPipelineList != null)
            {
                return findPipelineList;
            }
            else
            {
                return null;
            }
        }

        /// <summary> 노즐에서 연결된 파이프라인을 찾고 필터링된 파이프라인으로 반환 </summary>
        public List<FilteredPipeline> GetFilteredPipeline(Nozzle fromNozzle)
        {
            // 파이프라인 그룹 생성
            var pipelineGroup = GetPipelineGroup(fromNozzle);

            if (pipelineGroup == null)
            {
                Debug.LogWarning("파이프라인 분류 에러");
                return null;
            }

            SetMainBusPipeAll(pipelineGroup);

            List<FilteredPipeline> resultList = new();

            // 파이프라인 분리
            foreach (var list in pipelineGroup)
            {
                var nozzlePair = GetNozzlePair(list, out var pipelineList);

                List<List<Pipe>> splitPipeList = SplitPipeLine(list);

                if (nozzlePair.from == null || nozzlePair.to == null)
                {
                    break;
                }

                FilteredPipeline pipeline = new()
                {
                    RootObject = nozzlePair.from.RootObj,
                    FromNozzle = nozzlePair.from,
                    ToNozzle = nozzlePair.to,
                };

                // 분류된 라인이 하나면 장비에서 장비로 바로 가는 라인
                if (splitPipeList.Count == 1)
                {
                    pipeline.FromPipeList = splitPipeList[0];
                    pipeline.IsMainBusRoute = false;
                }
                // 분류된 라인이 3개면 메인버스를 경유하는 라인
                else if (splitPipeList.Count == 3)
                {
                    pipeline.FromPipeList = splitPipeList[0];
                    pipeline.MainBusPipeList = splitPipeList[1];

                    // 현재 From과 To 경로의 경우 각 노즐부터 경로가 시작되어 요소들의 순서 변경
                    splitPipeList[2].Reverse();
                    pipeline.ToPipeList = splitPipeList[2];

                    pipeline.IsMainBusRoute = true;
                }
                else
                {
                    Debug.LogWarning("파이프 라인 분류 에러");
                    continue;
                }

                resultList.Add(pipeline);
            }

            return resultList.Count > 0 ? resultList : null;
        }

        /// <summary> 전체 노즐에 대한 파이프라인 분류 후 새로 파이프라인 생성</summary>
        public void AllNozzlePipelineGenerate()
        {
            Dictionary<Nozzle, bool> nozzleList = GetAllNozzleList();

            // 기존 파이프라인 리스트 복사
            // 나중에 동작 끝난 후 삭제용
            List<Pipeline> prevPipelineList = new List<Pipeline>(ObjectManager.Instance.PipelineList);

            List<Nozzle> keys = new List<Nozzle>(nozzleList.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                Nozzle key = keys[i];
                bool value = nozzleList[key];

                // 해당 노즐이 From 혹은 To에서 파이프라인이 생성이 안된 경우에만
                if (!value)
                {
                    List<List<ModelObject>> pipelineGroup = GetPipelineGroup(key);

                    if (pipelineGroup != null)
                    {
                        foreach (var list in pipelineGroup)
                        {
                            // To에 닿은 노즐은 파이프라인 생성 안하도록 
                            if (list[list.Count - 1] is Nozzle toNozzle)
                            {
                                nozzleList[toNozzle] = true;
                            }
                        }
                        // 파이프라인 생성
                        List<Pipeline> pipelineList = GeneratedPipeLine(pipelineGroup);

                        if (pipelineList != null)
                        {
                            PipelineList.AddRange(pipelineList);
                        }
                    }
                }
            }

            foreach (var item in prevPipelineList)
            {
                //item.gameObject.SetActive(false);
                ObjectManager.Instance.DeleteObject(item);
            }
        }

        /// <summary> 전체 노즐에 대한 필터링된 파이프라인 생성 </summary>
        public void AllNozzleFilterPipelineGenerate()
        {
            FilterPipelineList.Clear();
            Dictionary<Nozzle, bool> nozzleList = GetAllNozzleList();

            List<Nozzle> keys = new List<Nozzle>(nozzleList.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                Nozzle key = keys[i];
                bool value = nozzleList[key];

                // 해당 노즐이 From 혹은 To에서 파이프라인이 생성이 안된 경우에만
                if (!value)
                {
                    var pipelineList = GetFilteredPipeline(key);

                    if (pipelineList != null)
                    {
                        foreach (var list in pipelineList)
                        {
                            // To에 닿은 노즐은 파이프라인 생성 안하도록 
                            nozzleList[list.ToNozzle] = true;
                        }

                        FilterPipelineList.AddRange(pipelineList);
                    }
                }
            }
        }

        /// <summary> 파이프라인 그룹으로 새로운 파이프라인 생성 </summary>
        public List<Pipeline> GeneratedPipeLine(List<List<ModelObject>> pipelineGroupList)
        {
            List<Pipeline> pipelineList = new();
            foreach (var pipelineGroup in pipelineGroupList)
            {
                Nozzle fromNozzle = null;
                Nozzle toNozzle = null;
                List<Pipe> pipeList = new();
                bool hasFromNozzle = false;
                List<(Vector3 pos, Vector3 dir)> pathList = new();

                // 그룹 리스트를 순회하며 노즐과 파이프 분류
                for (int i = 0; i < pipelineGroup.Count; i++)
                {
                    if (pipelineGroup[i] is Nozzle nozzle)
                    {
                        if (!hasFromNozzle)
                        {
                            fromNozzle = nozzle;
                            hasFromNozzle = true;
                            fromNozzle.ObjectType = EObjectType.FromNozzle;
                        }
                        else
                        {
                            toNozzle = nozzle;
                            toNozzle.ObjectType = EObjectType.ToNozzle;
                        }
                    }
                    else if (pipelineGroup[i] is Pipe pipe)
                    {
                        pipeList.Add(pipe);
                    }
                }

                if (pipeList.Count > 0)
                {
                    pathList = GetPipelinePath(pipeList, fromNozzle);
                }

                if (fromNozzle != null && toNozzle != null && pipeList.Count > 0)
                {
                    List<Pipe> pipeInstList = new();

                    // 하위 파이프라인 복사
                    foreach (Pipe obj in pipeList)
                    {
                        Pipe duplicatedObject = Instantiate(obj);
                        pipeInstList.Add(duplicatedObject);
                    }

                    // 새 파이프라인 생성
                    GameObject newPipelineObj = new GameObject();
                    Pipeline newPipeline = newPipelineObj.AddComponent<Pipeline>();
                    newPipeline.SetInfo(fromNozzle, toNozzle, 1f, pathList);
                    newPipeline.AddPipe(pipeInstList);
                    pipelineList.Add(newPipeline);

                    //// 메인버스 파이프 분류
                    //SetMainBusPipe(newPipeline);
                }
            }

            return pipelineList.Count > 0 ? pipelineList : null;
        }

        /// <summary> 파이프 리스트로 경로 반환 </summary>
        public List<(Vector3 pos, Vector3 dir)> GetPipelinePath(List<Pipe> pipeList, Nozzle fromNozzle)
        {
            if (pipeList == null)
            {
                return null;
            }

            List<(Vector3 pos, Vector3 dir)> pathList = new();

            for (int i = 0; i < pipeList.Count; i++)
            {
                // 경로 노드 생성
                if (pipeList.Count == 1)
                {
                    // 경로가 하나인 경우
                    Vector3 normalDir = Vector3.Normalize(fromNozzle.transform.position - pipeList[i].transform.position);
                    Vector3 pos = pipeList[i].GetBoundsEndPointInDirection(-normalDir);

                    pathList.Add((pos, normalDir));
                }
                else if (i == pipeList.Count - 1)
                {
                    // 마지막 경로인 경우
                    Vector3 normalDir = Vector3.Normalize(pipeList[i - 1].transform.position - pipeList[i].transform.position);
                    Vector3 pos = pipeList[i].GetBoundsEndPointInDirection(-normalDir);

                    pathList.Add((pos, normalDir));
                }
                else
                {
                    Vector3 normalDir = Vector3.Normalize(pipeList[i + 1].transform.position - pipeList[i].transform.position);
                    Vector3 pos = pipeList[i].GetBoundsEndPointInDirection(normalDir);

                    pathList.Add((pos, normalDir));
                }
            }

            return pathList.Count > 0 ? pathList : null;
        }

        /// <summary> 전달받은 파이프들만 활성화하고 나머지는 비활성화하는 함수 </summary>
        public void OnlyActivePipe(List<Pipe> pipeList)
        {
            if (pipeList == null)
            {
                return;
            }

            AllHidePipe(ObjectManager.Instance.PipelineList);

            foreach (var item in pipeList)
            {
                item.gameObject.SetActive(true);
            }
        }

        /// <summary> 전체 파이프를 비활성화하는 함수 </summary>
        private void AllHidePipe(List<Pipeline> pipelines)
        {
            foreach (var pipeline in pipelines)
            {
                foreach (var pipe in pipeline.PipeList)
                {
                    _hideObjectList.Add(pipe.gameObject);
                    pipe.gameObject.SetActive(false);
                }
            }
        }

        /// <summary> 전달받은 파이프들만 활성화하고 나머지는 비활성화하는 함수 </summary>
        public void AllActivePipe()
        {
            foreach (var pipe in _hideObjectList)
            {
                pipe.gameObject.SetActive(true);
            }
        }


        /// <summary>
        /// 파이프라인 그룹에서 From, To 노즐을 분류하는 함수
        /// - out으로 pipelineList 반환
        /// </summary>
        private (Nozzle from, Nozzle to) GetNozzlePair(List<ModelObject> pipelineGroup, out List<Pipe> pipeList)
        {
            pipeList = null;
            if (pipelineGroup == null)
            {
                return (null, null);
            }

            Nozzle fromNozzle = null;
            Nozzle toNozzle = null;
            bool hasFromNozzle = false;
            pipeList = new();

            // 그룹 리스트를 순회하며 노즐과 파이프 분류
            for (int i = 0; i < pipelineGroup.Count; i++)
            {
                if (pipelineGroup[i] is Nozzle nozzle)
                {
                    if (!hasFromNozzle)
                    {
                        fromNozzle = nozzle;
                        hasFromNozzle = true;
                        fromNozzle.ObjectType = EObjectType.FromNozzle;
                    }
                    else
                    {
                        toNozzle = nozzle;
                        toNozzle.ObjectType = EObjectType.ToNozzle;
                    }
                }
                else if (pipelineGroup[i] is Pipe pipe)
                {
                    pipeList.Add(pipe);
                }
            }

            return (fromNozzle, toNozzle);
        }

        /// <summary> 모든 노즐을 갖고 있는 Dictionary 반환 - bool 값은 from 혹은 to에 파이프라인이 생성되었는지 확인용 </summary>
        private Dictionary<Nozzle, bool> GetAllNozzleList()
        {
            if (ObjectManager.Instance.NozzleList != null)
            {
                Dictionary<Nozzle, bool> resultNozzleList = new();

                foreach (var (key, value) in ObjectManager.Instance.NozzleList)
                {
                    resultNozzleList.Add(value, false);
                }
                return resultNozzleList;
            }

            return null;
        }

        /// <summary> 전체 파이프라인 그룹에서 메인버스 파이프 적용 </summary>
        private void SetMainBusPipeAll(List<List<ModelObject>> pipelineGroupList)
        {
            foreach (var pipelineGroup in pipelineGroupList)
            {
                foreach (var item in pipelineGroup)
                {
                    if (item is Pipe pipe)
                    {
                        SetMainBusPipe(pipe);
                    }
                }
            }
        }

        /// <summary> 메인버스 영역에 포함되어 있는 파이프인지 체크 후 타입 변경 </summary>
        private void SetMainBusPipe(Pipe pipe)
        {
            foreach (var item in MainBusManager.Instance.MainBusColliderList)
            {
                if (IsIntersectingByThreshold(pipe.MyGeometryObject.MyCollider, item, 0.5f))
                {
                    pipe.ObjectType = EObjectType.MainBusPipe;
                    ObjectManager.Instance.ChangeColor(pipe.MyGeometryObject.MyMeshRender, pipe);
                    break;
                }
            }
        }

        /// <summary> 파이프라인 분류(0: From, 1: MainBus, 2: To) </summary>
        private List<List<Pipe>> SplitPipeLine(List<ModelObject> pipelineGroup)
        {
            List<List<Pipe>> result = new List<List<Pipe>>(); ;

            EObjectType currentObjectType = pipelineGroup.First().ObjectType;
            foreach (var item in pipelineGroup)
            {
                EObjectType objectType = item.ObjectType;
                if (item is Pipe pipe)
                {
                    if (currentObjectType != objectType)
                    {
                        currentObjectType = objectType;
                        result.Add(new());
                    }

                    result[^1].Add(pipe);
                }
            }

            return result;
        }

        #region 파이프라인 검사 관련
        /// <summary>
        /// 넘겨받은 리스트의 마지막에서 연결된 파이프를 모두 찾는 함수
        /// - 못찾으면 null 반환
        /// </summary>
        private List<List<ModelObject>> TraverseConnectedPipe(List<ModelObject> pipelineGroup)
        {
            List<List<ModelObject>> finalPaths = new();
            HashSet<ModelObject> visitedObjects = new();
            Stack<List<ModelObject>> stack = new();
            stack.Push(pipelineGroup);

            while (stack.Count > 0)
            {
                List<ModelObject> currentPath = stack.Peek();
                ModelObject lastObj = currentPath[currentPath.Count - 1];

                List<Pipe> nextPipeList = FindConnectedPipe(lastObj, pipelineGroup, visitedObjects);

                if (nextPipeList != null && nextPipeList.Count > 0)
                {
                    foreach (var pipe in nextPipeList)
                    {
                        visitedObjects.Add(pipe);
                    }

                    if (nextPipeList.Count == 1)
                    {
                        // 파이프가 하나로 이어질 때
                        Pipe nextPipe = nextPipeList[0];
                        currentPath.Add(nextPipe);
                    }
                    else
                    {
                        // 파이프가 여러갈래로 갈라질 때
                        List<List<ModelObject>> splitList = SplitPipelineList(nextPipeList, currentPath);

                        // 현재 리스트 제거 후 새로운 갈래 리스트 생성
                        stack.Pop();

                        foreach (var list in splitList)
                        {
                            stack.Push(list);
                        }
                    }
                }
                else
                {
                    // 더 이상 연결된 파이프가 없을 때 노즐 탐색
                    if (lastObj != null)
                    {
                        // 영역 내 모든 노즐을 가져옴
                        List<Nozzle> toNozzleList = FindConnectedNozzle(lastObj);
                        if (toNozzleList != null)
                        {
                            // 노즐들 중 해당 노즐의 방향과 맞는 파이프랑 마지막 파이프 비교
                            // 영역 내 노즐이 여러개가 잡히는 것에 대한 처리
                            foreach (var nozzle in toNozzleList)
                            {
                                Pipe connectedPipe = FindConnectedPipeForNozzle(nozzle);
                                if (connectedPipe != null)
                                {
                                    if (lastObj == connectedPipe)
                                    {
                                        currentPath.Add(nozzle);
                                        finalPaths.Add(currentPath);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    stack.Pop();
                }
            }

            // 경로를 찾지 못했을 경우
            return finalPaths.Count > 0 ? finalPaths : null;
        }

        /// <summary> 매개변수로 받은 파이프들을 마지막으로 하는 리스트 생성 후 반환 </summary>
        private List<List<ModelObject>> SplitPipelineList(List<Pipe> lastPipeList, List<ModelObject> originList)
        {
            List<List<ModelObject>> resultList = new();

            foreach (var pipe in lastPipeList)
            {
                List<ModelObject> newList = new();
                newList.AddRange(originList);
                newList.Add(pipe);
                resultList.Add(newList);
            }

            if (resultList.Count > 0)
            {
                return resultList;
            }
            else
            {
                return null;
            }
        }

        /// <summary> 노즐 or 파이프에서 연결된 파이프를 반환하는 함수 </summary>
        private List<Pipe> FindConnectedPipe(ModelObject obj, List<ModelObject> pipelineGroup, HashSet<ModelObject> visitedObjects)
        {
            Bounds objBounds = obj.MyGeometryObject.MyCollider.bounds;
            Vector3 halfExtents = new Vector3(objBounds.size.x / 2, objBounds.size.y / 2, objBounds.size.z / 2) + PipeFindOffset;
            LayerMask layer = 1 << LayerMask.NameToLayer(ObjectManager.Instance.ObjectLayerName);
            Collider[] hitColliders = Physics.OverlapBox(objBounds.center, halfExtents, Quaternion.identity, layer);

            // 찾은 파이프를 저장할 리스트
            List<Pipe> findPipeList = new();

            // 충돌된 오브젝트들에서 파이프를 저장
            foreach (var hitCollider in hitColliders)
            {
                GeometryObject findObj = hitCollider.GetComponent<GeometryObject>();
                if (findObj != null)
                {
                    if (findObj.MyObject != obj)
                    {
                        if (findObj.MyObject is Pipe pipe)
                        {
                            // 이미 방문한 오브젝트면 제외
                            if (!visitedObjects.Contains(pipe))
                            {
                                if (!pipelineGroup.Contains(pipe))
                                {
                                    float distance = GetMeshDistance(obj, pipe);

                                    Debug.Log($"{obj.name}~{pipe.name}까지의 거리: {distance}");

                                    if (distance < OverlapDistanceOffset) // 임계값 설정
                                    {
                                        findPipeList.Add(pipe);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            if (findPipeList.Count > 0)
            {
                return findPipeList;
            }
            else
            {
                return null;
            }
        }

        /// <summary> 주어진 파이프라인에서 연결된 다른 노즐을 찾는 메서드</summary>
        private List<Nozzle> FindConnectedNozzle(ModelObject obj)
        {
            List<Nozzle> nozzleList = new();

            Bounds objBounds = obj.MyGeometryObject.MyCollider.bounds;
            Vector3 halfExtents = new Vector3(objBounds.size.x / 2, objBounds.size.y / 2, objBounds.size.z / 2) + NozzleFindOffset;
            LayerMask layer = 1 << LayerMask.NameToLayer(ObjectManager.Instance.ObjectLayerName);

            Collider[] hitColliders = Physics.OverlapBox(obj.transform.position, halfExtents, obj.transform.rotation, layer);

            // 충돌된 오브젝트들 중 노즐을 찾아 반환
            foreach (var hitCollider in hitColliders)
            {
                GeometryObject findObj = hitCollider.GetComponent<GeometryObject>();
                if (findObj != null)
                {
                    if (findObj.MyObject is Nozzle nozzle)
                    {
                        nozzleList.Add(nozzle);
                    }
                }
            }
            return nozzleList.Count > 0 ? nozzleList : null;
        }

        /// <summary> 노즐에 연결된 파이프를 찾는 함수 </summary>
        private Pipe FindConnectedPipeForNozzle(Nozzle nozzle)
        {
            Vector3 dirVector = nozzle.DirVector;
            Dictionary<Pipe, float> pipeDic = new();

            // 노즐 회전 시 월드 기준 Bounds 때문에 NozzleStartPosition이 노즐에 딱 붙어서 안생김
            // 사이즈가 매우 작은 파이프의 경우 안걸릴 수 있어서
            // 노즐의 중심점부터 오프셋까지의 거리만큼 검사
            Vector3 NozzleStartPos = nozzle.GetPipeStartPosition(dirVector, OverlapDistanceOffset);
            float maxDistance = Vector3.Distance(NozzleStartPos, nozzle.transform.position);

            LayerMask layer = 1 << LayerMask.NameToLayer(ObjectManager.Instance.ObjectLayerName);

            // 노즐 시작 지점에서 노즐의 방향으로 레이를 쏴 파이프 검사
            RaycastHit[] hits = Physics.RaycastAll(nozzle.transform.position, dirVector, maxDistance, layer);

            foreach (var hit in hits)
            {
                GeometryObject findObj = hit.collider.GetComponent<GeometryObject>();
                if (findObj != null)
                {
                    if (findObj.MyObject is Pipe pipe)
                    {
                        if (!pipeDic.ContainsKey(pipe))
                        {
                            float distance = Vector3.Distance(nozzle.transform.position, pipe.transform.position);
                            pipeDic.Add(pipe, distance);
                        }
                    }
                }
            }

            if (pipeDic.Count > 0)
            {
                // 가장 가까운 파이프 반환
                return pipeDic.Aggregate((x, y) => x.Value < y.Value ? x : y).Key;
            }

            return null;
        }

        /// <summary> 오브젝트간의 가장 가까운 메쉬의 정점을 찾고 거리를 계산하여 반환 </summary>
        public float GetMeshDistance(ModelObject obj1, ModelObject obj2)
        {
            Mesh mesh1 = obj1.MyGeometryObject.MyMeshFilter.mesh;
            Mesh mesh2 = obj2.MyGeometryObject.MyMeshFilter.mesh;

            Vector3[] vertices1 = mesh1.vertices;
            Vector3[] vertices2 = mesh2.vertices;

            float minDistance = float.MaxValue;
            Vector3 closestPoint1 = Vector3.zero;
            Vector3 closestPoint2 = Vector3.zero;

            object lockObj = new object();

            obj1.MyGeometryObject.transform.TransformPoints(vertices1);
            obj2.MyGeometryObject.transform.TransformPoints(vertices2);

            // 성능상 이슈로 병렬 처리
            Parallel.For(0, vertices1.Length, i =>
            {
                Vector3 worldVertex1 = vertices1[i];
                for (int j = 0; j < vertices2.Length; j++)
                {
                    Vector3 worldVertex2 = vertices2[j];
                    float distance = Vector3.Distance(worldVertex1, worldVertex2);
                    if (distance < minDistance)
                    {
                        // distance값 여러 쓰레드 경합 방지
                        lock (lockObj)
                        {
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                closestPoint1 = worldVertex1;
                                closestPoint2 = worldVertex2;
                            }
                        }
                    }
                }
            });

            return minDistance;
        }

        /// <summary> 전달된 비중 값 만큼 충돌했는지 체크 </summary>
        private bool IsIntersectingByThreshold(Collider col1, Collider col2, float threshold)
        {
            Bounds bounds1 = col1.bounds;
            Bounds bounds2 = col2.bounds;

            // 두 콜라이더의 경계 영역이 교차하는지 확인
            if (bounds1.Intersects(bounds2))
            {
                // 교차된 영역 계산
                Bounds intersection = GetIntersectionBounds(bounds1, bounds2);

                // 교차된 영역과 첫번째 매개변수 콜라이더의 부피를 비교
                float intersectionVolume = intersection.size.x * intersection.size.y * intersection.size.z;
                float col1Volume = bounds1.size.x * bounds1.size.y * bounds1.size.z;
                return intersectionVolume >= (col1Volume * threshold);
            }

            return false;
        }

        /// <summary> 교차된 영역을 반환 </summary>
        private Bounds GetIntersectionBounds(Bounds b1, Bounds b2)
        {
            Vector3 min = Vector3.Max(b1.min, b2.min);
            Vector3 max = Vector3.Min(b1.max, b2.max);

            return new Bounds((min + max) / 2, max - min);
        }
        #endregion
    }
}
