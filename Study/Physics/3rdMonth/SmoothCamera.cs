using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    public Transform target;
    public float smooth = 5f;

    /// <summary>
    /// 렌더 계층 카메라 업데이트
    /// </summary>
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.position + new Vector3(0, 3, -6);
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smooth);
    }
}
