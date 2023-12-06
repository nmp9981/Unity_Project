using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using static ShipExample.A10Authoring;
using static ShipExample.A20Authoring;
using static ShipExample.A30Authoring;
using static ShipExample.C30Authoring;

public class SelectedObjectVisual : MonoBehaviour
{
    public GameObject cursor;
    public float distance;

    private Entity _targetEntity;//원이 따라다닐 Entity
    private Transform _transform;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    void LateUpdate()
    {
        //엔티티의 위치를 얻어 업데이트
        if (Input.GetMouseButton(0))
        {
            var mousePosition = Input.mousePosition;
            _targetEntity = SelectEntity(mousePosition);
        }

        if (_targetEntity == Entity.Null)
        {
            cursor.SetActive(false);
            return;
        }
        Debug.Log("레이4");
        cursor.SetActive(true);

        Vector3 targetPosition = World.DefaultGameObjectInjectionWorld.EntityManager
                        .GetComponentData<LocalTransform>(_targetEntity).Position;
        _transform.position = targetPosition;
    }
    private Entity SelectEntity(Vector3 mousePosition)
    {
        //마우스 클릭 위치로 엔티티가 선택된다.
        if (Camera.main == null) return Entity.Null;
        var mainCam = Camera.main;
        var ray = mainCam.ScreenPointToRay(mousePosition);//화면 좌표로
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.blue);
        if (!Physics.Raycast(ray, out UnityEngine.RaycastHit hit, 100f)) return Entity.Null;
        Debug.Log("레이2");
        var planePos = hit.point;

        //각 오브젝트마다
        var query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(C30Object));
        Entity closestEntity = Entity.Null;
        var minSqrDist = distance * distance;
        foreach (var entity in query.ToEntityArray(Unity.Collections.Allocator.Temp))
        {
            Debug.Log("레이3");
            var entityPos = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<LocalTransform>(entity).Position;
            var dist = (planePos - (Vector3)entityPos).sqrMagnitude;
            if (dist < minSqrDist)
            {
                minSqrDist = dist;
                closestEntity = entity;
            }
        }
        return closestEntity;
    }
}
