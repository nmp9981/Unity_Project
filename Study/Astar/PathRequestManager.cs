using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

struct PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public UnityAction<Vector3[], bool> callback;

    public PathRequest(Vector3 nStart, Vector3 nEnd, UnityAction<Vector3[],bool> nCallback)
    {
        pathStart = nStart;
        pathEnd = nEnd;
        callback = nCallback;
    }
}
public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;//현재 처리할 길찾기 요청

    static PathRequestManager instance;
    PathFinding pathFinding;

    bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathFinding = GetComponent<PathFinding>();
    }
    /// <summary>
    /// 오브젝트들이 요청하는 함수
    /// </summary>
    /// <param name="pathStart"></param>
    /// <param name="pathEnd"></param>
    /// <param name="callback"></param>
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, UnityAction<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }
    /// <summary>
    /// 큐순서대로 길찾기 요청을 꺼내서 PathFinding 알고리즘 시작 함수
    /// </summary>
    void TryProcessNext()
    {
        //진행중이 아니고 요청이 남아잇으면
        if(!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathFinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }
    /// <summary>
    /// 길찾기가 완료된 요청을 처리하고 오브젝트에게 이동시작명령 콜백함수 실행
    /// </summary>
    /// <param name="path"></param>
    /// <param name="success"></param>
    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }
}
