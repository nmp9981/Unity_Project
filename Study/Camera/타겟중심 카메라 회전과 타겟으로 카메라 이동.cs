/// <summary>
/// 기능 : 타겟 중심 카메라 회전
/// </summary>
/// <param name="mouseX">마우스 좌우 변화량</param>
/// <param name="mouseY">마우스 상하 변화량</param>
/// <param name="selectModel">선택된 모델</param>
public virtual void RotationInTarget(float mouseX, float mouseY, ModelObject selectModel)
{
    Vector3 point = selectModel.transform.position;

    cameraController.camera.transform.RotateAround(point, Vector3.right, -mouseY);
    cameraController.camera.transform.RotateAround(point, Vector3.up, mouseX);
    cameraController.camera.transform.LookAt(point);

    CameraMoveToTarget();//타겟으로 카메라 이동
}
/// <summary>
/// 기능 : 타겟으로 카메라 이동
/// </summary>
public virtual void CameraMoveToTarget()
{
    var selectModel = SystemManager.Instance.SelectedModel;

    //선택한 오브젝트가 없으면 작동X
    if(selectModel == null)
    {
        return;
    }
    //선택한 오브젝트 크기 구하기
    Bounds targetBounds = TargetObjectBound(selectModel);

    //카메라와 오브젝트 사이의 거리
    float targetToDist = Mathf.Max(targetBounds.size.x, targetBounds.size.y, targetBounds.size.z);

    //카메라와 오브젝트 사이의 방향
    Vector3 targetDir = (cameraController.camera.transform.position - selectModel.gameObject.transform.position).normalized;

    //카메라가 타깃으로 이동한 뒤 위치
    cameraController.camera.transform.position = selectModel.gameObject.transform.position + targetToDist * targetDir;

    //카메라가 타깃을 바라보게
    cameraController.camera.transform.LookAt(selectModel.gameObject.transform.position);
}
/// <summary>
/// 기능 : 타겟 오브젝트 Bound구하기
/// </summary>
/// <param name="model">구하는 오브젝트</param>
/// <returns>타겟 오브젝트 크기</returns>
public static Bounds TargetObjectBound(ModelObject model)
{
    Bounds targetBounds = default;

    //오브젝트가 없으면 Bound는 0사이즈
    if (model == null) return targetBounds;

    foreach (MeshRenderer mesh in model.gameObject.GetComponentsInChildren<MeshRenderer>())
    {
        if (targetBounds == default)
        {
            targetBounds = mesh.bounds;
        }
        else
        {
            targetBounds.Encapsulate(mesh.bounds);
        }
    }
    return targetBounds;
}
