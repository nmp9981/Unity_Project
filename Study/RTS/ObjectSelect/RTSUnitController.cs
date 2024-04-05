using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSUnitController : MonoBehaviour
{
    [SerializeField]
    private UnitSpawner unitSpawner;
    private List<UnitController> selectedUnitList;//플레이어가 클릭 or 드래그로 선택한 유닛
    public List<UnitController> UnitList { private set; get; }//맵에 존재하는 모든 유닛

    private void Awake()
    {
        selectedUnitList = new List<UnitController>();
        UnitList = unitSpawner.SpawnUnits();
    }
    //마우스 클릭으로 유닛을 선택 시 호출
    public void Clickselectunit(UnitController newUnit)
    {
        //기존에 선택된 유닛 해제
        DeselectAll();
        SelectUnit(newUnit);
    }
    //shifr+마우스 클릭으로 유닛을 선택 시 호출
    public void ShiftClickselectunit(UnitController newUnit)
    {
        //기존에 선택되어 있는 유닛을 선택
        if (selectedUnitList.Contains(newUnit))
        {
            DeselectUnit(newUnit);
        }
        else//새 유닛
        {
            SelectUnit(newUnit);
        }
    }
    //마우스 드래그로 유닛 선택할 때 호출
    public void DragSelectUnit(UnitController newUnit)
    {
        //새로운 유닛 선택
        if (!selectedUnitList.Contains(newUnit))
        {
            SelectUnit(newUnit);
        }
    }
    //선택한 모든 유닛을 이동할 때 호출
    public void MoveSelectedUnits(Vector3 end)
    {
        for(int i = 0; i < selectedUnitList.Count; i++)
        {
            selectedUnitList[i].MoveTo(end);
        }
    }
    //모든 유닛 선택 해제
    public void DeselectAll()
    {
        for(int i = 0; i < selectedUnitList.Count; ++i)
        {
            selectedUnitList[i].DeselectUnit();
        }
        selectedUnitList.Clear();
    }
    //매개변수로 받아온 newUnit 선택 설정
    void SelectUnit(UnitController newUnit)
    {
        newUnit.SelectUnit();//유닛 선택시 호출
        selectedUnitList.Add(newUnit);//선택한 유닛 정보를 리스트에 저장
    }
    //매개변수로 받아온 newUnit 선택 해제 설정
    void DeselectUnit(UnitController newUnit)
    {
        newUnit.DeselectUnit();//유닛 해제시 호출
        selectedUnitList.Remove(newUnit);//선택한 유닛 정보를 리스트에서 삭제
    }
}
