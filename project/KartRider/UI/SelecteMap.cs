using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelecteMap : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)//마우스 클릭 이벤트 함수
    {
        if (eventData.button == PointerEventData.InputButton.Left)//우클릭시
        {
            //맵 태그 아닌 경우 빈 문자열
            string mapName = string.Empty;
            if (gameObject.tag != "Map") mapName = string.Empty;
            else mapName = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            
            GameManager.Instance.CurrentMap = mapName;
            MenuUIManager.selectMapText.text = GameManager.Instance.CurrentMap;
            GameManager.Instance.CurrentMapIndex = GameManager.mapDictoinaty[MenuUIManager.selectMapText.text];
        }
    }
}
