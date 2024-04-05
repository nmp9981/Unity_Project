using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClick : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerUnit;
    [SerializeField]
    private LayerMask layerGround;

    private Camera mainCamera;
    private RTSUnitController rtsUnitController;

    void Awake()
    {
        mainCamera = Camera.main;
        rtsUnitController = GetComponent<RTSUnitController>();
    }

    void Update()
    {
        //마우스 왼쪽 클릭으로 unit 선택/해제
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            //광선에 부딪히는 오브젝트가 있을 때(유닛 클릭)
            if(Physics.Raycast(ray,out hit, Mathf.Infinity, layerUnit))
            {
                if (hit.transform.GetComponent<UnitController>() == null) return;//유닛이 아니면(유닛은 unitcontroller컴포넌트가 있다)

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    //선택되지 않은 유닛을 선택하면 선택목록에 추가, 선택된 유닛을 선택하면 선택목록에서 제거
                    rtsUnitController.ShiftClickselectunit(hit.transform.GetComponent<UnitController>());
                }
                else
                {
                    //선택 목록에 있는 모든 유닛정보를 삭제하고 현재 유닛을 선택목록에 추가
                    rtsUnitController.Clickselectunit(hit.transform.GetComponent<UnitController>());
                }
            }
            else//광선에 부딪히는 오브젝트가 없을 때(ㅇ유닛 선택 안함)
            {
                if (!Input.GetKey(KeyCode.LeftShift))//왼쪽 쉬프트 키를 누르고 있지않으면 선택목록에 있는 모든 유닛 정보 삭제
                {
                    rtsUnitController.DeselectAll();
                }
            }
        }
        //마우스 우클릭으로 유닛 이동
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            //광선에 부딪히는 오브젝트가 있을 때(유닛 클릭)
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerGround))
            {
                rtsUnitController.MoveSelectedUnits(hit.point);
            }
        }
    }
}
